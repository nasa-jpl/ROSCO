using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RockCollect
{
    public partial class Form1 : Form
    {
        Workflow Workflow;
        string SessionDirectory;
        string OutputDirectory;
        StatusWindow statusWindow;

        public Form1()
        {
            InitializeComponent();
        }

        private void RefreshWorkflowButtons()
        {
            this.buttonNext.Enabled = true;
            this.buttonPrevious.Enabled = !this.Workflow.AtFirstStage();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            OutputDirectory = Path.Combine(Directory.GetCurrentDirectory(), "ImageOutput");

            SessionDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Sessions");
            if (!Directory.Exists(SessionDirectory))
                Directory.CreateDirectory(SessionDirectory);

            DateTime time = DateTime.Now;
            SessionDirectory = Path.Combine(SessionDirectory,
                string.Format("{0}-{1}-{2}_{3}-{4}-{5}",
                time.Year.ToString("D4"), time.Month.ToString("D2"), time.Day.ToString("D2"),
                time.Hour.ToString("D2"), time.Minute.ToString("D2"), time.Second.ToString("D2")));

            if (Directory.Exists(SessionDirectory))
                throw new Exception("Directory already exists");

            Directory.CreateDirectory(SessionDirectory);

            statusWindow = new StatusWindow();
            statusWindow.Show();

            Workflow = new Workflow(OutputDirectory, SessionDirectory, this.panelWorkArea, this.statusWindow,
                new List<Stage>{
                    new Stages.ChooseImage(),
                    new Stages.TileSelect(),
                    new Stages.ImageThreshold(),
                    new Stages.RefineShadows(),
                    new Stages.ReviewRocks()
                });

            this.Workflow.NextStage();
            RefreshWorkflowButtons();
        }

        public void NextStage()
        {
            this.Workflow.NextStage();
            RefreshWorkflowButtons();
        }

        public void PrevStage()
        {
            this.Workflow.PreviousStage();
            RefreshWorkflowButtons();
        }
        private void buttonNext_Click(object sender, EventArgs e)
        {
            NextStage();
        }

        private void buttonPrevious_Click(object sender, EventArgs e)
        {
            PrevStage();
        }
    }
}
