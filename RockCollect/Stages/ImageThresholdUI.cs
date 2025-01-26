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
    public partial class ImageThresholdUI : UserControl
    {
        public ImageThreshold Stage;

        public ImageThresholdUI(ImageThreshold stage)
        {
            InitializeComponent();
            Stage = stage;
            stage.OnTeardownUI = () => {
                if (this.pictureBoxTile.Image != null)
                {
                    //attempt to avoid intermittent System.ArgumentException: Parameter is not valid.
                    this.pictureBoxTile.Image.Dispose();
                    this.pictureBoxTile.Image = null;
                }
            };
        }

        private void RefreshUI(bool updatedSource = false)
        {
            Bitmap bmp = null;
            if (radioButtonInput.Checked)
            {
               bmp = Stage.GetTileBitmap();
            }
            else if (radioButtonGamma.Checked)
            {
                bmp = Stage.GetGammaImage().ToBitmap();
            }
            else if (radioButtonGammaThresh.Checked)
            {
                bmp = Stage.GetShadowBitmap();
            }
            else if (radioButtonFinalOverlay.Checked)
            {
                bmp = Stage.GetOverlayBitmap();
            }

            //if(this.pictureBoxTile.Image != null)
            //{
            //    this.pictureBoxTile.Image.Dispose();
            //}

            this.pictureBoxTile.Image = bmp;
            
            //bmp.Dispose();

            this.labelGammaVal.Text = Stage.GetGamma().ToString("F2");
            this.labelThresholdVal.Text = Stage.GetThresholdOverride().ToString();

            ImageThresholdStatusUI thresholdStatusUI = (ImageThresholdStatusUI)Stage.StatusControl;
            if (updatedSource)
            {
                thresholdStatusUI.UpdateSourceData(Stage.GetTileImage());
            }
            thresholdStatusUI.UpdateGammaHistogram(Stage.GetGammaImage());
        }

        int GammaToTrackBarValue(float gamma)
        {
            float curStageGammaPct = (gamma - ImageThreshold.MIN_GAMMA) / (ImageThreshold.MAX_GAMMA - ImageThreshold.MIN_GAMMA);
            return (int)(curStageGammaPct * (trackBarGamma.Maximum - trackBarGamma.Minimum) + trackBarGamma.Minimum);
        }

        float TrackBarValueToGamma(int trackbarVal)
        {
            float trackbarPct = (trackbarVal - trackBarGamma.Minimum) / (float)(trackBarGamma.Maximum - trackBarGamma.Minimum);
            return trackbarPct * (ImageThreshold.MAX_GAMMA - ImageThreshold.MIN_GAMMA) + ImageThreshold.MIN_GAMMA;
        }

        private void ImageThresholdUI_Load(object sender, EventArgs e)
        {
            this.trackBarGamma.Value = GammaToTrackBarValue(Stage.GetGamma());
            this.trackBarThreshold.Value = Stage.GetThresholdOverride();
            RefreshUI(true);
        }

        private void trackBarThreshold_ValueChanged(object sender, EventArgs e)
        {
            Stage.SetThresholdOverride(trackBarThreshold.Value);
            RefreshUI();
        }

        private void trackBarGamma_ValueChanged(object sender, EventArgs e)
        {
            Stage.SetGamma(TrackBarValueToGamma(trackBarGamma.Value));
            RefreshUI();
        }

        private void ImageThresholdUI_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible == true)
            {
                this.trackBarGamma.Value = GammaToTrackBarValue(Stage.GetGamma());
                this.trackBarThreshold.Value = Stage.GetThresholdOverride();
                RefreshUI(true);
            }
        }

        private void radioButtonInput_CheckedChanged(object sender, EventArgs e)
        {
            if(radioButtonInput.Checked)
            {
                RefreshUI();
            }
        }

        private void radioButtonGamma_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonGamma.Checked)
            {
                RefreshUI();
            }
        }

        private void radioButtonGammaThresh_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonGammaThresh.Checked)
            {
                RefreshUI();
            }
        }

        private void radioButtonFinalOverlay_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonFinalOverlay.Checked)
            {
                RefreshUI();
            }
        }

        private void trackBarThreshold_ValueChanged_1(object sender, EventArgs e)
        {
            Stage.SetThresholdOverride(trackBarThreshold.Value);
            RefreshUI();
        }
    }
}
