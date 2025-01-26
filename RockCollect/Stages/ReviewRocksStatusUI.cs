using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace RockCollect.Stages
{
    public partial class ReviewRocksStatusUI : UserControl
    {
        ReviewRocksUI reviewRocksUI;
        Bitmap sourceBitmap;
        RockDetector.DetectionResults ActiveResults;
        RockDetector.DetectionResults PassiveResults;

        private readonly int SelectedWindowPixels = 75;
        private readonly int SelectedWindowZoom = 4;

        private readonly int curveResolution = 200;
        float curveMinDiameter = 0.001f;

        private readonly int minNumCFARocks = 5;
        int idxBestFitSeries;
        int idxRocksSeries;
        int idxTooBigRocksSeries;
        int idxCFARocksSeries;

        public ReviewRocksStatusUI(UserControl reviewRocksMainUI)
        {
            InitializeComponent();
            reviewRocksUI = (ReviewRocksUI)reviewRocksMainUI;
            reviewRocksUI.Stage.OnTeardownStatusUI = () => {
                if (this.pictureBoxSelectedRock.Image != null)
                {
                    //attempt to avoid intermittent System.ArgumentException: Parameter is not valid.
                    this.pictureBoxSelectedRock.Image.Dispose();
                    this.pictureBoxSelectedRock.Image = null;
                }
            };

            float[] rockCoveragePct = new float[] { 0.02f, 0.05f, 0.1f, 0.2f, 0.3f, 0.4f };

            for (int idxSeries=0; idxSeries < rockCoveragePct.Length; idxSeries++)
            {
                PlotCFACurve(curveMinDiameter, curveResolution, idxSeries, rockCoveragePct[idxSeries]);
            }

            idxBestFitSeries = rockCoveragePct.Length;
            idxRocksSeries = idxBestFitSeries + 1;
            idxTooBigRocksSeries = idxRocksSeries + 1;
            idxCFARocksSeries = idxTooBigRocksSeries + 1;
        }

        private void PlotCFACurve(float minDiameter, int curveResolution, int idxSeries, double totalRockCoverage)
        {
            double maxDiameter = FindCFAZeroCrossing(minDiameter, totalRockCoverage);

            double[] diameters = new double[curveResolution];
            for (int idx = 0; idx < curveResolution; idx++)
            {
                double pct = idx / (curveResolution - 1.0);
                diameters[idx] = minDiameter + pct * (maxDiameter - minDiameter);
            }

            for (int idxPoint = 0; idxPoint < diameters.Length; idxPoint++)
            {
                double y = RockDetector.DetectionResults.GetCFAValue(totalRockCoverage, diameters[idxPoint]);
                this.chartCFA.Series[ idxSeries].Points.AddXY(diameters[idxPoint], y);
            }
        }

        private static double FindCFAZeroCrossing(float minDiameter, double k)
        { 
            //find max diameter (0 crossing)
            double maxDiameter = 0;
            double searchMin = minDiameter;
            double range = 10;
            double interations = 100;
            for (int idx = 0; idx < interations; idx++)
            {
                range = range * 0.5;
                double curDiameter = searchMin + range;
                double cfa = RockDetector.DetectionResults.GetCFAValue(k, curDiameter);

                if (cfa > 1E-7)
                {
                    searchMin = curDiameter;
                }
            }

            maxDiameter = searchMin + range;
            return maxDiameter;
        }

        float SrcToCrop(float loc, float cropUpperLeft, float zoom)
        {
            return (loc - cropUpperLeft) * zoom; //BUGBUG: look for off by one
        }

        public static Bitmap ResizeImage(Bitmap srcBitmap, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destBitmap = new Bitmap(width, height);

            destBitmap.SetResolution(srcBitmap.HorizontalResolution, srcBitmap.VerticalResolution);

            using (var graphics = Graphics.FromImage(destBitmap))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(srcBitmap, destRect, 0, 0, srcBitmap.Width, srcBitmap.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destBitmap;
        }

        public void UpdateDataGrid(RockDetector.DetectionResults results, RockDetector.DetectionResults passiveResults)
        {
            lock (this.dataGridView1)
            {
                ActiveResults = results;
                PassiveResults = passiveResults;
                //sourceBitmap = null;

                this.dataGridView1.Rows.Clear();
                this.chartCFA.Series[idxBestFitSeries].Points.Clear(); //best fit
                this.chartCFA.Series[idxRocksSeries].Points.Clear(); //too small
                this.chartCFA.Series[idxTooBigRocksSeries].Points.Clear(); //too big rocks
                this.chartCFA.Series[idxCFARocksSeries].Points.Clear(); //cfa rocks

                this.LabelRocks.Text = "Rocks";
                this.labelColorKey.Text = "";

                if (results?.outRocks == null)
                    return;

                this.LabelRocks.Text = "Rocks" + ActiveResults.Name;

                string colorKey = "Green: " + ActiveResults.Name;
                if (PassiveResults != null)
                {
                    colorKey += " Blue: " + PassiveResults.Name;
                }
                this.labelColorKey.Text = colorKey;

                this.reviewRocksUI.Stage.GetDeltas(results, passiveResults, out float[] deltaPos, out float[] deltaWidth);

                for (int idx = 0; idx < results.outRocks.Length; idx++)
                {
                    this.dataGridView1.Rows.Add(new object[] { results.outRocks[idx].id,
                        results.outRocks[idx].rockWidth * this.reviewRocksUI.Stage.GetGroundSamplingDistance(),
                        results.outRocks[idx].score,
                        deltaPos.Length > 0 ? deltaPos[idx] * this.reviewRocksUI.Stage.GetGroundSamplingDistance() : 0,  
                        deltaWidth.Length > 0 ? deltaWidth[idx] * this.reviewRocksUI.Stage.GetGroundSamplingDistance() : 0});
                }

                //update rocks on cfa chart
                var sortedRocks = results.outRocks.OrderByDescending(r => r.rockWidth);
                double areaM2 = 0;
                double GSD = this.reviewRocksUI.Stage.GetGroundSamplingDistance();
                this.reviewRocksUI.Stage.GetTileWidthHeight(out int tileWidth, out int tileHeight);
                double tileAreaM2 = (tileWidth * GSD) * (tileHeight * GSD);
                int cfaRocks = 0;
                foreach(var rock in sortedRocks)
                {
                    double rockWidthM = rock.rockWidth * GSD;
                    areaM2 += Math.PI * Math.Pow(rockWidthM * 0.5, 2);

                    if (rockWidthM > RockDetector.DetectionResults.maxCFAM)
                    {
                        this.chartCFA.Series[idxTooBigRocksSeries].Points.AddXY(rockWidthM, areaM2 / tileAreaM2);
                    }
                    else if (rockWidthM >= RockDetector.DetectionResults.minCFAM && rockWidthM <= RockDetector.DetectionResults.maxCFAM)
                    {
                        this.chartCFA.Series[idxCFARocksSeries].Points.AddXY(rockWidthM, areaM2 / tileAreaM2);
                        cfaRocks++;
                    }
                    else
                    {
                        this.chartCFA.Series[idxRocksSeries].Points.AddXY(rockWidthM, areaM2 / tileAreaM2);
                    }
                }

                //update best fit line
                if (cfaRocks >= minNumCFARocks)
                {
                    double coverage = results.FitCFA(tileAreaM2, GSD, out double goodnessOfFitR);
                    this.labelRockCoverage.Text = coverage.ToString("F2");
                    this.labelGoodnessFit.Text = goodnessOfFitR.ToString("F2");
                    PlotCFACurve(curveMinDiameter, curveResolution, idxBestFitSeries, coverage);
                }
                else
                {
                    this.labelRockCoverage.Text = "Too few CFA rocks";
                    this.labelGoodnessFit.Text = "";
                }

                this.labelNumCFARocks.Text = cfaRocks.ToString();

                //if (sourceBitmap != null)
                //{
                //    ((IDisposable)sourceBitmap).Dispose();
                //    sourceBitmap = null;
                //}
            }
        }

        private void dataGridView1_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1 || e.RowIndex >= ActiveResults.outRocks.Length)
            {
                reviewRocksUI.SetSelectedRockUI(-1, ActiveResults);
                this.pictureBoxSelectedRock.Image = null;
            }
            else
            {
                bool compareCell = e.ColumnIndex == 3 || e.ColumnIndex == 4;

                int label = (int)this.dataGridView1.Rows[e.RowIndex].Cells[0].Value;
                reviewRocksUI.SetSelectedRockUI(label, ActiveResults);
                var rock = ActiveResults.outRocks.Where(x => x.id == label).First();

                int desiredSize = SelectedWindowPixels;

                int distToLeft = (int)rock.rockX;
                int distToRight = (ActiveResults.Width - (int)rock.rockX - 1);
                bool clipLeft = distToLeft < desiredSize;
                bool clipRight = distToRight < desiredSize;

                int upperLeftX = 0;
                if (clipLeft)
                {
                    upperLeftX = 0;
                }
                else if (clipRight)
                {
                    upperLeftX = ActiveResults.Width - desiredSize - 1;
                }
                else
                {
                    upperLeftX = (int)(rock.rockX - desiredSize / 2);
                }

                int distToTop = (int)rock.rockY;
                int distToBottom = (ActiveResults.Height - (int)rock.rockY - 1);
                bool clipTop = distToTop < desiredSize;
                bool clipBottom = distToBottom < desiredSize;

                int upperLeftY = 0;
                if (clipTop)
                {
                    upperLeftY = 0;
                }
                else if (clipBottom)
                {
                    upperLeftY = ActiveResults.Height - desiredSize - 1;
                }
                else
                {
                    upperLeftY = (int)(rock.rockY - desiredSize / 2);
                }

                Rectangle rect = new Rectangle(upperLeftX, upperLeftY, desiredSize, desiredSize);

                if (sourceBitmap == null)
                {
                    sourceBitmap = reviewRocksUI.Stage.GetTileImage();
                }

                using (Bitmap srcBitmapClone = sourceBitmap.Clone(rect, sourceBitmap.PixelFormat))
                {
                    Bitmap selectedBitmap = ResizeImage(srcBitmapClone, SelectedWindowPixels * SelectedWindowZoom, SelectedWindowPixels * SelectedWindowZoom);
                    using (Graphics grf = Graphics.FromImage(selectedBitmap))
                    {
                        if (compareCell && reviewRocksUI.Stage.GetMatchingRock(rock, PassiveResults, out RockDetector.OUTROCK passiveRock))
                        {
                            using (Pen brush = new Pen(Color.Blue))
                            {
                                float centroidXInCrop = SrcToCrop(passiveRock.rockX, upperLeftX, SelectedWindowZoom);
                                float centroidYInCrop = SrcToCrop(passiveRock.rockY, upperLeftY, SelectedWindowZoom);
                                float widthInCrop = passiveRock.rockWidth * SelectedWindowZoom;
                                float upperLeftXInCrop = centroidXInCrop - widthInCrop / 2.0f;
                                float upperLeftYInCrop = centroidYInCrop - widthInCrop / 2.0f;

                                grf.DrawEllipse(brush, upperLeftXInCrop, upperLeftYInCrop, widthInCrop, widthInCrop);
                                grf.DrawEllipse(brush, centroidXInCrop, centroidYInCrop, 1, 1);
                            }
                        }

                        using (Pen brush = new Pen(Color.GreenYellow))
                        {
                            float centroidXInCrop = SrcToCrop(rock.rockX, upperLeftX, SelectedWindowZoom);
                            float centroidYInCrop = SrcToCrop(rock.rockY, upperLeftY, SelectedWindowZoom);
                            float widthInCrop = rock.rockWidth * SelectedWindowZoom;
                            float upperLeftXInCrop = centroidXInCrop - widthInCrop / 2.0f;
                            float upperLeftYInCrop = centroidYInCrop - widthInCrop / 2.0f;

                            grf.DrawEllipse(brush, upperLeftXInCrop, upperLeftYInCrop, widthInCrop, widthInCrop);
                            grf.DrawEllipse(brush, centroidXInCrop, centroidYInCrop, 1, 1);
                        }
                    }
                    this.pictureBoxSelectedRock.Image = selectedBitmap;
                }
            }
        }
    }
}
