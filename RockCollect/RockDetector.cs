using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Optimization;
using MathNet.Numerics.Optimization.ObjectiveFunctions;

namespace RockCollect
{
    public class RockDetector
    {
        static public readonly float SHADOW_SPLIT_RATIO = 0.5f;
        public const int TILESIZE = 550;

        static public readonly float MIN_VALID_MIN_SHADOW_AREA = 3;
        static public readonly float MAX_VALID_MIN_SHADOW_AREA = TILESIZE * 0.25f;
        static public readonly float DISABLE_MIN_SHADOW_AREA = 2;
        static public readonly float DEFAULT_MIN_SHADOW_AREA = MIN_VALID_MIN_SHADOW_AREA;

        static public readonly float MIN_VALID_MAX_SHADOW_AREA = 2;
        static public readonly float MAX_VALID_MAX_SHADOW_AREA = TILESIZE * 4;
        static public readonly float DISABLE_MAX_SHADOW_AREA = (TILESIZE * 0.5f) * (TILESIZE * 0.5f);
        static public readonly float DEFAULT_MAX_SHADOW_AREA = DISABLE_MAX_SHADOW_AREA;

        static public readonly float MIN_VALID_ASPECT = 1;
        static public readonly float MAX_VALID_ASPECT = TILESIZE * 0.01f;
        static public readonly float DISABLE_ASPECT = TILESIZE;
        static public readonly float DEFAULT_ASPECT = DISABLE_ASPECT;

        static public readonly float MIN_VALID_MEAN_GRADIENT = 0.01f;
        static public readonly float MAX_VALID_MEAN_GRADIENT = 50;
        static public readonly float DISABLE_GRADIENT = 0;
        static public readonly float DEFAULT_GRADIENT = DISABLE_GRADIENT;

        static public readonly float MIN_VALID_SPLIT = MIN_VALID_MIN_SHADOW_AREA;
        static public readonly float MAX_VALID_SPLIT = MAX_VALID_MAX_SHADOW_AREA;
        static public readonly float DISABLE_SPLIT = DISABLE_MAX_SHADOW_AREA;
        static public readonly float DEFAULT_SPLIT = DISABLE_SPLIT;

        static public readonly float MIN_VALID_CONFIDENCE = 0.1f;
        static public readonly float MAX_VALID_CONFIDENCE = 1;
        static public readonly float DISABLE_CONFIDENCE = 0;
        static public readonly float DEFAULT_CONFIDENCE = DISABLE_CONFIDENCE;

        static public readonly int DISABLE_GAMMA_THRESH_OVERRIDE = 0;

        [DllImport("RockDetectorShared.dll", CallingConvention = CallingConvention.Cdecl)]
        static public extern int threshold_from_files([MarshalAs(UnmanagedType.LPStr)] string inputImagePath,
            float metGamma,
            int thresholdOverride,
            [MarshalAs(UnmanagedType.LPStr)] string outputImagePath,
            ref int selectedThreshold);

        [DllImport("RockDetectorShared.dll", CallingConvention = CallingConvention.Cdecl)]
        static public extern int gamma_from_files([MarshalAs(UnmanagedType.LPStr)] string inputImagePath,
            float metGamma,
            [MarshalAs(UnmanagedType.LPStr)] string outputImagePath);

        [DllImport("RockDetectorShared.dll", CallingConvention = CallingConvention.Cdecl)]
        static public extern int write_param_file([MarshalAs(UnmanagedType.LPStr)] string outputParamPath,
            ref INSETTINGS settings);

        [DllImport("RockDetectorShared.dll", CallingConvention = CallingConvention.Cdecl)]
        static public extern int detect_from_files([MarshalAs(UnmanagedType.LPStr)] string inputImagePath,
            [MarshalAs(UnmanagedType.LPStr)] string paramsPath, 
            [MarshalAs(UnmanagedType.LPStr)] string outputRockListPath);
        
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct OUTROCK
        {
            public int id;
            public int tileR;
            public int tileC;
            public float shaX;
            public float shaY;
            public float rockX;
            public float rockY;
            public float tileShaX;
            public float tileShaY;
            public int shaArea;
            public float shaLen;
            public float rockWidth;
            public float rockHeight;
            public float score;
            public float gradMean;
            public float compact;
            public float extent;
            public int Class;
            public float gamma;
            public float shaEllipseMajor;
            public float shaEllipseMinor;
            public float shaEllipseTheta;
        }

