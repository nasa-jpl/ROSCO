using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RockCollect
{
    public class StageData
    {
        public Dictionary<string, string> Data { get; set; }

        public StageData() { Data = new Dictionary<string, string>(); }
            
    }

    public abstract class Stage
    {
        public UserControl Control;
        public UserControl StatusControl;
        private string StageDir;
        private string FinalOutputDir;
        private Panel WorkArea;
        private Form StatusForm;
        private Workflow parentWorkflow;
        protected Logger Log;
        public StageData inData;
        public StageData outData;

        public Action OnTeardownUI;
        public Action OnTeardownStatusUI;

        public enum Dir { Stage, Input, Output, Debug, FinalOutput};
        
        public virtual void Init(Logger log,  string stageDirectory, string finalOutputDirectory, Workflow workflow)
        {
            Log = log;
            StageDir = stageDirectory;
            FinalOutputDir = finalOutputDirectory;
            inData = new StageData();
            outData = new StageData();
            parentWorkflow = workflow;
               
            EnsureDirectories();
        }

        public abstract string GetName();
        public abstract UserControl CreateUI();
        public virtual UserControl CreateStatusUI(UserControl mainUI)
        {
            return null;
        }

        public abstract float GetDataVersion();

        public virtual bool LoadInput(string directory)
        {
            inData.Data = new Dictionary<string, string>();

            string inputPath = GetInputJSONPath();
            if (File.Exists(inputPath))
            {
                var jsonString = File.ReadAllText(inputPath);

                //read the partial version
                inData = JsonSerializer.Deserialize<StageData>(jsonString);

                return true;
            }
            return false;
        }

        protected virtual bool ValidateInputs()
        {
            return false;
        }

        public void SetFinalOutputDirectory(string dir)
        {
            FinalOutputDir = dir;
        }

        public string GetFinalOutputDirectory(string imagePath)
        {
            string dir = FinalOutputDir.TrimEnd(new char[] { '/', '\\' });

            if (string.IsNullOrEmpty(imagePath)) return dir;

            string imageName = Path.GetFileNameWithoutExtension(imagePath);
            if (!dir.EndsWith(imageName)) dir = Path.Combine(dir, imageName);

            return dir;
        }

        public virtual string GetFinalOutputDirectory()
        {
            return GetFinalOutputDirectory(inData.Data.ContainsKey("IMAGE_PATH") ? inData.Data["IMAGE_PATH"] : null);
        }

        public string GetDirectory(Dir dir)
        {
            switch (dir)
            {
                case Dir.Stage: return StageDir;
                case Dir.Input: return Path.Combine(StageDir, "Input");
                case Dir.Output: return Path.Combine(StageDir, "Output");
                case Dir.Debug: return Path.Combine(StageDir, "Logs");
                case Dir.FinalOutput: return GetFinalOutputDirectory();
                default: throw new Exception("Unknown directory type");
            }
        }

        public void EnsureDirectories()
        {
            foreach (Dir dirType in Enum.GetValues(typeof(Dir)))
            {
                string dir = GetDirectory(dirType);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
            }
        }

        //index -> col, row
        public static void GetTileAddress(int index, int numTilesHorizontal, out int tileCol, out int tileRow)
        {
            tileCol = index % numTilesHorizontal;
            tileRow = index / numTilesHorizontal;
        }

        //col, row -> index
        public static int GetTileIndex(int col, int row, int numTilesHorizontal)
        {
            return row * numTilesHorizontal + col;
        }

        public static string GetTileOutputName(int tileCol, int tileRow)
        {
            return string.Format("Tile_{0}_{1}", tileCol.ToString("D6"), tileRow.ToString("D6"));
        }

        public string GetTileJSON(int tileCol, int tileRow)
        {
            return Path.Combine(GetDirectory(Dir.FinalOutput), GetTileOutputName(tileCol, tileRow) + ".json");
        }

        public virtual bool Activate(Panel workArea, Form statusForm, bool forward)
        {
            WorkArea = workArea;
            StatusForm = statusForm;

            LogInfo("Activate");

            LoadInput(GetDirectory(Dir.Input));
            ValidateInputs();
            SetupUI();

            return true;
        }
        
        public virtual bool Deactivate(bool forward)
        {
            if (!SaveOutput()) return false;
            if (!TeardownUI()) return false;
            return true;
        }

        public virtual string GetOutputJSONPath()
        {
           return Path.Combine(GetDirectory(Dir.Output), "output.json");
        }

        public string GetInputJSONPath()
        {
            return Path.Combine(GetDirectory(Dir.Input), "output.json");
        }

        public virtual bool SaveOutput()
        {
            outData.Data = new Dictionary<string, string>();

            this.outData.Data.Add("STAGE", GetName());
            this.outData.Data.Add("VERSION", GetDataVersion().ToString("F2"));
            this.outData.Data.Add("TIMESTAMP", DateTime.Now.ToString());
            this.outData.Data.Add("USER", System.Security.Principal.WindowsIdentity.GetCurrent().Name);

            return true;
        }

        public virtual bool SetupUI()
        {
            if(this.Control == null)
            {
                this.Control = CreateUI();
               
            }

            if (this.StatusControl == null)
            {
                this.StatusControl = CreateStatusUI(this.Control);
            }

            this.Control.Anchor = (AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom);
            this.Control.Size = this.WorkArea.Size;
            this.WorkArea.Controls.Add(this.Control);

            if (this.StatusControl != null)
            {
                this.StatusControl.Anchor = (AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom);
                this.StatusControl.Size = this.StatusForm.Size;
                this.StatusForm.Controls.Add(this.StatusControl);
            }
            return true;
        }

        public virtual bool TeardownUI()
        {
            if (OnTeardownUI != null) OnTeardownUI();

            if (this.Control != null)
            {
                WorkArea.Controls.Remove(this.Control);
            }

            if (OnTeardownStatusUI != null) OnTeardownStatusUI();

            if (this.StatusControl != null)
            {
                this.StatusForm.Controls.Remove(this.StatusControl);
            }

            return true;
        }

        public void LogError(string msg)
        {
            Log.Error(GetName() + ": " + msg);
        }

        public void LogWarn(string msg)
        {
            Log.Warn(GetName() + ":" + msg);
        }

        public void LogInfo(string msg)
        {
            Log.Info(GetName() + ":" + msg);
        }

        public bool WriteOutputJSON()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(outData, outData.GetType(), options);
            File.WriteAllText(this.GetOutputJSONPath(), jsonString);
            return true;
        }

        protected void InputToOutput(string key)
        {
            this.outData.Data.Add(key, this.inData.Data[key]);
        }
    }
}
