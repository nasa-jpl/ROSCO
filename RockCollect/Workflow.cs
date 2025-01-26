using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RockCollect
{
    class Workflow
    {
        private List<Stage> Stages;
        private int ActiveStageIndex = -1;
        private string SessionDirectory;
        private string CurImageDirectory;
        private string OutputDirectory;
        private Panel WorkArea;
        private Logger Log;
        private Form StatusForm;
        public Workflow(string outputDirectory, string sessionDirectory, Panel workArea, Form statusForm, List<Stage> stages)
        {
            SessionDirectory = sessionDirectory;
            Stages = stages;
            WorkArea = workArea;
            StatusForm = statusForm;
            OutputDirectory = outputDirectory;
            Log = new Logger(Path.Combine(sessionDirectory, "log.txt"));

            for (int idx = 0; idx < Stages.Count; idx++)
            {
                var stage = stages.ElementAt(idx);
                stage.Init(Log, Path.Combine(sessionDirectory, string.Format("{0}_{1}", idx.ToString("D2"), stage.GetName())), OutputDirectory);
            }
        }

        public bool AtFirstStage() { return ActiveStageIndex == 0; }
        public bool AtLastStage() { return ActiveStageIndex == Stages.Count - 1; }

        public bool AtValidStage() { return ActiveStageIndex >= 0 && ActiveStageIndex < Stages.Count; }

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
                Files.CopyDirectory(Stages.ElementAt(stageIndex-1).GetDirectory(Stage.Dir.Output), Stages.ElementAt(stageIndex).GetDirectory(Stage.Dir.Input), false);
            }
        }

        public void NextStage()
        {
            Stage activeStage = AtValidStage() ? Stages.ElementAt(ActiveStageIndex): null;

            if (AtValidStage())
            {
                if (false == activeStage.Deactivate(true))
                    return;
            }

            if (AtFirstStage())
            {
                //create image directory
                CurImageDirectory = Path.Combine(OutputDirectory, Path.GetFileNameWithoutExtension(activeStage.outData.Data["IMAGE_PATH"]));
                if (!Directory.Exists(CurImageDirectory))
                {
                    Directory.CreateDirectory(CurImageDirectory);
                }
            }

            Stage nextStage = null;
            if (!AtLastStage())
            {
                ActiveStageIndex++;
                nextStage = Stages.ElementAt(ActiveStageIndex);

                if (activeStage != null)
                {
                    Files.CopyDirectory(activeStage.GetDirectory(Stage.Dir.Output), nextStage.GetDirectory(Stage.Dir.Input));
                }
            }
            else
            {
                ActiveStageIndex = 1;
                nextStage = Stages.ElementAt(ActiveStageIndex);

                //save final output
                if (activeStage != null)
                {
                    Files.CopyDirectory(activeStage.GetDirectory(Stage.Dir.Output), CurImageDirectory, false);
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
