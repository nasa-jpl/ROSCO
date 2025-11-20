namespace RockCollect
{
    partial class ChooseTile
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label2 = new System.Windows.Forms.Label();
            this.numericUpDownTileCol = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.numericUpDownTileRow = new System.Windows.Forms.NumericUpDown();
            this.labelTileColMinMax = new System.Windows.Forms.Label();
            this.labelTileRowMinMax = new System.Windows.Forms.Label();
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonMostRecent = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTileCol)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTileRow)).BeginInit();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 63);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 13);
            this.label2.TabIndex = 40;
            this.label2.Text = "column";
            // 
            // numericUpDownTileCol
            // 
            this.numericUpDownTileCol.Location = new System.Drawing.Point(59, 61);
            this.numericUpDownTileCol.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDownTileCol.Name = "numericUpDownTileCol";
            this.numericUpDownTileCol.Size = new System.Drawing.Size(55, 20);
            this.numericUpDownTileCol.TabIndex = 39;
            this.numericUpDownTileCol.ValueChanged += new System.EventHandler(this.numericUpDownTileCol_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(26, 98);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(24, 13);
            this.label1.TabIndex = 42;
            this.label1.Text = "row";
            // 
            // numericUpDownTileRow
            // 
            this.numericUpDownTileRow.Location = new System.Drawing.Point(59, 96);
            this.numericUpDownTileRow.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDownTileRow.Name = "numericUpDownTileRow";
            this.numericUpDownTileRow.Size = new System.Drawing.Size(55, 20);
            this.numericUpDownTileRow.TabIndex = 41;
            this.numericUpDownTileRow.ValueChanged += new System.EventHandler(this.numericUpDownTileRow_ValueChanged);
            // 
            // labelTileColMinMax
            // 
            this.labelTileColMinMax.AutoSize = true;
            this.labelTileColMinMax.Location = new System.Drawing.Point(136, 63);
            this.labelTileColMinMax.Name = "labelTileColMinMax";
            this.labelTileColMinMax.Size = new System.Drawing.Size(72, 13);
            this.labelTileColMinMax.TabIndex = 43;
            this.labelTileColMinMax.Text = "min: 0, max: 0";
            // 
            // labelTileRowMinMax
            // 
            this.labelTileRowMinMax.AutoSize = true;
            this.labelTileRowMinMax.Location = new System.Drawing.Point(136, 98);
            this.labelTileRowMinMax.Name = "labelTileRowMinMax";
            this.labelTileRowMinMax.Size = new System.Drawing.Size(72, 13);
            this.labelTileRowMinMax.TabIndex = 44;
            this.labelTileRowMinMax.Text = "min: 0, max: 0";
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(22, 139);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 45;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(129, 139);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 46;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonMostRecent
            // 
            this.buttonMostRecent.Location = new System.Drawing.Point(12, 21);
            this.buttonMostRecent.Name = "buttonMostRecent";
            this.buttonMostRecent.Size = new System.Drawing.Size(203, 23);
            this.buttonMostRecent.TabIndex = 47;
            this.buttonMostRecent.Text = "Set to Most Recently Tuned Tile";
            this.buttonMostRecent.UseVisualStyleBackColor = true;
            this.buttonMostRecent.Click += new System.EventHandler(this.buttonMostRecent_Click);
            // 
            // ChooseTile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(235, 182);
            this.Controls.Add(this.buttonMostRecent);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.labelTileRowMinMax);
            this.Controls.Add(this.labelTileColMinMax);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numericUpDownTileRow);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.numericUpDownTileCol);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "ChooseTile";
            this.Text = "Choose Tile";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTileCol)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTileRow)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numericUpDownTileCol;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numericUpDownTileRow;
        private System.Windows.Forms.Label labelTileColMinMax;
        private System.Windows.Forms.Label labelTileRowMinMax;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonMostRecent;
    }
}
