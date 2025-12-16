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

        public void SetNewImage(string path)
        {
            ImagePath = path;
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
        public void SetSubSolarAzimuth(float subsolarAzimuthDegrees)
        {
            SubSolarAzimuthDegrees = subsolarAzimuthDegrees;
        }

        public void SetSolarIncidence(float solarIncidenceDegrees)
        {
            SolarIncidenceDegrees = solarIncidenceDegrees;
        }

        internal void SetComparisonRocklist(string rocklistPath)
        {
            ComparisonRocklistPath = rocklistPath;
        }

        public void SetShapeFile(string path)
        {
            ShapeFilePath = path;
        }
    }
}
