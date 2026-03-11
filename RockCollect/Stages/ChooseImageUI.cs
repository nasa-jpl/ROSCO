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

        private string edrIndexPath;

        public const string DEF_EDR_INDEX = "EDRCUMINDEX.TAB";

        public ChooseImageUI(ChooseImage stage)
        {
            InitializeComponent();
            Stage = stage;
            Stage.SetGroundSamplingDistance((float)numericGSD.Value);
            Stage.SetSolarIncidence((float)numericIncidence.Value);
            Stage.SetSubSolarAzimuth((float)numericAzimuth.Value);

            labelStatusStorageFolder.Text = "Storge Folder: " + stage.GetFinalOutputDirectory(null);

            string defEDRIndexPath = Path.Combine(Directory.GetCurrentDirectory(), DEF_EDR_INDEX);
            if (File.Exists(defEDRIndexPath))
            {
                edrIndexPath = defEDRIndexPath;
                labelStatusEDRIndex.Text = "EDR Index: " + defEDRIndexPath;
                buttonAutoFillFromEDRIndex.Enabled = !string.IsNullOrEmpty(Stage.GetImagePath());
            }
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
                    Stage.SetImagePath(openFileDialog.FileName);
                    labelStatusImage.Text = "Image Selected: " + openFileDialog.FileName;
                    if (string.IsNullOrEmpty(edrIndexPath))
                    {
                        string path = Path.Combine(Path.GetDirectoryName(openFileDialog.FileName), DEF_EDR_INDEX);
                        if (File.Exists(path))
                        {
                            edrIndexPath = path;
                            labelStatusEDRIndex.Text = "EDR Index: " + path;
                        }
                    } 
                    buttonAutoFillFromEDRIndex.Enabled = !string.IsNullOrEmpty(edrIndexPath);
                }
            }
        }

        private void buttonStorageFolder_Click(object sender, EventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.SelectedPath = Directory.GetCurrentDirectory();

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    Stage.ParentWorkflow.SetFinalOutputDirectory(dialog.SelectedPath);
                    labelStatusStorageFolder.Text = "Storge Folder: " + Stage.GetFinalOutputDirectory(null);
                }
            }
        }

        private void buttonEDRIndex_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = Directory.GetCurrentDirectory();
                openFileDialog.Filter = "EDR Index (*.TAB)|*.TAB|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = false;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    edrIndexPath = openFileDialog.FileName;
                    labelStatusEDRIndex.Text = "EDR Index: " + openFileDialog.FileName;
                    buttonAutoFillFromEDRIndex.Enabled = !string.IsNullOrEmpty(Stage.GetImagePath());
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
                    Stage.SetComparisonRocklist(openFileDialog.FileName);
                    labelStatusRocklist.Text = "Rocklist Selected: " + openFileDialog.FileName;
                    buttonAutoFillFromComparisonRocklist.Enabled = true;
                }
            }
        }

        private void buttonAutoFillFromComparisonRocklist_Click(object sender, EventArgs e)
        {
            string path = Stage.GetComparisonRocklist();
            if (string.IsNullOrEmpty(path))
            {
                return;
            }
            
            try
            {
                Rocklist rocklist = new Rocklist(path);
                numericGSD.Value = (decimal)rocklist.paramList.GSD_resolution;
                numericIncidence.Value = (decimal)rocklist.paramList.sun_incidence_angle;
                numericAzimuth.Value = (decimal)rocklist.paramList.sun_azimuth_angle;
                Stage.SetGroundSamplingDistance(rocklist.paramList.GSD_resolution);
                Stage.SetSolarIncidence(rocklist.paramList.sun_incidence_angle);
                Stage.SetSubSolarAzimuth(rocklist.paramList.sun_azimuth_angle);
            }
            catch
            {
                MessageBox.Show("Failed to parse rocklist: " + path,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonAutoFillFromEDRIndex_Click(object sender, EventArgs e)
        {
            string path = Stage.GetImagePath();
            if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(edrIndexPath))
            {
                return;
            }

            if (!File.Exists(edrIndexPath))
            {
                MessageBox.Show("EDR Index not found: " + edrIndexPath,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string imageFile = Path.GetFileNameWithoutExtension(path);
            string imageID = imageFile.Contains('.') ? imageFile.Split(new char[] { '.' }, 2)[0] : imageFile;
            if (!imageID.EndsWith("_RED"))
            {
                imageID = imageID + "_RED";
            }

            Console.WriteLine(string.Format("Searching for image ID {0} in EDR index {1}...", imageID, edrIndexPath));

            Cursor.Current = Cursors.WaitCursor;

            float gsd = float.MaxValue;
            float incidence = float.MaxValue;
            float azimuth = float.MaxValue;

            try
            {
                //this approach is translated from get_rosco_info.py by Marshall Trautman

                //https://hirise-pds.lpl.arizona.edu/PDS/INDEX/EDRCUMINDEX.TAB 
                //https://hirise-pds.lpl.arizona.edu/PDS/INDEX/EDRCUMINDEX.LBL

                bool foundMatch = false;

                using (StreamReader reader = new StreamReader(edrIndexPath))
                {
                    int n = 0;
                    int m = 0;
                    string line;
                    do
                    {
                        line = reader.ReadLine();

                        bool spew = false;
                        if (!string.IsNullOrEmpty(line) && line.Contains(imageID) && line.Length >= 889)
                        {
                            spew = true;
                            foundMatch = true;
                            ++m;

                            //field info from EDRCUMINDEX.LBL; START_BYTE is 1 based
                            //SCALED_PIXEL_WIDTH START_BYTE = 733 BYTES = 17
                            string gsdStr = line.Substring(732, 17).Trim();
                            if (float.TryParse(gsdStr, out float gsdVal) && gsdVal < gsd)
                            {
                                gsd = gsdVal;
                            }

                            //INCIDENCE_ANGLE START_BYTE = 760 BYTES = 7
                            string incStr = line.Substring(759, 7).Trim();
                            if (float.TryParse(incStr, out float incVal) && incVal < incidence)
                            {
                                incidence = incVal;
                            }

                            //SUB_SOLAR_AZIMUTH START_BYTE = 880 BYTES = 10
                            string azStr = line.Substring(879, 10).Trim();
                            if (float.TryParse(azStr, out float azVal) && azVal < azimuth)
                            {
                                azimuth = azVal;
                            }
                        }

                        if (line != null)
                        {
                            ++n;
                        }
                        else
                        {
                            spew = true;
                        }

                        spew = spew || (n % 100000 == 0);

                        if (spew)
                        {
                            Console.WriteLine(string.Format("Searching for image ID {0} in EDR index {1}, " +
                                                            "checked {2} lines, got {3} matches",
                                                            imageID, edrIndexPath, n, m));
                        }

                    } while (line != null);
                }

                if (!foundMatch)
                {
                    MessageBox.Show(string.Format("Image ID {0} not found in EDR Index {1}", imageID, edrIndexPath),
                                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (gsd == float.MaxValue || incidence == float.MaxValue || azimuth == float.MaxValue)
                {
                    MessageBox.Show(string.Format("Failed to parse valid values for Image ID {0} from EDR Index {1}",
                                                  imageID, edrIndexPath),
                                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                float origAz = azimuth;
                azimuth = 180 - azimuth;
                if (azimuth < 0)
                {
                    azimuth += 360;
                }
                Console.WriteLine(string.Format("converted azimuth from {0} to {1}", origAz, azimuth));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to parse EDR Index: " + edrIndexPath + "\n" + ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }

            numericGSD.Value = (decimal)gsd;
            numericIncidence.Value = (decimal)incidence;
            numericAzimuth.Value = (decimal)azimuth;

            Stage.SetGroundSamplingDistance(gsd);
            Stage.SetSolarIncidence(incidence);
            Stage.SetSubSolarAzimuth(azimuth);
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
                    this.labelShapeFile.Text =
                        "Shape File Selected: " + Path.GetFileNameWithoutExtension(openFileDialog.FileName);
                }
            }
        }

        private void ChooseImageUI_Load(object sender, EventArgs e)
        {
        }

        private void ROSCO_TITLE_Click(object sender, EventArgs e)
        {
        }
    }
}
