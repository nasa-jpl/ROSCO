using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RockCollect.Stages
{
    public class ImageThreshold : Stage
    {
        float Gamma;
        int ThresholdOverride;

        string TilePath;
        Image TileImage;

        string GammaPath;
        Image GammaImage;

        string ShadowPath;
        Image ShadowImage;
        
        Image OverlayImage;

        int thresholdInGamma;

        static public readonly float DATA_VERSION = 0.1f;
        static public readonly float DEFAULT_GAMMA = 3.0f;
        static public readonly int DEFAULT_THRESHOLDOVERRIDE = 0; //DISABLED
        static public readonly float MAX_GAMMA = 5.0f;
        static public readonly float MIN_GAMMA = 0.00001f;

        public override float GetDataVersion() { return DATA_VERSION; }

        public override string GetName()
        {
            return "ImageThreshold";
        }
        
        public override UserControl CreateUI()
        {
            return new ImageThresholdUI(this);
        }

        public override UserControl CreateStatusUI(UserControl mainUI)
        {
            return new ImageThresholdStatusUI(mainUI);
        }

        public override void Init(Logger log, string stageDirectory, string finalOutputDirectory)
        {
            base.Init(log, stageDirectory, finalOutputDirectory);

            Gamma = DEFAULT_GAMMA;
            ThresholdOverride = DEFAULT_THRESHOLDOVERRIDE;
        }

        public override bool LoadInput(string directory)
        {
            bool result = base.LoadInput(directory);
            if (result == false)
                return false;

            if (!this.inData.Data.ContainsKey("TILE_PATH"))
                return false;

            TilePath = inData.Data["TILE_PATH"];
            if (!File.Exists(TilePath))
                throw new Exception(string.Format("Input tile image {0} doesn't exist", TilePath));

            GDALSerializer.LoadMetadata(TilePath, out int widthPixels, out int heightPixels, out int bands, out Type[] dataTypes);
            TileImage = GDALSerializer.Load(TilePath, 0, 0, widthPixels, heightPixels);

            if (TileImage == null)
                return false;

            ShadowPath = Path.Combine(GetDirectory(Dir.Output), "shadow.pgm");
            GammaPath = Path.Combine(GetDirectory(Dir.Output), "gamma.pgm");

            UpdateShadowAndOverlay();
            return true;
        }

        public override bool SaveOutput()
        {
            if (base.SaveOutput())
            {
                // tile image
                if (TileImage == null)
                    return false;

                if (string.IsNullOrEmpty(TilePath))
                    return false;

                string tilePath = Path.Combine(GetDirectory(Dir.Output), "tile.pgm"); //TODO:need a data passthru, copies image input to output and updates paths
                GDALSerializer.Save(TileImage, tilePath, null);

                this.outData.Data.Add("TILE_PATH", tilePath);

                InputToOutput("GSD");
                InputToOutput("AZIMUTH");
                InputToOutput("INCIDENCE");
                InputToOutput("IMAGE_PATH");
                InputToOutput("TILE_INDEX");
                InputToOutput("TILE_COL");
                InputToOutput("TILE_ROW");
                InputToOutput("TILE_GROUP");
                InputToOutput("TILES_HORIZONTAL");
                InputToOutput("TILES_VERTICAL");
                InputToOutput("COMPARISON_ROCKLIST");
                
                // shadow image
                if (ShadowImage == null)
                    return false;

                if (string.IsNullOrEmpty(ShadowPath))
                    return false;

                string shadowPath = Path.Combine(GetDirectory(Dir.Output), "shadow.pgm");
                GDALSerializer.Save(ShadowImage, shadowPath, null);
                this.outData.Data.Add("SHADOW_PATH", ShadowPath);

                // gamma image
                if (GammaImage == null)
                    return false;

                if (string.IsNullOrEmpty(GammaPath))
                    return false;

                string gammaPath = Path.Combine(GetDirectory(Dir.Output), "gamma.pgm");
                GDALSerializer.Save(GammaImage, gammaPath, null);
                this.outData.Data.Add("GAMMA_PATH", GammaPath);

                // shadow blob
                string shadowBlobPath = Path.Combine(GetDirectory(Dir.Output), "shadowBlobs.ppm");
                int numShadows = 0;

                Console.WriteLine("running rock detector on tile to make shadow blob image");

                if(0 == RockDetector.shadows_from_files(shadowPath, shadowBlobPath, ref numShadows)) //BUGBUG: pre-splitting ids...wont match
                {
                    throw new Exception("Failed to generate shadow blob image");
                }

                if (!File.Exists(shadowBlobPath))
                    throw new Exception("Failed to find shadow blob image");

                this.outData.Data.Add("SHADOW_BLOB_PATH", shadowBlobPath);

                // gamma
                this.outData.Data.Add("GAMMA", Gamma.ToString());
                this.outData.Data.Add("GAMMA_THRESHOLD_OVERRIDE", ThresholdOverride.ToString());

                if (!WriteOutputJSON())
                    return false;

                return true;
            }

            return false;
        }


        public Dictionary<float, int> SweepShadowBlobsForGamma(float stepSize, string shadowPath)
        {
            Dictionary<float, int> countByGamma = new Dictionary<float, int>();
            if (string.IsNullOrEmpty(TilePath))
                return countByGamma;


            float GSD = float.Parse(inData.Data["GSD"]);
            float Azimuth = float.Parse(inData.Data["AZIMUTH"]);
            float Incidence = float.Parse(inData.Data["INCIDENCE"]);

            float range = MAX_GAMMA - MIN_GAMMA;
            int steps = (int)Math.Ceiling(range / stepSize);

            RockDetector.INSETTINGS settings = new RockDetector.INSETTINGS {
                Gamma = 1, //set in loop
                Incidence = Incidence,
                Azimuth = Azimuth,
                MinShadowArea = RockDetector.DISABLE_MIN_SHADOW_AREA,
                GSD = GSD,
                Confidence = RockDetector.DISABLE_CONFIDENCE,
                MinShadowSplit = RockDetector.DISABLE_SPLIT,
                SplittingRatio = RockDetector.SHADOW_SPLIT_RATIO,
                ShadowAspect = RockDetector.DISABLE_ASPECT,
                MeanGradient = RockDetector.DISABLE_GRADIENT,
                MaxShadowArea = RockDetector.DISABLE_MAX_SHADOW_AREA,
                GammaThresholdOverride = RockDetector.DISABLE_GAMMA_THRESH_OVERRIDE
            };

            Console.WriteLine(string.Format("running rock detector on tile {0} times with gamma {1} to {2} " +
                                            "to compute shadow blob pixels by gamma",
                                            steps, MIN_GAMMA, MAX_GAMMA));

            for (int idxStep = 0; idxStep < steps; idxStep++)
            {
                settings.Gamma = MIN_GAMMA + idxStep * stepSize;
                var results = new RockDetector.DetectionResults("SweepGamma", TileImage.Width, TileImage.Height);

                int numOutRocks = 0;
                byte [] detectImage = new byte[TileImage.Width * TileImage.Height];
                int success = RockDetector.detect_tile_rocks(TilePath, results.outLabelImage, detectImage, 0, 0,
                                            TileImage.Height, TileImage.Width, TileImage.Width, TileImage.Height,
                                            ref settings, results.outRocks, ref numOutRocks);

                Array.Resize<RockDetector.OUTROCK>(ref results.outRocks, numOutRocks);
                if (success == 1 && numOutRocks > 0)
                {                    
                    countByGamma.Add(settings.Gamma, results.GetBlobPixels().Count);
                }
                else
                {
                    countByGamma.Add(settings.Gamma, 0);
                }
            }

            return countByGamma;
        }

        public Dictionary<float, int> SweepShadowPixelsForGamma(float stepSize, string shadowPath)
        {
            Dictionary<float, int> countByGamma = new Dictionary<float, int>();
            if (string.IsNullOrEmpty(TilePath))
                return countByGamma;

            float range = MAX_GAMMA - MIN_GAMMA;
            int steps = (int)Math.Ceiling(range / stepSize);

            Console.WriteLine(string.Format("running rock detector on tile {0} times with gamma {1} to {2} " +
                                            "to compute shadow pixels by gamma",
                                            steps, MIN_GAMMA, MAX_GAMMA));

            for (int idxStep = 1; idxStep < steps; idxStep++) //gamma 0 is too many pixels
            {
                float gamma = MIN_GAMMA + idxStep * stepSize;
                var tmpImage = CreateShadowImage(TilePath, shadowPath, gamma, 0, out int thresholdInGamma, true);
                int shadowPixels = 0;
                for (int idxPixel = 0; idxPixel < tmpImage.Width * tmpImage.Height; idxPixel++)
                {
                    if (tmpImage.DataByBand[0][idxPixel] == 0)
                    {
                        shadowPixels++;
                    }
                }

                countByGamma.Add(gamma, shadowPixels);
            }

            return countByGamma;
        }

        private void UpdateShadowAndOverlay()
        {
            GammaImage = CreateGammaImage(TilePath, GammaPath);
            ShadowImage = CreateShadowImage(TilePath, ShadowPath, Gamma, ThresholdOverride, out thresholdInGamma);
            OverlayImage = CreateOverlayImage(TileImage, ShadowImage,1); //green
        }

        public int GetThresholdInGamma()
        {
            return thresholdInGamma;
        }

        public Image GetTileImage()
        {
            return TileImage;
        }
        public Bitmap GetTileBitmap()
        {
            return TileImage.ToBitmap();
        }

        public Image GetGammaImage()
        {
            return GammaImage;
        }

        public Bitmap GetShadowBitmap()
        {
            return ShadowImage.ToBitmap();
        }

        public Bitmap GetOverlayBitmap()
        {
            return OverlayImage.ToBitmap();
        }

        public void SetGamma(float gamma)
        {
            if (gamma < 0)
                throw new Exception("Invalid gamma");

            if (Gamma != gamma)
            {
                Gamma = gamma;
                UpdateShadowAndOverlay();
            }
        }

        public float GetGamma()
        {
            return Gamma;
        }

        public void SetThresholdOverride(int threshold)
        {
            if (ThresholdOverride != threshold)
            {
                ThresholdOverride = threshold;
                UpdateShadowAndOverlay();
            }
        }

        public int GetThresholdOverride()
        {
            return ThresholdOverride;
        }

        private Image CreateGammaImage(string tilePath, string gammaPath)
        {
            if (!File.Exists(tilePath))
            {
                throw new Exception("No tile image");
            }

            if (File.Exists(gammaPath))
            {
                File.Delete(gammaPath);
            }

            Console.WriteLine("running rock detector on tile to make gamma image");

            if( 0 == RockDetector.gamma_from_files(tilePath, Gamma, gammaPath))
            {
                throw new Exception("failed to make gamma image");
            }

            if (!File.Exists(gammaPath))
            {
                throw new Exception("failed to find gamma image");
            }

            GDALSerializer.LoadMetadata(gammaPath, out int widthPixels, out int heightPixels, out int bands, out Type[] dataTypes);
            Image gammaImage = GDALSerializer.Load(gammaPath, 0, 0, widthPixels, heightPixels);

            if (gammaImage == null)
            {
                throw new Exception("failed to load shadow image");
            }
            return gammaImage;
        }

        private Image CreateShadowImage(string tilePath, string shadowPath, float gamma, int thresholdOverride, out int thresholdInGamma, bool quiet = false)
        {
            if (!File.Exists(tilePath))
            {
                throw new Exception("No tile image");
            }

            if (File.Exists(shadowPath))
            {
                File.Delete(shadowPath);
            }

            if (!quiet)
            {
                Console.WriteLine("running rock detector on tile to make shadow image");
            }

            thresholdInGamma = 0;
            if (0 == RockDetector.threshold_from_files(tilePath, gamma, thresholdOverride, shadowPath, ref thresholdInGamma))
            {
                throw new Exception("failed to make shadow image");
            }

            if (!File.Exists(shadowPath))
            {
                throw new Exception("failed to find shadow image");
            }

            GDALSerializer.LoadMetadata(shadowPath, out int widthPixels, out int heightPixels, out int bands, out Type[] dataTypes);
            Image shadowImage = GDALSerializer.Load(shadowPath, 0, 0, widthPixels, heightPixels);

            if(shadowImage == null)
            {
                throw new Exception("failed to load shadow image");
            }
            return shadowImage;
        }

        static public Image CreateOverlayImage(Image tileImage, Image shadowImage, int overlayBand)
        {
            if (tileImage.Width != shadowImage.Width || tileImage.Height != shadowImage.Height)
                throw new Exception("mismatched image sizes for overlay");

            if (tileImage.Bands != 1 || shadowImage.Bands != 1)
                throw new NotImplementedException("expecting single channel image");

            Image overlayImage = new Image(tileImage.Width, tileImage.Height, 3);

            for(int idx=0; idx < tileImage.Width * tileImage.Height; idx++)
            {
                for (int idxBand = 0; idxBand < 3; idxBand++)
                {
                    if (idxBand != overlayBand)
                    {
                        overlayImage.DataByBand[idxBand][idx] = tileImage.DataByBand[0][idx];
                    }
                    else
                    {
                        if (shadowImage.DataByBand[0][idx] == 255)
                        {
                            overlayImage.DataByBand[idxBand][idx] = tileImage.DataByBand[0][idx];
                        }
                        else if (shadowImage.DataByBand[0][idx] == 0)
                        {
                            overlayImage.DataByBand[idxBand][idx] = 255;
                        }
                        else
                        {
                            throw new Exception("unexpected values in binary shadow image");
                        }
                    }
                }               
            }

            return overlayImage;
        }
    }
}
