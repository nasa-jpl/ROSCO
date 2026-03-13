using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RockCollect
{
    public class Workflow
    {
        private List<Stage> Stages;
        private int ActiveStageIndex = -1;
        private string SessionDirectory;
        private string FinalOutputDirectory;
        private Panel WorkArea;
        private Logger Log;
        private Form StatusForm;
        public Workflow(string outputDirectory, string sessionDirectory, Panel workArea, Form statusForm,
                        List<Stage> stages)
        {
            SessionDirectory = sessionDirectory;
            Stages = stages;
            WorkArea = workArea;
            StatusForm = statusForm;
            Log = new Logger(Path.Combine(sessionDirectory, "log.txt"));

            for (int idx = 0; idx < Stages.Count; idx++)
            {
                var stage = stages.ElementAt(idx);
                string stageDir = Path.Combine(sessionDirectory,
                                               string.Format("{0}_{1}", idx.ToString("D2"), stage.GetName()));
                stage.Init(Log, stageDir, outputDirectory, this);
            }
        }

        public int AtStage() { return ActiveStageIndex; }

        public bool AtFirstStage() { return ActiveStageIndex == 0; }

        public bool AtLastStage() { return ActiveStageIndex == Stages.Count - 1; }

        public bool AtValidStage() { return ActiveStageIndex >= 0 && ActiveStageIndex < Stages.Count; }

        public void SetFinalOutputDirectory(string dir)
        {
            if (!AtFirstStage())
            {
                throw new Exception("cannot change final output directory after first stage");
            }
            foreach (Stage stage in Stages)
            {
                stage.SetFinalOutputDirectory(dir); 
            }
        }

        public void RefreshDirectoriesForStagesFrom(int stageIndex)
        {
            for(int idxStage = stageIndex; idxStage < Stages.Count(); idxStage++)
            {
                var stage = Stages.ElementAt(idxStage);
                Directory.Delete(stage.GetDirectory(Stage.Dir.Stage), true);
                stage.EnsureDirectories();
            }

            if (stageIndex > 0)
            {
                Files.CopyDirectory(Stages.ElementAt(stageIndex-1).GetDirectory(Stage.Dir.Output),
                                    Stages.ElementAt(stageIndex).GetDirectory(Stage.Dir.Input), false);
            }
        }

        public void NextStage()
        {
            Stage activeStage = AtValidStage() ? Stages.ElementAt(ActiveStageIndex): null;

            if (AtValidStage())
            {
                if (!activeStage.Deactivate(true)) return;
            }

            if (AtFirstStage())
            {
                FinalOutputDirectory = activeStage.GetFinalOutputDirectory();

                if (!Directory.Exists(FinalOutputDirectory))
                {
                    Directory.CreateDirectory(FinalOutputDirectory);
                }
                else
                {
                    Regex tileRegex = new Regex(@"^Tile_\d+_\d+.json$");

                    List<string> existing = Directory.GetFiles(FinalOutputDirectory)
                        .Where(path => tileRegex.IsMatch(Path.GetFileName(path)))
                        .ToList();

                    Console.WriteLine("found {0} existing tile settings", existing.Count);

                    if (existing.Count > 0)
                    {
                        string savePath = null;
                        for (int i = 0; savePath == null; i++)
                        {
                            savePath = string.Format("{0}_SAVE_{1}", FinalOutputDirectory, i);
                            if (Directory.Exists(savePath) || File.Exists(savePath)) savePath = null;
                        }

                        //verify that all existing GSD, AZIMUTH, and INCIDENCE equal the values set in ChooseImage
                        //TileSelect.cs will separately check for invalid or partial tile settings
                        var ci = activeStage as RockCollect.Stages.ChooseImage;
                        float gsd = ci.GetGroundSamplingDistance();
                        float azimuth = ci.GetSubSolarAzimuth();
                        float incidence = ci.GetSolarIncidence();
                        string msg = null;
                        foreach (string file in existing)
                        {
                            try
                            {
                                var data = JsonSerializer.Deserialize<StageData>(File.ReadAllText(file));
                                string reason = CheckFloat(data.Data, "GSD", gsd);
                                if (reason == null)
                                {
                                    reason = CheckFloat(data.Data, "AZIMUTH", azimuth);
                                }
                                if (reason == null)
                                {
                                    reason = CheckFloat(data.Data, "INCIDENCE", incidence);
                                }
                                if (reason != null)
                                {
                                    msg = string.Format("{0} in {1}", reason, file);
                                    break;
                                }
                            }
                            catch (Exception ex)
                            {
                                msg = string.Format("error parsing JSON {0}: {1}", file, ex.Message);
                                break;
                            }
                        }

                        bool move = false;
                        if (msg != null)
                        {
                            MessageBox.Show(
                                string.Format("Existing tile settings at {0} contain invalid settings: {1}.  " +
                                              "Moving them to {2}.", FinalOutputDirectory, msg, savePath),
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            move = true;
                        }
                        else
                        {
                            DialogResult result =
                                MessageBox.Show(string.Format("Use {0} existing tile settings at {1}?  " +
                                                              "If not they will be moved to {2}.",
                                                              existing.Count, FinalOutputDirectory, savePath),
                                                "Use Existing Tiles", MessageBoxButtons.YesNo, MessageBoxIcon.Question); 
                            if (result == DialogResult.No)
                            {
                                move = true;
                            }
                        }
                            
                        if (move)
                        {
                            Directory.Move(FinalOutputDirectory, savePath);
                            Directory.CreateDirectory(FinalOutputDirectory);
                            MessageBox.Show(string.Format("Archived existing tile settings from {0} to {1}.",
                                                          FinalOutputDirectory, savePath),
                                            "Moved Existing Tiles");
                        }
                    }
                }

                Console.WriteLine("saving per-tile settings at \"{0}\"", FinalOutputDirectory);
            }

            Stage nextStage = null;
            if (!AtLastStage())
            {
                ActiveStageIndex++;
                nextStage = Stages.ElementAt(ActiveStageIndex);

                if (activeStage != null)
                {
                    Files.CopyDirectory(activeStage.GetDirectory(Stage.Dir.Output),
                                        nextStage.GetDirectory(Stage.Dir.Input));
                }
            }
            else
            {
                ActiveStageIndex = 1;
                nextStage = Stages.ElementAt(ActiveStageIndex);

                //save final output
                if (activeStage != null)
                {
                    Files.CopyDirectory(activeStage.GetDirectory(Stage.Dir.Output), FinalOutputDirectory, false);
                }

                //refresh directories for repeating stages
                RefreshDirectoriesForStagesFrom(ActiveStageIndex);
            }

            nextStage.Activate(WorkArea, StatusForm, true);
        }

        public void PreviousStage()
        {
            Stage activeStage = null;
            if (AtValidStage() && !AtFirstStage())
            {
                activeStage = Stages.ElementAt(ActiveStageIndex);
                if (!activeStage.Deactivate(false)) return;

                ActiveStageIndex--;
                Stage nextStage = Stages.ElementAt(ActiveStageIndex);

                nextStage.Activate(WorkArea, StatusForm, false);
            }
        }

        private static string CheckFloat(Dictionary<string, string> strings, string key, float expected)
        {
            if (!strings.ContainsKey(key)) return $"{key} not present";
            
            float v;
            try { v = float.Parse(strings[key]); }
            catch (FormatException) { return $"value \"{strings[key]}\" for {key} not a valid float"; }
            
            if (Math.Abs(v - expected) < 0.0001) return null;

            return $"expected {expected} for {key}, got {v}";
        }
    }
}
