using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using RockCollect.Stages;

namespace RockCollect
{
    public partial class ChooseTile : Form
    {
        TileSelect Stage;
        DateTime? recent;

        public ChooseTile(TileSelect stage)
        {
            Stage = stage;
            InitializeComponent();
            int maxCol = Stage.GetNumTilesHorizontal() - 1;
            int maxRow = Stage.GetNumTilesVertical() - 1;
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
    }
}
