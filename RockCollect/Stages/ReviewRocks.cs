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
    public class ReviewRocks : Stage
    {
        static public readonly float DATA_VERSION = 0.1f;
        public const int TILESIZE = 500;

        string TilePath;
        Image TileImage;
        Bitmap DetectionsBitmap;
        RockDetector.Settings settings;

        Rocklist comparisonRocklist;
        RockDetector.DetectionResults comparisonDetections;
        Bitmap ComparisonDetectionsBitmap;

        int TileIndex;
        int NumTilesHorizontal;
        int NumTilesVertical;

        public override void Init(Logger log, string stageDirectory, string finalOutputDirectory)
        {
            base.Init(log, stageDirectory, finalOutputDirectory);

            settings = new RockDetector.Settings();
            settings.Confidence = RockDetector.DEFAULT_CONFIDENCE;
        }

        public override UserControl CreateUI()
        {
            return new ReviewRocksUI(this);
        }

        public override UserControl CreateStatusUI(UserControl mainUI)
        {
            return new ReviewRocksStatusUI(mainUI);
        }

        public override string GetName()
        {
            return "ReviewRocks";
        }

        public override float GetDataVersion() { return DATA_VERSION; }

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

            settings.Version = float.Parse(inData.Data["SETTINGS_VERSION"]);
            settings.Gamma = float.Parse(inData.Data["GAMMA"]);
            settings.GSD = float.Parse(inData.Data["GSD"]);
            settings.Azimuth = float.Parse(inData.Data["AZIMUTH"]);
            settings.Incidence = float.Parse(inData.Data["INCIDENCE"]);
            settings.MinShadowArea = float.Parse(inData.Data["MINSHADOWAREA"]);
            settings.MaxShadowArea = float.Parse(inData.Data["MAXSHADOWAREA"]);
            settings.ShadowAspect = float.Parse(inData.Data["SHADOWASPECT"]);
            settings.MeanGradient = float.Parse(inData.Data["MEANGRADIENT"]);
            settings.MinShadowSplit = float.Parse(inData.Data["MINSHADOWSPLIT"]);
            settings.GammaThresholdOverride = int.Parse(inData.Data["GAMMA_THRESHOLD_OVERRIDE"]);

            UpdateDetections(out RockDetector.DetectionResults results, out DetectionsBitmap);

            TileIndex = int.Parse(inData.Data["TILE_INDEX"]);
            NumTilesHorizontal = int.Parse(inData.Data["TILES_HORIZONTAL"]);
            NumTilesVertical = int.Parse(inData.Data["TILES_VERTICAL"]);
            try
            {
                string rocklist_path = inData.Data["COMPARISON_ROCKLIST"];
                if (!string.IsNullOrEmpty(rocklist_path))
                {
                    comparisonRocklist = new Rocklist(rocklist_path);
                    comparisonDetections = CreateDetectionResultsFromRocklist("Theirs",comparisonRocklist);
                    ComparisonDetectionsBitmap = UpdateDetectionsBitmap(comparisonDetections.outRocks);
                }
            }
            catch
            {
            }

            return true;
        }

        internal Bitmap GetIdenticalDetectionsImage()
        {
            return UpdateDetectionsBitmap(GetMatchedIdenticalRocks()?.outRocks);
        }

        internal Bitmap GetOnlyTheirsDetectionsImage()
        {
            return UpdateDetectionsBitmap(GetOrphanRocksTheirs()?.outRocks);
        }

        internal Bitmap GetBothDifferentDetectionsImage()
        {
            return UpdateDetectionsBitmap(GetYourMatchedDifferentRocks()?.outRocks);
        }

        internal Bitmap GetOnlyYoursDetectionsImage()
        {
            return UpdateDetectionsBitmap(GetOrphanRocksYours()?.outRocks);
        }

        public RockDetector.DetectionResults GetComparisonDetections()
        {
            return comparisonDetections;
        }

        private RockDetector.DetectionResults CreateDetectionResultsFromRocklist(string name, Rocklist comparisonRocklist)
        {
            RockDetector.DetectionResults results = new RockDetector.DetectionResults(name, TileImage.Width, TileImage.Height);

            int curTileR = TileIndex / NumTilesHorizontal;
            int curTileC = TileIndex % NumTilesHorizontal;

            //convert all pixel locations to local and filter
            int outRockIdx = 0;
            foreach (var inRock in comparisonRocklist.rocksById)
            {
                if (curTileR != inRock.Value.TileR || curTileC != inRock.Value.TileC)
                    continue;

                results.outRocks[outRockIdx].id = inRock.Key;

                results.outRocks[outRockIdx].rockX = inRock.Value.RockX - TILESIZE * curTileC;
                results.outRocks[outRockIdx].rockY = inRock.Value.RockY - TILESIZE * curTileR;
                results.outRocks[outRockIdx].rockWidth = inRock.Value.RockWidth;
                results.outRocks[outRockIdx].score = inRock.Value.Score;

                //dont need yet
                //results.outRocks[outRockIdx].tileR;
                //results.outRocks[outRockIdx].tileC;
                //results.outRocks[outRockIdx].shaX;
                //results.outRocks[outRockIdx].shaY;
                //results.outRocks[outRockIdx].tileShaX;
                //results.outRocks[outRockIdx].tileShaY;
                //results.outRocks[outRockIdx].shaArea;
                //results.outRocks[outRockIdx].shaLen;
                //results.outRocks[outRockIdx].gradMean;
                //results.outRocks[outRockIdx].compact;
                //results.outRocks[outRockIdx].extent;
                //results.outRocks[outRockIdx].shaEllipseMajor;
                //results.outRocks[outRockIdx].shaEllipseMinor;
                //results.outRocks[outRockIdx].shaEllipseTheta;
                //results.outRocks[outRockIdx].rockHeight;

                outRockIdx++;
            }

            //TODO: run fresh detect with settings and buildout blob image?

            results.outRocks = results.outRocks.Take(outRockIdx).ToArray();

            return results;
        }

        public void GetTileWidthHeight(out int tileWidth, out int tileHeight)
        {
            tileWidth = TileImage.Width;
            tileHeight = TileImage.Height;
        }

        public Bitmap GetTileImage()
        {
            return TileImage.ToBitmap();
        }

        public Bitmap GetDetectionsImage()
        {
            return DetectionsBitmap.Clone(new Rectangle(0, 0, DetectionsBitmap.Width, DetectionsBitmap.Height), DetectionsBitmap.PixelFormat);
        }

        public Bitmap GetComparisonDetectionsImage()
        {
            return ComparisonDetectionsBitmap.Clone(new Rectangle(0, 0, ComparisonDetectionsBitmap.Width, ComparisonDetectionsBitmap.Height), ComparisonDetectionsBitmap.PixelFormat);
        }

        public RockDetector.DetectionResults GetOrphanRocksYours()
        {
            UpdateDetections(out RockDetector.DetectionResults yours, out Bitmap dummy);
            dummy.Dispose();
            return GetOrphanRocks(yours, comparisonDetections);
        }

        public RockDetector.DetectionResults GetOrphanRocksTheirs()
        {
            UpdateDetections(out RockDetector.DetectionResults yours, out Bitmap dummy);
            dummy.Dispose();
            return GetOrphanRocks(comparisonDetections, yours);
        }

        RockDetector.DetectionResults GetOrphanRocks(RockDetector.DetectionResults r0, RockDetector.DetectionResults r1)
        {
            if (r0.outRocks == null)
                return null;

            if (r1.outRocks == null)
                return r0;

            var unmatched = r0.outRocks.Where(rock0 => !r1.outRocks.Where(rock1 => MatchPixelLocation(ref rock0, ref rock1, rock0.rockWidth)).Any());
            if (unmatched.Any())
            {
                RockDetector.DetectionResults orphans = new RockDetector.DetectionResults(r0.Name, TileImage.Width, TileImage.Height);
                orphans.outRocks = unmatched.ToArray();
                return orphans;
            }

            return null;
        }
        public RockDetector.DetectionResults GetMatchedIdenticalRocks()
        {
            UpdateDetections(out RockDetector.DetectionResults yours, out Bitmap dummy);
            dummy.Dispose();
            return GetIdenticalRocks(yours,comparisonDetections);
        }

        RockDetector.DetectionResults GetIdenticalRocks(RockDetector.DetectionResults r0, RockDetector.DetectionResults r1)
        {
            if (r0.outRocks == null)
                return null;

            if (r1.outRocks == null)
                return r0;

            var matched = r0.outRocks.Where(rock0 => r1.outRocks.Where(rock1 => MatchPartialRock(ref rock0, ref rock1)).Any());
            if (matched.Any())
            {
                RockDetector.DetectionResults identical = new RockDetector.DetectionResults(r0.Name, TileImage.Width, TileImage.Height);
                identical.outRocks = matched.ToArray();
                return identical;
            }

            return null;
        }

        static private bool MatchPixelLocation(ref RockDetector.OUTROCK r0, ref RockDetector.OUTROCK r1, float distance)
        {
            double distPixels = Math.Sqrt(Math.Pow(r0.rockX - r1.rockX, 2) + Math.Pow(r0.rockY - r1.rockY, 2));
            return distPixels < distance;
        }

        RockDetector.DetectionResults MatchDifferentRocks(RockDetector.DetectionResults r0, RockDetector.DetectionResults r1)
        {
            if (r0.outRocks == null)
                return null;

            if (r1.outRocks == null)
                return r0;

            var matched = r0.outRocks.Where(rock0 => r1.outRocks.Where(rock1 => MatchPixelLocation(ref rock0, ref rock1, rock0.rockWidth)).Any());

            if (matched.Any())
            {
                RockDetector.DetectionResults different = new RockDetector.DetectionResults(r0.Name, TileImage.Width, TileImage.Height);
                different.outRocks = matched.ToArray();
                return different;
            }

            return null;
        }

        public RockDetector.DetectionResults GetYourMatchedDifferentRocks()
        {
            UpdateDetections(out RockDetector.DetectionResults yours, out Bitmap dummy);
            dummy.Dispose();
            return MatchDifferentRocks(yours, comparisonDetections);
        }

        static private bool MatchPartialRock(ref RockDetector.OUTROCK r0, ref RockDetector.OUTROCK r1)
        {
            bool match = true;

            match &= r0.rockX == r1.rockX;
            match &= r0.rockY == r1.rockY;
            match &= r0.rockWidth == r1.rockWidth;
            match &= r0.rockHeight == r1.rockHeight;

            return match;
        }

        public bool GetMatchingRock(RockDetector.OUTROCK rock0, RockDetector.DetectionResults r1, out RockDetector.OUTROCK outRock)
        {
            outRock = new RockDetector.OUTROCK();

            if (r1 == null)
                return false;

            for (int idxR1 = 0; idxR1 < r1.outRocks.Length; idxR1++)
            {
                if (MatchPixelLocation(ref rock0, ref r1.outRocks[idxR1], rock0.rockWidth))
                {
                    outRock = r1.outRocks[idxR1];
                    return true;
                }
            }

            return false;
        }

        public void GetDeltas(RockDetector.DetectionResults r0, RockDetector.DetectionResults r1, out float[] deltaPos, out float[] deltaWidth)
        {
            deltaPos = new float[] { };
            deltaWidth = new float[] { };

            if (r0?.outRocks == null || r1?.outRocks == null)
                return;

            deltaPos = new float[r0.outRocks.Length];
            deltaWidth = new float[r0.outRocks.Length];

            for (int idxR0=0; idxR0 < r0.outRocks.Length; idxR0++)
            {
                deltaPos[idxR0] = 0;
                deltaWidth[idxR0] = 0;

                for(int idxR1=0; idxR1 < r1.outRocks.Length; idxR1++)
                {
                    if(MatchPixelLocation(ref r0.outRocks[idxR0], ref r1.outRocks[idxR1], r0.outRocks[idxR0].rockWidth))
                    {
                        deltaPos[idxR0] = (float)Math.Sqrt(Math.Pow(r0.outRocks[idxR0].rockX - r1.outRocks[idxR1].rockX, 2) + Math.Pow(r0.outRocks[idxR0].rockY - r1.outRocks[idxR1].rockY, 2));
                        deltaWidth[idxR0] = r0.outRocks[idxR0].rockWidth - r1.outRocks[idxR1].rockWidth;
                        break;
                    }
                }
            }
        }

        private bool UpdateDetections(out RockDetector.DetectionResults results, out Bitmap detectionsBitmap)
        {
            results = null;
            detectionsBitmap = null;

            if (string.IsNullOrEmpty(TilePath))
                return false;

            results = new RockDetector.DetectionResults("Yours", TileImage.Width, TileImage.Height);


            int numOutRocks = 0;
            RockDetector.INSETTINGS inSettings = RockDetector.CreateInSettings(settings);
            byte[] detectImage = new byte[TileImage.Width * TileImage.Height];
            int success = RockDetector.detect_tile_rocks(TilePath, results.outLabelImage, detectImage, 0, 0,
                                        TileImage.Height, TileImage.Width, TileImage.Width, TileImage.Height,
                                        ref inSettings, results.outRocks, ref numOutRocks);

            if (success == 1 && numOutRocks > 0)
            {
                results.outRocks = results.outRocks.Take(numOutRocks).ToArray();
            }
            else
            {
                results.outRocks = null;
            }

            detectionsBitmap = UpdateDetectionsBitmap(results.outRocks);

            return success == 1;
        }

        private Bitmap UpdateDetectionsBitmap(RockDetector.OUTROCK[] rocks)
        {
            Image detectionsImage = new Image(TileImage.Width, TileImage.Height, 3);

            for (int idxPixel = 0; idxPixel < TileImage.Width * TileImage.Height; idxPixel++)
            {
                detectionsImage.DataByBand[0][idxPixel] = TileImage.DataByBand[0][idxPixel];
                detectionsImage.DataByBand[1][idxPixel] = TileImage.DataByBand[0][idxPixel];
                detectionsImage.DataByBand[2][idxPixel] = TileImage.DataByBand[0][idxPixel];
            }

            Bitmap detectionsBitmap = detectionsImage.ToBitmap();

            if (rocks != null)
            {
                using (Graphics grf = Graphics.FromImage(detectionsBitmap))
                {
                    //ellipse
                    using (Pen brush = new Pen(Color.OrangeRed))
                    {
                        foreach (var rock in rocks)
                        {
                            float upperLeftX = rock.rockX - rock.rockWidth / 2.0f;
                            float upperLeftY = rock.rockY - rock.rockWidth / 2.0f;
                            grf.DrawEllipse(brush, upperLeftX, upperLeftY, rock.rockWidth, rock.rockWidth);
                        }
                    }
                }
            }

            return detectionsBitmap;
        }

        public float GetConfidence()
        {
            return settings.Confidence;
        }

        public float GetGroundSamplingDistance()
        {
            return settings.GSD;
        }

        public void SetConfidence(float confidence, out RockDetector.DetectionResults results)
        {
            // minShadowAreaPixelsSq = Math.Min(MAX_VALID_MIN_SHADOW_AREA, Math.Max(MIN_VALID_MIN_SHADOW_AREA, minShadowAreaPixelsSq)); //valid

            // if (minShadowAreaPixelsSq != MinShadowArea)
            {
                settings.Confidence = confidence;
                UpdateDetections(out results, out DetectionsBitmap);
            }
        }

        static public string GetTileOutputName(int tileIndex, int numTilesHorizontal)
        {
            TileSelect.GetTileAddress(tileIndex, numTilesHorizontal, out int tileCol, out int tileRow);
            return string.Format("Tile_{0}_{1}", tileCol.ToString("D6"), tileRow.ToString("D6"));
        }

        public string GetOutRockslistPath()
        {
            return Path.Combine(GetDirectory(Dir.Output),
                                GetTileOutputName(TileIndex, NumTilesHorizontal) + "_Rocks.txt");
        }

        public string GetOutRocksParamsPath()
        {
            return Path.Combine(GetDirectory(Dir.Output),
                                GetTileOutputName(TileIndex, NumTilesHorizontal) + "_Params.txt");
        }

        public override string GetOutputJSONPath()
        {
            return Path.Combine(GetDirectory(Dir.Output),
                                GetTileOutputName(TileIndex, NumTilesHorizontal) + ".json");
        }

        public override bool SaveOutput()
        {
            //TODO: save previous

            string outRocksListPath = GetOutRockslistPath();
            if (File.Exists(outRocksListPath))
            {
                File.Delete(outRocksListPath);
            }

            string outJSONPath = GetOutputJSONPath();
            if (File.Exists(outJSONPath))
            {
                File.Delete(outJSONPath);
            }

            string outParamsPath = GetOutRocksParamsPath();
            if (File.Exists(outParamsPath))
            {
                File.Delete(outParamsPath);
            }

            if (base.SaveOutput())
            {
                settings.Write(this.outData.Data);

                InputToOutput("IMAGE_PATH");
                InputToOutput("TILE_PATH");
                InputToOutput("TILE_INDEX");
                InputToOutput("TILE_COL");
                InputToOutput("TILE_ROW");
                InputToOutput("TILE_GROUP");
                InputToOutput("TILES_HORIZONTAL");
                InputToOutput("TILES_VERTICAL");
                InputToOutput("COMPARISON_ROCKLIST");

                if (!WriteOutputJSON())
                    return false;

                RockDetector.INSETTINGS inSettings = RockDetector.CreateInSettings(settings);
                if (0 == RockDetector.write_param_file(outParamsPath, ref inSettings))
                {
                    return false;
                }

                if(0 == RockDetector.detect_from_files(TilePath, outParamsPath, outRocksListPath))
                {
                    return false;
                }

                return true;
            }
            return false;
        }
    }
}
