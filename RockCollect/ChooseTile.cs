using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RockCollect
{
    public partial class ChooseTile : Form
    {
        public ChooseTile()
        {
            InitializeComponent();
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
            //TODO
        }

        private void numericUpDownTileCol_ValueChanged(object sender, EventArgs e)
        {
            int col = (int)numericUpDownTileCol.Value;
            //TODO enforce range
        }

        private void numericUpDownTileRow_ValueChanged(object sender, EventArgs e)
        {
            int row = (int)numericUpDownTileRow.Value;
            //TODO enforce range
        }
    }
}
