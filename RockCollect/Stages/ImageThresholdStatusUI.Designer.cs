namespace RockCollect.Stages
{
    partial class ImageThresholdStatusUI
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title1 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title2 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea3 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title3 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea4 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title4 = new System.Windows.Forms.DataVisualization.Charting.Title();
            this.chartSourceHisto = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chartGamma = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.labelGammaThreshold = new System.Windows.Forms.Label();
            this.chartPixelsVGamma = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chartShadowBlobVGamma = new System.Windows.Forms.DataVisualization.Charting.Chart();
            ((System.ComponentModel.ISupportInitialize)(this.chartSourceHisto)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartGamma)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartPixelsVGamma)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartShadowBlobVGamma)).BeginInit();
            this.SuspendLayout();
            // 
            // chartSourceHisto
            // 
            chartArea1.AxisX.IntervalOffsetType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Number;
            chartArea1.AxisX.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Number;
            chartArea1.AxisX.IsLabelAutoFit = false;
            chartArea1.AxisX.LabelStyle.Angle = 90;
            chartArea1.AxisX.MajorGrid.Enabled = false;
            chartArea1.AxisX.Maximum = 256D;
            chartArea1.AxisX.Minimum = 0D;
            chartArea1.AxisX.Title = "Pixel Luminance Value";
            chartArea1.AxisY.MajorGrid.Enabled = false;
            chartArea1.AxisY.Maximum = 100D;
            chartArea1.AxisY.Minimum = 0D;
            chartArea1.AxisY.Title = "Normalized Percentage of Pixels";
            chartArea1.AxisY.TitleFont = new System.Drawing.Font("Microsoft Sans Serif", 6F);
            chartArea1.Name = "ChartArea1";
            this.chartSourceHisto.ChartAreas.Add(chartArea1);
            this.chartSourceHisto.Location = new System.Drawing.Point(124, 56);
            this.chartSourceHisto.Name = "chartSourceHisto";
            this.chartSourceHisto.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.SeaGreen;
            series1.ChartArea = "ChartArea1";
            series1.Name = "Series1";
            this.chartSourceHisto.Series.Add(series1);
            this.chartSourceHisto.Size = new System.Drawing.Size(870, 481);
            this.chartSourceHisto.TabIndex = 0;
            title1.Name = "Title1";
            title1.Text = "Input Image Intensity Histogram";
            this.chartSourceHisto.Titles.Add(title1);
            // 
            // chartGamma
            // 
            chartArea2.AxisX.Crossing = -1.7976931348623157E+308D;
            chartArea2.AxisX.IsLabelAutoFit = false;
            chartArea2.AxisX.LabelStyle.Angle = 90;
            chartArea2.AxisX.MajorGrid.Enabled = false;
            chartArea2.AxisX.Maximum = 256D;
            chartArea2.AxisX.Minimum = 0D;
            chartArea2.AxisX.Title = "Pixel Luminance Value (excluding White)";
            chartArea2.AxisY.MajorGrid.Enabled = false;
            chartArea2.AxisY.Maximum = 100D;
            chartArea2.AxisY.Minimum = 0D;
            chartArea2.AxisY.Title = "Normalized Percentage of Pixels";
            chartArea2.AxisY.TitleFont = new System.Drawing.Font("Microsoft Sans Serif", 6F);
            chartArea2.Name = "Image After Gamma";
            this.chartGamma.ChartAreas.Add(chartArea2);
            this.chartGamma.Location = new System.Drawing.Point(124, 564);
            this.chartGamma.Name = "chartGamma";
            this.chartGamma.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Berry;
            series2.ChartArea = "Image After Gamma";
            series2.Name = "Series1";
            this.chartGamma.Series.Add(series2);
            this.chartGamma.Size = new System.Drawing.Size(870, 501);
            this.chartGamma.TabIndex = 1;
            title2.Name = "Image After Gamma Intensity Histogram";
            title2.Text = "Image After Gamma Intensity Histogram";
            this.chartGamma.Titles.Add(title2);
            // 
            // labelGammaThreshold
            // 
            this.labelGammaThreshold.AutoSize = true;
            this.labelGammaThreshold.Location = new System.Drawing.Point(133, 1098);
            this.labelGammaThreshold.Name = "labelGammaThreshold";
            this.labelGammaThreshold.Size = new System.Drawing.Size(0, 25);
            this.labelGammaThreshold.TabIndex = 2;
            // 
            // chartPixelsVGamma
            // 
            chartArea3.AxisX.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Number;
            chartArea3.AxisX.Maximum = 10D;
            chartArea3.AxisX.Minimum = 0D;
            chartArea3.AxisX.Title = "Gamma Parameter";
            chartArea3.AxisY.MajorGrid.Enabled = false;
            chartArea3.AxisY.Title = "Shadow Pixels";
            chartArea3.Name = "ChartArea1";
            this.chartPixelsVGamma.ChartAreas.Add(chartArea3);
            legend1.Enabled = false;
            legend1.Name = "Legend1";
            this.chartPixelsVGamma.Legends.Add(legend1);
            this.chartPixelsVGamma.Location = new System.Drawing.Point(1069, 56);
            this.chartPixelsVGamma.Name = "chartPixelsVGamma";
            series3.ChartArea = "ChartArea1";
            series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series3.Legend = "Legend1";
            series3.Name = "Series1";
            this.chartPixelsVGamma.Series.Add(series3);
            this.chartPixelsVGamma.Size = new System.Drawing.Size(811, 481);
            this.chartPixelsVGamma.TabIndex = 3;
            this.chartPixelsVGamma.Text = "Pixels vs Gamma";
            title3.Name = "Title1";
            title3.Text = "Shadow pixels vs Gamma";
            this.chartPixelsVGamma.Titles.Add(title3);
            // 
            // chartShadowBlobVGamma
            // 
            chartArea4.AxisX.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Number;
            chartArea4.AxisX.Maximum = 10D;
            chartArea4.AxisX.Minimum = 0D;
            chartArea4.AxisX.Title = "Gamma Parameter";
            chartArea4.AxisY.MajorGrid.Enabled = false;
            chartArea4.AxisY.Title = "Shadow Blobs";
            chartArea4.Name = "ChartArea1";
            this.chartShadowBlobVGamma.ChartAreas.Add(chartArea4);
            legend2.Enabled = false;
            legend2.Name = "Legend1";
            this.chartShadowBlobVGamma.Legends.Add(legend2);
            this.chartShadowBlobVGamma.Location = new System.Drawing.Point(1069, 584);
            this.chartShadowBlobVGamma.Name = "chartShadowBlobVGamma";
            series4.ChartArea = "ChartArea1";
            series4.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series4.Legend = "Legend1";
            series4.Name = "Series1";
            this.chartShadowBlobVGamma.Series.Add(series4);
            this.chartShadowBlobVGamma.Size = new System.Drawing.Size(811, 481);
            this.chartShadowBlobVGamma.TabIndex = 4;
            this.chartShadowBlobVGamma.Text = "Blobs vs Gamma";
            title4.Name = "Title1";
            title4.Text = "Shadow Blobs vs Gamma";
            this.chartShadowBlobVGamma.Titles.Add(title4);
            // 
            // ImageThresholdStatusUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.chartShadowBlobVGamma);
            this.Controls.Add(this.chartPixelsVGamma);
            this.Controls.Add(this.labelGammaThreshold);
            this.Controls.Add(this.chartGamma);
            this.Controls.Add(this.chartSourceHisto);
            this.MinimumSize = new System.Drawing.Size(2500, 1500);
            this.Name = "ImageThresholdStatusUI";
            this.Size = new System.Drawing.Size(2500, 1500);
            ((System.ComponentModel.ISupportInitialize)(this.chartSourceHisto)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartGamma)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartPixelsVGamma)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartShadowBlobVGamma)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart chartSourceHisto;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartGamma;
        private System.Windows.Forms.Label labelGammaThreshold;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartPixelsVGamma;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartShadowBlobVGamma;
    }
}
