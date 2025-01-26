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
    public class RefineShadows : Stage
    {
        static public readonly float DATA_VERSION = 0.1f;

        Image ShadowImage;
        Image CurrentBlobImage;

        string TilePath;
        Image TileImage;

        RockDetector.Settings settings;

        public override void Init(Logger log, string stageDirectory, string finalOutputDirectory)
        {
            base.Init(log, stageDirectory, finalOutputDirectory);

            settings = new RockDetector.Settings();

            settings.MinShadowArea = RockDetector.DEFAULT_MIN_SHADOW_AREA;
            settings.MaxShadowArea = RockDetector.DEFAULT_MAX_SHADOW_AREA;
            settings.ShadowAspect = RockDetector.DEFAULT_ASPECT;
            settings.MeanGradient = RockDetector.DEFAULT_GRADIENT;
            settings.MinShadowSplit = RockDetector.DEFAULT_SPLIT;
        }

        public override UserControl CreateUI()
        {
            return new RefineShadowsUI(this);
        }

        public override UserControl CreateStatusUI(UserControl mainUI)
        {
            return new RefineShadowsStatusUI(mainUI);
        }

        public override float GetDataVersion() { return DATA_VERSION; }

        public override string GetName()
        {
            return "RefineShadows";
        }

        public override bool LoadInput(string directory)
        {
            bool result = base.LoadInput(directory);
            if (result == false)
                return false;

            //tile image
            if (!this.inData.Data.ContainsKey("TILE_PATH"))
                return false;

            TilePath = inData.Data["TILE_PATH"];
            if (!File.Exists(TilePath))
                throw new Exception(string.Format("Input tile image {0} doesn't exist", TilePath));

            GDALSerializer.LoadMetadata(TilePath, out int widthPixels, out int heightPixels, out int bands, out Type[] dataTypes);

            if (bands != 1)
                throw new NotImplementedException("expecting single band tile image");

            TileImage = GDALSerializer.Load(TilePath, 0, 0, widthPixels, heightPixels);

            if (TileImage == null)
                return false;

            settings.GSD = float.Parse(inData.Data["GSD"]);
            settings.Azimuth = float.Parse(inData.Data["AZIMUTH"]);
            settings.Incidence = float.Parse(inData.Data["INCIDENCE"]);
            settings.Gamma = float.Parse(inData.Data["GAMMA"]);
            settings.GammaThresholdOverride = int.Parse(inData.Data["GAMMA_THRESHOLD_OVERRIDE"]);

            //shadow image
            if (!this.inData.Data.ContainsKey("SHADOW_PATH")) //TODO: copy along input
                return false;

            var shadowImagePath = inData.Data["SHADOW_PATH"];
            if (!File.Exists(shadowImagePath))
                throw new Exception(string.Format("Input shadow tile image {0} doesn't exist", shadowImagePath));

            ShadowImage = GDALSerializer.Load(shadowImagePath, 0, 0, TileImage.Width, TileImage.Height);

            if (ShadowImage == null)
                return false;

            ShadowImage = ImageThreshold.CreateOverlayImage(TileImage, ShadowImage, 0); //red

            UpdateDetections(out RockDetector.DetectionResults results);
            return true;
        }

        private void UpdateCurrentBlobsImage(Dictionary<int, List<int>> lookup)
        {
            if (CurrentBlobImage == null || 
                CurrentBlobImage.Width != TileImage.Width ||
                CurrentBlobImage.Height != TileImage.Height)
            {
                CurrentBlobImage = new Image(TileImage.Width, TileImage.Height, 3);
            }
            for (int idxPixel = 0; idxPixel < TileImage.Width * TileImage.Height; idxPixel++)
            {
                CurrentBlobImage.DataByBand[0][idxPixel] = ShadowImage.DataByBand[0][idxPixel];
                CurrentBlobImage.DataByBand[1][idxPixel] = ShadowImage.DataByBand[1][idxPixel];
                CurrentBlobImage.DataByBand[2][idxPixel] = ShadowImage.DataByBand[2][idxPixel];
            }

            foreach (var blob in lookup)
            {
                foreach (var blobPixel in blob.Value)
                {
                    CurrentBlobImage.DataByBand[0][blobPixel] = ShadowImage.DataByBand[1][blobPixel]; //reset the red amount
                    CurrentBlobImage.DataByBand[2][blobPixel] = 255;                                  //blue
                }
            }
        }
        
        bool UpdateDetections(out RockDetector.DetectionResults results)
        {
            results = null;

            if (string.IsNullOrEmpty(TilePath))
                return false;

            results = new RockDetector.DetectionResults("Shadows", TileImage.Width, TileImage.Height);
                                  
            int numOutRocks = 0;
            RockDetector.INSETTINGS inSettings = RockDetector.CreateInSettings(settings);
            byte[] detectImage = new byte[TileImage.Width * TileImage.Height];
            int success = RockDetector.detect_tile_rocks(TilePath, results.outLabelImage, detectImage, 0, 0,
                                        TileImage.Height, TileImage.Width, TileImage.Height, TileImage.Width,
                                        ref inSettings, results.outRocks, ref numOutRocks);
           
            if (success == 1 && numOutRocks > 0)
            {
                results.outRocks = results.outRocks.Take(numOutRocks).ToArray();
            }
            else
            {
                results.outRocks = null;
            }
            UpdateCurrentBlobsImage(results.GetBlobPixels());
        
            return success == 1;
        }

        public float GetMinShadowArea()
        {
            return settings.MinShadowArea;
        }

        public float GetMaxShadowArea()
        {
            return settings.MaxShadowArea;
        }

        public float GetShadowAspect()
        {
            return settings.ShadowAspect;
        }

        public float GetMeanGradient()
        {
            return settings.MeanGradient;
        }

        public float GetMinShadowSplit()
        {
            return settings.MinShadowSplit;
        }

        public void SetMinShadowArea(float minShadowAreaPixelsSq, out RockDetector.DetectionResults results)
        {
           // minShadowAreaPixelsSq = Math.Min(MAX_VALID_MIN_SHADOW_AREA, Math.Max(MIN_VALID_MIN_SHADOW_AREA, minShadowAreaPixelsSq)); //valid

           // if (minShadowAreaPixelsSq != MinShadowArea)
            {
                settings.MinShadowArea = minShadowAreaPixelsSq;
                UpdateDetections(out results);
            }
        }

        public void SetMaxShadowArea(float maxShadowAreaPixelsSq, out RockDetector.DetectionResults results)
        {
            //maxShadowAreaPixelsSq = Math.Min(MAX_VALID_MAX_SHADOW_AREA, Math.Max(MIN_VALID_MAX_SHADOW_AREA, maxShadowAreaPixelsSq)); //valid

            //if (maxShadowAreaPixelsSq != MaxShadowArea)
            {
                settings.MaxShadowArea = maxShadowAreaPixelsSq;
                UpdateDetections(out results);
            }
        }

        public void SetShadowAspect(float shadowAspect, out RockDetector.DetectionResults results)
        {
            //shadowAspect = Math.Min(MAX_VALID_ASPECT, Math.Max(MIN_VALID_ASPECT, shadowAspect)); //valid

            //if (shadowAspect != ShadowAspect)
            {
                settings.ShadowAspect = shadowAspect;
                UpdateDetections(out results);
            }
        }

        public void SetMeanGradient(float meanGradient, out RockDetector.DetectionResults results)
        {
            //meanGradient = Math.Min(MAX_VALID_MEAN_GRADIENT, Math.Max(MIN_VALID_MEAN_GRADIENT, meanGradient)); //valid

            //if (meanGradient != MeanGradient)
            {
                settings.MeanGradient = meanGradient;
                UpdateDetections(out results);
            }
        }

        public void SetMinShadowSplit(float shadowSplit, out RockDetector.DetectionResults results)
        {
            //shadowSplit = Math.Min(MAX_VALID_SPLIT, Math.Max(MIN_VALID_SPLIT, shadowSplit)); //valid

            //if (shadowSplit != MinShadowSplit)
            {
                settings.MinShadowSplit = shadowSplit;
                UpdateDetections(out results);
            }
        }

        public Bitmap GetTileBitmap()
        {
            return TileImage.ToBitmap();
        }

        public Bitmap GetCurrentBlobsImage()
        {
            return CurrentBlobImage?.ToBitmap();
        }

        public Bitmap GetShadowImage()
        {
            return ShadowImage?.ToBitmap();
        }

        public override bool SaveOutput()
        {
            if (base.SaveOutput())
            {
                InputToOutput("IMAGE_PATH");
                InputToOutput("TILE_PATH");
                InputToOutput("TILE_INDEX");
                InputToOutput("TILE_COL");
                InputToOutput("TILE_ROW");
                InputToOutput("TILE_GROUP");
                InputToOutput("TILES_HORIZONTAL");
                InputToOutput("TILES_VERTICAL");
                InputToOutput("COMPARISON_ROCKLIST");

                settings.Write(this.outData.Data);

                if (!WriteOutputJSON())
                    return false;

                return true;
            }
            return false;
        }
    }
}
