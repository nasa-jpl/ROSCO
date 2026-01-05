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
    public partial class ReviewRocksUI : UserControl
    {
        bool resetting = false;

        public ReviewRocks Stage;

        public ReviewRocksUI(ReviewRocks stage)
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

        private void InitializeTrackBarValues(bool fromReset = false)
        {
            resetting = true;
            Util.RefreshTrackbar(Stage.GetConfidence(), RockDetector.MIN_VALID_CONFIDENCE,
                                 RockDetector.MAX_VALID_CONFIDENCE, RockDetector.DISABLE_CONFIDENCE,
                                 trackBarConfidence, labelConfidenceVal, checkBoxConfidence);
            if (!fromReset)
            {
                resetting = false;
            }
        }

        Bitmap GetDetectionsImage()
        {
            if(radioButtonYours.Checked)
            {
                return Stage.GetDetectionsImage();
            }
            else if(radioButtonTheirs.Checked)
            {
                return Stage.GetComparisonDetectionsImage();
            }
            else if(radioButtonOnlyYours.Checked)
            {
                return Stage.GetOnlyYoursDetectionsImage();
            }
            else if(radioButtonOnlyTheirs.Checked)
            {
                return Stage.GetOnlyTheirsDetectionsImage();
            }
            else if(radioButtonBothIdentical.Checked)
            {
                return Stage.GetIdenticalDetectionsImage();
            }
            else if(radioButtonBothDifferent.Checked)
            {
                return Stage.GetBothDifferentDetectionsImage();
            }

            return null;
        }

        internal void SetSelectedRockUI(int label, RockDetector.DetectionResults results)
        {
            Bitmap detectionsImage = GetDetectionsImage();

            if (label >= 0)
            {
                using (Graphics grf = Graphics.FromImage(detectionsImage))
                {
                    //ellipse
                    using (Pen brush = new Pen(Color.Blue))
                    {
                        var rock = results.outRocks.Where(x => x.id == label).First();
                        float upperLeftX = rock.rockX - rock.rockWidth / 2.0f;
                        float upperLeftY = rock.rockY - rock.rockWidth / 2.0f;
                        grf.DrawEllipse(brush, upperLeftX, upperLeftY, rock.rockWidth, rock.rockWidth);
                    }
                }
            }

            System.Drawing.Image old = this.pictureBoxTile.Image;
            if (old != null)
            {
                this.pictureBoxTile.Image = null;
                old.Dispose();
            }

            this.pictureBoxTile.Image = detectionsImage;
        }

        private void RefreshUI(RockDetector.DetectionResults results, RockDetector.DetectionResults passiveResults)
        {
            System.Drawing.Image old = this.pictureBoxTile.Image;
            if (old != null)
            {
                this.pictureBoxTile.Image = null;
                old.Dispose();
            }

            Bitmap detectionsImage = GetDetectionsImage();            
            this.pictureBoxTile.Image = detectionsImage;
            this.labelNumRocks.Text = (results?.outRocks == null ? 0 : results.outRocks.Length).ToString();
            ReviewRocksStatusUI reviewRocksStatusUI = (ReviewRocksStatusUI)Stage.StatusControl;
            reviewRocksStatusUI.UpdateDataGrid(results, passiveResults);
            
        }

        private void EnableCompareUI(bool enable)
        {
            radioButtonTheirs.Enabled = enable;
            radioButtonOnlyYours.Enabled = enable;
            radioButtonOnlyTheirs.Enabled = enable;
            radioButtonBothIdentical.Enabled = enable;
            radioButtonBothDifferent.Enabled = enable;
        }

        private void ReviewRocksUI_Load(object sender, EventArgs e)
        {
            InitializeTrackBarValues();
            var comparisonResults = Stage.GetComparisonDetections();
            EnableCompareUI(comparisonResults != null);
            Stage.SetConfidence(Stage.GetConfidence(), out RockDetector.DetectionResults results);
            RefreshUI(results, comparisonResults);
        }

        private void ReviewRocksUI_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible == true)
            {
                InitializeTrackBarValues();
                var comparisonResults = Stage.GetComparisonDetections();
                EnableCompareUI(comparisonResults != null);
                Stage.SetConfidence(Stage.GetConfidence(), out RockDetector.DetectionResults results);
                RefreshUI(results, comparisonResults);
            }
        }

        private void trackBarConfidence_ValueChanged(object sender, EventArgs e)
        {
            if (resetting) return;
            float confidence =
                Util.RemapValues(trackBarConfidence.Value, trackBarConfidence.Minimum, trackBarConfidence.Maximum,
                                 RockDetector.MIN_VALID_CONFIDENCE, RockDetector.MAX_VALID_CONFIDENCE);

            Stage.SetConfidence(confidence, out RockDetector.DetectionResults results);

            labelConfidenceVal.Text = Stage.GetConfidence().ToString("F2");
            RefreshUI(results, Stage.GetComparisonDetections());
        }

        private void checkBoxConfidence_CheckedChanged(object sender, EventArgs e)
        {
            if (resetting) return;
            RockDetector.DetectionResults results = null;
            if (checkBoxConfidence.Checked)
            {
                Stage.SetConfidence(RockDetector.MIN_VALID_CONFIDENCE, out results);
            }
            else
            {
                Stage.SetConfidence(RockDetector.DISABLE_CONFIDENCE, out results);
            }

            Util.RefreshTrackbar(Stage.GetConfidence(), RockDetector.MIN_VALID_CONFIDENCE,
                                 RockDetector.MAX_VALID_CONFIDENCE, RockDetector.DISABLE_CONFIDENCE,
                                 trackBarConfidence, labelConfidenceVal, checkBoxConfidence);
            RefreshUI(results, Stage.GetComparisonDetections());
        }

        private void radioButtonYours_CheckedChanged(object sender, EventArgs e)
        {
            if (resetting) return;
            if (radioButtonYours.Checked)
            {
                Stage.SetConfidence(Stage.GetConfidence(), out RockDetector.DetectionResults results);
                this.trackBarConfidence.Enabled = true;
                this.checkBoxConfidence.Enabled = true;
                RefreshUI(results, Stage.GetComparisonDetections());
            }
        }

        private void radioButtonTheirs_CheckedChanged(object sender, EventArgs e)
        {
            if (resetting) return;
            if (radioButtonTheirs.Checked)
            {
                RockDetector.DetectionResults theirs = Stage.GetComparisonDetections();
                this.trackBarConfidence.Enabled = false;
                this.checkBoxConfidence.Enabled = false;
                Stage.SetConfidence(Stage.GetConfidence(), out RockDetector.DetectionResults yours);

                RefreshUI(theirs, yours);
            }
        }

        private void radioButtonOnlyYours_CheckedChanged(object sender, EventArgs e)
        {
            if (resetting) return;
            if (radioButtonOnlyYours.Checked)
            {
                this.trackBarConfidence.Enabled = false;
                this.checkBoxConfidence.Enabled = false;
                RefreshUI(Stage.GetOrphanRocksYours(), Stage.GetComparisonDetections());
            }
        }

        private void radioButtonOnlyTheirs_CheckedChanged(object sender, EventArgs e)
        {
            if (resetting) return;
            if (radioButtonOnlyTheirs.Checked)
            {
                this.trackBarConfidence.Enabled = false;
                this.checkBoxConfidence.Enabled = false;
                Stage.SetConfidence(Stage.GetConfidence(), out RockDetector.DetectionResults yours);

                RefreshUI(Stage.GetOrphanRocksTheirs(),yours);
            }
        }

        private void radioButtonBothIdentical_CheckedChanged(object sender, EventArgs e)
        {
            if (resetting) return;
            if (radioButtonBothIdentical.Checked)
            {
                this.trackBarConfidence.Enabled = false;
                this.checkBoxConfidence.Enabled = false;
                RefreshUI(Stage.GetMatchedIdenticalRocks(), Stage.GetMatchedIdenticalRocks());
            }
        }

        private void radioButtonBothDifferent_CheckedChanged(object sender, EventArgs e)
        {
            if (resetting) return;
            if (radioButtonBothDifferent.Checked)
            {
                this.trackBarConfidence.Enabled = false;
                this.checkBoxConfidence.Enabled = false;
                RefreshUI(Stage.GetYourMatchedDifferentRocks(),Stage.GetComparisonDetections());
            }
        }

        private void buttonReset_Click(object sender, EventArgs e)
        {
            resetting = true;
            Stage.ResetToDefaults(out RockDetector.DetectionResults results);
            InitializeTrackBarValues(fromReset: true);
            RefreshUI(results, Stage.GetComparisonDetections());
            resetting = false;
        }
    }
}
