using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RockCollect.Stages
{
    public partial class ImageThresholdStatusUI : UserControl
    {
        ImageThresholdUI ThresholdUI;

        public ImageThresholdStatusUI(UserControl thresholdUI)
        {
            InitializeComponent();
            this.chartSourceHisto.ChartAreas[0].AxisX.Interval = 10;
            this.chartGamma.ChartAreas[0].AxisX.Interval = 10;
            ThresholdUI = (ImageThresholdUI)thresholdUI;

            this.chartPixelsVGamma.ChartAreas[0].AxisX.Interval = 1;
            this.chartShadowBlobVGamma.ChartAreas[0].AxisX.Interval = 1;
        }

        public void UpdateSourceData(Image image)
        {
            UpdateHistogram(image, this.chartSourceHisto, true);
            UpdateGammaSweeps();
        }

        public void UpdateGammaSweeps()
        {
            //update gamma sweep charts
            chartPixelsVGamma.Series[0].Points.Clear();
            Dictionary<float, int> pixelCounts = this.ThresholdUI.Stage.SweepShadowPixelsForGamma(0.25f, "gammatemp.png"); //todo: path
            foreach (var entry in pixelCounts)
            {
                chartPixelsVGamma.Series[0].Points.AddXY(entry.Key,entry.Value);
            }

            chartShadowBlobVGamma.Series[0].Points.Clear();
            Dictionary<float, int> blobCounts = this.ThresholdUI.Stage.SweepShadowBlobsForGamma(0.25f,  "gammatemp.png"); //todo: path
            foreach (var entry in blobCounts)
            {
                this.chartShadowBlobVGamma.Series[0].Points.AddXY(entry.Key, entry.Value);
            }
        }       

        public void UpdateGammaHistogram(Image image)
        {
            UpdateHistogram(image, this.chartGamma, false);
            this.labelGammaThreshold.Text = "Threshold chosen: " + ThresholdUI.Stage.GetThresholdInGamma().ToString();
        }

        private void UpdateHistogram(Image image, System.Windows.Forms.DataVisualization.Charting.Chart chart, bool includeWhite)
        {
            if (image.Bands > 1)
                throw new NotImplementedException("only doing grayscale histograms");

            chart.Series[0].Points.Clear();
            int[] countColor = new int[256];
            for (int idx = 0; idx < image.Width * image.Height; idx++)
            {
                int lum = image.DataByBand[0][idx];
                countColor[lum] = countColor[lum] + 1;
            }

            int maxValue = 0;
            int iterMax = includeWhite ? 256 : 255;
            for (int idx = 0; idx < iterMax; idx++)
            {
                maxValue = Math.Max(maxValue, countColor[idx]);
            }

            int numPixels = image.Width * image.Height;
            for (int idx = 0; idx < iterMax; idx++)
            {
                //normalize
                int ptIdx = chart.Series[0].Points.AddXY(idx.ToString(), (countColor[idx] / (float)maxValue) * 100);
            }
        }
    }
}
