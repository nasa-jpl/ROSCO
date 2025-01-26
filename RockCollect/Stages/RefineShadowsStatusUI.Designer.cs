namespace RockCollect.Stages
{
    partial class RefineShadowsStatusUI
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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Label = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ShadowArea = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Aspect = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MeanGradient = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LabelShadows = new System.Windows.Forms.Label();
            this.pictureBoxSelectedShadow = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSelectedShadow)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Label,
            this.ShadowArea,
            this.Aspect,
            this.MeanGradient});
            this.dataGridView1.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGridView1.Location = new System.Drawing.Point(38, 69);
            this.dataGridView1.MinimumSize = new System.Drawing.Size(850, 1000);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersWidth = 82;
            this.dataGridView1.RowTemplate.Height = 33;
            this.dataGridView1.Size = new System.Drawing.Size(850, 1000);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.CellMouseEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellMouseEnter);
            // 
            // Label
            // 
            this.Label.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.Label.HeaderText = "Label";
            this.Label.MinimumWidth = 10;
            this.Label.Name = "Label";
            this.Label.ReadOnly = true;
            this.Label.Width = 110;
            // 
            // ShadowArea
            // 
            this.ShadowArea.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.ShadowArea.HeaderText = "Shadow Area";
            this.ShadowArea.MinimumWidth = 10;
            this.ShadowArea.Name = "ShadowArea";
            this.ShadowArea.ReadOnly = true;
            this.ShadowArea.Width = 170;
            // 
            // Aspect
            // 
            this.Aspect.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.Aspect.HeaderText = "Ellipse Axes Ratio";
            this.Aspect.MinimumWidth = 10;
            this.Aspect.Name = "Aspect";
            this.Aspect.ReadOnly = true;
            this.Aspect.Width = 167;
            // 
            // MeanGradient
            // 
            this.MeanGradient.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.MeanGradient.HeaderText = "Mean Gradient";
            this.MeanGradient.MinimumWidth = 10;
            this.MeanGradient.Name = "MeanGradient";
            this.MeanGradient.ReadOnly = true;
            this.MeanGradient.Width = 183;
            // 
            // LabelShadows
            // 
            this.LabelShadows.AutoSize = true;
            this.LabelShadows.Location = new System.Drawing.Point(33, 22);
            this.LabelShadows.Name = "LabelShadows";
            this.LabelShadows.Size = new System.Drawing.Size(100, 25);
            this.LabelShadows.TabIndex = 1;
            this.LabelShadows.Text = "Shadows";
            // 
            // pictureBoxSelectedShadow
            // 
            this.pictureBoxSelectedShadow.Location = new System.Drawing.Point(909, 109);
            this.pictureBoxSelectedShadow.Name = "pictureBoxSelectedShadow";
            this.pictureBoxSelectedShadow.Size = new System.Drawing.Size(600, 600);
            this.pictureBoxSelectedShadow.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxSelectedShadow.TabIndex = 2;
            this.pictureBoxSelectedShadow.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(917, 69);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(249, 25);
            this.label1.TabIndex = 3;
            this.label1.Text = "Zoomed View of Shadow";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(904, 723);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(242, 25);
            this.label2.TabIndex = 4;
            this.label2.Text = "Blue: Shadow Perimeter";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(904, 757);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(529, 25);
            this.label3.TabIndex = 5;
            this.label3.Text = "Yellow: Circle around centroid (radius: shadow length)";
            // 
            // RefineShadowsStatusUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBoxSelectedShadow);
            this.Controls.Add(this.LabelShadows);
            this.Controls.Add(this.dataGridView1);
            this.MinimumSize = new System.Drawing.Size(2500, 1500);
            this.Name = "RefineShadowsStatusUI";
            this.Size = new System.Drawing.Size(2500, 1500);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSelectedShadow)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label LabelShadows;
        private System.Windows.Forms.PictureBox pictureBoxSelectedShadow;
        private System.Windows.Forms.DataGridViewTextBoxColumn Label;
        private System.Windows.Forms.DataGridViewTextBoxColumn ShadowArea;
        private System.Windows.Forms.DataGridViewTextBoxColumn Aspect;
        private System.Windows.Forms.DataGridViewTextBoxColumn MeanGradient;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
    }
}
