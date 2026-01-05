using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using RockCollect.Stages;

namespace RockCollect
{
    public partial class ChooseTile : Form
    {
        public static void ChooseExistingTile(string dir, int numCols, int numRows, Action<int, int> callback)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = dir;
                openFileDialog.Filter = "Tile (Tile_*_*.json)|Tile_*_*.json|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = false;
                
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string pattern = @"Tile_(\d+)_(\d+).json$";
                    Match match = Regex.Match(openFileDialog.FileName, pattern);
                    if (match.Success)
                    {
                        int x = int.Parse(match.Groups[1].Value);
                        int y = int.Parse(match.Groups[2].Value);
                        if ((x >= 0 && x < numCols) && (y >=0 && y < numRows)) callback(x, y);
                        else
                        {
                            MessageBox.Show(
                                string.Format("Invalid tile (col={0}, row={1}), must be in range (0, 0) to ({2}, {3})",
                                              x, y, numCols - 1, numRows - 1),
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show(
                            string.Format("Invalid tile filename \"{0}\", must be in the form Tile_######_######.json",
                                          Path.GetFileName(openFileDialog.FileName)),
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        TileSelect Stage;
        DateTime? recent;
        int numCols, numRows;
        string storageDir;

        public ChooseTile(TileSelect stage)
        {
            Stage = stage;
            InitializeComponent();
            storageDir = Stage.GetFinalOutputDirectory();
            numCols = Stage.GetNumTilesHorizontal();
            numRows = Stage.GetNumTilesVertical();
            int maxCol = numCols - 1;
            int maxRow = numRows - 1;
            numericUpDownTileCol.Minimum = 0;
            numericUpDownTileCol.Maximum = maxCol;
            numericUpDownTileRow.Minimum = 0;
            numericUpDownTileRow.Maximum = maxRow;
            labelTileColMinMax.Text = "min: 0, max: " + maxCol;
            labelTileRowMinMax.Text = "min: 0, max: " + maxRow;
        }

        public int GetTileCol()
        {
            return (int)numericUpDownTileCol.Value;
        }

        public int GetTileRow()
        {
            return (int)numericUpDownTileRow.Value;
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void buttonMostRecent_Click(object sender, EventArgs e)
        {
            int idx = Stage.GetMostRecentlyTunedTile(recent);
            if (idx >= 0)
            {
                Stage.GetTileAddress(idx, out int x, out int y);
                numericUpDownTileCol.Value = x;
                numericUpDownTileRow.Value = y;
                recent = File.GetLastWriteTimeUtc(Stage.GetTileJSON(idx));
            }
        }

        private void buttonChooseExisting_Click(object sender, EventArgs e)
        {
            ChooseExistingTile(storageDir, numCols, numRows, (x, y) => {
                numericUpDownTileCol.Value = x;
                numericUpDownTileRow.Value = y;
            });
        }
    }
}
