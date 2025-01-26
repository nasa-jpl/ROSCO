using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RockCollect
{
    public class ParamList
    {
        public float GSD_resolution;
        public float gamma;
        public float sun_azimuth_angle;
        public float sun_incidence_angle;
        public float min_shadow_size;
        public float max_shadow_size;
        public float confidence_threshold;
        public float min_split_shadow_size;
        public float mean_gradient;
        public float shadow_excentricity;
        public int gamma_threshold_override;

        public bool Matches(ParamList p)
        {
            return GSD_resolution == p.GSD_resolution &&
                   gamma == p.gamma &&
                   sun_azimuth_angle == p.sun_azimuth_angle &&
                   sun_incidence_angle == p.sun_incidence_angle &&
                   min_shadow_size == p.min_shadow_size &&
                   max_shadow_size == p.max_shadow_size &&
                   confidence_threshold == p.confidence_threshold &&
                   min_split_shadow_size == p.min_split_shadow_size &&
                   mean_gradient == p.mean_gradient &&
                   shadow_excentricity == p.shadow_excentricity &&
                   gamma_threshold_override == p.gamma_threshold_override;
        }

        public void Save(string path)
        {
            using (StreamWriter sw = new StreamWriter(path))
            {
                sw.WriteLine("rd_gamma                        {0}", this.gamma);
                sw.WriteLine("rd_ground_resolution            {0}", this.GSD_resolution);
                sw.WriteLine("rd_sun_incidence_angle          {0}", this.sun_incidence_angle);
                sw.WriteLine("rd_sun_azimuth_angle            {0}", this.sun_azimuth_angle);
                sw.WriteLine("rd_min_shadow_size              {0}", (int)this.min_shadow_size);
                sw.WriteLine("rd_max_shadow_size              {0}", (int)this.max_shadow_size);
                sw.WriteLine("rd_mean_gradient_threshold      {0}", this.mean_gradient);
                sw.WriteLine("rd_confidence_threshold         {0}", this.confidence_threshold);
                sw.WriteLine("rd_min_split_shadow_size        {0}", (int)this.min_split_shadow_size);
                sw.WriteLine("rd_spliting_ratio               0.5"); //fixed
                sw.WriteLine("rd_min_hazard_size              1.0"); //fixed
                sw.WriteLine("rd_rock_elongate_ratio          {0}", this.shadow_excentricity);
                sw.WriteLine("rd_gamma_threshold_override     {0}", this.gamma_threshold_override); 
            }
        }
    }

    public class Rock
    {
        public int Id;
        public int TileR;
        public int TileC;
        public float ShaX;
        public float ShaY;
        public float RockX;
        public float RockY;
        public float TileShaX;
        public float TileShaY;
        public float ShaArea;
        public float ShaLen;
        public float RockWidth;
        public float RockHeight;
        public float Score;
        public float GradMean;
        public float Compact;
        public float Extent;
        public int Class;
        public float Gamma;

        public float rockXStd;
        public float rockYStd;
        public float rockWidthStd;
        public float rockHeightStd;
        public float shaXStd;
        public float shaYStd;
        public float shaAreaStd;
        public float shaLenStd;
        public float scoreStd;
        public float gradMeanStd;
        public float CompactStd;
        public float ExentStd;

        public int SimpleHash()
        {
            return TileR + TileC;
        }
    };

    public class Rocklist
    {
        public List<string> invalidRocks = new List<string>();
        public Dictionary<int, List<Rock>> rocksByHash = new Dictionary<int, List<Rock>>();
        public Dictionary<int, Rock> rocksById = new Dictionary<int, Rock>();
        public ParamList paramList = null;
        public RockListParser parser = null;

        public int NumberOfRocks
        {
            get
            {
                int totalRocks = 0;
                foreach (var rockValues in rocksByHash)
                {
                    foreach (var rock in rockValues.Value)
                    {
                        totalRocks++;
                    }
                }
                return totalRocks;
            }
        }

        public Rocklist(string filePath)
        {
            string[] contentsFile = File.ReadAllLines(filePath);
            bool isCombined = Rocklist.IsCombinedFormat(filePath);
            bool isLegacy = Rocklist.IsLegacyFormat(filePath,contentsFile);
            
            if (isCombined)
            {
                parser = new CombinedRockListParser();
            }
            else if (isLegacy)
            {
                parser = new LegacyRockListParser();
            }
            else
            {
                parser = new CurrentRockListParser();
            }

            paramList = parser.ReadHeader(contentsFile);
            parser.ParseRocks(contentsFile, out rocksByHash, out rocksById, out invalidRocks);
        }

        public Rocklist(ParamList inParams, Dictionary<int, List<Rock>> inRocks)
        {
            paramList = inParams;
            rocksByHash = inRocks;
        }

        public Rocklist(ParamList inParams, List<Rock> inRocks, RockListParser inParser)
        {
            parser = inParser;
            paramList = inParams;
            rocksByHash = new Dictionary<int, List<Rock>>();
            foreach (var rock in inRocks)
            {
                int hash = rock.SimpleHash();
                if (!rocksByHash.ContainsKey(hash))
                    rocksByHash[hash] = new List<Rock>();

                rocksByHash[hash].Add(rock);
            }
        }

        static public bool IsLegacyFormat(string filepath, string[] rocklist)
        {
            LegacyRockListParser parser = new LegacyRockListParser();
            if (rocklist[0].Split(' ')[0] == "version")
                return false;
            return rocklist[parser.GetColumnHeaderLineIndex()][0] == '%' && System.IO.Path.GetExtension(filepath) == ".txt";
        }

        static public bool IsCombinedFormat(string filepath)
        {
            return System.IO.Path.GetExtension(filepath) == ".csv";
        }

        public void Save(string filePath)
        {
            List<Rock> flatRocks = new List<Rock>();

            foreach (var rocklist in rocksByHash.Values)
            {
                foreach (var rock in rocklist)
                {
                    flatRocks.Add(rock);
                }
            }

            var sortedRocks = flatRocks.OrderBy(x => x.Id);

            using (StreamWriter file = new System.IO.StreamWriter(filePath))
            {
                file.WriteLine(parser.WriteHeader(paramList));
                file.WriteLine(parser.GetColumnHeadersString());
                foreach (var rock in sortedRocks)
                {
                    file.WriteLine(parser.WriteRock(rock));
                }
            }
        }

        public void SaveInvalid(string filePath)
        {
            using (StreamWriter file = new System.IO.StreamWriter(filePath))
            {
                file.WriteLine(parser.WriteHeader(paramList));
                file.WriteLine(parser.GetColumnHeadersString());
                foreach (var line in invalidRocks)
                {
                    file.WriteLine(line);
                }
            }
        }
    }
}
