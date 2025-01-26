using System;
using System.Collections.Generic;

namespace RockCollect
{
    public class CurrentRockListParser : RockListParser
    {
        const double ROCKLIST_FILE_VERSION = 2;
        public int GetColumnHeaderLineIndex() { return 12; }

        static private string columnHeader = "id, tileR, tileC, shaX, shaY, rockX, rockY, tileShaX, tileShaY, shaArea, shaLen, rockWidth, rockHeight, score, gradMean, Compact, Exent, Class, gamma";
        public string GetColumnHeadersString()
        {
            return columnHeader;
        }

        public string FileExtension() { return ".txt"; }


        public ParamList ReadHeader(string[] fileContents)
        {
            char[] separator = new char[] { ' ' };
            ParamList p = new ParamList();
            try
            {
                float version = float.Parse(fileContents[0].Split(separator, StringSplitOptions.RemoveEmptyEntries)[1]);
                if(ROCKLIST_FILE_VERSION != 2)
                {
                    throw new SystemException("Old data format");
                }
                p.GSD_resolution = float.Parse(fileContents[1].Split(separator, StringSplitOptions.RemoveEmptyEntries)[1]);
                p.gamma = float.Parse(fileContents[2].Split(separator, StringSplitOptions.RemoveEmptyEntries)[1]);
                p.sun_azimuth_angle = float.Parse(fileContents[3].Split(separator, StringSplitOptions.RemoveEmptyEntries)[1]);
                p.sun_incidence_angle = float.Parse(fileContents[4].Split(separator, StringSplitOptions.RemoveEmptyEntries)[1]);
                p.min_shadow_size = float.Parse(fileContents[5].Split(separator, StringSplitOptions.RemoveEmptyEntries)[1]);
                p.max_shadow_size = float.Parse(fileContents[6].Split(separator, StringSplitOptions.RemoveEmptyEntries)[1]);
                p.confidence_threshold = float.Parse(fileContents[7].Split(separator, StringSplitOptions.RemoveEmptyEntries)[1]);
                p.min_split_shadow_size = float.Parse(fileContents[8].Split(separator, StringSplitOptions.RemoveEmptyEntries)[1]);
                p.mean_gradient = float.Parse(fileContents[9].Split(separator, StringSplitOptions.RemoveEmptyEntries)[1]);
                p.shadow_excentricity = float.Parse(fileContents[10].Split(separator, StringSplitOptions.RemoveEmptyEntries)[2]);
                p.gamma_threshold_override = int.Parse(fileContents[11].Split(separator, StringSplitOptions.RemoveEmptyEntries)[1]);
            }
            catch
            {
                //bad header
                return null;
            }
            return p;
        }

        public string WriteHeader(ParamList paramList)
        {
            return  "version " + ROCKLIST_FILE_VERSION + "\n" +
                    "%GSD_resolution " + paramList.GSD_resolution + "\n" +
                    "%gamma " + paramList.gamma + "\n" +
                    "%sun_azimuth_angle " + paramList.sun_azimuth_angle + "\n" +
                    "%sun_incidence_angle " + paramList.sun_incidence_angle + "\n" +
                    "%min_shadow_size " + paramList.min_shadow_size + "\n" +
                    "%max_shadow_size " + paramList.max_shadow_size + "\n" +
                    "%confidence_threshold " + paramList.confidence_threshold + "\n" +
                    "%min_split_shadow_size " + paramList.min_split_shadow_size + "\n" +
                    "%mean_gradient " + paramList.mean_gradient + "\n" +
                    "%shadow excentricity " + paramList.shadow_excentricity + "\n" +
                    "%gamma_threshold_override " + paramList.gamma_threshold_override + "\n";
        }

        public void ParseRocks(string[] fileContents, out Dictionary<int, List<Rock>> rocksByHash,
            out Dictionary<int, Rock> rocksById, out List<string> invalidRocks)
        {
            rocksByHash = new Dictionary<int, List<Rock>>();
            rocksById = new Dictionary<int, Rock>();
            invalidRocks = new List<string>();

            for (int idxCurLine = GetColumnHeaderLineIndex() + 1; idxCurLine < fileContents.Length; idxCurLine++)
            {
                string curLine = fileContents[idxCurLine];

                try
                {
                    Rock curRock = ReadRock(curLine);

                    int hash = curRock.SimpleHash();
                    if (!rocksByHash.ContainsKey(hash))
                        rocksByHash[hash] = new List<Rock>();

                    rocksByHash[hash].Add(curRock);
                    rocksById.Add(curRock.Id, curRock);
                }
                catch (System.FormatException)
                {
                    invalidRocks.Add(curLine);
                }
            }
        }

        private static readonly char[] separator = new char[] { ',' };

        public Rock ReadRock(string line)
        {
            string[] vars = line.Split(separator);
            Rock rock = new Rock();
            rock.Id = int.Parse(vars[0]);
            rock.TileR = int.Parse(vars[1]);
            rock.TileC = int.Parse(vars[2]);
            rock.ShaX = float.Parse(vars[3]);
            rock.ShaY = float.Parse(vars[4]);
            rock.RockX = float.Parse(vars[5]);
            rock.RockY = float.Parse(vars[6]);
            rock.TileShaX = float.Parse(vars[7]);
            rock.TileShaY = float.Parse(vars[8]);
            rock.ShaArea = int.Parse(vars[9]);
            rock.ShaLen = float.Parse(vars[10]);
            rock.RockWidth = float.Parse(vars[11]);
            rock.RockHeight = float.Parse(vars[12]);
            rock.Score = float.Parse(vars[13]);
            rock.GradMean = float.Parse(vars[14]);
            rock.Compact = float.Parse(vars[15]);
            rock.Extent = float.Parse(vars[16]);
            rock.Class = int.Parse(vars[17]);
            rock.Gamma = float.Parse(vars[18]);

            return rock;
        }

        public string WriteRock(Rock rock)
        {
            string sep = new string(separator);
            return "   " + rock.Id + sep +
            "   " + rock.TileR + sep +
            "   " + rock.TileC + sep +
            "   " + rock.ShaX + sep +
            "   " + rock.ShaY + sep +
            "   " + rock.RockX + sep +
            "   " + rock.RockY + sep +
            "   " + rock.TileShaX + sep +
            "   " + rock.TileShaY + sep +
            "   " + (int)rock.ShaArea + sep +
            "   " + rock.ShaLen + sep +
            "   " + rock.RockWidth + sep +
            "   " + rock.RockHeight + sep +
            "   " + rock.Score + sep +
            "   " + rock.GradMean + sep +
            "   " + rock.Compact + sep +
            "   " + rock.Extent + sep +
            "   " + rock.Class + sep +
            "   " + rock.Gamma;
        }
    }
}
