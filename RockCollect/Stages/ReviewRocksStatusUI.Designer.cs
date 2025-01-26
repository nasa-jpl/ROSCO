namespace RockCollect.Stages
{
    partial class ReviewRocksStatusUI
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series11 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series12 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series13 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series14 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series15 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series16 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series17 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series18 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series19 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series20 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Label = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RockDiameter = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Confidence = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PosDeltaToOther = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SizeDeltaToOther = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LabelRocks = new System.Windows.Forms.Label();
            this.pictureBoxSelectedRock = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.labelColorKey = new System.Windows.Forms.Label();
            this.chartCFA = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.label2 = new System.Windows.Forms.Label();
            this.labelRockCoverage = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.labelNumCFARocks = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.labelGoodnessFit = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSelectedRock)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartCFA)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Label,
            this.RockDiameter,
            this.Confidence,
            this.PosDeltaToOther,
            this.SizeDeltaToOther});
            this.dataGridView1.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGridView1.Location = new System.Drawing.Point(61, 739);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(4);
            this.dataGridView1.MinimumSize = new System.Drawing.Size(850, 1000);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersWidth = 82;
            this.dataGridView1.RowTemplate.Height = 33;
            this.dataGridView1.Size = new System.Drawing.Size(1178, 1000);
            this.dataGridView1.TabIndex = 1;
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
            // RockDiameter
            // 
            this.RockDiameter.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.RockDiameter.HeaderText = "Rock Diameter (meters)";
            this.RockDiameter.MinimumWidth = 10;
            this.RockDiameter.Name = "RockDiameter";
            this.RockDiameter.ReadOnly = true;
            this.RockDiameter.Width = 259;
            // 
            // Confidence
            // 
            this.Confidence.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.Confidence.HeaderText = "Confidence";
            this.Confidence.MinimumWidth = 10;
            this.Confidence.Name = "Confidence";
            this.Confidence.ReadOnly = true;
            this.Confidence.Width = 166;
            // 
            // PosDeltaToOther
            // 
            this.PosDeltaToOther.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.PosDeltaToOther.HeaderText = "Position Difference (meters)";
            this.PosDeltaToOther.MinimumWidth = 10;
            this.PosDeltaToOther.Name = "PosDeltaToOther";
            this.PosDeltaToOther.ReadOnly = true;
            this.PosDeltaToOther.Width = 295;
            // 
            // SizeDeltaToOther
            // 
            this.SizeDeltaToOther.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.SizeDeltaToOther.HeaderText = "Size Difference (meters)";
            this.SizeDeltaToOther.MinimumWidth = 10;
            this.SizeDeltaToOther.Name = "SizeDeltaToOther";
            this.SizeDeltaToOther.ReadOnly = true;
            this.SizeDeltaToOther.Width = 263;
            // 
            // LabelRocks
            // 
            this.LabelRocks.AutoSize = true;
            this.LabelRocks.Location = new System.Drawing.Point(57, 710);
            this.LabelRocks.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LabelRocks.Name = "LabelRocks";
            this.LabelRocks.Size = new System.Drawing.Size(72, 25);
            this.LabelRocks.TabIndex = 2;
            this.LabelRocks.Text = "Rocks";
            // 
            // pictureBoxSelectedRock
            // 
            this.pictureBoxSelectedRock.Location = new System.Drawing.Point(62, 93);
            this.pictureBoxSelectedRock.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBoxSelectedRock.Name = "pictureBoxSelectedRock";
            this.pictureBoxSelectedRock.Size = new System.Drawing.Size(600, 600);
            this.pictureBoxSelectedRock.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxSelectedRock.TabIndex = 3;
            this.pictureBoxSelectedRock.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(66, 52);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(221, 25);
            this.label1.TabIndex = 4;
            this.label1.Text = "Zoomed View of Rock";
            // 
            // labelColorKey
            // 
            this.labelColorKey.AutoSize = true;
            this.labelColorKey.Location = new System.Drawing.Point(45, 1800);
            this.labelColorKey.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.labelColorKey.Name = "labelColorKey";
            this.labelColorKey.Size = new System.Drawing.Size(0, 25);
            this.labelColorKey.TabIndex = 5;
            // 
            // chartCFA
            // 
            chartArea2.AxisX.IsLogarithmic = true;
            chartArea2.AxisX.LabelStyle.Interval = 0D;
            chartArea2.AxisX.LabelStyle.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Number;
            chartArea2.AxisX.MajorGrid.Enabled = false;
            chartArea2.AxisX.MajorGrid.Interval = 0D;
            chartArea2.AxisX.MajorGrid.IntervalOffset = 0D;
            chartArea2.AxisX.MajorGrid.IntervalOffsetType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Number;
            chartArea2.AxisX.MajorGrid.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Number;
            chartArea2.AxisX.Maximum = 10D;
            chartArea2.AxisX.Minimum = 0.1D;
            chartArea2.AxisX.Title = "Rock Diameter (m)";
            chartArea2.AxisY.IsLogarithmic = true;
            chartArea2.AxisY.MajorGrid.Enabled = false;
            chartArea2.AxisY.Maximum = 1D;
            chartArea2.AxisY.Minimum = 1E-05D;
            chartArea2.AxisY.Title = "CFA (Rock diam. >= D)";
            chartArea2.Name = "ChartArea1";
            this.chartCFA.ChartAreas.Add(chartArea2);
            legend2.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Bottom;
            legend2.Name = "Legend1";
            this.chartCFA.Legends.Add(legend2);
            this.chartCFA.Location = new System.Drawing.Point(1271, 96);
            this.chartCFA.Name = "chartCFA";
            series11.ChartArea = "ChartArea1";
            series11.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series11.Color = System.Drawing.Color.Gainsboro;
            series11.Legend = "Legend1";
            series11.LegendText = "0.02";
            series11.Name = "Series02";
            series12.ChartArea = "ChartArea1";
            series12.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series12.Color = System.Drawing.Color.LightGray;
            series12.Legend = "Legend1";
            series12.LegendText = "0.05";
            series12.Name = "Series05";
            series13.ChartArea = "ChartArea1";
            series13.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series13.Color = System.Drawing.Color.Silver;
            series13.Legend = "Legend1";
            series13.LegendText = "0.1";
            series13.Name = "Series1";
            series14.ChartArea = "ChartArea1";
            series14.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series14.Color = System.Drawing.Color.DarkGray;
            series14.Legend = "Legend1";
            series14.LegendText = "0.2";
            series14.Name = "Series2";
            series15.ChartArea = "ChartArea1";
            series15.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series15.Color = System.Drawing.Color.Gray;
            series15.Legend = "Legend1";
            series15.LegendText = "0.3";
            series15.Name = "Series3";
            series16.ChartArea = "ChartArea1";
            series16.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series16.Color = System.Drawing.Color.DimGray;
            series16.Legend = "Legend1";
            series16.LegendText = "0.4";
            series16.Name = "Series4";
            series17.ChartArea = "ChartArea1";
            series17.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series17.Legend = "Legend1";
            series17.LegendText = "Best Fit";
            series17.Name = "SeriesFit";
            series18.ChartArea = "ChartArea1";
            series18.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
            series18.Color = System.Drawing.Color.Black;
            series18.Legend = "Legend1";
            series18.LegendText = "Too Small Rocks";
            series18.MarkerColor = System.Drawing.Color.Black;
            series18.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Triangle;
            series18.Name = "SeriesPoints";
            series19.ChartArea = "ChartArea1";
            series19.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
            series19.Legend = "Legend1";
            series19.LegendText = "Too Big Rocks";
            series19.MarkerBorderColor = System.Drawing.Color.DarkRed;
            series19.MarkerColor = System.Drawing.Color.DarkRed;
            series19.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Triangle;
            series19.Name = "SeriesBiggerThanCFA";
            series20.ChartArea = "ChartArea1";
            series20.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
            series20.Color = System.Drawing.Color.Red;
            series20.Legend = "Legend1";
            series20.LegendText = "CFA Rocks";
            series20.MarkerColor = System.Drawing.Color.Cyan;
            series20.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Triangle;
            series20.Name = "SeriesCFAPoint";
            this.chartCFA.Series.Add(series11);
            this.chartCFA.Series.Add(series12);
            this.chartCFA.Series.Add(series13);
            this.chartCFA.Series.Add(series14);
            this.chartCFA.Series.Add(series15);
            this.chartCFA.Series.Add(series16);
            this.chartCFA.Series.Add(series17);
            this.chartCFA.Series.Add(series18);
            this.chartCFA.Series.Add(series19);
            this.chartCFA.Series.Add(series20);
            this.chartCFA.Size = new System.Drawing.Size(1484, 1503);
            this.chartCFA.TabIndex = 6;
            this.chartCFA.Text = "CFA";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(827, 112);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(227, 25);
            this.label2.TabIndex = 7;
            this.label2.Text = "Fit Rock Coverage (k):";
            // 
            // labelRockCoverage
            // 
            this.labelRockCoverage.AutoSize = true;
            this.labelRockCoverage.Location = new System.Drawing.Point(1072, 112);
            this.labelRockCoverage.Name = "labelRockCoverage";
            this.labelRockCoverage.Size = new System.Drawing.Size(0, 25);
            this.labelRockCoverage.TabIndex = 8;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(827, 159);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(231, 25);
            this.label3.TabIndex = 9;
            this.label3.Text = "Number of CFA Rocks:";
            // 
            // labelNumCFARocks
            // 
            this.labelNumCFARocks.AutoSize = true;
            this.labelNumCFARocks.Location = new System.Drawing.Point(1077, 158);
            this.labelNumCFARocks.Name = "labelNumCFARocks";
            this.labelNumCFARocks.Size = new System.Drawing.Size(0, 25);
            this.labelNumCFARocks.TabIndex = 10;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(832, 205);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(197, 25);
            this.label4.TabIndex = 11;
            this.label4.Text = "Goodness of Fit (r):";
            // 
            // labelGoodnessFit
            // 
            this.labelGoodnessFit.AutoSize = true;
            this.labelGoodnessFit.Location = new System.Drawing.Point(1077, 205);
            this.labelGoodnessFit.Name = "labelGoodnessFit";
            this.labelGoodnessFit.Size = new System.Drawing.Size(0, 25);
            this.labelGoodnessFit.TabIndex = 12;
            // 
            // ReviewRocksStatusUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.labelGoodnessFit);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.labelNumCFARocks);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.labelRockCoverage);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.chartCFA);
            this.Controls.Add(this.labelColorKey);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBoxSelectedRock);
            this.Controls.Add(this.LabelRocks);
            this.Controls.Add(this.dataGridView1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimumSize = new System.Drawing.Size(3300, 1650);
            this.Name = "ReviewRocksStatusUI";
            this.Size = new System.Drawing.Size(3300, 1650);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSelectedRock)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartCFA)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label LabelRocks;
        private System.Windows.Forms.PictureBox pictureBoxSelectedRock;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Label;
        private System.Windows.Forms.DataGridViewTextBoxColumn RockDiameter;
        private System.Windows.Forms.DataGridViewTextBoxColumn Confidence;
        private System.Windows.Forms.DataGridViewTextBoxColumn PosDeltaToOther;
        private System.Windows.Forms.DataGridViewTextBoxColumn SizeDeltaToOther;
        private System.Windows.Forms.Label labelColorKey;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartCFA;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelRockCoverage;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label labelNumCFARocks;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label labelGoodnessFit;
    }
}
