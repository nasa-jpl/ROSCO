using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using RockCollect;

namespace RockCollect.Stages
{
    public partial class TileSelectUI : UserControl
    {
        TileSelect Stage;

        public TileSelectUI(TileSelect stage)
        {
            InitializeComponent();
            Stage = stage;
            stage.OnTeardownUI = () => {
                System.Drawing.Image old = this.pictureBoxTile.Image;
                if (old != null)
                {
                    //attempt to avoid intermittent System.ArgumentException: Parameter is not valid.
                    this.pictureBoxTile.Image = null;
                    old.Dispose();
                }
            };
            textBox.Text = "Storage folder: " + stage.GetFinalOutputDirectory();
        }

        private void RefreshInitialUI()
        {
            this.labelImageName.Text = Stage.GetImageName();

            Stage.GetWidthHeightPixels(out int widthPixels, out int heightPixels);
            this.labelResolutionVal.Text = string.Format("{0} x {1}", widthPixels, heightPixels);

            int horizTiles = Stage.GetNumTilesHorizontal();
            int vertTiles = Stage.GetNumTilesVertical();
            this.labelTotalTilesVal.Text = string.Format("{0} ({1} x {2})",
                                                         horizTiles * vertTiles, horizTiles, vertTiles);

            this.labelRemainingVal.Text = Stage.GetRemainingTilesToTune().ToString();

            this.labelSkippedTiles.Text = Stage.GetSkippedTiles().ToString();

            this.labelTunedTiles.Text = Stage.CountTunedTiles().ToString();

            this.labelGroupVal.Text = "";

            this.labelRunnableTiles.Text = Stage.CountRunnableTiles().ToString();

            EnableCopySettings(Stage.GetActiveTile() >= 0);
        }

        public void RefreshSelectedUI()
        {
            bool selectedTile = Stage.GetActiveTileAddress(out int tileCol, out int tileRow);
            if (selectedTile)
            {
                this.labelSelectedTileVal.Text = string.Format("{0}, {1}", tileCol, tileRow);

                System.Drawing.Image old = this.pictureBoxTile.Image;
                if (old != null)
                {
                    this.pictureBoxTile.Image = null;
                    old.Dispose();
                }

                Bitmap activeTileBmp = Stage.GetActiveTileBitmap();                                
                this.pictureBoxTile.Image = activeTileBmp;
                
                Stage.GetActiveTileResolution(out int widthPixels, out int heightPixels);
                this.labelSelectedTilePixelsVal.Text = string.Format("{0} x {1}", widthPixels, heightPixels);

                this.labelRemainingVal.Text = Stage.GetRemainingTilesToTune().ToString();
                
                this.labelSkippedTiles.Text = Stage.GetSkippedTiles().ToString();

                this.labelGroupVal.Text = Stage.GetActiveTileGroup();
            }
            else
            {
                this.labelSelectedTileVal.Text = "";

                System.Drawing.Image old = this.pictureBoxTile.Image;
                if (old != null)
                {
                    this.pictureBoxTile.Image = null;
                    old.Dispose();
                }

                this.labelSelectedTilePixelsVal.Text = "";
                this.labelGroupVal.Text = "";
            }
            this.labelRemainingVal.Text = Stage.GetRemainingTilesToTune().ToString();
        }

        private void TileSelectUI_Load(object sender, EventArgs e)
        {
            RefreshInitialUI();
            RefreshSelectedUI();
        }

        private void buttonManualChooseTile_Click(object sender, EventArgs e)
        {
            using (ChooseTile dialog = new ChooseTile(Stage))
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    Stage.ChooseTile(dialog.GetTileCol(), dialog.GetTileRow());
                    RefreshSelectedUI();
                }
            }
        }

        private void buttonAutoChooseTile_Click(object sender, EventArgs e)
        {
            Stage.AutoChooseTile();
            RefreshSelectedUI();
        }

        public void EnableCopySettings(bool enable)
        {
            buttonCopySettingsManual.Enabled = enable;
            buttonCopySettingsClosest.Enabled = enable;
            buttonCopySettingsMostRecent.Enabled = enable;
        }

        private void buttonCopySettingsManual_Click(object sender, EventArgs e)
        {
            int activeTile = Stage.GetActiveTile();
            if (activeTile < 0) return;
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = Stage.GetFinalOutputDirectory();
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
                        if ((x >= 0 && x < Stage.GetTilesHorizontal()) && (y >=0 && y < Stage.GetTilesVertical()))
                        {
                            Stage.CopySettings(Stage.GetTileIndex(x, y), activeTile, confirm: true);
                        }
                        else
                        {
                            MessageBox.Show(
                                string.Format("Invalid tile (col={0}, row={1}), must be in range (0, 0) to ({2}, {3})",
                                              x, y, Stage.GetTilesHorizontal() - 1, Stage.GetTilesVertical() - 1),
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

        private void buttonCopySettingsFromClosest_Click(object sender, EventArgs e)
        {
            if (Stage.GetActiveTileAddress(out int x, out int y))
            {
                int idx = Stage.GetClosestTunedTile(Stage.GetTileIndex(x, y), (i) => File.Exists(Stage.GetTileJSON(i)));
                if (idx >= 0)
                {
                    Stage.CopySettings(idx, Stage.GetActiveTile(), confirm: true);
                }
                else
                {
                    MessageBox.Show(string.Format("No closest tuned tile found to tile ({0}, {1}).", x, y),
                                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void buttonCopySettingsFromMostRecent_Click(object sender, EventArgs e)
        {
            int idx = Stage.GetMostRecentlyTunedTile();
            if (idx >= 0)
            {
                Stage.CopySettings(idx, Stage.GetActiveTile());
            }
                else
                {
                    MessageBox.Show("No most recently tuned tile found.",
                                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
        }

        private void TileSelectUI_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible == true)
            {
                RefreshInitialUI();
                RefreshSelectedUI();
            }
        }

        private void numericUpDownSkips_ValueChanged(object sender, EventArgs e)
        {
            Stage.SetSkips((int)numericUpDownSkips.Value);
            this.labelRemainingVal.Text = Stage.GetRemainingTilesToTune().ToString();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.InitialDirectory = Directory.GetCurrentDirectory();
                saveFileDialog.Filter = "Rocklist (*.txt)|*.txt|All files (*.*)|*.*";
                saveFileDialog.FilterIndex = 1;
                saveFileDialog.RestoreDirectory = false;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Stage.SaveRocklist(saveFileDialog.FileName);
                }
            }
        }
    }
}
