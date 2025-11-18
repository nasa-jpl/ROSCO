using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RockCollect;

namespace RockCollect.Stages
{

    public partial class RefineShadowsUI : UserControl
    {
        public RefineShadows Stage;
        public RefineShadowsUI(RefineShadows stage)
        {
            InitializeComponent();
            Stage = stage;
            stage.OnTeardownUI = () => {
                System.Drawing.Image old = this.pictureBoxTile.Image;
                if (old != null)
                {
                    //attempt to avoid intermittent System.ArgumentException: Parameter is not valid.
                    this.pictureBoxTile.Image = null;
                    old.Dispose();
                }
            };
        }

        private void RefreshUI(RockDetector.DetectionResults results)
        {
            System.Drawing.Image old = this.pictureBoxTile.Image;
            if (old != null)
            {
                this.pictureBoxTile.Image = null;
                old.Dispose();
            }

            this.pictureBoxTile.Image = Stage.GetCurrentBlobsImage();

            RefineShadowsStatusUI shadowstatusUI = (RefineShadowsStatusUI)Stage.StatusControl;
            shadowstatusUI.UpdateDataGrid(results);
        }

        private void InitializeTrackBarValues()
        {
            Util.RefreshTrackbar(Stage.GetShadowAspect(), RockDetector.MIN_VALID_ASPECT, RockDetector.MAX_VALID_ASPECT,
                                 RockDetector.DISABLE_ASPECT, trackBarAspect, labelAspectVal, checkBoxAspect);
            Util.RefreshTrackbar(Stage.GetMinShadowArea(), RockDetector.MIN_VALID_MIN_SHADOW_AREA,
                                 RockDetector.MAX_VALID_MIN_SHADOW_AREA, RockDetector.DISABLE_MIN_SHADOW_AREA,
                                 trackBarMinArea, labelMinAreaVal, null);
            Util.RefreshTrackbar(Stage.GetMaxShadowArea(), RockDetector.MIN_VALID_MAX_SHADOW_AREA,
                                 RockDetector.MAX_VALID_MAX_SHADOW_AREA, RockDetector.DISABLE_MAX_SHADOW_AREA,
                                 trackBarMaxArea, labelMaxAreaVal, checkBoxMaxArea);
            Util.RefreshTrackbar(Stage.GetMeanGradient(), RockDetector.MIN_VALID_MEAN_GRADIENT,
                                 RockDetector.MAX_VALID_MEAN_GRADIENT, RockDetector.DISABLE_GRADIENT, trackBarGradient,
                                 labelGradientVal, checkBoxGradient);
            Util.RefreshTrackbar(Stage.GetMinShadowSplit(), RockDetector.MIN_VALID_SPLIT, RockDetector.MAX_VALID_SPLIT,
                                 RockDetector.DISABLE_SPLIT, trackBarSplit, labelSplitVal, checkBoxSplit);
        }

        internal void SetSelectedShadowsUI(int label, RockDetector.DetectionResults results)
        {
            Bitmap blobImage = Stage.GetCurrentBlobsImage();
            if (label >= 0)
            {
                List<int> selectedPIxels = results.GetPixelsForLabel(label);
                foreach (var idxPixel in selectedPIxels)
                {
                    int x = idxPixel % results.Width;
                    int y = idxPixel / results.Width;

                    blobImage.SetPixel(x, y, Color.YellowGreen);
                }
            }

            System.Drawing.Image old = this.pictureBoxTile.Image;
            if (old != null)
            {
                this.pictureBoxTile.Image = null;
                old.Dispose();
            }

            this.pictureBoxTile.Image = blobImage;
        }

        private void RefineShadowsUI_Load(object sender, EventArgs e)
        {
            InitializeTrackBarValues();

            Stage.SetMinShadowArea(Stage.GetMinShadowArea(), out RockDetector.DetectionResults results);
            RefreshUI(results);
        }

