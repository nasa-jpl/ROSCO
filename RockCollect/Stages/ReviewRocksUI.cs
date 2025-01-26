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
    public partial class ReviewRocksUI : UserControl
    {
        public ReviewRocks Stage;

        public ReviewRocksUI(ReviewRocks stage)
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

        private void InitializeTrackBarValues()
        {
            RefreshTrackbar(Stage.GetConfidence(), RockDetector.MIN_VALID_CONFIDENCE, RockDetector.MAX_VALID_CONFIDENCE, RockDetector.DISABLE_CONFIDENCE, trackBarConfidence, labelConfidenceVal, checkBoxConfidence);
        }

        float RemapValues(float fromValue, float fromMin, float fromMax, float toMin, float toMax)
        {
            float pct = (fromValue - fromMin) / (fromMax - fromMin);
            return (pct * (toMax - toMin) + toMin);
        }

        private void RefreshTrackbar(float stageVal, float stageMin, float stageMax, float stageDisable, TrackBar trackBar, Label valLabel, CheckBox check)
        {
            if (stageVal == stageDisable)
            {
                valLabel.Text = "Disabled";

                if (trackBar.Enabled == true)
                {
                    trackBar.Enabled = false;
                    valLabel.Enabled = false;
                }

                if (check != null && check.Checked == true)
                {
                    check.Checked = false;
                }
            }
            else
            {
                if (trackBar.Enabled == false)
                {
                    trackBar.Enabled = true;
                    valLabel.Enabled = true;
                }
                trackBar.Value = (int)RemapValues(stageVal, stageMin, stageMax, trackBar.Minimum, trackBar.Maximum);
                valLabel.Text = stageVal.ToString("F2");

                if (check != null && check.Checked == false)
                {
                    check.Checked = true;
                }
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

            if (this.pictureBoxTile.Image != null)
            {
                ((IDisposable)this.pictureBoxTile.Image).Dispose();
                this.pictureBoxTile.Image = null;
            }

            this.pictureBoxTile.Image = detectionsImage;
           
        }

        private void RefreshUI(RockDetector.DetectionResults results, RockDetector.DetectionResults passiveResults)
        {
            if (this.pictureBoxTile.Image != null)
            {
                ((IDisposable)this.pictureBoxTile.Image).Dispose();
                this.pictureBoxTile.Image = null;
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
            float confidence = RemapValues(trackBarConfidence.Value, trackBarConfidence.Minimum, trackBarConfidence.Maximum,
                                           RockDetector.MIN_VALID_CONFIDENCE, RockDetector.MAX_VALID_CONFIDENCE);

            Stage.SetConfidence(confidence, out RockDetector.DetectionResults results);

            labelConfidenceVal.Text = Stage.GetConfidence().ToString("F2");
            RefreshUI(results, Stage.GetComparisonDetections());
        }

        private void checkBoxConfidence_CheckedChanged(object sender, EventArgs e)
        {
            RockDetector.DetectionResults results = null;
            if (checkBoxConfidence.Checked)
            {
                Stage.SetConfidence(RockDetector.MIN_VALID_CONFIDENCE, out results);
            }
            else
            {
                Stage.SetConfidence(RockDetector.DISABLE_CONFIDENCE, out results);
            }

            RefreshTrackbar(Stage.GetConfidence(), RockDetector.MIN_VALID_CONFIDENCE, RockDetector.MAX_VALID_CONFIDENCE, RockDetector.DISABLE_CONFIDENCE, trackBarConfidence, labelConfidenceVal, checkBoxConfidence);
            RefreshUI(results, Stage.GetComparisonDetections());
        }

        private void radioButtonYours_CheckedChanged(object sender, EventArgs e)
        {
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
            if (radioButtonOnlyYours.Checked)
            {
                this.trackBarConfidence.Enabled = false;
                this.checkBoxConfidence.Enabled = false;
                RefreshUI(Stage.GetOrphanRocksYours(), Stage.GetComparisonDetections());
            }
        }

        private void radioButtonOnlyTheirs_CheckedChanged(object sender, EventArgs e)
        {
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
            if (radioButtonBothIdentical.Checked)
            {
                this.trackBarConfidence.Enabled = false;
                this.checkBoxConfidence.Enabled = false;
                RefreshUI(Stage.GetMatchedIdenticalRocks(), Stage.GetMatchedIdenticalRocks());
            }
        }

        private void radioButtonBothDifferent_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonBothDifferent.Checked)
            {
                this.trackBarConfidence.Enabled = false;
                this.checkBoxConfidence.Enabled = false;
                RefreshUI(Stage.GetYourMatchedDifferentRocks(),Stage.GetComparisonDetections());
            }
        }
    }
}
