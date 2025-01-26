namespace RockCollect.Stages
{
    partial class RefineShadowsUI
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
            this.labelMinArea = new System.Windows.Forms.Label();
            this.trackBarMinArea = new System.Windows.Forms.TrackBar();
            this.labelMinAreaVal = new System.Windows.Forms.Label();
            this.labelMaxAreaVal = new System.Windows.Forms.Label();
            this.labelMaxArea = new System.Windows.Forms.Label();
            this.trackBarMaxArea = new System.Windows.Forms.TrackBar();
            this.labelGradientVal = new System.Windows.Forms.Label();
            this.labelGradient = new System.Windows.Forms.Label();
            this.trackBarGradient = new System.Windows.Forms.TrackBar();
            this.labelAspectVal = new System.Windows.Forms.Label();
            this.labelAspect = new System.Windows.Forms.Label();
            this.trackBarAspect = new System.Windows.Forms.TrackBar();
            this.labelSplitVal = new System.Windows.Forms.Label();
            this.labelSplit = new System.Windows.Forms.Label();
            this.trackBarSplit = new System.Windows.Forms.TrackBar();
            this.checkBoxMaxArea = new System.Windows.Forms.CheckBox();
            this.checkBoxAspect = new System.Windows.Forms.CheckBox();
            this.checkBoxGradient = new System.Windows.Forms.CheckBox();
            this.checkBoxSplit = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.pictureBoxTile = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarMinArea)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarMaxArea)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarGradient)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarAspect)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarSplit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTile)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(4, 6);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(248, 37);
            this.label1.TabIndex = 0;
            this.label1.Text = "Refine Shadows";
            // 
            // labelMinArea
            // 
            this.labelMinArea.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelMinArea.AutoSize = true;
            this.labelMinArea.Location = new System.Drawing.Point(1170, 81);
            this.labelMinArea.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.labelMinArea.Name = "labelMinArea";
            this.labelMinArea.Size = new System.Drawing.Size(279, 25);
            this.labelMinArea.TabIndex = 7;
            this.labelMinArea.Text = "Min Shadow Area (pixels^2)";
            // 
            // trackBarMinArea
            // 
            this.trackBarMinArea.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBarMinArea.LargeChange = 100;
            this.trackBarMinArea.Location = new System.Drawing.Point(1178, 129);
            this.trackBarMinArea.Margin = new System.Windows.Forms.Padding(6);
            this.trackBarMinArea.Maximum = 1000;
            this.trackBarMinArea.Name = "trackBarMinArea";
            this.trackBarMinArea.Size = new System.Drawing.Size(516, 90);
            this.trackBarMinArea.SmallChange = 5;
            this.trackBarMinArea.TabIndex = 6;
            this.trackBarMinArea.ValueChanged += new System.EventHandler(this.trackBarMinArea_ValueChanged);
            // 
            // labelMinAreaVal
            // 
            this.labelMinAreaVal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelMinAreaVal.AutoSize = true;
            this.labelMinAreaVal.Location = new System.Drawing.Point(1602, 81);
            this.labelMinAreaVal.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelMinAreaVal.Name = "labelMinAreaVal";
            this.labelMinAreaVal.Size = new System.Drawing.Size(0, 25);
            this.labelMinAreaVal.TabIndex = 19;
            // 
            // labelMaxAreaVal
            // 
            this.labelMaxAreaVal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelMaxAreaVal.AutoSize = true;
            this.labelMaxAreaVal.Location = new System.Drawing.Point(1602, 208);
            this.labelMaxAreaVal.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelMaxAreaVal.Name = "labelMaxAreaVal";
            this.labelMaxAreaVal.Size = new System.Drawing.Size(0, 25);
            this.labelMaxAreaVal.TabIndex = 22;
            // 
            // labelMaxArea
            // 
            this.labelMaxArea.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelMaxArea.AutoSize = true;
            this.labelMaxArea.Location = new System.Drawing.Point(1170, 208);
            this.labelMaxArea.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.labelMaxArea.Name = "labelMaxArea";
            this.labelMaxArea.Size = new System.Drawing.Size(285, 25);
            this.labelMaxArea.TabIndex = 21;
            this.labelMaxArea.Text = "Max Shadow Area (pixels^2)";
            // 
            // trackBarMaxArea
            // 
            this.trackBarMaxArea.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBarMaxArea.LargeChange = 100;
            this.trackBarMaxArea.Location = new System.Drawing.Point(1178, 256);
            this.trackBarMaxArea.Margin = new System.Windows.Forms.Padding(6);
            this.trackBarMaxArea.Maximum = 1000;
            this.trackBarMaxArea.Name = "trackBarMaxArea";
            this.trackBarMaxArea.Size = new System.Drawing.Size(516, 90);
            this.trackBarMaxArea.SmallChange = 5;
            this.trackBarMaxArea.TabIndex = 20;
            this.trackBarMaxArea.ValueChanged += new System.EventHandler(this.trackBarMaxArea_ValueChanged);
            // 
            // labelGradientVal
            // 
            this.labelGradientVal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelGradientVal.AutoSize = true;
            this.labelGradientVal.Location = new System.Drawing.Point(1602, 488);
            this.labelGradientVal.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelGradientVal.Name = "labelGradientVal";
            this.labelGradientVal.Size = new System.Drawing.Size(0, 25);
            this.labelGradientVal.TabIndex = 25;
            // 
            // labelGradient
            // 
            this.labelGradient.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelGradient.AutoSize = true;
            this.labelGradient.Location = new System.Drawing.Point(1170, 488);
            this.labelGradient.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.labelGradient.Name = "labelGradient";
            this.labelGradient.Size = new System.Drawing.Size(385, 25);
            this.labelGradient.TabIndex = 24;
            this.labelGradient.Text = "Minimum Mean Shadow Edge Gradient";
            // 
            // trackBarGradient
            // 
            this.trackBarGradient.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBarGradient.LargeChange = 100;
            this.trackBarGradient.Location = new System.Drawing.Point(1178, 537);
            this.trackBarGradient.Margin = new System.Windows.Forms.Padding(6);
            this.trackBarGradient.Maximum = 1000;
            this.trackBarGradient.Name = "trackBarGradient";
            this.trackBarGradient.Size = new System.Drawing.Size(516, 90);
            this.trackBarGradient.SmallChange = 5;
            this.trackBarGradient.TabIndex = 23;
            this.trackBarGradient.ValueChanged += new System.EventHandler(this.trackBarGradient_ValueChanged);
            // 
            // labelAspectVal
            // 
            this.labelAspectVal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelAspectVal.AutoSize = true;
            this.labelAspectVal.Location = new System.Drawing.Point(1602, 350);
            this.labelAspectVal.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelAspectVal.Name = "labelAspectVal";
            this.labelAspectVal.Size = new System.Drawing.Size(0, 25);
            this.labelAspectVal.TabIndex = 28;
            // 
            // labelAspect
            // 
            this.labelAspect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelAspect.AutoSize = true;
            this.labelAspect.Location = new System.Drawing.Point(1170, 350);
            this.labelAspect.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.labelAspect.Name = "labelAspect";
            this.labelAspect.Size = new System.Drawing.Size(395, 25);
            this.labelAspect.TabIndex = 27;
            this.labelAspect.Text = "Maximum Ratio Of Shadow Ellipse Axes";
            // 
            // trackBarAspect
            // 
            this.trackBarAspect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBarAspect.LargeChange = 100;
            this.trackBarAspect.Location = new System.Drawing.Point(1178, 398);
            this.trackBarAspect.Margin = new System.Windows.Forms.Padding(6);
            this.trackBarAspect.Maximum = 1000;
            this.trackBarAspect.Name = "trackBarAspect";
            this.trackBarAspect.Size = new System.Drawing.Size(516, 90);
            this.trackBarAspect.SmallChange = 5;
            this.trackBarAspect.TabIndex = 26;
            this.trackBarAspect.ValueChanged += new System.EventHandler(this.trackBarAspect_ValueChanged);
            // 
            // labelSplitVal
            // 
            this.labelSplitVal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelSplitVal.AutoSize = true;
            this.labelSplitVal.Location = new System.Drawing.Point(1600, 629);
            this.labelSplitVal.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelSplitVal.Name = "labelSplitVal";
            this.labelSplitVal.Size = new System.Drawing.Size(0, 25);
            this.labelSplitVal.TabIndex = 31;
            // 
            // labelSplit
            // 
            this.labelSplit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelSplit.AutoSize = true;
            this.labelSplit.Location = new System.Drawing.Point(1170, 629);
            this.labelSplit.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.labelSplit.Name = "labelSplit";
            this.labelSplit.Size = new System.Drawing.Size(409, 25);
            this.labelSplit.TabIndex = 30;
            this.labelSplit.Text = "Minimum Area To Split Shadow (pixels^2)";
            // 
            // trackBarSplit
            // 
            this.trackBarSplit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBarSplit.LargeChange = 100;
            this.trackBarSplit.Location = new System.Drawing.Point(1176, 677);
            this.trackBarSplit.Margin = new System.Windows.Forms.Padding(6);
            this.trackBarSplit.Maximum = 1000;
            this.trackBarSplit.Name = "trackBarSplit";
            this.trackBarSplit.Size = new System.Drawing.Size(516, 90);
            this.trackBarSplit.SmallChange = 5;
            this.trackBarSplit.TabIndex = 29;
            this.trackBarSplit.ValueChanged += new System.EventHandler(this.trackBarSplit_ValueChanged);
            // 
            // checkBoxMaxArea
            // 
            this.checkBoxMaxArea.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxMaxArea.AutoSize = true;
            this.checkBoxMaxArea.Location = new System.Drawing.Point(1136, 256);
            this.checkBoxMaxArea.Margin = new System.Windows.Forms.Padding(6);
            this.checkBoxMaxArea.Name = "checkBoxMaxArea";
            this.checkBoxMaxArea.Size = new System.Drawing.Size(28, 27);
            this.checkBoxMaxArea.TabIndex = 32;
            this.checkBoxMaxArea.UseVisualStyleBackColor = true;
            this.checkBoxMaxArea.CheckedChanged += new System.EventHandler(this.checkBoxMaxArea_CheckedChanged);
            // 
            // checkBoxAspect
            // 
            this.checkBoxAspect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxAspect.AutoSize = true;
            this.checkBoxAspect.Location = new System.Drawing.Point(1136, 398);
            this.checkBoxAspect.Margin = new System.Windows.Forms.Padding(6);
            this.checkBoxAspect.Name = "checkBoxAspect";
            this.checkBoxAspect.Size = new System.Drawing.Size(28, 27);
            this.checkBoxAspect.TabIndex = 33;
            this.checkBoxAspect.UseVisualStyleBackColor = true;
            this.checkBoxAspect.CheckedChanged += new System.EventHandler(this.checkBoxAspect_CheckedChanged);
            // 
            // checkBoxGradient
            // 
            this.checkBoxGradient.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxGradient.AutoSize = true;
            this.checkBoxGradient.Location = new System.Drawing.Point(1136, 537);
            this.checkBoxGradient.Margin = new System.Windows.Forms.Padding(6);
            this.checkBoxGradient.Name = "checkBoxGradient";
            this.checkBoxGradient.Size = new System.Drawing.Size(28, 27);
            this.checkBoxGradient.TabIndex = 34;
            this.checkBoxGradient.UseVisualStyleBackColor = true;
            this.checkBoxGradient.CheckedChanged += new System.EventHandler(this.checkBoxGradient_CheckedChanged);
            // 
            // checkBoxSplit
            // 
            this.checkBoxSplit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxSplit.AutoSize = true;
            this.checkBoxSplit.Location = new System.Drawing.Point(1136, 677);
            this.checkBoxSplit.Margin = new System.Windows.Forms.Padding(6);
            this.checkBoxSplit.Name = "checkBoxSplit";
            this.checkBoxSplit.Size = new System.Drawing.Size(28, 27);
            this.checkBoxSplit.TabIndex = 35;
            this.checkBoxSplit.UseVisualStyleBackColor = true;
            this.checkBoxSplit.CheckedChanged += new System.EventHandler(this.checkBoxSplit_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(332, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(600, 25);
            this.label2.TabIndex = 36;
            this.label2.Text = "Use the sliders to remove shadows that are not rock shadows";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(1131, 938);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(291, 25);
            this.label3.TabIndex = 37;
            this.label3.Text = "Blue pixels: shadows to keep";
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(1131, 1003);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(310, 25);
            this.label4.TabIndex = 38;
            this.label4.Text = "Red pixels: shadows to discard";
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(1131, 1060);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(500, 25);
            this.label5.TabIndex = 39;
            this.label5.Text = "Green pixels: shadow in the selected detail window";
            // 
            // pictureBoxTile
            // 
            this.pictureBoxTile.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBoxTile.BackColor = System.Drawing.SystemColors.ControlDark;
            this.pictureBoxTile.Location = new System.Drawing.Point(12, 62);
            this.pictureBoxTile.Margin = new System.Windows.Forms.Padding(6);
            this.pictureBoxTile.Name = "pictureBoxTile";
            this.pictureBoxTile.Size = new System.Drawing.Size(1100, 1058);
            this.pictureBoxTile.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxTile.TabIndex = 4;
            this.pictureBoxTile.TabStop = false;
            // 
            // RefineShadowsUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.checkBoxSplit);
            this.Controls.Add(this.checkBoxGradient);
            this.Controls.Add(this.checkBoxAspect);
            this.Controls.Add(this.checkBoxMaxArea);
            this.Controls.Add(this.labelSplitVal);
            this.Controls.Add(this.labelSplit);
            this.Controls.Add(this.trackBarSplit);
            this.Controls.Add(this.labelAspectVal);
            this.Controls.Add(this.labelAspect);
            this.Controls.Add(this.trackBarAspect);
            this.Controls.Add(this.labelGradientVal);
            this.Controls.Add(this.labelGradient);
            this.Controls.Add(this.trackBarGradient);
            this.Controls.Add(this.labelMaxAreaVal);
            this.Controls.Add(this.labelMaxArea);
            this.Controls.Add(this.trackBarMaxArea);
            this.Controls.Add(this.labelMinAreaVal);
            this.Controls.Add(this.labelMinArea);
            this.Controls.Add(this.trackBarMinArea);
            this.Controls.Add(this.pictureBoxTile);
            this.Controls.Add(this.label1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimumSize = new System.Drawing.Size(1700, 1154);
            this.Name = "RefineShadowsUI";
            this.Size = new System.Drawing.Size(1700, 1154);
            this.Load += new System.EventHandler(this.RefineShadowsUI_Load);
            this.VisibleChanged += new System.EventHandler(this.RefineShadowsUI_VisibleChanged);
            ((System.ComponentModel.ISupportInitialize)(this.trackBarMinArea)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarMaxArea)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarGradient)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarAspect)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarSplit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTile)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelMinArea;
        private System.Windows.Forms.TrackBar trackBarMinArea;
        private System.Windows.Forms.Label labelMinAreaVal;
        private System.Windows.Forms.Label labelMaxAreaVal;
        private System.Windows.Forms.Label labelMaxArea;
        private System.Windows.Forms.TrackBar trackBarMaxArea;
        private System.Windows.Forms.Label labelGradientVal;
        private System.Windows.Forms.Label labelGradient;
        private System.Windows.Forms.TrackBar trackBarGradient;
        private System.Windows.Forms.Label labelAspectVal;
        private System.Windows.Forms.Label labelAspect;
        private System.Windows.Forms.TrackBar trackBarAspect;
        private System.Windows.Forms.Label labelSplitVal;
        private System.Windows.Forms.Label labelSplit;
        private System.Windows.Forms.TrackBar trackBarSplit;
        private System.Windows.Forms.CheckBox checkBoxMaxArea;
        private System.Windows.Forms.CheckBox checkBoxAspect;
        private System.Windows.Forms.CheckBox checkBoxGradient;
        private System.Windows.Forms.CheckBox checkBoxSplit;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.PictureBox pictureBoxTile;
    }
}
