using System;
using System.IO;
using System.Windows.Forms;

namespace RockCollect.Stages
{
    public class ChooseImage : Stage
    {
        private string ImagePath;
        private string ShapeFilePath;
        private string ComparisonRocklistPath;
        private float GroundSamplingDistance;
        private float SubSolarAzimuthDegrees;
        private float SolarIncidenceDegrees;

        static public readonly float DATA_VERSION = 0.1f;

        public override float GetDataVersion() { return DATA_VERSION; }

        public override string GetName()
        {
            return "ChooseImage";
        }

        public override UserControl CreateUI()
        {
            return new ChooseImageUI(this);
        }

        public override string GetFinalOutputDirectory()
        {
            return GetFinalOutputDirectory(outData.Data.ContainsKey("IMAGE_PATH") ? outData.Data["IMAGE_PATH"] : null);
        }

        public void SetImagePath(string path)
        {
            ImagePath = path;
        }

        public string GetImagePath()
        {
            return ImagePath;
        }

        public override bool SaveOutput()
        {
            if (!base.SaveOutput()) return false;
            
            if (string.IsNullOrEmpty(ImagePath)) return false;
            
            string dir = GetFinalOutputDirectory(ImagePath);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            this.outData.Data.Add("IMAGE_PATH", ImagePath);
            this.outData.Data.Add("SHAPE_FILE", ShapeFilePath);
            this.outData.Data.Add("COMPARISON_ROCKLIST", ComparisonRocklistPath);
            this.outData.Data.Add("GSD", GroundSamplingDistance.ToString("F5"));
            this.outData.Data.Add("AZIMUTH", SubSolarAzimuthDegrees.ToString("F5"));
            this.outData.Data.Add("INCIDENCE", SolarIncidenceDegrees.ToString("F5"));
            
            if (!WriteOutputJSON()) return false;

            return true;
        }

        public void SetGroundSamplingDistance(float metersPerPixel)
        {
            GroundSamplingDistance = metersPerPixel;
        }

        public float GetGroundSamplingDistance()
        {
            return GroundSamplingDistance;
        }

        public void SetSubSolarAzimuth(float subsolarAzimuthDegrees)
        {
            SubSolarAzimuthDegrees = subsolarAzimuthDegrees;
        }

        public float GetSubSolarAzimuth()
        {
            return SubSolarAzimuthDegrees;
        }

        public void SetSolarIncidence(float solarIncidenceDegrees)
        {
            SolarIncidenceDegrees = solarIncidenceDegrees;
        }

        public float GetSolarIncidence()
        {
            return SolarIncidenceDegrees;
        }

        public void SetComparisonRocklist(string rocklistPath)
        {
            ComparisonRocklistPath = rocklistPath;
        }

        public string GetComparisonRocklist()
        {
            return ComparisonRocklistPath;
        }

        public void SetShapeFile(string path)
        {
            ShapeFilePath = path;
        }

        public override bool Deactivate(bool forward)
        {
            string msg = null;
            if (string.IsNullOrEmpty(ImagePath))
            {
                msg = "No image file selected.";
            }
            else if (!File.Exists(ImagePath))
            {
                msg = string.Format("Image \"{0}\" not found.", ImagePath); 
            }
            else if (!string.IsNullOrEmpty(ShapeFilePath) && !File.Exists(ShapeFilePath))
            {
                msg = string.Format("Shape file \"{0}\" not found.", ShapeFilePath); 
            }
            else if (!string.IsNullOrEmpty(ComparisonRocklistPath) && !File.Exists(ComparisonRocklistPath))
            {
                msg = string.Format("Comparison rock list \"{0}\" not found.", ComparisonRocklistPath); 
            }
            else if (GroundSamplingDistance < RockDetector.MIN_VALID_GSD ||
                     GroundSamplingDistance > RockDetector.MAX_VALID_GSD)
            {
                msg = string.Format("Ground sampling distance {0} not in range {1} to {2}.", GroundSamplingDistance,
                                    RockDetector.MIN_VALID_GSD, RockDetector.MAX_VALID_GSD);
            }
            else if (SubSolarAzimuthDegrees < RockDetector.MIN_VALID_AZIMUTH ||
                     SubSolarAzimuthDegrees > RockDetector.MAX_VALID_AZIMUTH)
            {
                msg = string.Format("Sub-solar azimuth angle {0} not in range {1} to {2}.", SubSolarAzimuthDegrees,
                                    RockDetector.MIN_VALID_AZIMUTH, RockDetector.MAX_VALID_AZIMUTH);
            }
            else if (SolarIncidenceDegrees < RockDetector.MIN_VALID_INCIDENCE ||
                     SolarIncidenceDegrees > RockDetector.MAX_VALID_INCIDENCE)
            {
                msg = string.Format("Solar incidence angle {0} not in range {1} to {2}.", SolarIncidenceDegrees,
                                    RockDetector.MIN_VALID_INCIDENCE, RockDetector.MAX_VALID_INCIDENCE);
            }

            if (msg != null)
            {
                MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return base.Deactivate(forward);
        }
    }
}
