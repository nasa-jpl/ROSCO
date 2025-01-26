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
    public partial class TileSelectUI : UserControl
    {
        TileSelect Stage;

        public TileSelectUI(TileSelect stage)
        {
            InitializeComponent();
            Stage = stage;
            stage.OnTeardownUI = () => {
                if (this.pictureBoxTile.Image != null)
                {
                    //attempt to avoid intermittent System.ArgumentException: Parameter is not valid.
                    this.pictureBoxTile.Image.Dispose();
                    this.pictureBoxTile.Image = null;
                }
            };
        }

        private void RefreshInitialUI()
        {
            this.labelImageName.Text = Stage.GetImageName();

            Stage.GetWidthHeightPixels(out int widthPixels, out int heightPixels);
            this.labelResolutionVal.Text = string.Format("{0} x {1}", widthPixels, heightPixels);

            int horizTiles = Stage.GetNumTilesHorizontal();
            int vertTiles = Stage.GetNumTilesVertical();
            this.labelTotalTilesVal.Text = string.Format("{0} ({1} x {2})", horizTiles * vertTiles, horizTiles, vertTiles);

            this.labelRemainingVal.Text = Stage.GetRemainingTilesToTune().ToString();

            this.labelTilesToVisitVal.Text = Stage.GetTilesToVisit().ToString();

            this.labelGroupVal.Text = "";

            this.labelRunnableTiles.Text = Stage.CountRunnableTiles().ToString();
            this.labelTunedTiles.Text = Stage.CountTunedTiles().ToString();
        }

        private void RefreshSelectedUI()
        {
            bool selectedTile = Stage.GetActiveTileAddress(out int tileCol, out int tileRow);
            if (selectedTile)
            {
                this.labelSelectedTileVal.Text = string.Format("{0}, {1}", tileCol, tileRow);

                if (this.pictureBoxTile.Image != null)
                {
                    ((IDisposable)this.pictureBoxTile.Image).Dispose();
                    this.pictureBoxTile.Image = null;
                }

                Bitmap activeTileBmp = Stage.GetActiveTileBitmap();                                
                this.pictureBoxTile.Image = activeTileBmp;
                
                Stage.GetActiveTileResolution(out int widthPixels, out int heightPixels);
                this.labelSelectedTilePixelsVal.Text = string.Format("{0} x {1}", widthPixels, heightPixels);

                this.labelGroupVal.Text = Stage.GetActiveTileGroup();
            }
            else
            {
                this.labelSelectedTileVal.Text = "";

                if (this.pictureBoxTile.Image != null)
                {
                    ((IDisposable)this.pictureBoxTile.Image).Dispose();
                    this.pictureBoxTile.Image = null;
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

        private void buttonChooseTile_Click(object sender, EventArgs e)
        {
            Stage.ChooseTile();
            RefreshSelectedUI();
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