        //keep in sync with _RD_PARMS
        [StructLayoutAttribute(LayoutKind.Sequential)] 
        public struct INSETTINGS
        {
            public float Gamma;
            public float Incidence;     //sun incidence
            public float Azimuth;       //sun azimuth
            public float MinShadowArea; //pixels^2
            public float GSD;           //meters per pixel
            public float Confidence;    //threshold below which rocks are suppressed
            public float MinShadowSplit; //pixels^2, the minimum area to consider for shadow splitting
            public float SplittingRatio; //0.5
            public float ShadowAspect;  //maximum ratio of shadow ellipse major axis to minor axis
            public float MeanGradient;  //the average
            public float MaxShadowArea; //pixels^2
            public int GammaThresholdOverride; // skip gmet and use this if > 0
        };

        [DllImport("RockDetectorShared.dll", CallingConvention = CallingConvention.Cdecl)]
        static public extern int detect_per_tile_settings([MarshalAs(UnmanagedType.LPStr)] string inputImagePath, [MarshalAs(UnmanagedType.LPStr)] string outputRockListPath, int numTileSettings, [In] INSETTINGS[] inSettings);


        [DllImport("RockDetectorShared.dll", CallingConvention = CallingConvention.Cdecl)]
        static public extern int detect_tile_rocks([MarshalAs(UnmanagedType.LPStr)] string inputImagePath,
                                                [In, Out] ushort[] labelImage, [In, Out] byte[] subimg,
                                                int tileRow, int tileCol, int fullRows, int fullCols,
                                                int numTileRows, int numTileCols, ref INSETTINGS settings,
                                                [In, Out] OUTROCK[] outRocks, ref int numOutRocks);

        [DllImport("RockDetectorShared.dll", CallingConvention = CallingConvention.Cdecl)]
        static public extern int shadows_from_files([MarshalAs(UnmanagedType.LPStr)] string inputThresholdImagePath, 
            [MarshalAs(UnmanagedType.LPStr)] string outputShadowImagePath, ref int outNumShadows);

        static public INSETTINGS CreateInSettings(Settings settings)
        {
            INSETTINGS results = new INSETTINGS();
            results.Gamma = settings.Gamma;
            results.GSD = settings.GSD;
            results.Azimuth = settings.Azimuth;
            results.Incidence = settings.Incidence;
            results.MinShadowArea = settings.MinShadowArea;
            results.MaxShadowArea = settings.MaxShadowArea;
            results.ShadowAspect = settings.ShadowAspect;
            results.MeanGradient = settings.MeanGradient;
            results.MinShadowSplit = settings.MinShadowSplit;
            results.Confidence = settings.Confidence;
            results.SplittingRatio = SHADOW_SPLIT_RATIO;
            results.GammaThresholdOverride = settings.GammaThresholdOverride;
            return results;
        }

        public class Settings
        {
            public float Version = 0.02f;
            public float Gamma;
            public float GSD;
            public float Azimuth;
            public float Incidence;

            public float MinShadowArea; //pixels^2
            public float MaxShadowArea; //pixels^2
            public float ShadowAspect;  //maximum ratio of shadow ellipse major axis to minor axis
            public float MeanGradient;  //the average
            public float MinShadowSplit; //pixels^2, the minimum area to consider for shadow splitting
            public float Confidence;    //threshold below which rocks are suppressed
            public int GammaThresholdOverride; //

            public Settings()
            { }

            public Settings(Dictionary<string, string> strings)
            {
                Populate(strings);
            }

