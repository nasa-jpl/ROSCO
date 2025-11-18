using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace RockCollect
{
    class Util
    {
        public static float RemapValues(float fromValue, float fromMin, float fromMax, float toMin, float toMax)
        {
            float pct = (fromValue - fromMin) / (fromMax - fromMin);
            return (pct * (toMax - toMin) + toMin);
        }
        
        public static void RefreshTrackbar(float stageVal, float stageMin, float stageMax, float stageDisable,
                                           TrackBar trackBar, Label valLabel, CheckBox check)
        {
            if (stageVal == stageDisable)
            {
                valLabel.Text = "Disabled";
                
                trackBar.Value = (int)RemapValues(stageVal, stageMin, stageMax, trackBar.Minimum, trackBar.Maximum);
                
                if (trackBar.Enabled)
                {
                    trackBar.Enabled = false;
                    valLabel.Enabled = false;
                }
                
                if (check != null && check.Checked)
                {
                    check.Checked = false;
                }
            }
            else
            {
                if (!trackBar.Enabled)
                {
                    trackBar.Enabled = true;
                    valLabel.Enabled = true;
                }
                trackBar.Value = (int)RemapValues(stageVal, stageMin, stageMax, trackBar.Minimum, trackBar.Maximum);
                valLabel.Text = stageVal.ToString("F2");
                
                if (check != null && !check.Checked)
                {
                    check.Checked = true;
                }
            }
        }
    }
}

