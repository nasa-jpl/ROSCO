using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace RockCollect.Stages
{
    public partial class ChooseImageUI : UserControl
    {
        ChooseImage Stage;

        public ChooseImageUI(ChooseImage stage)
        {
            InitializeComponent();
            Stage = stage;
            Stage.SetGroundSamplingDistance((float)numericGSD.Value);
            Stage.SetSolarIncidence((float)numericIncidence.Value);
            Stage.SetSubSolarAzimuth((float)numericAzimuth.Value);
        }

        private void buttonNewSession_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = Directory.GetCurrentDirectory();
                openFileDialog.Filter = GDALSerializer.GetFilePickerFilter();
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = false;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Stage.SetNewImage(openFileDialog.FileName);
                    this.labelStatusImage.Text = "Image Selected: " + Path.GetFileNameWithoutExtension(openFileDialog.FileName);
                }
            }
        }

        private void numericGSD_ValueChanged(object sender, EventArgs e)
        {
            Stage.SetGroundSamplingDistance((float)numericGSD.Value);
        }

        private void numericIncidence_ValueChanged(object sender, EventArgs e)
        {
            Stage.SetSolarIncidence((float)numericIncidence.Value);
        }

        private void numericAzimuth_ValueChanged(object sender, EventArgs e)
        {
            Stage.SetSubSolarAzimuth((float)numericAzimuth.Value);
        }

        private void buttonRocklist_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = Directory.GetCurrentDirectory();
                openFileDialog.Filter = "Rocklist (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = false;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        Rocklist rocklist = new Rocklist(openFileDialog.FileName);
                        Stage.SetComparisonRocklist(openFileDialog.FileName);
                        this.labelStatusRocklist.Text = "Rocklist Selected: " + Path.GetFileNameWithoutExtension(openFileDialog.FileName);
                        numericGSD.Value = (decimal)rocklist.paramList.GSD_resolution;
                        numericIncidence.Value = (decimal)rocklist.paramList.sun_incidence_angle;
                        numericAzimuth.Value = (decimal)rocklist.paramList.sun_azimuth_angle;
                    }
                    catch
                    {
                        this.labelStatusRocklist.Text = "Failed to parse rocklist: " + Path.GetFileNameWithoutExtension(openFileDialog.FileName);
                    }
                }
            }
        }

        private void buttonShapeFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = Directory.GetCurrentDirectory();
                openFileDialog.Filter = "Shape (*.shp)|*.shp|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = false;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Stage.SetShapeFile(openFileDialog.FileName);
                    this.labelShapeFile.Text = "Shape File Selected: " + Path.GetFileNameWithoutExtension(openFileDialog.FileName);
                }
            }
        }

        private void ChooseImageUI_Load(object sender, EventArgs e)
        {
        }
    }
}
