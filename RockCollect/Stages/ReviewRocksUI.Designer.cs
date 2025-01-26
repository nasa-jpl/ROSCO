namespace RockCollect.Stages
{
    partial class ReviewRocksUI
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
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBoxTile = new System.Windows.Forms.PictureBox();
            this.trackBarConfidence = new System.Windows.Forms.TrackBar();
            this.labelConfidence = new System.Windows.Forms.Label();
            this.labelConfidenceVal = new System.Windows.Forms.Label();
            this.checkBoxConfidence = new System.Windows.Forms.CheckBox();
            this.labelNumRocks = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonBothDifferent = new System.Windows.Forms.RadioButton();
            this.radioButtonBothIdentical = new System.Windows.Forms.RadioButton();
            this.radioButtonOnlyTheirs = new System.Windows.Forms.RadioButton();
            this.radioButtonOnlyYours = new System.Windows.Forms.RadioButton();
            this.radioButtonTheirs = new System.Windows.Forms.RadioButton();
            this.radioButtonYours = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTile)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarConfidence)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(32, 21);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(358, 37);
            this.label1.TabIndex = 0;
            this.label1.Text = "Review Rock Detections";
            // 
            // pictureBoxTile
            // 
            this.pictureBoxTile.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBoxTile.BackColor = System.Drawing.SystemColors.ControlDark;
            this.pictureBoxTile.Location = new System.Drawing.Point(40, 73);
            this.pictureBoxTile.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.pictureBoxTile.Name = "pictureBoxTile";
            this.pictureBoxTile.Size = new System.Drawing.Size(1100, 1058);
            this.pictureBoxTile.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxTile.TabIndex = 5;
            this.pictureBoxTile.TabStop = false;
            // 
            // trackBarConfidence
            // 
            this.trackBarConfidence.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBarConfidence.LargeChange = 100;
            this.trackBarConfidence.Location = new System.Drawing.Point(1184, 127);
            this.trackBarConfidence.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.trackBarConfidence.Maximum = 1000;
            this.trackBarConfidence.Name = "trackBarConfidence";
            this.trackBarConfidence.Size = new System.Drawing.Size(516, 90);
            this.trackBarConfidence.SmallChange = 5;
            this.trackBarConfidence.TabIndex = 7;
            this.trackBarConfidence.ValueChanged += new System.EventHandler(this.trackBarConfidence_ValueChanged);
            // 
            // labelConfidence
            // 
            this.labelConfidence.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelConfidence.AutoSize = true;
            this.labelConfidence.Location = new System.Drawing.Point(1188, 85);
            this.labelConfidence.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.labelConfidence.Name = "labelConfidence";
            this.labelConfidence.Size = new System.Drawing.Size(329, 25);
            this.labelConfidence.TabIndex = 8;
            this.labelConfidence.Text = "Confidence (vs. Reference Rock)";
            // 
            // labelConfidenceVal
            // 
            this.labelConfidenceVal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelConfidenceVal.AutoSize = true;
            this.labelConfidenceVal.Location = new System.Drawing.Point(1600, 85);
            this.labelConfidenceVal.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelConfidenceVal.Name = "labelConfidenceVal";
            this.labelConfidenceVal.Size = new System.Drawing.Size(0, 25);
            this.labelConfidenceVal.TabIndex = 20;
            // 
            // checkBoxConfidence
            // 
            this.checkBoxConfidence.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxConfidence.AutoSize = true;
            this.checkBoxConfidence.Location = new System.Drawing.Point(1152, 127);
            this.checkBoxConfidence.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.checkBoxConfidence.Name = "checkBoxConfidence";
            this.checkBoxConfidence.Size = new System.Drawing.Size(28, 27);
            this.checkBoxConfidence.TabIndex = 33;
            this.checkBoxConfidence.UseVisualStyleBackColor = true;
            this.checkBoxConfidence.CheckedChanged += new System.EventHandler(this.checkBoxConfidence_CheckedChanged);
            // 
            // labelNumRocks
            // 
            this.labelNumRocks.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelNumRocks.AutoSize = true;
            this.labelNumRocks.Location = new System.Drawing.Point(1544, 577);
            this.labelNumRocks.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelNumRocks.Name = "labelNumRocks";
            this.labelNumRocks.Size = new System.Drawing.Size(24, 25);
            this.labelNumRocks.TabIndex = 36;
            this.labelNumRocks.Text = "0";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(1180, 577);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(181, 25);
            this.label2.TabIndex = 35;
            this.label2.Text = "Number of rocks: ";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.radioButtonBothDifferent);
            this.groupBox1.Controls.Add(this.radioButtonBothIdentical);
            this.groupBox1.Controls.Add(this.radioButtonOnlyTheirs);
            this.groupBox1.Controls.Add(this.radioButtonOnlyYours);
            this.groupBox1.Controls.Add(this.radioButtonTheirs);
            this.groupBox1.Controls.Add(this.radioButtonYours);
            this.groupBox1.Location = new System.Drawing.Point(1180, 235);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Size = new System.Drawing.Size(444, 327);
            this.groupBox1.TabIndex = 37;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Select Data";
            // 
            // radioButtonBothDifferent
            // 
            this.radioButtonBothDifferent.AutoSize = true;
            this.radioButtonBothDifferent.Location = new System.Drawing.Point(22, 271);
            this.radioButtonBothDifferent.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.radioButtonBothDifferent.Name = "radioButtonBothDifferent";
            this.radioButtonBothDifferent.Size = new System.Drawing.Size(264, 29);
            this.radioButtonBothDifferent.TabIndex = 5;
            this.radioButtonBothDifferent.TabStop = true;
            this.radioButtonBothDifferent.Text = "Rocks in both, different";
            this.radioButtonBothDifferent.UseVisualStyleBackColor = true;
            this.radioButtonBothDifferent.CheckedChanged += new System.EventHandler(this.radioButtonBothDifferent_CheckedChanged);
            // 
            // radioButtonBothIdentical
            // 
            this.radioButtonBothIdentical.AutoSize = true;
            this.radioButtonBothIdentical.Location = new System.Drawing.Point(22, 227);
            this.radioButtonBothIdentical.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.radioButtonBothIdentical.Name = "radioButtonBothIdentical";
            this.radioButtonBothIdentical.Size = new System.Drawing.Size(266, 29);
            this.radioButtonBothIdentical.TabIndex = 4;
            this.radioButtonBothIdentical.TabStop = true;
            this.radioButtonBothIdentical.Text = "Rocks in both, identical";
            this.radioButtonBothIdentical.UseVisualStyleBackColor = true;
            this.radioButtonBothIdentical.CheckedChanged += new System.EventHandler(this.radioButtonBothIdentical_CheckedChanged);
            // 
            // radioButtonOnlyTheirs
            // 
            this.radioButtonOnlyTheirs.AutoSize = true;
            this.radioButtonOnlyTheirs.Location = new System.Drawing.Point(22, 179);
            this.radioButtonOnlyTheirs.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.radioButtonOnlyTheirs.Name = "radioButtonOnlyTheirs";
            this.radioButtonOnlyTheirs.Size = new System.Drawing.Size(231, 29);
            this.radioButtonOnlyTheirs.TabIndex = 3;
            this.radioButtonOnlyTheirs.TabStop = true;
            this.radioButtonOnlyTheirs.Text = "Rocks only in theirs";
            this.radioButtonOnlyTheirs.UseVisualStyleBackColor = true;
            this.radioButtonOnlyTheirs.CheckedChanged += new System.EventHandler(this.radioButtonOnlyTheirs_CheckedChanged);
            // 
            // radioButtonOnlyYours
            // 
            this.radioButtonOnlyYours.AutoSize = true;
            this.radioButtonOnlyYours.Location = new System.Drawing.Point(22, 135);
            this.radioButtonOnlyYours.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.radioButtonOnlyYours.Name = "radioButtonOnlyYours";
            this.radioButtonOnlyYours.Size = new System.Drawing.Size(231, 29);
            this.radioButtonOnlyYours.TabIndex = 2;
            this.radioButtonOnlyYours.TabStop = true;
            this.radioButtonOnlyYours.Text = "Rocks only in yours";
            this.radioButtonOnlyYours.UseVisualStyleBackColor = true;
            this.radioButtonOnlyYours.CheckedChanged += new System.EventHandler(this.radioButtonOnlyYours_CheckedChanged);
            // 
            // radioButtonTheirs
            // 
            this.radioButtonTheirs.AutoSize = true;
            this.radioButtonTheirs.Location = new System.Drawing.Point(22, 88);
            this.radioButtonTheirs.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.radioButtonTheirs.Name = "radioButtonTheirs";
            this.radioButtonTheirs.Size = new System.Drawing.Size(200, 29);
            this.radioButtonTheirs.TabIndex = 1;
            this.radioButtonTheirs.Text = "Their Detections";
            this.radioButtonTheirs.UseVisualStyleBackColor = true;
            this.radioButtonTheirs.CheckedChanged += new System.EventHandler(this.radioButtonTheirs_CheckedChanged);
            // 
            // radioButtonYours
            // 
            this.radioButtonYours.AutoSize = true;
            this.radioButtonYours.Checked = true;
            this.radioButtonYours.Location = new System.Drawing.Point(22, 40);
            this.radioButtonYours.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.radioButtonYours.Name = "radioButtonYours";
            this.radioButtonYours.Size = new System.Drawing.Size(197, 29);
            this.radioButtonYours.TabIndex = 0;
            this.radioButtonYours.TabStop = true;
            this.radioButtonYours.Text = "Your Detections";
            this.radioButtonYours.UseVisualStyleBackColor = true;
            this.radioButtonYours.CheckedChanged += new System.EventHandler(this.radioButtonYours_CheckedChanged);
            // 
            // ReviewRocksUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.labelNumRocks);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.checkBoxConfidence);
            this.Controls.Add(this.labelConfidenceVal);
            this.Controls.Add(this.labelConfidence);
            this.Controls.Add(this.trackBarConfidence);
            this.Controls.Add(this.pictureBoxTile);
            this.Controls.Add(this.label1);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MinimumSize = new System.Drawing.Size(1700, 1154);
            this.Name = "ReviewRocksUI";
            this.Size = new System.Drawing.Size(1700, 1154);
            this.Load += new System.EventHandler(this.ReviewRocksUI_Load);
            this.VisibleChanged += new System.EventHandler(this.ReviewRocksUI_VisibleChanged);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTile)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarConfidence)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBoxTile;
        private System.Windows.Forms.TrackBar trackBarConfidence;
        private System.Windows.Forms.Label labelConfidence;
        private System.Windows.Forms.Label labelConfidenceVal;
        private System.Windows.Forms.CheckBox checkBoxConfidence;
        private System.Windows.Forms.Label labelNumRocks;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButtonTheirs;
        private System.Windows.Forms.RadioButton radioButtonYours;
        private System.Windows.Forms.RadioButton radioButtonOnlyYours;
        private System.Windows.Forms.RadioButton radioButtonBothDifferent;
        private System.Windows.Forms.RadioButton radioButtonBothIdentical;
        private System.Windows.Forms.RadioButton radioButtonOnlyTheirs;
    }
}
