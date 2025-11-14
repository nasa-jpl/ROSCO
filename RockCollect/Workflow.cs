using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

        public void SetOutputDirectory(string dir)
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
                if (!activeStage.Deactivate(true))
                {
                    return;
                }
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

                    int numExisting = Directory.GetFiles(FinalOutputDirectory)
                        .Where(path => tileRegex.IsMatch(Path.GetFileName(path)))
                        .Count();

                    Console.WriteLine("found {0} existing tile settings", numExisting);

                    if (numExisting > 0)
                    {
                        DialogResult result =
                            MessageBox.Show(string.Format("Use {0} existing tile settings at {1}?",
                                                          numExisting, FinalOutputDirectory),
                                            "Use Existing Tiles",
                                            MessageBoxButtons.YesNo,
                                            MessageBoxIcon.Question); 

                        if (result == DialogResult.No)
                        {
                            for (int i = 0; true; i++)
                            {
                                string savePath = string.Format("{0}_SAVE_{1}", FinalOutputDirectory, i);
                                if (!Directory.Exists(savePath) && !File.Exists(savePath))
                                {
                                    Directory.Move(FinalOutputDirectory, savePath);
                                    Directory.CreateDirectory(FinalOutputDirectory);
                                    MessageBox.Show(string.Format("Archived existing tile settings from {0} to {1}",
                                                                  FinalOutputDirectory, savePath),
                                                    "Moved Existing Tiles");
                                    break;
                                }
                            }
                        }
                    }
                }

                Console.WriteLine("saving per-tile settings and results at \"{0}\"", FinalOutputDirectory);
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
                if (false == activeStage.Deactivate(false))
                    return;

                ActiveStageIndex--;
                Stage nextStage = Stages.ElementAt(ActiveStageIndex);

                nextStage.Activate(WorkArea, StatusForm, false);
            }
        }
    }
}