            public void Populate(Dictionary<string, string> strings)
            {
                if (strings.ContainsKey("SETTINGS_VERSION")) Version = float.Parse(strings["SETTINGS_VERSION"]);
                if (strings.ContainsKey("GAMMA")) Gamma = float.Parse(strings["GAMMA"]);
                if (strings.ContainsKey("GSD")) GSD = float.Parse(strings["GSD"]);
                if (strings.ContainsKey("AZIMUTH")) Azimuth = float.Parse(strings["AZIMUTH"]);
                if (strings.ContainsKey("INCIDENCE")) Incidence = float.Parse(strings["INCIDENCE"]);
                if (strings.ContainsKey("MINSHADOWAREA")) MinShadowArea = float.Parse(strings["MINSHADOWAREA"]);
                if (strings.ContainsKey("MAXSHADOWAREA")) MaxShadowArea = float.Parse(strings["MAXSHADOWAREA"]);
                if (strings.ContainsKey("SHADOWASPECT")) ShadowAspect = float.Parse(strings["SHADOWASPECT"]);
                if (strings.ContainsKey("MEANGRADIENT")) MeanGradient = float.Parse(strings["MEANGRADIENT"]);
                if (strings.ContainsKey("MINSHADOWSPLIT")) MinShadowSplit = float.Parse(strings["MINSHADOWSPLIT"]);
                if (strings.ContainsKey("CONFIDENCE")) Confidence = float.Parse(strings["CONFIDENCE"]);
                if (strings.ContainsKey("GAMMA_THRESHOLD_OVERRIDE")) GammaThresholdOverride = int.Parse(strings["GAMMA_THRESHOLD_OVERRIDE"]);

            }
            public void Write(Dictionary<string, string> strings)
            {
                strings.Add("SETTINGS_VERSION", Version.ToString());
                strings.Add("GAMMA", Gamma.ToString());
                strings.Add("GSD", GSD.ToString());
                strings.Add("AZIMUTH", Azimuth.ToString());
                strings.Add("INCIDENCE", Incidence.ToString());
                strings.Add("MINSHADOWAREA", MinShadowArea.ToString()); //pixels^2
                strings.Add("MAXSHADOWAREA", MaxShadowArea.ToString()); //pixels^2
                strings.Add("SHADOWASPECT", ShadowAspect.ToString());  //maximum ratio of shadow ellipse major axis to minor axis
                strings.Add("MEANGRADIENT", MeanGradient.ToString());  //the average
                strings.Add("MINSHADOWSPLIT", MinShadowSplit.ToString()); //pixels^2, the minimum area to consider for shadow splitting
                strings.Add("CONFIDENCE", Confidence.ToString());
                strings.Add("GAMMA_THRESHOLD_OVERRIDE", GammaThresholdOverride.ToString());

            }
        };

        public class DetectionResults
        {
            public string Name;
            static public int outRocksCapacity = 20000; //keep in sync with MAXLAB
            public RockDetector.OUTROCK[] outRocks;
            public ushort[] outLabelImage; //TODO: allocate on demand
            public int Width;
            public int Height;

            private Dictionary<int, List<int>> pixelsByLabel;
            private Dictionary<int, List<int>> perimeterPixelsByLabel;

            public DetectionResults(string name, int width, int height)
            {
                Width = width;
                Height = height;
                Name = name;
                outRocks = new RockDetector.OUTROCK[outRocksCapacity];
                outLabelImage = new ushort[Width * Height];

            }

            public Dictionary<int, List<int>> GetBlobPixels()
            {
                if (pixelsByLabel != null)
                    return pixelsByLabel;

                pixelsByLabel = new Dictionary<int, List<int>>();
                perimeterPixelsByLabel = new Dictionary<int, List<int>>();

                if (outRocks == null)
                    return pixelsByLabel;

                int numOutRocks = outRocks.Length;
                if (numOutRocks == 0)
                    return pixelsByLabel;

                bool[] validLabels = new bool[outRocksCapacity];
                for (int idx = 0; idx < numOutRocks; idx++)
                {
                    validLabels[outRocks[idx].id] = true;
                }

                for (int idx = 0; idx < outLabelImage.Length; idx++)
                {
                    ushort cur = outLabelImage[idx];
                    if (cur == 0)
                        continue;

                    if (validLabels[cur] == false)
                        continue;

                    if (!pixelsByLabel.ContainsKey(cur))
                    {
                        pixelsByLabel.Add(cur, new List<int>());
                        perimeterPixelsByLabel.Add(cur, new List<int>());
                    }
                    pixelsByLabel[cur].Add(idx);

                    int row = idx / Width;
                    if (row > 0)
                    {
                        int upIdx = idx - Width;
                        if (outLabelImage[upIdx] != cur)
                        {
                            perimeterPixelsByLabel[cur].Add(idx);
                            continue;
                        }
                    }

                    int col = idx % Width;
                    if (col != 0)
                    {
                        int leftIdx = idx - 1;
                        if (outLabelImage[leftIdx] != cur)
                        {
                            perimeterPixelsByLabel[cur].Add(idx);
                            continue;
                        }
                    }

                    if (col != Width - 1)
                    {
                        int rightIdx = idx + 1;
                        if (outLabelImage[rightIdx] != cur)
                        {
                            perimeterPixelsByLabel[cur].Add(idx);
                            continue;
                        }
                    }

                    if (row < Height - 1)
                    {
                        int bottomIdx = idx + Width;
                        if (outLabelImage[bottomIdx] != cur)
                        {
                            perimeterPixelsByLabel[cur].Add(idx);
                            continue;
                        }
                    }
                }

                return pixelsByLabel;
            }

            public List<int> GetPixelsForLabel(int label)
            {
                if (pixelsByLabel == null)
                {
                    GetBlobPixels();
                }

                if (!pixelsByLabel.ContainsKey(label))
                {
                    return new List<int>();
                }
                else
                {
                    return pixelsByLabel[label];
                }
            }

