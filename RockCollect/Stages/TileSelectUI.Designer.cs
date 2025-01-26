namespace RockCollect.Stages
{
    partial class TileSelectUI
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
            this.labelStage = new System.Windows.Forms.Label();
            this.pictureBoxTile = new System.Windows.Forms.PictureBox();
            this.buttonChooseTile = new System.Windows.Forms.Button();
            this.labelImageName = new System.Windows.Forms.Label();
            this.labelTotalTiles = new System.Windows.Forms.Label();
            this.labelImageResolution = new System.Windows.Forms.Label();
            this.labelResolutionVal = new System.Windows.Forms.Label();
            this.labelTotalTilesVal = new System.Windows.Forms.Label();
            this.labelSelectedTile = new System.Windows.Forms.Label();
            this.labelSelectedTileVal = new System.Windows.Forms.Label();
            this.labelRemaining = new System.Windows.Forms.Label();
            this.labelRemainingVal = new System.Windows.Forms.Label();
            this.checkBoxDropOuts = new System.Windows.Forms.CheckBox();
            this.checkBoxNoisy = new System.Windows.Forms.CheckBox();
            this.checkBoxVerticalStripes = new System.Windows.Forms.CheckBox();
            this.labelQuality = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.labelSelectedTilePixelsVal = new System.Windows.Forms.Label();
            this.numericUpDownSkips = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonSave = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.labelTilesToVisitVal = new System.Windows.Forms.Label();
            this.labelTilesToVisit = new System.Windows.Forms.Label();
            this.labelTileGroup = new System.Windows.Forms.Label();
            this.labelGroupVal = new System.Windows.Forms.Label();
            this.labelRunnableTiles = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.labelTunedTiles = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTile)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSkips)).BeginInit();
            this.SuspendLayout();
            // 
            // labelStage
            // 
            this.labelStage.AutoSize = true;
            this.labelStage.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelStage.Location = new System.Drawing.Point(5, 5);
            this.labelStage.Name = "labelStage";
            this.labelStage.Size = new System.Drawing.Size(82, 20);
            this.labelStage.TabIndex = 0;
            this.labelStage.Text = "Tile Select";
            // 
            // pictureBoxTile
            // 
            this.pictureBoxTile.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBoxTile.BackColor = System.Drawing.SystemColors.ControlDark;
            this.pictureBoxTile.Location = new System.Drawing.Point(9, 28);
            this.pictureBoxTile.Name = "pictureBoxTile";
            this.pictureBoxTile.Size = new System.Drawing.Size(550, 550);
            this.pictureBoxTile.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxTile.TabIndex = 1;
            this.pictureBoxTile.TabStop = false;
            // 
            // buttonChooseTile
            // 
            this.buttonChooseTile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonChooseTile.Location = new System.Drawing.Point(580, 216);
            this.buttonChooseTile.Name = "buttonChooseTile";
            this.buttonChooseTile.Size = new System.Drawing.Size(75, 23);
            this.buttonChooseTile.TabIndex = 2;
            this.buttonChooseTile.Text = "Choose Tile";
            this.buttonChooseTile.UseVisualStyleBackColor = true;
            this.buttonChooseTile.Click += new System.EventHandler(this.buttonChooseTile_Click);
            // 
            // labelImageName
            // 
            this.labelImageName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelImageName.AutoSize = true;
            this.labelImageName.Location = new System.Drawing.Point(577, 30);
            this.labelImageName.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelImageName.Name = "labelImageName";
            this.labelImageName.Size = new System.Drawing.Size(0, 13);
            this.labelImageName.TabIndex = 3;
            // 
            // labelTotalTiles
            // 
            this.labelTotalTiles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelTotalTiles.AutoSize = true;
            this.labelTotalTiles.Location = new System.Drawing.Point(577, 78);
            this.labelTotalTiles.Name = "labelTotalTiles";
            this.labelTotalTiles.Size = new System.Drawing.Size(144, 13);
            this.labelTotalTiles.TabIndex = 4;
            this.labelTotalTiles.Text = "Total Tiles (Columns, Rows): ";
            // 
            // labelImageResolution
            // 
            this.labelImageResolution.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelImageResolution.AutoSize = true;
            this.labelImageResolution.Location = new System.Drawing.Point(577, 54);
            this.labelImageResolution.Name = "labelImageResolution";
            this.labelImageResolution.Size = new System.Drawing.Size(154, 13);
            this.labelImageResolution.TabIndex = 5;
            this.labelImageResolution.Text = "Image Pixels (Columns, Rows): ";
            // 
            // labelResolutionVal
            // 
            this.labelResolutionVal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelResolutionVal.AutoSize = true;
            this.labelResolutionVal.Location = new System.Drawing.Point(738, 54);
            this.labelResolutionVal.Name = "labelResolutionVal";
            this.labelResolutionVal.Size = new System.Drawing.Size(0, 13);
            this.labelResolutionVal.TabIndex = 6;
            // 
            // labelTotalTilesVal
            // 
            this.labelTotalTilesVal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelTotalTilesVal.AutoSize = true;
            this.labelTotalTilesVal.Location = new System.Drawing.Point(738, 78);
            this.labelTotalTilesVal.Name = "labelTotalTilesVal";
            this.labelTotalTilesVal.Size = new System.Drawing.Size(0, 13);
            this.labelTotalTilesVal.TabIndex = 7;
            // 
            // labelSelectedTile
            // 
            this.labelSelectedTile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelSelectedTile.AutoSize = true;
            this.labelSelectedTile.Location = new System.Drawing.Point(580, 249);
            this.labelSelectedTile.Name = "labelSelectedTile";
            this.labelSelectedTile.Size = new System.Drawing.Size(144, 13);
            this.labelSelectedTile.TabIndex = 8;
            this.labelSelectedTile.Text = "Selected Tile (Column, Row):";
            // 
            // labelSelectedTileVal
            // 
            this.labelSelectedTileVal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelSelectedTileVal.AutoSize = true;
            this.labelSelectedTileVal.Location = new System.Drawing.Point(734, 249);
            this.labelSelectedTileVal.Name = "labelSelectedTileVal";
            this.labelSelectedTileVal.Size = new System.Drawing.Size(0, 13);
            this.labelSelectedTileVal.TabIndex = 9;
            // 
            // labelRemaining
            // 
            this.labelRemaining.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelRemaining.AutoSize = true;
            this.labelRemaining.Location = new System.Drawing.Point(577, 153);
            this.labelRemaining.Name = "labelRemaining";
            this.labelRemaining.Size = new System.Drawing.Size(129, 13);
            this.labelRemaining.TabIndex = 10;
            this.labelRemaining.Text = "Remaining Tiles To Tune:";
            // 
            // labelRemainingVal
            // 
            this.labelRemainingVal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelRemainingVal.AutoSize = true;
            this.labelRemainingVal.Location = new System.Drawing.Point(734, 153);
            this.labelRemainingVal.Name = "labelRemainingVal";
            this.labelRemainingVal.Size = new System.Drawing.Size(0, 13);
            this.labelRemainingVal.TabIndex = 11;
            // 
            // checkBoxDropOuts
            // 
            this.checkBoxDropOuts.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxDropOuts.AutoSize = true;
            this.checkBoxDropOuts.Enabled = false;
            this.checkBoxDropOuts.Location = new System.Drawing.Point(579, 341);
            this.checkBoxDropOuts.Name = "checkBoxDropOuts";
            this.checkBoxDropOuts.Size = new System.Drawing.Size(95, 17);
            this.checkBoxDropOuts.TabIndex = 12;
            this.checkBoxDropOuts.Text = "Data Dropouts";
            this.checkBoxDropOuts.UseVisualStyleBackColor = true;
            // 
            // checkBoxNoisy
            // 
            this.checkBoxNoisy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxNoisy.AutoSize = true;
            this.checkBoxNoisy.Enabled = false;
            this.checkBoxNoisy.Location = new System.Drawing.Point(580, 364);
            this.checkBoxNoisy.Name = "checkBoxNoisy";
            this.checkBoxNoisy.Size = new System.Drawing.Size(84, 17);
            this.checkBoxNoisy.TabIndex = 13;
            this.checkBoxNoisy.Text = "Noisy Image";
            this.checkBoxNoisy.UseVisualStyleBackColor = true;
            // 
            // checkBoxVerticalStripes
            // 
            this.checkBoxVerticalStripes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxVerticalStripes.AutoSize = true;
            this.checkBoxVerticalStripes.Enabled = false;
            this.checkBoxVerticalStripes.Location = new System.Drawing.Point(581, 387);
            this.checkBoxVerticalStripes.Name = "checkBoxVerticalStripes";
            this.checkBoxVerticalStripes.Size = new System.Drawing.Size(96, 17);
            this.checkBoxVerticalStripes.TabIndex = 14;
            this.checkBoxVerticalStripes.Text = "Vertical Stripes";
            this.checkBoxVerticalStripes.UseVisualStyleBackColor = true;
            // 
            // labelQuality
            // 
            this.labelQuality.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelQuality.AutoSize = true;
            this.labelQuality.Enabled = false;
            this.labelQuality.Location = new System.Drawing.Point(580, 320);
            this.labelQuality.Name = "labelQuality";
            this.labelQuality.Size = new System.Drawing.Size(39, 13);
            this.labelQuality.TabIndex = 15;
            this.labelQuality.Text = "Quality";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(580, 274);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(129, 13);
            this.label1.TabIndex = 16;
            this.label1.Text = "Tile Pixels (Column, Row):";
            // 
            // labelSelectedTilePixelsVal
            // 
            this.labelSelectedTilePixelsVal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelSelectedTilePixelsVal.AutoSize = true;
            this.labelSelectedTilePixelsVal.Location = new System.Drawing.Point(734, 274);
            this.labelSelectedTilePixelsVal.Name = "labelSelectedTilePixelsVal";
            this.labelSelectedTilePixelsVal.Size = new System.Drawing.Size(0, 13);
            this.labelSelectedTilePixelsVal.TabIndex = 17;
            // 
            // numericUpDownSkips
            // 
            this.numericUpDownSkips.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownSkips.Location = new System.Drawing.Point(706, 128);
            this.numericUpDownSkips.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDownSkips.Name = "numericUpDownSkips";
            this.numericUpDownSkips.Size = new System.Drawing.Size(55, 20);
            this.numericUpDownSkips.TabIndex = 18;
            this.numericUpDownSkips.ValueChanged += new System.EventHandler(this.numericUpDownSkips_ValueChanged);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(577, 129);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(120, 13);
            this.label2.TabIndex = 19;
            this.label2.Text = "For each tile tuned, skip";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(767, 129);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(25, 13);
            this.label3.TabIndex = 20;
            this.label3.Text = "tiles";
            // 
            // buttonSave
            // 
            this.buttonSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSave.Location = new System.Drawing.Point(580, 533);
            this.buttonSave.Margin = new System.Windows.Forms.Padding(2);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(241, 19);
            this.buttonSave.TabIndex = 22;
            this.buttonSave.Text = "Run All Tiles and Save Combined Rocklist";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(595, 554);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(216, 13);
            this.label5.TabIndex = 23;
            this.label5.Text = "Un-tuned tiles will use nearest tuned settings";
            // 
            // labelTilesToVisitVal
            // 
            this.labelTilesToVisitVal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelTilesToVisitVal.AutoSize = true;
            this.labelTilesToVisitVal.Location = new System.Drawing.Point(737, 103);
            this.labelTilesToVisitVal.Name = "labelTilesToVisitVal";
            this.labelTilesToVisitVal.Size = new System.Drawing.Size(0, 13);
            this.labelTilesToVisitVal.TabIndex = 25;
            // 
            // labelTilesToVisit
            // 
            this.labelTilesToVisit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelTilesToVisit.AutoSize = true;
            this.labelTilesToVisit.Location = new System.Drawing.Point(576, 103);
            this.labelTilesToVisit.Name = "labelTilesToVisit";
            this.labelTilesToVisit.Size = new System.Drawing.Size(73, 13);
            this.labelTilesToVisit.TabIndex = 24;
            this.labelTilesToVisit.Text = "Tiles To Visit: ";
            // 
            // labelTileGroup
            // 
            this.labelTileGroup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelTileGroup.AutoSize = true;
            this.labelTileGroup.Location = new System.Drawing.Point(580, 296);
            this.labelTileGroup.Name = "labelTileGroup";
            this.labelTileGroup.Size = new System.Drawing.Size(135, 13);
            this.labelTileGroup.TabIndex = 26;
            this.labelTileGroup.Text = "Tile Group from Shape File:";
            // 
            // labelGroupVal
            // 
            this.labelGroupVal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelGroupVal.AutoSize = true;
            this.labelGroupVal.Location = new System.Drawing.Point(734, 296);
            this.labelGroupVal.Name = "labelGroupVal";
            this.labelGroupVal.Size = new System.Drawing.Size(0, 13);
            this.labelGroupVal.TabIndex = 27;
            // 
            // labelRunnableTiles
            // 
            this.labelRunnableTiles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelRunnableTiles.AutoSize = true;
            this.labelRunnableTiles.Location = new System.Drawing.Point(738, 475);
            this.labelRunnableTiles.Name = "labelRunnableTiles";
            this.labelRunnableTiles.Size = new System.Drawing.Size(0, 13);
            this.labelRunnableTiles.TabIndex = 29;
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(650, 475);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(81, 13);
            this.label6.TabIndex = 28;
            this.label6.Text = "Runnable Tiles:";
            // 
            // labelTunedTiles
            // 
            this.labelTunedTiles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelTunedTiles.AutoSize = true;
            this.labelTunedTiles.Location = new System.Drawing.Point(738, 501);
            this.labelTunedTiles.Name = "labelTunedTiles";
            this.labelTunedTiles.Size = new System.Drawing.Size(0, 13);
            this.labelTunedTiles.TabIndex = 31;
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(576, 501);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(155, 13);
            this.label8.TabIndex = 30;
            this.label8.Text = "Tiles with saved tuned settings:";
            // 
            // TileSelectUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.labelTunedTiles);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.labelRunnableTiles);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.labelGroupVal);
            this.Controls.Add(this.labelTileGroup);
            this.Controls.Add(this.labelTilesToVisitVal);
            this.Controls.Add(this.labelTilesToVisit);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.numericUpDownSkips);
            this.Controls.Add(this.labelSelectedTilePixelsVal);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.labelQuality);
            this.Controls.Add(this.checkBoxVerticalStripes);
            this.Controls.Add(this.checkBoxNoisy);
            this.Controls.Add(this.checkBoxDropOuts);
            this.Controls.Add(this.labelRemainingVal);
            this.Controls.Add(this.labelRemaining);
            this.Controls.Add(this.labelSelectedTileVal);
            this.Controls.Add(this.labelSelectedTile);
            this.Controls.Add(this.labelTotalTilesVal);
            this.Controls.Add(this.labelResolutionVal);
            this.Controls.Add(this.labelImageResolution);
            this.Controls.Add(this.labelTotalTiles);
            this.Controls.Add(this.labelImageName);
            this.Controls.Add(this.buttonChooseTile);
            this.Controls.Add(this.pictureBoxTile);
            this.Controls.Add(this.labelStage);
            this.Name = "TileSelectUI";
            this.Size = new System.Drawing.Size(850, 600);
            this.Load += new System.EventHandler(this.TileSelectUI_Load);
            this.VisibleChanged += new System.EventHandler(this.TileSelectUI_VisibleChanged);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTile)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSkips)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelStage;
        private System.Windows.Forms.PictureBox pictureBoxTile;
        private System.Windows.Forms.Button buttonChooseTile;
        private System.Windows.Forms.Label labelImageName;
        private System.Windows.Forms.Label labelTotalTiles;
        private System.Windows.Forms.Label labelImageResolution;
        private System.Windows.Forms.Label labelResolutionVal;
        private System.Windows.Forms.Label labelTotalTilesVal;
        private System.Windows.Forms.Label labelSelectedTile;
        private System.Windows.Forms.Label labelSelectedTileVal;
        private System.Windows.Forms.Label labelRemaining;
        private System.Windows.Forms.Label labelRemainingVal;
        private System.Windows.Forms.CheckBox checkBoxDropOuts;
        private System.Windows.Forms.CheckBox checkBoxNoisy;
        private System.Windows.Forms.CheckBox checkBoxVerticalStripes;
        private System.Windows.Forms.Label labelQuality;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelSelectedTilePixelsVal;
        private System.Windows.Forms.NumericUpDown numericUpDownSkips;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label labelTilesToVisitVal;
        private System.Windows.Forms.Label labelTilesToVisit;
        private System.Windows.Forms.Label labelTileGroup;
        private System.Windows.Forms.Label labelGroupVal;
        private System.Windows.Forms.Label labelRunnableTiles;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label labelTunedTiles;
        private System.Windows.Forms.Label label8;
    }
}
