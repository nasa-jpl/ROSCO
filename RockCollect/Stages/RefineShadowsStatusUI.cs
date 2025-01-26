using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;

namespace RockCollect.Stages
{
    public partial class RefineShadowsStatusUI : UserControl
    {
        RefineShadowsUI shadowsUI;
        RockDetector.DetectionResults Results;
        Bitmap sourceBitmap;
        private readonly int SelectedWindowPixels = 75;
        private readonly int SelectedWindowZoom = 4;

        public RefineShadowsStatusUI(UserControl refineShadowsUI)
        {
            InitializeComponent();
            shadowsUI = (RefineShadowsUI)refineShadowsUI;
            shadowsUI.Stage.OnTeardownStatusUI = () => {
                if (this.pictureBoxSelectedShadow.Image != null)
                {
                    //attempt to avoid intermittent System.ArgumentException: Parameter is not valid.
                    this.pictureBoxSelectedShadow.Image.Dispose();
                    this.pictureBoxSelectedShadow.Image = null;
                }
            };
        }

        public void UpdateDataGrid(RockDetector.DetectionResults results)
        {
            lock (this.dataGridView1)
            {
                Results = results;
                this.dataGridView1.Rows.Clear();

                if (results.outRocks == null)
                    return;

                for (int idx=0; idx < results.outRocks.Length; idx++)
                {
                    this.dataGridView1.Rows.Add(new object[] { results.outRocks[idx].id, results.outRocks[idx].shaArea, results.outRocks[idx].extent, results.outRocks[idx].gradMean });
                }
            }

            if(sourceBitmap != null)
            {
                ((IDisposable)sourceBitmap).Dispose();                
                sourceBitmap = null;
            }
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

        float SrcToCrop(float loc, float cropUpperLeft, float zoom)
        {
           return (loc - cropUpperLeft) * zoom; //BUGBUG: look for off by one
        }


        void DrawEllipse(Graphics grf, Pen brush, int cols, int rows, float x0, float y0, float major, float minor, float theta)
        {
            float max = major * minor * 2;
            float step = 2 * (float)Math.PI / max;

            for (int i = 0; i < max; i++)
            {
                float xa0 = major * (float)Math.Cos(i * step);
                float ya0 = minor * (float)Math.Sin(i * step);
                float xb0 = major * (float)Math.Cos(((i + 1) % (max)) * step);
                float yb0 = minor * (float)Math.Sin(((i + 1) % (max)) * step);

                float xa1 = (int)(xa0 * Math.Cos(theta) - ya0 * Math.Sin(theta) + x0 + 0.5);
                float ya1 = (int)(xa0 * Math.Sin(theta) + ya0 * Math.Cos(theta) + y0 + 0.5);
                float xb1 = (int)(xb0 * Math.Cos(theta) - yb0 * Math.Sin(theta) + x0 + 0.5);
                float yb1 = (int)(xb0 * Math.Sin(theta) + yb0 * Math.Cos(theta) + y0 + 0.5);

                if (xa1 > 0 && xa1 < cols && ya1 > 0 && ya1 < rows &&
                    xb1 > 0 && xb1 < cols && yb1 > 0 && yb1 < rows)
                {
                    grf.DrawLine(brush, xa1, ya1, xb1, yb1);
                }
            }
        }

        private void dataGridView1_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1 || e.RowIndex >= Results.outRocks.Length)
            {
                shadowsUI.SetSelectedShadowsUI(-1, Results);

                //if (this.pictureBoxSelectedShadow.Image != null)
                //{
                //    ((IDisposable)this.pictureBoxSelectedShadow.Image).Dispose();
                //    this.pictureBoxSelectedShadow.Image = null;
                //}                
            }
            else
            {
                int label = (int)this.dataGridView1.Rows[e.RowIndex].Cells[0].Value;
                shadowsUI.SetSelectedShadowsUI(label, Results);

                var rock = Results.outRocks.Where(x => x.id == label).First();

                int desiredSize = SelectedWindowPixels;

                int distToLeft = (int)rock.rockX;
                int distToRight = (Results.Width - (int)rock.rockX - 1);
                bool clipLeft = distToLeft < desiredSize;
                bool clipRight = distToRight < desiredSize;

                int upperLeftX = 0;
                if (clipLeft)
                {
                    upperLeftX = 0;
                }
                else if(clipRight)
                {
                    upperLeftX = Results.Width - desiredSize - 1;
                }
                else
                {
                    upperLeftX = (int)(rock.rockX - desiredSize / 2);
                }

                int distToTop = (int)rock.rockY;
                int distToBottom = (Results.Height - (int)rock.rockY - 1);
                bool clipTop = distToTop < desiredSize;
                bool clipBottom = distToBottom < desiredSize;

                int upperLeftY = 0;
                if (clipTop)
                {
                    upperLeftY = 0;
                }
                else if (clipBottom)
                {
                    upperLeftY = Results.Height - desiredSize - 1;
                }
                else
                {
                    upperLeftY = (int)(rock.rockY - desiredSize / 2);
                }

                Rectangle rect = new Rectangle(upperLeftX, upperLeftY, desiredSize, desiredSize);

                if (sourceBitmap == null)
                { 
                    sourceBitmap = shadowsUI.Stage.GetTileBitmap(); 
                }

                Bitmap selectedBitmap = null;
                using (Bitmap srcBitmapClone = sourceBitmap.Clone(rect, sourceBitmap.PixelFormat))
                {
                    selectedBitmap = ResizeImage(srcBitmapClone, SelectedWindowPixels * SelectedWindowZoom, SelectedWindowPixels * SelectedWindowZoom);
                }

                using (Graphics grf = Graphics.FromImage(selectedBitmap))
                {            
                    //perimeter
                    using (Brush brush = new SolidBrush(Color.Blue))
                    {
                        var pixels = Results.GetPerimeterPixelsForLabel(label);
                        if (pixels.Count >= 2)
                        {
                            for (int idx = 0; idx < pixels.Count - 2; idx++)
                            {
                                int pixel = pixels[idx];
                                float col = SrcToCrop(pixel % Results.Width, upperLeftX, SelectedWindowZoom);
                                float row = SrcToCrop(pixel / Results.Width, upperLeftY, SelectedWindowZoom);

                                grf.FillRectangle(brush, col, row, 2, 2); //BUGBUG: look for scaling about pixel centers vs from top left
                            }
                        }
                    }

                    using (Pen brush = new Pen(Color.GreenYellow))
                    {
                        float centroidXInCrop = SrcToCrop(rock.shaX, upperLeftX, SelectedWindowZoom);
                        float centroidYInCrop = SrcToCrop(rock.shaY, upperLeftY, SelectedWindowZoom);

                        DrawEllipse(grf, brush, selectedBitmap.Width, selectedBitmap.Height, centroidXInCrop, centroidYInCrop, rock.shaEllipseMajor * SelectedWindowZoom, rock.shaEllipseMinor * SelectedWindowZoom, rock.shaEllipseTheta);
                    }

                }

                if (this.pictureBoxSelectedShadow.Image != null)
                {
                    this.pictureBoxSelectedShadow.Image.Dispose();
                }

                this.pictureBoxSelectedShadow.Image = selectedBitmap;

                selectedBitmap.Dispose();
            }
        }
    }
}