            public List<int> GetPerimeterPixelsForLabel(int label)
            {
                if (perimeterPixelsByLabel == null)
                {
                    GetBlobPixels();
                }

                if (!perimeterPixelsByLabel.ContainsKey(label))
                {
                    return new List<int>();
                }

                return perimeterPixelsByLabel[label];
            }

            public double GetAreaOfRocksGreaterOrEqualToDiameter(float diameter)
            {
                if (this.outRocks == null)
                    return 0;

                var filteredRocks = this.outRocks.Where(r => r.rockWidth >= diameter);

                double area = 0;
                foreach (var rock in filteredRocks)
                {
                    area += Math.PI * Math.Pow(rock.rockWidth / 2.0, 2);
                }

                return area;
            }

            static public double GetCFAValue(double percentRockCoverage, double greaterThanOrEqualToDiameter)
            {
                return percentRockCoverage * Math.Exp(-(1.79 + 0.152 / percentRockCoverage) * greaterThanOrEqualToDiameter);
            }

            private double[] GetCumFracAreaByDiameterForRocks(double[] diameters, double tileAreaMetersSq, double GSD)
            {
                var sortedRocks = outRocks.OrderByDescending(r => r.rockWidth);

                double[] cfas = new double[diameters.Length];

                //TODO: more efficient to keep previous results

                for (int idxDiam = diameters.Length - 1; idxDiam >= 0; idxDiam--)
                {
                    double areaM2 = 0;
                    double curDiam = diameters[idxDiam];
                    for (int idxRock = 0; idxRock < outRocks.Length; idxRock++)
                    {
                        double rockDiamPixels = sortedRocks.ElementAt(idxRock).rockWidth;
                        double rockDiamMeters = rockDiamPixels * GSD;
                 
                        if (rockDiamMeters < curDiam)
                            break;

                        areaM2 += Math.PI * Math.Pow(rockDiamMeters * 0.5, 2);
                    }
                    cfas[idxDiam] = areaM2 / tileAreaMetersSq;
                }

                return cfas;
            }

            static private double[] GetDiameters(double minDiameter, double maxDiamater, double bucketSizeMeters)
            {
                double range = maxDiamater - minDiameter;
                int steps = (int)Math.Ceiling(range / bucketSizeMeters);
                double[] diameters = new double[steps];
                for (int idx = 0; idx < steps; idx++)
                {
                    diameters[idx] = minDiameter + bucketSizeMeters * idx;
                }

                return diameters;
            }

            static private double[] GetCumFracAreaByDiameterForCoverage(double[] diameters, double percentRockCoverage)
            {
                double[] cfa = new double[diameters.Length];
                for (int idx = 0; idx < diameters.Length; idx++)
                {
                    cfa[idx] = GetCFAValue(percentRockCoverage, diameters[idx]);
                }

                return cfa;
            }

            static public readonly double minCFAM = 1.5;
            static public readonly double maxCFAM = 2.25;
            public double FitCFA(double tileAreaMeters, double GSD, out double fitGoodness)
            {
                double minDiameter = minCFAM;
                double maxDiameter = maxCFAM;
                double bucketSizeMeters = 0.001;
                double[] diameters = GetDiameters(minDiameter, maxDiameter, bucketSizeMeters);
                double[] detectionsAreaByDiameter = GetCumFracAreaByDiameterForRocks(diameters, tileAreaMeters, GSD);

                double lowerBound = 0.0001;
                double upperBound = 1.0;
                double initialGuess = 0.1;
                Vector<double> upperBoundV = Vector<double>.Build.Dense(new double[] { upperBound });
                Vector<double> lowerBoundV = Vector<double>.Build.Dense(new double[] { lowerBound });

                //var CalcError = new Func<double, double>(p =>
                //{
                //    double totalRockCoverage = p > 0 ? p : 1E-8; //k

                //    double[] cfaAreaByDiameter = GetCumFracAreaByDiameterForCoverage(diameters, totalRockCoverage);

                //    double error = 0;
                //    for (int idx = 0; idx < diameters.Length; idx++)
                //    {
                //        error += Math.Pow(detectionsAreaByDiameter[idx] - cfaAreaByDiameter[idx],2);
                //    }

                //    return error;
                //});

                Func<double, double, double> CalcCFA = (k, D) =>
                {
                    return GetCFAValue(k, D);
                };


                double fitK = MathNet.Numerics.Fit.Curve(diameters, detectionsAreaByDiameter, CalcCFA, initialGuess);
                fitGoodness = MathNet.Numerics.GoodnessOfFit.R(detectionsAreaByDiameter, GetCumFracAreaByDiameterForCoverage(diameters, fitK));
                return Math.Max(1E-8, fitK);
            }
        }
    }
}
