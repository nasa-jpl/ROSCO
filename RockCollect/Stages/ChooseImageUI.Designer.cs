namespace RockCollect.Stages
{
    partial class ChooseImageUI
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
            this.buttonNewSession = new System.Windows.Forms.Button();
            this.numericGSD = new System.Windows.Forms.NumericUpDown();
            this.numericIncidence = new System.Windows.Forms.NumericUpDown();
            this.numericAzimuth = new System.Windows.Forms.NumericUpDown();
            this.labelGSD = new System.Windows.Forms.Label();
            this.labelUnitsGSD = new System.Windows.Forms.Label();
            this.labelIncidence = new System.Windows.Forms.Label();
            this.labelUnitsIncidence = new System.Windows.Forms.Label();
            this.labelSubSolar = new System.Windows.Forms.Label();
            this.labelUnitsSubsolar = new System.Windows.Forms.Label();
            this.labelStage = new System.Windows.Forms.Label();
            this.labelStatusImage = new System.Windows.Forms.Label();
            this.buttonRocklist = new System.Windows.Forms.Button();
            this.labelStatusRocklist = new System.Windows.Forms.Label();
            this.buttonShapeFile = new System.Windows.Forms.Button();
            this.labelShapeFile = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numericGSD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericIncidence)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericAzimuth)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonNewSession
            // 
            this.buttonNewSession.Location = new System.Drawing.Point(347, 128);
            this.buttonNewSession.Margin = new System.Windows.Forms.Padding(2);
            this.buttonNewSession.Name = "buttonNewSession";
            this.buttonNewSession.Size = new System.Drawing.Size(184, 28);
            this.buttonNewSession.TabIndex = 0;
            this.buttonNewSession.Text = "Open New Image";
            this.buttonNewSession.UseVisualStyleBackColor = true;
            this.buttonNewSession.Click += new System.EventHandler(this.buttonNewSession_Click);
            // 
            // numericGSD
            // 
            this.numericGSD.DecimalPlaces = 5;
            this.numericGSD.Location = new System.Drawing.Point(408, 284);
            this.numericGSD.Margin = new System.Windows.Forms.Padding(2);
            this.numericGSD.Name = "numericGSD";
            this.numericGSD.Size = new System.Drawing.Size(88, 20);
            this.numericGSD.TabIndex = 1;
            this.numericGSD.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numericGSD.Value = new decimal(new int[] {
            3,
            0,
            0,
            65536});
            this.numericGSD.ValueChanged += new System.EventHandler(this.numericGSD_ValueChanged);
            // 
            // numericIncidence
            // 
            this.numericIncidence.DecimalPlaces = 5;
            this.numericIncidence.Location = new System.Drawing.Point(408, 319);
            this.numericIncidence.Margin = new System.Windows.Forms.Padding(2);
            this.numericIncidence.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.numericIncidence.Name = "numericIncidence";
            this.numericIncidence.Size = new System.Drawing.Size(88, 20);
            this.numericIncidence.TabIndex = 2;
            this.numericIncidence.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numericIncidence.Value = new decimal(new int[] {
            45,
            0,
            0,
            0});
            this.numericIncidence.ValueChanged += new System.EventHandler(this.numericIncidence_ValueChanged);
            // 
            // numericAzimuth
            // 
            this.numericAzimuth.DecimalPlaces = 5;
            this.numericAzimuth.Location = new System.Drawing.Point(408, 358);
            this.numericAzimuth.Margin = new System.Windows.Forms.Padding(2);
            this.numericAzimuth.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.numericAzimuth.Name = "numericAzimuth";
            this.numericAzimuth.Size = new System.Drawing.Size(88, 20);
            this.numericAzimuth.TabIndex = 3;
            this.numericAzimuth.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numericAzimuth.Value = new decimal(new int[] {
            133,
            0,
            0,
            0});
            this.numericAzimuth.ValueChanged += new System.EventHandler(this.numericAzimuth_ValueChanged);
            // 
            // labelGSD
            // 
            this.labelGSD.AutoSize = true;
            this.labelGSD.Location = new System.Drawing.Point(271, 287);
            this.labelGSD.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelGSD.Name = "labelGSD";
            this.labelGSD.Size = new System.Drawing.Size(133, 13);
            this.labelGSD.TabIndex = 4;
            this.labelGSD.Text = "Ground Sampling Distance";
            // 
            // labelUnitsGSD
            // 
            this.labelUnitsGSD.AutoSize = true;
            this.labelUnitsGSD.Location = new System.Drawing.Point(506, 287);
            this.labelUnitsGSD.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelUnitsGSD.Name = "labelUnitsGSD";
            this.labelUnitsGSD.Size = new System.Drawing.Size(80, 13);
            this.labelUnitsGSD.TabIndex = 5;
            this.labelUnitsGSD.Text = "meters per pixel";
            // 
            // labelIncidence
            // 
            this.labelIncidence.AutoSize = true;
            this.labelIncidence.Location = new System.Drawing.Point(271, 323);
            this.labelIncidence.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelIncidence.Name = "labelIncidence";
            this.labelIncidence.Size = new System.Drawing.Size(106, 13);
            this.labelIncidence.TabIndex = 6;
            this.labelIncidence.Text = "Sun Incidence Angle";
            // 
            // labelUnitsIncidence
            // 
            this.labelUnitsIncidence.AutoSize = true;
            this.labelUnitsIncidence.Location = new System.Drawing.Point(506, 323);
            this.labelUnitsIncidence.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelUnitsIncidence.Name = "labelUnitsIncidence";
            this.labelUnitsIncidence.Size = new System.Drawing.Size(45, 13);
            this.labelUnitsIncidence.TabIndex = 7;
            this.labelUnitsIncidence.Text = "degrees";
            // 
            // labelSubSolar
            // 
            this.labelSubSolar.AutoSize = true;
            this.labelSubSolar.Location = new System.Drawing.Point(271, 360);
            this.labelSubSolar.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelSubSolar.Name = "labelSubSolar";
            this.labelSubSolar.Size = new System.Drawing.Size(121, 13);
            this.labelSubSolar.TabIndex = 8;
            this.labelSubSolar.Text = "Sub-solar Azimuth Angle";
            // 
            // labelUnitsSubsolar
            // 
            this.labelUnitsSubsolar.AutoSize = true;
            this.labelUnitsSubsolar.Location = new System.Drawing.Point(506, 360);
            this.labelUnitsSubsolar.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelUnitsSubsolar.Name = "labelUnitsSubsolar";
            this.labelUnitsSubsolar.Size = new System.Drawing.Size(45, 13);
            this.labelUnitsSubsolar.TabIndex = 9;
            this.labelUnitsSubsolar.Text = "degrees";
            // 
            // labelStage
            // 
            this.labelStage.AutoSize = true;
            this.labelStage.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelStage.Location = new System.Drawing.Point(5, 5);
            this.labelStage.Name = "labelStage";
            this.labelStage.Size = new System.Drawing.Size(113, 20);
            this.labelStage.TabIndex = 10;
            this.labelStage.Text = "Choose Image";
            // 
            // labelStatusImage
            // 
            this.labelStatusImage.AutoSize = true;
            this.labelStatusImage.Location = new System.Drawing.Point(555, 136);
            this.labelStatusImage.Name = "labelStatusImage";
            this.labelStatusImage.Size = new System.Drawing.Size(84, 13);
            this.labelStatusImage.TabIndex = 11;
            this.labelStatusImage.Text = "Image Selected:";
            this.labelStatusImage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttonRocklist
            // 
            this.buttonRocklist.Location = new System.Drawing.Point(347, 215);
            this.buttonRocklist.Margin = new System.Windows.Forms.Padding(2);
            this.buttonRocklist.Name = "buttonRocklist";
            this.buttonRocklist.Size = new System.Drawing.Size(184, 31);
            this.buttonRocklist.TabIndex = 12;
            this.buttonRocklist.Text = "Load Comparison Rocklist";
            this.buttonRocklist.UseVisualStyleBackColor = true;
            this.buttonRocklist.Click += new System.EventHandler(this.buttonRocklist_Click);
            // 
            // labelStatusRocklist
            // 
            this.labelStatusRocklist.AutoSize = true;
            this.labelStatusRocklist.Location = new System.Drawing.Point(555, 224);
            this.labelStatusRocklist.Name = "labelStatusRocklist";
            this.labelStatusRocklist.Size = new System.Drawing.Size(93, 13);
            this.labelStatusRocklist.TabIndex = 13;
            this.labelStatusRocklist.Text = "Rocklist Selected:";
            this.labelStatusRocklist.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttonShapeFile
            // 
            this.buttonShapeFile.Location = new System.Drawing.Point(347, 172);
            this.buttonShapeFile.Margin = new System.Windows.Forms.Padding(2);
            this.buttonShapeFile.Name = "buttonShapeFile";
            this.buttonShapeFile.Size = new System.Drawing.Size(184, 28);
            this.buttonShapeFile.TabIndex = 14;
            this.buttonShapeFile.Text = "Open New Shape File";
            this.buttonShapeFile.UseVisualStyleBackColor = true;
            this.buttonShapeFile.Click += new System.EventHandler(this.buttonShapeFile_Click);
            // 
            // labelShapeFile
            // 
            this.labelShapeFile.AutoSize = true;
            this.labelShapeFile.Location = new System.Drawing.Point(555, 180);
            this.labelShapeFile.Name = "labelShapeFile";
            this.labelShapeFile.Size = new System.Drawing.Size(105, 13);
            this.labelShapeFile.TabIndex = 15;
            this.labelShapeFile.Text = "Shape File Selected:";
            this.labelShapeFile.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ChooseImageUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.labelShapeFile);
            this.Controls.Add(this.buttonShapeFile);
            this.Controls.Add(this.labelStatusRocklist);
            this.Controls.Add(this.buttonRocklist);
            this.Controls.Add(this.labelStatusImage);
            this.Controls.Add(this.labelStage);
            this.Controls.Add(this.labelUnitsSubsolar);
            this.Controls.Add(this.labelSubSolar);
            this.Controls.Add(this.labelUnitsIncidence);
            this.Controls.Add(this.labelIncidence);
            this.Controls.Add(this.labelUnitsGSD);
            this.Controls.Add(this.labelGSD);
            this.Controls.Add(this.numericAzimuth);
            this.Controls.Add(this.numericIncidence);
            this.Controls.Add(this.numericGSD);
            this.Controls.Add(this.buttonNewSession);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MinimumSize = new System.Drawing.Size(850, 600);
            this.Name = "ChooseImageUI";
            this.Size = new System.Drawing.Size(850, 600);
            this.Load += new System.EventHandler(this.ChooseImageUI_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numericGSD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericIncidence)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericAzimuth)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonNewSession;
        private System.Windows.Forms.NumericUpDown numericGSD;
        private System.Windows.Forms.NumericUpDown numericIncidence;
        private System.Windows.Forms.NumericUpDown numericAzimuth;
        private System.Windows.Forms.Label labelGSD;
        private System.Windows.Forms.Label labelUnitsGSD;
        private System.Windows.Forms.Label labelIncidence;
        private System.Windows.Forms.Label labelUnitsIncidence;
        private System.Windows.Forms.Label labelSubSolar;
        private System.Windows.Forms.Label labelUnitsSubsolar;
        private System.Windows.Forms.Label labelStage;
        private System.Windows.Forms.Label labelStatusImage;
        private System.Windows.Forms.Button buttonRocklist;
        private System.Windows.Forms.Label labelStatusRocklist;
        private System.Windows.Forms.Button buttonShapeFile;
        private System.Windows.Forms.Label labelShapeFile;
    }
}