        private void RefineShadowsUI_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible == true)
            {
                InitializeTrackBarValues();
                Stage.SetMinShadowArea(Stage.GetMinShadowArea(), out RockDetector.DetectionResults results);
                RefreshUI(results);
            }
        }

        private void trackBarMinArea_ValueChanged(object sender, EventArgs e)
        {
            float minShadowArea =
                Util.RemapValues(trackBarMinArea.Value, trackBarMinArea.Minimum, trackBarMinArea.Maximum,
                                 RockDetector.MIN_VALID_MIN_SHADOW_AREA, RockDetector.MAX_VALID_MIN_SHADOW_AREA);
            
            Stage.SetMinShadowArea(minShadowArea, out RockDetector.DetectionResults results);

            labelMinAreaVal.Text = Stage.GetMinShadowArea().ToString("F2");
            RefreshUI(results);
        }

        private void trackBarMaxArea_ValueChanged(object sender, EventArgs e)
        {
            float maxShadowArea =
                Util.RemapValues(trackBarMaxArea.Value, trackBarMaxArea.Minimum, trackBarMaxArea.Maximum,
                                 RockDetector.MIN_VALID_MAX_SHADOW_AREA, RockDetector.MAX_VALID_MAX_SHADOW_AREA);
            
            
            Stage.SetMaxShadowArea(maxShadowArea, out RockDetector.DetectionResults results);

            labelMaxAreaVal.Text = Stage.GetMaxShadowArea().ToString("F2");
            RefreshUI(results);
        }

        private void trackBarAspect_ValueChanged(object sender, EventArgs e)
        {
            float aspect = Util.RemapValues(trackBarAspect.Value, trackBarAspect.Minimum, trackBarAspect.Maximum,
                                            RockDetector.MIN_VALID_ASPECT, RockDetector.MAX_VALID_ASPECT);
            
            Stage.SetShadowAspect(aspect, out RockDetector.DetectionResults results);

            labelAspectVal.Text = Stage.GetShadowAspect().ToString("F2");
            RefreshUI(results);
        }

        private void trackBarGradient_ValueChanged(object sender, EventArgs e)
        {
            float gradient =
                Util.RemapValues(trackBarGradient.Value, trackBarGradient.Minimum, trackBarGradient.Maximum,
                                 RockDetector.MIN_VALID_MEAN_GRADIENT, RockDetector.MAX_VALID_MEAN_GRADIENT);
            
            
            Stage.SetMeanGradient(gradient, out RockDetector.DetectionResults results);

            labelGradientVal.Text = Stage.GetMeanGradient().ToString("F2");
            RefreshUI(results);
        }

        private void trackBarSplit_ValueChanged(object sender, EventArgs e)
        {
            float split = Util.RemapValues(trackBarSplit.Value, trackBarSplit.Minimum, trackBarSplit.Maximum,
                                           RockDetector.MIN_VALID_SPLIT, RockDetector.MAX_VALID_SPLIT);

            Stage.SetMinShadowSplit(split, out RockDetector.DetectionResults results);

            labelSplitVal.Text = Stage.GetMinShadowSplit().ToString("F2");
            RefreshUI(results);
        }

        private void checkBoxMaxArea_CheckedChanged(object sender, EventArgs e)
        {
            RockDetector.DetectionResults results = null;
            if (checkBoxMaxArea.Checked)
            {
                Stage.SetMaxShadowArea(RockDetector.MAX_VALID_MAX_SHADOW_AREA, out results);
            }
            else
            {
                Stage.SetMaxShadowArea(RockDetector.DISABLE_MAX_SHADOW_AREA, out results);
            }
            Util.RefreshTrackbar(Stage.GetMaxShadowArea(), RockDetector.MIN_VALID_MAX_SHADOW_AREA,
                                 RockDetector.MAX_VALID_MAX_SHADOW_AREA, RockDetector.DISABLE_MAX_SHADOW_AREA,
                                 trackBarMaxArea, labelMaxAreaVal, checkBoxMaxArea);
            RefreshUI(results);
        }

        private void checkBoxAspect_CheckedChanged(object sender, EventArgs e)
        {
            RockDetector.DetectionResults results = null;
            if (checkBoxAspect.Checked)
            {
                Stage.SetShadowAspect(RockDetector.MAX_VALID_ASPECT, out results);
            }
            else
            {
                Stage.SetShadowAspect(RockDetector.DISABLE_ASPECT, out results);
            }

            Util.RefreshTrackbar(Stage.GetShadowAspect(), RockDetector.MIN_VALID_ASPECT,
                                 RockDetector.MAX_VALID_ASPECT, RockDetector.DISABLE_ASPECT,
                                 trackBarAspect, labelAspectVal, checkBoxAspect);
            RefreshUI(results);
        }

        private void checkBoxGradient_CheckedChanged(object sender, EventArgs e)
        {
            RockDetector.DetectionResults results = null;
            if (checkBoxGradient.Checked)
            {
                Stage.SetMeanGradient(RockDetector.MIN_VALID_MEAN_GRADIENT, out results);
            }
            else
            {
                Stage.SetMeanGradient(RockDetector.DISABLE_GRADIENT, out results);
            }
            Util.RefreshTrackbar(Stage.GetMeanGradient(), RockDetector.MIN_VALID_MEAN_GRADIENT,
                                 RockDetector.MAX_VALID_MEAN_GRADIENT, RockDetector.DISABLE_GRADIENT,
                                 trackBarGradient, labelGradientVal, checkBoxGradient);
            RefreshUI(results);
        }

        private void checkBoxSplit_CheckedChanged(object sender, EventArgs e)
        {
            RockDetector.DetectionResults results = null;
            if (checkBoxSplit.Checked)
            {
                Stage.SetMinShadowSplit(RockDetector.MAX_VALID_SPLIT, out results);
            }
            else
            {
                Stage.SetMinShadowSplit(RockDetector.DISABLE_SPLIT, out results);
            }
            Util.RefreshTrackbar(Stage.GetMinShadowSplit(), RockDetector.MIN_VALID_SPLIT,
                                 RockDetector.MAX_VALID_SPLIT, RockDetector.DISABLE_SPLIT, trackBarSplit,
                                 labelSplitVal, checkBoxSplit);
            RefreshUI(results);
        }

        private void buttonReset_Click(object sender, EventArgs e)
        {
            Stage.ResetToDefaults(out RockDetector.DetectionResults results);

            RefreshUI(results);

            checkBoxMaxArea.Checked = Stage.GetMaxShadowArea() != RockDetector.DISABLE_MAX_SHADOW_AREA;
            checkBoxAspect.Checked = Stage.GetShadowAspect() != RockDetector.DISABLE_ASPECT;
            checkBoxGradient.Checked = Stage.GetMeanGradient() != RockDetector.DISABLE_GRADIENT;
            checkBoxSplit.Checked = Stage.GetMinShadowSplit() != RockDetector.DISABLE_SPLIT;

            InitializeTrackBarValues();
        }
    }
}
