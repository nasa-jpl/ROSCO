namespace RockCollect.Stages
{
    partial class ImageThresholdUI
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBoxTile = new System.Windows.Forms.PictureBox();
            this.labelStage = new System.Windows.Forms.Label();
            this.trackBarGamma = new System.Windows.Forms.TrackBar();
            this.labelGamma = new System.Windows.Forms.Label();
            this.textBoxHint = new System.Windows.Forms.TextBox();
            this.labelQuality = new System.Windows.Forms.Label();
            this.checkBoxNoGoodGamma = new System.Windows.Forms.CheckBox();
            this.labelGammaVal = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonFinalOverlay = new System.Windows.Forms.RadioButton();
            this.radioButtonGammaThresh = new System.Windows.Forms.RadioButton();
            this.radioButtonGamma = new System.Windows.Forms.RadioButton();
            this.radioButtonInput = new System.Windows.Forms.RadioButton();
            this.trackBarThreshold = new System.Windows.Forms.TrackBar();
            this.labelThresholdVal = new System.Windows.Forms.Label();
            this.labelThreshold = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTile)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarGamma)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarThreshold)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBoxTile
            // 
            this.pictureBoxTile.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBoxTile.BackColor = System.Drawing.SystemColors.ControlDark;
            this.pictureBoxTile.Location = new System.Drawing.Point(9, 33);
            this.pictureBoxTile.Name = "pictureBoxTile";
            this.pictureBoxTile.Size = new System.Drawing.Size(550, 550);
            this.pictureBoxTile.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxTile.TabIndex = 3;
            this.pictureBoxTile.TabStop = false;
            // 
            // labelStage
            // 
            this.labelStage.AutoSize = true;
            this.labelStage.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelStage.Location = new System.Drawing.Point(5, 5);
            this.labelStage.Name = "labelStage";
            this.labelStage.Size = new System.Drawing.Size(128, 20);
            this.labelStage.TabIndex = 2;
            this.labelStage.Text = "Image Threshold";
            // 
            // trackBarGamma
            // 
            this.trackBarGamma.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBarGamma.LargeChange = 100;
            this.trackBarGamma.Location = new System.Drawing.Point(576, 167);
            this.trackBarGamma.Maximum = 1000;
            this.trackBarGamma.Name = "trackBarGamma";
            this.trackBarGamma.Size = new System.Drawing.Size(258, 45);
            this.trackBarGamma.SmallChange = 5;
            this.trackBarGamma.TabIndex = 4;
            this.trackBarGamma.ValueChanged += new System.EventHandler(this.trackBarGamma_ValueChanged);
            // 
            // labelGamma
            // 
            this.labelGamma.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelGamma.AutoSize = true;
            this.labelGamma.Location = new System.Drawing.Point(573, 142);
            this.labelGamma.Name = "labelGamma";
            this.labelGamma.Size = new System.Drawing.Size(43, 13);
            this.labelGamma.TabIndex = 5;
            this.labelGamma.Text = "Gamma";
            // 
            // textBoxHint
            // 
            this.textBoxHint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxHint.BackColor = System.Drawing.SystemColors.ControlLight;
            this.textBoxHint.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxHint.Enabled = false;
            this.textBoxHint.Location = new System.Drawing.Point(576, 38);
            this.textBoxHint.Multiline = true;
            this.textBoxHint.Name = "textBoxHint";
            this.textBoxHint.Size = new System.Drawing.Size(258, 64);
            this.textBoxHint.TabIndex = 6;
            this.textBoxHint.Text = "Tune this parameter to the highest value that covers all rock shadows with green." +
    " Don\'t worry about false positives.";
            // 
            // labelQuality
            // 
            this.labelQuality.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelQuality.AutoSize = true;
            this.labelQuality.Enabled = false;
            this.labelQuality.Location = new System.Drawing.Point(581, 516);
            this.labelQuality.Name = "labelQuality";
            this.labelQuality.Size = new System.Drawing.Size(39, 13);
            this.labelQuality.TabIndex = 16;
            this.labelQuality.Text = "Quality";
            // 
            // checkBoxNoGoodGamma
            // 
            this.checkBoxNoGoodGamma.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxNoGoodGamma.AutoSize = true;
            this.checkBoxNoGoodGamma.Enabled = false;
            this.checkBoxNoGoodGamma.Location = new System.Drawing.Point(580, 545);
            this.checkBoxNoGoodGamma.Name = "checkBoxNoGoodGamma";
            this.checkBoxNoGoodGamma.Size = new System.Drawing.Size(194, 17);
            this.checkBoxNoGoodGamma.TabIndex = 17;
            this.checkBoxNoGoodGamma.Text = "No gamma value did well on this tile";
            this.checkBoxNoGoodGamma.UseVisualStyleBackColor = true;
            // 
            // labelGammaVal
            // 
            this.labelGammaVal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelGammaVal.AutoSize = true;
            this.labelGammaVal.Location = new System.Drawing.Point(803, 141);
            this.labelGammaVal.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelGammaVal.Name = "labelGammaVal";
            this.labelGammaVal.Size = new System.Drawing.Size(0, 13);
            this.labelGammaVal.TabIndex = 18;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.AutoSize = true;
            this.groupBox1.Controls.Add(this.radioButtonFinalOverlay);
            this.groupBox1.Controls.Add(this.radioButtonGammaThresh);
            this.groupBox1.Controls.Add(this.radioButtonGamma);
            this.groupBox1.Controls.Add(this.radioButtonInput);
            this.groupBox1.Location = new System.Drawing.Point(580, 232);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox1.Size = new System.Drawing.Size(246, 140);
            this.groupBox1.TabIndex = 19;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Image";
            // 
            // radioButtonFinalOverlay
            // 
            this.radioButtonFinalOverlay.AutoSize = true;
            this.radioButtonFinalOverlay.Checked = true;
            this.radioButtonFinalOverlay.Location = new System.Drawing.Point(10, 106);
            this.radioButtonFinalOverlay.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.radioButtonFinalOverlay.Name = "radioButtonFinalOverlay";
            this.radioButtonFinalOverlay.Size = new System.Drawing.Size(86, 17);
            this.radioButtonFinalOverlay.TabIndex = 7;
            this.radioButtonFinalOverlay.TabStop = true;
            this.radioButtonFinalOverlay.Text = "Final Overlay";
            this.radioButtonFinalOverlay.UseVisualStyleBackColor = true;
            this.radioButtonFinalOverlay.CheckedChanged += new System.EventHandler(this.radioButtonFinalOverlay_CheckedChanged);
            // 
            // radioButtonGammaThresh
            // 
            this.radioButtonGammaThresh.AutoSize = true;
            this.radioButtonGammaThresh.Location = new System.Drawing.Point(10, 81);
            this.radioButtonGammaThresh.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.radioButtonGammaThresh.Name = "radioButtonGammaThresh";
            this.radioButtonGammaThresh.Size = new System.Drawing.Size(132, 17);
            this.radioButtonGammaThresh.TabIndex = 6;
            this.radioButtonGammaThresh.Text = "Gamma and Threshold";
            this.radioButtonGammaThresh.UseVisualStyleBackColor = true;
            this.radioButtonGammaThresh.CheckedChanged += new System.EventHandler(this.radioButtonGammaThresh_CheckedChanged);
            // 
            // radioButtonGamma
            // 
            this.radioButtonGamma.AutoSize = true;
            this.radioButtonGamma.Location = new System.Drawing.Point(10, 56);
            this.radioButtonGamma.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.radioButtonGamma.Name = "radioButtonGamma";
            this.radioButtonGamma.Size = new System.Drawing.Size(83, 17);
            this.radioButtonGamma.TabIndex = 5;
            this.radioButtonGamma.Text = "Gamma only";
            this.radioButtonGamma.UseVisualStyleBackColor = true;
            this.radioButtonGamma.CheckedChanged += new System.EventHandler(this.radioButtonGamma_CheckedChanged);
            // 
            // radioButtonInput
            // 
            this.radioButtonInput.AutoSize = true;
            this.radioButtonInput.Location = new System.Drawing.Point(10, 30);
            this.radioButtonInput.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.radioButtonInput.Name = "radioButtonInput";
            this.radioButtonInput.Size = new System.Drawing.Size(81, 17);
            this.radioButtonInput.TabIndex = 4;
            this.radioButtonInput.Text = "Input Image";
            this.radioButtonInput.UseVisualStyleBackColor = true;
            this.radioButtonInput.CheckedChanged += new System.EventHandler(this.radioButtonInput_CheckedChanged);
            // 
            // trackBarThreshold
            // 
            this.trackBarThreshold.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBarThreshold.LargeChange = 10;
            this.trackBarThreshold.Location = new System.Drawing.Point(576, 413);
            this.trackBarThreshold.Maximum = 255;
            this.trackBarThreshold.Name = "trackBarThreshold";
            this.trackBarThreshold.Size = new System.Drawing.Size(258, 45);
            this.trackBarThreshold.TabIndex = 20;
            this.trackBarThreshold.ValueChanged += new System.EventHandler(this.trackBarThreshold_ValueChanged_1);
            // 
            // labelThresholdVal
            // 
            this.labelThresholdVal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelThresholdVal.AutoSize = true;
            this.labelThresholdVal.Location = new System.Drawing.Point(807, 386);
            this.labelThresholdVal.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelThresholdVal.Name = "labelThresholdVal";
            this.labelThresholdVal.Size = new System.Drawing.Size(0, 13);
            this.labelThresholdVal.TabIndex = 22;
            // 
            // labelThreshold
            // 
            this.labelThreshold.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelThreshold.AutoSize = true;
            this.labelThreshold.Location = new System.Drawing.Point(577, 387);
            this.labelThreshold.Name = "labelThreshold";
            this.labelThreshold.Size = new System.Drawing.Size(97, 13);
            this.labelThreshold.TabIndex = 21;
            this.labelThreshold.Text = "Threshold Override";
            // 
            // ImageThresholdUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.labelThresholdVal);
            this.Controls.Add(this.labelThreshold);
            this.Controls.Add(this.trackBarThreshold);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.labelGammaVal);
            this.Controls.Add(this.checkBoxNoGoodGamma);
            this.Controls.Add(this.labelQuality);
            this.Controls.Add(this.textBoxHint);
            this.Controls.Add(this.labelGamma);
            this.Controls.Add(this.trackBarGamma);
            this.Controls.Add(this.pictureBoxTile);
            this.Controls.Add(this.labelStage);
            this.MinimumSize = new System.Drawing.Size(850, 600);
            this.Name = "ImageThresholdUI";
            this.Size = new System.Drawing.Size(850, 600);
            this.Load += new System.EventHandler(this.ImageThresholdUI_Load);
            this.VisibleChanged += new System.EventHandler(this.ImageThresholdUI_VisibleChanged);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTile)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarGamma)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarThreshold)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxTile;
        private System.Windows.Forms.Label labelStage;
        private System.Windows.Forms.TrackBar trackBarGamma;
        private System.Windows.Forms.Label labelGamma;
        private System.Windows.Forms.TextBox textBoxHint;
        private System.Windows.Forms.Label labelQuality;
        private System.Windows.Forms.CheckBox checkBoxNoGoodGamma;
        private System.Windows.Forms.Label labelGammaVal;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButtonFinalOverlay;
        private System.Windows.Forms.RadioButton radioButtonGammaThresh;
        private System.Windows.Forms.RadioButton radioButtonGamma;
        private System.Windows.Forms.RadioButton radioButtonInput;
        private System.Windows.Forms.TrackBar trackBarThreshold;
        private System.Windows.Forms.Label labelThresholdVal;
        private System.Windows.Forms.Label labelThreshold;
    }
}
