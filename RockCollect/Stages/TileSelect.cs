using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

using NetTopologySuite.Geometries;
using NetTopologySuite.Features;
using NetTopologySuite.IO.Esri;
using NetTopologySuite.IO.Esri.Dbf;
using NetTopologySuite.IO.Esri.Dbf.Fields;
using NetTopologySuite.IO.Esri.Shapefiles.Writers;

namespace RockCollect.Stages
{
    public class TileSelect : Stage
    {
        public const int TILESIZE = 500;
        public const int TILEOVERLAP = 50;

        protected List<int> remainingTilesToTune;
        protected HashSet<int> skippedTiles = new HashSet<int>();

        static public readonly float DATA_VERSION = 0.1f;

        string ImagePath;
        int HeightPixels = 0;
        int WidthPixels = 0;
        int TilesHorizontal = 0;
        int TilesVertical = 0;
        int ActiveTile = -1;
        Image ActiveImage;
        int Skips = 0;

        class ShapeData
        {
            public int tileX, tileY;
            public bool visit, run;
            public string grp;
        }
        ShapeData[] TileShapeData;

        public override float GetDataVersion() { return DATA_VERSION; }

        public override string GetName()
        {
            return "TileSelect";
        }
        public override UserControl CreateUI()
        {
            return new TileSelectUI(this);
        }

        public void SetSkips(int skips)
        {
            Skips = skips >= 0 ? skips : 0;
        }

        public override bool LoadInput(string directory)
        {
            if (!base.LoadInput(directory)) return false;

            if (!this.inData.Data.ContainsKey("IMAGE_PATH")) return false;

            ImagePath = inData.Data["IMAGE_PATH"];
            if (!File.Exists(ImagePath)) throw new Exception(string.Format("Input image {0} doesn't exist", ImagePath));

            if ((WidthPixels == 0 || HeightPixels == 0) &&
                !GDALSerializer.LoadMetadata(ImagePath, out WidthPixels, out HeightPixels, out int bands,
                                             out Type[] bandDataType))
            {
                return false;
            }

            if (TilesHorizontal == 0 || TilesVertical == 0)
            {
                GetNumTiles(WidthPixels, HeightPixels, out TilesHorizontal, out TilesVertical);
            }
            
            //cull out already tuned tiles
            var alreadyTuned = new HashSet<int>();
            var partiallyTuned = new HashSet<string>();
            for (int y = 0; y < TilesVertical; y++)
            {
                for (int x = 0; x < TilesHorizontal; x++)
                {
                    string file = GetTileJSON(x, y);
                    if (File.Exists(file))
                    {
                        string reason = null;
                        if (ValidTileJSON(file, (r) => { reason = r; })) alreadyTuned.Add(GetTileIndex(x, y));
                        else
                        {
                            Console.WriteLine($"partial/invalid tile JSON \"{file}\": {reason}");
                            partiallyTuned.Add(file);
                        }
                    }
                }
            }

            if (partiallyTuned.Count > 0)
            {
                string dir = Path.Combine(GetFinalOutputDirectory(), "partial_settings");
                var result = MessageBox.Show(
                    string.Format("Found {0} partially or invalidly tuned tiles.  " +
                                  "See console logs for details.  " +
                                  "These will not be considered already tuned, but if you re-tune them, " +
                                  "any valid settings they contain will be used as starting values.  " +
                                  "Do you want to move these {0} files to {1} so they will be fully ignored?",
                                  partiallyTuned.Count, dir),
                    "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                
                if (result == DialogResult.Yes)
                {
                    if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                    foreach (string fromFile in partiallyTuned)
                    {
                        string toFile = Path.Combine(dir, Path.GetFileName(fromFile));
                        if (File.Exists(toFile)) File.Delete(toFile);
                        File.Move(fromFile, toFile);
                    }
                }
            }

            Console.WriteLine(string.Format("found {0} already tuned and {1} partial/invalid tiles in {2}",
                                            alreadyTuned.Count, partiallyTuned.Count, GetFinalOutputDirectory()));
            
            skippedTiles.RemoveWhere(tile => alreadyTuned.Contains(tile));

            remainingTilesToTune = Enumerable.Range(0, TilesHorizontal * TilesVertical).ToList();
            remainingTilesToTune = remainingTilesToTune.Where(tile => !alreadyTuned.Contains(tile)).ToList();
            remainingTilesToTune = remainingTilesToTune.Where(tile => !skippedTiles.Contains(tile)).ToList();

            if (TileShapeData != null)
            {
                var tilesToVisit = new HashSet<int>();
                int nt = TilesHorizontal * TilesVertical;
                for (int i = 0; i < nt; i++)
                {
                    if (TileShapeData[i] != null && TileShapeData[i].visit) tilesToVisit.Add(i);
                }
                remainingTilesToTune = remainingTilesToTune.Where(tile => tilesToVisit.Contains(tile)).ToList();
            }
            else if (inData.Data.ContainsKey("SHAPE_FILE") && !string.IsNullOrEmpty(inData.Data["SHAPE_FILE"]))
            {
                ParseShapeFile(inData.Data["SHAPE_FILE"]);
            }

            return true;
        }

        public override bool SaveOutput()
        {
            if (!base.SaveOutput()) return false;
            
            if (ActiveImage == null) return false;
                
            if (string.IsNullOrEmpty(ImagePath)) return false;

            if (ActiveTile == -1) return false;

            string rawTilePath = Path.Combine(GetDirectory(Dir.Output), "rawtile.pgm");
            GDALSerializer.Save(ActiveImage, rawTilePath, null);
            
            string tilePath = Path.Combine(GetDirectory(Dir.Output), "tile.pgm");
            tilePath = CreateMonoImage(rawTilePath, tilePath, 0); //RED band //TODO: expose the multichannel to single channel approach

            InputToOutput("IMAGE_PATH");
            InputToOutput("COMPARISON_ROCKLIST");
            InputToOutput("GSD");
            InputToOutput("AZIMUTH");
            InputToOutput("INCIDENCE");
            
            GetActiveTileAddress(out int x, out int y);
            this.outData.Data.Add("TILE_PATH", tilePath);
            this.outData.Data.Add("TILE_INDEX", ActiveTile.ToString());
            this.outData.Data.Add("TILE_COL", x.ToString());
            this.outData.Data.Add("TILE_ROW", y.ToString());
            this.outData.Data.Add("TILE_GROUP", GetActiveTileGroup());
            this.outData.Data.Add("TILES_HORIZONTAL", TilesHorizontal.ToString());
            this.outData.Data.Add("TILES_VERTICAL", TilesVertical.ToString());
            
            if (!WriteOutputJSON()) return false;
            
            return true;
        }

        private string CreateMonoImage(string tilePath, string monoPath, int bandSelect)
        {
            if (!File.Exists(tilePath))
            {
                throw new Exception("No tile image");
            }

            if (File.Exists(monoPath))
            {
                File.Delete(monoPath);
            }

            GDALSerializer.LoadMetadata(tilePath, out int widthPixels, out int heightPixels, out int bands, out Type[] dataTypes);
            Image tileImage = GDALSerializer.Load(tilePath, 0, 0, widthPixels, heightPixels);

            if (tileImage == null)
            {
                throw new Exception("failed to load shadow image");
            }

            if (bandSelect >= tileImage.Bands)
            {
                throw new Exception("requested invalid band");
            }

            if (tileImage.Bands > 1)
            {
                Image monoImage = tileImage.CreateFromBand(bandSelect);
                if (!GDALSerializer.Save(monoImage, monoPath, null))
                {
                    throw new Exception("failed to save mono image");
                }
                return monoPath;
            }
            else
            {
                return tilePath;
            }
        }

        public override bool Deactivate(bool forward)
        {
            if (ActiveTile == -1)
            {
                return false;
            }

            return base.Deactivate(forward);
        }

        public void GetTileAddress(int tileIndex, out int tileCol, out int tileRow)
        {
            GetTileAddress(tileIndex, TilesHorizontal, out tileCol, out tileRow);
        }

        public int GetTileIndex(int col, int row)
        {
            return GetTileIndex(col, row, TilesHorizontal);
        }

        public string GetTileOutputName(int tileIndex)
        {
            GetTileAddress(tileIndex, TilesHorizontal, out int tileCol, out int tileRow);
            return GetTileOutputName(tileCol, tileRow);
        }

        public string GetTileJSON(int tileIndex)
        {
            GetTileAddress(tileIndex, TilesHorizontal, out int tileCol, out int tileRow);
            return GetTileJSON(tileCol, tileRow);
        }

        public bool ValidTileJSON(int tileIndex, Action<string> reason = null)
        {
            return ValidTileJSON(GetTileJSON(tileIndex), reason);
        }

        public bool ValidTileJSON(string file, Action<string> reason = null)
        {
            if (!File.Exists(file))
            {
                if (reason != null) reason("file not found");
                return false;
            }
            try
            {
                var data = JsonSerializer.Deserialize<StageData>(File.ReadAllText(file));
                return ValidTileJSON(data.Data, reason);
            }
            catch (Exception ex)
            {
                if (reason != null) reason($"error parsing JSON: {ex.Message}");
                return false;
            }
        }

        public bool ValidTileJSON(Dictionary<string, string> dictionary, Action<string> reason = null)
        {
            string r = RockDetector.Settings.CheckSettings(dictionary);
            if (string.IsNullOrEmpty(r)) return true;
            if (reason != null) reason(r);
            return false;
        }

        public int CountRunnableTiles()
        {
            var tsd = TileShapeData;
            if (tsd == null) return TilesVertical * TilesHorizontal;
            int n = 0;
            for (int y = 0; y < TilesVertical; y++)
            {
                for (int x = 0; x < TilesHorizontal; x++)
                {
                    int idx = GetTileIndex(x, y);
                    if (tsd[idx] != null && tsd[idx].run) n++;
                }
            }
            return n;
        }

        public int CountTunedTiles()
        {
            int n = 0;
            for (int y = 0; y < TilesVertical; y++)
            {
                for (int x = 0; x < TilesHorizontal; x++)
                {
                    if (ValidTileJSON(GetTileJSON(x, y))) n++;
                }
            }
            return n;
        }

        public int GetMostRecentlyTunedTile(DateTime? before = null)
        {
            DateTime? mostRecent = null;
            int ret = -1;
            for (int y = 0; y < TilesVertical; y++)
            {
                for (int x = 0; x < TilesHorizontal; x++)
                {
                    string file = GetTileJSON(x, y);
                    if (ValidTileJSON(file))
                    {
                        DateTime dt = File.GetLastWriteTimeUtc(file);
                        if ((mostRecent == null || dt > mostRecent) && (before == null || dt < before))
                        {
                            mostRecent = dt;
                            ret = GetTileIndex(x, y);
                        }
                    }
                }
            }
            return ret;
        }

        public bool CopySettings(int fromIndex, int toIndex, string subdir = null, bool confirm = false)
        {
            GetTileAddress(fromIndex, out int fromX, out int fromY);
            GetTileAddress(toIndex, out int toX, out int toY);

            if (fromX < 0 || fromY < 0 || fromX >= TilesHorizontal || fromY >= TilesVertical)
            {
                MessageBox.Show(
                    string.Format("Invalid tile to copy from (col={0}, row={1}), must be in range (0, 0) to ({2}, {3})",
                                  fromX, fromY, TilesHorizontal - 1, TilesVertical - 1),
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (toX < 0 || toY < 0 || toX >= TilesHorizontal || toY >= TilesVertical)
            {
                MessageBox.Show(
                    string.Format("Invalid tile to copy to (col={0}, row={1}), must be in range (0, 0) to ({2}, {3})",
                                  toX, toY, TilesHorizontal - 1, TilesVertical - 1),
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (confirm)
            {
                var result = MessageBox.Show(
                    string.Format("Copy settings for tile ({0}, {1}) to tile ({2}, {3})?", fromX, fromY, toX, toY),
                    "Confirmation", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Cancel) return false;
            }

            var tsd = TileShapeData;
            string toGrp = null;
            if (tsd != null && (tsd[fromIndex] != null || tsd[toIndex] != null))
            {
                string fromGrp = tsd[fromIndex] != null ? tsd[fromIndex].grp : null;
                toGrp = tsd[toIndex] != null ? tsd[toIndex].grp : null;
                if (fromGrp != toGrp)
                {
                    var result = MessageBox.Show(
                        string.Format("Tile ({0}, {1}) is in shape file group {2}, are you sure you want to copy its " +
                                      "settings to tile ({3}, {4}) in shape file group {5}?",
                                      fromX, fromY, fromGrp, toX, toY, toGrp),
                        "Confirmation", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                    if (result == DialogResult.Cancel) return false;
                }
            }

            string srcJSON = GetTileJSON(fromIndex);
            string dstJSON = GetTileJSON(toIndex);

            var data = JsonSerializer.Deserialize<StageData>(File.ReadAllText(srcJSON));

            string reason = null;
            if (!ValidTileJSON(data.Data, (r) => { reason = r; }))
            {
                MessageBox.Show(
                    string.Format("Invalid tile to copy from (col={0}, row={1}): {2}", fromX, fromY, reason),
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            data.Data.Remove("TILE_PATH");

            data.Data["COPIED_FROM"] = GetTileOutputName(fromIndex) + ".json";
            data.Data["TILE_INDEX"] = toIndex.ToString();
            data.Data["TILE_COL"] = toX.ToString();
            data.Data["TILE_ROW"] = toY.ToString();

            if (toGrp != null) data.Data["TILE_GROUP"] = toGrp;
            
            if (!string.IsNullOrEmpty(subdir))
            {
                string dir = Path.Combine(GetFinalOutputDirectory(), subdir);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                string dstName = GetTileOutputName(toIndex) + ".json";
                dstJSON = Path.Combine(dir, dstName);
            }

            var writeJSONOpts = new JsonSerializerOptions { WriteIndented = true };
            File.WriteAllText(dstJSON, JsonSerializer.Serialize(data, data.GetType(), writeJSONOpts));

            return true;
        }

        public int GetClosestTunedTile(int tileIndex, Func<int, bool> alreadyTuned)
        {
            GetTileAddress(tileIndex, out int x, out int y);

            var tsd = TileShapeData;

            //search in a ring of increasing radius until a tuned tile is found
            string grp = tsd != null && tsd[tileIndex] != null ? tsd[tileIndex].grp : null;

            int maxRadius = Math.Max(x, y);
            maxRadius = Math.Max(maxRadius, TilesHorizontal - x - 1);
            maxRadius = Math.Max(maxRadius, TilesVertical - y - 1);

//            string grpMsg = grp != null ? (" in shape file group \"" + grp + "\"") : "";
//            Console.WriteLine(string.Format("searching for tuned neighbor of tile at col {0}, row {1}, " +
//                                            "max radius {2}{3}", x, y, maxRadius, grpMsg));
            int ni = -1;
            for (int radius = 1; radius <= maxRadius && ni < 0; radius++)
            {
                //top and bottom rows of ring
                for (int ny = y - radius; ny <= y + radius && ni < 0; ny += 2 * radius)
                {
                    if (ny < 0 || ny >= TilesVertical) continue;
                    for (int nx = x - radius; nx <= x + radius && ni < 0 ; nx += 1)
                    {
                        if (nx < 0 || nx >= TilesHorizontal) continue;
                        ni = GetTileIndex(nx, ny);
                        if (!alreadyTuned(ni)) ni = -1;
                        else if (tsd != null && (tsd[ni] == null || tsd[ni].grp != grp)) ni = -1;
                    }
                }
                //left and right cols of ring
                for (int nx = x - radius; nx <= x + radius && ni < 0; nx += 2 * radius)
                {
                    if (nx < 0 || nx >= TilesHorizontal) continue;
                    for (int ny = y - radius + 1; ny < y + radius && ni < 0; ny += 1)
                    {
                        if (ny < 0 || ny >= TilesVertical) continue;
                        ni = GetTileIndex(nx, ny);
                        if (!alreadyTuned(ni)) ni = -1;
                        else if (tsd != null && (tsd[ni] == null || tsd[ni].grp != grp)) ni = -1;
                    }
                }
            }

            return ni;
        }

        internal void SaveRocklist(string fileName)
        {
            Console.WriteLine(string.Format("running rock detector on all runnable tiles, " +
                                            "saving resulting rocklist to \"{0}\"", fileName));

            int numTiles = TilesVertical * TilesHorizontal;
            var inSettings = new RockDetector.INSETTINGS[numTiles];
            var loaded = new bool[numTiles];

            var tsd = TileShapeData;
            var writeJSONOpts = new JsonSerializerOptions { WriteIndented = true };

            Console.WriteLine(string.Format("loading settings for already-tuned tiles from \"{0}\"",
                                            GetFinalOutputDirectory()));
            int numLoaded = 0;
            for (int y = 0; y < TilesVertical; y++)
            {
                for (int x = 0; x < TilesHorizontal; x++)
                {
                    string tileJSON = GetTileJSON(x, y);
                    if (ValidTileJSON(tileJSON))
                    {
                        Console.WriteLine(string.Format("loading settings for tile at col {0}, row {1} from {2}",
                                                        x, y, tileJSON));
                        var data = JsonSerializer.Deserialize<StageData>(File.ReadAllText(tileJSON));
                        int readX = data.Data.ContainsKey("TILE_COL") ? int.Parse(data.Data["TILE_COL"]) : -1;
                        int readY = data.Data.ContainsKey("TILE_ROW") ? int.Parse(data.Data["TILE_ROW"]) : -1;
                        if (x != readX || y != readY)
                        {
                            System.Windows.Forms.MessageBox.Show(
                                string.Format("JSON settings {0} for tile ({1}, 2}) have TILE_COL={3}, TILE_ROW={4}",
                                              tileJSON, x, y, readX, readY),
                                "Tile Settings Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        var settings = new RockDetector.Settings(data.Data);
                        int idx = GetTileIndex(x, y);
                        inSettings[idx] = RockDetector.CreateInSettings(settings);
                        loaded[idx] = true;
                        numLoaded++;
                        string was = data.Data.ContainsKey("TILE_GROUP") ? data.Data["TILE_GROUP"] : null;
                        if (tsd != null && tsd[idx] != null && was != tsd[idx].grp)
                        {
                            Console.WriteLine(string.Format("updating TILE_GROUP from \"{0}\" to \"{1}\" in {2}",
                                                            was, tsd[idx].grp, tileJSON));
                            data.Data["TILE_GROUP"] = tsd[idx].grp;
                            File.WriteAllText(tileJSON, JsonSerializer.Serialize(data, data.GetType(), writeJSONOpts));
                        } 
                    }
                }
            }
            Console.WriteLine(string.Format("loaded settings for {0} tuned tiles", numLoaded));

            if (tsd != null)
            {
                var loadedPerGroup = new Dictionary<string, int>();
                var runPerGroup = new Dictionary<string, int>();
                for (int y = 0; y < TilesVertical; y++)
                {
                    for (int x = 0; x < TilesHorizontal; x++)
                    {
                        int idx = GetTileIndex(x, y);
                        if (tsd[idx] != null)
                        {
                            string grp = tsd[idx].grp;
                            if (!loadedPerGroup.ContainsKey(grp)) loadedPerGroup[grp] = 0;
                            if (loaded[idx]) loadedPerGroup[grp] = loadedPerGroup[grp] + 1;
                            if (!runPerGroup.ContainsKey(grp)) runPerGroup[grp] = 0;
                            if (tsd[idx].run) runPerGroup[grp] = runPerGroup[grp] + 1;
                        }
                    }
                }
                foreach (string grp in loadedPerGroup.Keys)
                {
                    int nl = loadedPerGroup[grp];
                    Console.WriteLine(string.Format("loaded settings for {0} tuned tiles in shape file group \"{1}\"",
                                                    nl, grp));
                    if (nl == 0 && runPerGroup[grp] > 0)
                    {
                        System.Windows.Forms.MessageBox.Show(
                            string.Format("group \"{0}\" has 0 already tuned tiles but {1} tiles to run",
                                          grp, runPerGroup[grp]),
                            "Shape File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            int nc = 0, nf = 0;
            for (int y = 0; y < TilesVertical; y++)
            {
                for (int x = 0; x < TilesHorizontal; x++)
                {
                    int idx = GetTileIndex(x, y);
                    if (tsd != null && (tsd[idx] == null || !tsd[idx].run))
                    {
                        //Console.WriteLine(string.Format("not running detector for tile at col {0}, row {1}: " +
                        //                                "tile is not marked run=true in shape file", x, y));
                        continue;
                    }
                    if (!loaded[idx])
                    {
                        string grp = tsd != null && tsd[idx] != null ? tsd[idx].grp : null;
                        string grpMsg = grp != null ? (" in shape file group \"" + grp + "\"") : "";
                        int ni = GetClosestTunedTile(idx, (i) => loaded[i]);
                        if (ni >= 0)
                        {
                            GetTileAddress(ni, out int nx, out int ny);
//                            Console.WriteLine(string.Format("copying settings for tuned tile at col {0}, row {1} " +
//                                                            "for tile at col {2}, row {3}{4}", nx, ny, x, y, grpMsg));
                            inSettings[idx] = inSettings[ni];
                            if (CopySettings(ni, idx, "copied_settings")) nc++;
                            else nf++;
                        }
                        else
                        {
                            //MaxShadowArea = 0 will cause RockDetector.detect_per_tile_settings() to ignore the tile
                            Console.WriteLine(string.Format("failed to find tuned tile from which to copy settings " +
                                                            "for tile at col {0}, row {1}{2}", x, y, grpMsg));
                            nf++;
                        }
                    }
                }
            }

            Console.WriteLine(string.Format("copied settings for {0} un-tuned tiles from nearest tuned neighbor{1}",
                                            nc, tsd != null ? " in same shape file group" : ""));

            if (nf > 0)
            {
                var result = MessageBox.Show(
                    string.Format("Failed to copy settings for {0} un-tuned tiles from any tuned neighbor{1}.  " +
                                  "Run remaining tiles anyway?", nf, tsd != null ? " in same shape file group" : ""),
                    "Insufficient Tuned Tiles", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                if (result == DialogResult.No) return;
            }
            
            int nr = 0;
            for (int y = 0; y < TilesVertical; y++)
            {
                for (int x = 0; x < TilesHorizontal; x++)
                {
                    int idx = GetTileIndex(x, y);
                    //MaxShadowArea = 0 will cause RockDetector.detect_per_tile_settings() to ignore the tile
                    if (tsd != null && (tsd[idx] == null || !tsd[idx].run)) inSettings[idx].MaxShadowArea = 0;
                    else if (inSettings[idx].MaxShadowArea > 0) nr++;
                }
            }

            var res = MessageBox.Show(
                string.Format("Running rock detector on {0} runnable of {1} total tiles, " +
                              "saving resulting rocklist to \"{2}\".", nr, numTiles, fileName),
                "Running Rock Detector", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            if (res == DialogResult.Cancel) return;
            
            RockDetector.detect_per_tile_settings(ImagePath, fileName, numTiles, inSettings);
            //TODO: warn

            if (!File.Exists(fileName))
            {
                MessageBox.Show(string.Format("Rocklist file not found \"{0}\"", fileName),
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                RockListToShapeFile(fileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Error converting rocklist {0} to shape file: {1}",
                                              fileName, ex.Message),
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        public string GetImageName()
        {
            return Path.GetFileNameWithoutExtension(ImagePath);
        }

        public int GetRemainingTilesToTune()
        {
            if (remainingTilesToTune == null)
            {
                return 0;
            }

            int ret = remainingTilesToTune.Count();

            if (Skips > 0)
            {
                ret = ret / (Skips + 1);
            }

            return ret;
        }

        public int GetSkippedTiles()
        {
            return skippedTiles.Count();
        }

        public void GetWidthHeightPixels(out int widthPixels, out int heightPixels)
        {
            widthPixels = WidthPixels;
            heightPixels = HeightPixels;
        }

        public int GetTilesHorizontal()
        {
            return TilesHorizontal;
        }

        public int GetTilesVertical()
        {
            return TilesVertical;
        }

        public int GetActiveTile()
        {
            return ActiveTile;
        }

        public string GetActiveTileGroup()
        {
            if (ActiveTile < 0 || TileShapeData == null || TileShapeData[ActiveTile] == null) return "";
            else return TileShapeData[ActiveTile].grp;
        }

        public bool GetActiveTileAddress(out int tileCol, out int tileRow)
        {
            tileCol = 0;
            tileRow = 0;

            if (ActiveTile < 0)
                return false;

            GetTileAddress(ActiveTile, out tileCol, out tileRow);

            return true;
        }

        public bool GetActiveTileResolution(out int widthPixels, out int heightPixels)
        {
            widthPixels = 0;
            heightPixels = 0;

            if (ActiveTile < 0)
                return false;

            widthPixels = ActiveImage.Width;
            heightPixels = ActiveImage.Height;

            return true;
        }

        void GetAvailableTilePixels(int pixelCol, int pixelRow, int imageWidth, int imageHeight,
                                    out int availableWidth, out int availableHeight)
        {
            availableWidth = imageWidth - pixelCol;
            availableHeight = imageHeight - pixelRow;
        }

        void GetTilePixels(int tileCol, int tileRow, int pixelsPerTile, out int pixelCol, out int pixelRow)
        {
            pixelCol = tileCol * pixelsPerTile;
            pixelRow = tileRow * pixelsPerTile;
        }

        public override bool Activate(Panel workArea, Form statusForm, bool forward)
        {
            if (!base.Activate(workArea, statusForm, forward))
            {
                return false;
            }

            if (forward)
            {
                ClearTile();
            }

            return true;
        }

        void GetNumTiles(int pixelsWidth, int pixelsHeight, out int tilesHorizontal, out int tilesVertical)
        {
            tilesHorizontal = (int)Math.Ceiling(pixelsWidth / (double)TILESIZE);
            tilesVertical = (int)Math.Ceiling(pixelsHeight / (double)TILESIZE);
        }

        public int GetNumTilesHorizontal()
        {
            return TilesHorizontal;
        }

        public int GetNumTilesVertical()
        {
            return TilesVertical;
        }

        public Bitmap GetActiveTileBitmap()
        {
            return ActiveImage?.ToBitmap();
        }

        public bool AutoChooseTile()
        {
            if (remainingTilesToTune.Count == 0)
            {
                ClearTile();
                return false;
            }

            int newTile = -1;
            int oldIdx = ActiveTile >= 0 ? remainingTilesToTune.IndexOf(ActiveTile) : -1;
            if (oldIdx < 0) newTile = remainingTilesToTune[0];
            else
            {
                int skip = Skips > 0 ? Skips : 0;
                int newIdx = oldIdx + skip + 1;
                
                if (newIdx >= remainingTilesToTune.Count)
                {
                    ClearTile();
                    return false;
                }
                
                newTile = remainingTilesToTune[newIdx];
                
                skippedTiles.Add(remainingTilesToTune[oldIdx]);
                
                for (int i = 0; i < Skips; i++) skippedTiles.Add(remainingTilesToTune[oldIdx + 1 + i]);
                
                remainingTilesToTune.RemoveRange(oldIdx, 1 + skip);
            }

            GetTileAddress(newTile, out int tileCol, out int tileRow);

            ChooseTile(tileCol, tileRow); //sets ActiveTile

            return true;
        }

        public void ChooseTile(int tileCol, int tileRow)
        {
            if (tileCol < 0 || tileRow < 0 || tileCol >= TilesHorizontal || tileRow >= TilesVertical)
            {
                MessageBox.Show(
                    string.Format("Invalid tile (col={0}, row={1}), must be in range (0, 0) to ({2}, {3})",
                                  tileCol, tileRow, TilesHorizontal - 1, TilesVertical - 1),
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ClearTile();
                return;
            }

            string tileJSON = GetTileJSON(tileCol, tileRow);
            if (File.Exists(tileJSON))
            {
                MessageBox.Show(
                    string.Format("Re-tuning tile ({0}, {1}), will start with {2}settings from {3}",
                                  tileCol, tileRow, ValidTileJSON(tileJSON) ? "" : "partial ", tileJSON),
                    "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            ActiveTile = GetTileIndex(tileCol, tileRow);

            GetTilePixels(tileCol, tileRow, TILESIZE, out int pixelCol, out int pixelRow);
            GetAvailableTilePixels(pixelCol, pixelRow, this.WidthPixels, this.HeightPixels,
                                   out int availableWidth, out int availableHeight);

            Rectangle rect = new Rectangle(pixelCol, pixelRow,
                Math.Min(availableWidth, TILESIZE + TILEOVERLAP), Math.Min(availableHeight, TILESIZE + TILEOVERLAP));

            ActiveImage = GDALSerializer.Load(ImagePath, rect.X, rect.Y, rect.Width, rect.Height);

            (Control as TileSelectUI).EnableCopySettings(true);
        }

        public void ClearTile()
        {
            ActiveImage = null;
            ActiveTile = -1;
            (Control as TileSelectUI).EnableCopySettings(false);
            (Control as TileSelectUI).RefreshSelectedUI();
        }

        public void ParseShapeFile(string shapeFilePath)
        {
            string dbfFilePath = Path.ChangeExtension(shapeFilePath, ".dbf");

            if (!File.Exists(dbfFilePath))
            {
                throw new Exception(string.Format("Shape file {0} doesn't exist", dbfFilePath));
            }

            var dbf = new DbfReader(dbfFilePath);
            Console.WriteLine(string.Format("Loading DBF {0} with {1} fields, {2} records",
                                            dbfFilePath, dbf.Fields.Count, dbf.RecordCount));

            bool hasTileXField = false;
            bool hasTileYField = false;
            bool hasRunField = false, runIsBool = false;
            bool hasVisitField = false, visitIsBool = false;
            bool hasGroupField = false;

            foreach (var f in dbf.Fields)
            {
                //Console.WriteLine(string.Format("DBF {0} field {1} has type {2}", dbfFilePath, f.Name, f.FieldType));
                //tile_num_x N
                //tile_num_y N
                //tl_px_col N
                //tl_px_row N
                //run C
                //visit C
                //group C
                if (f.Name == "tile_num_x" && f.FieldType == DbfType.Numeric)
                {
                    hasTileXField = true;
                }
                else if (f.Name == "tile_num_y" && f.FieldType == DbfType.Numeric)
                {
                    hasTileYField = true;
                }
                else if (f.Name == "run" && (f.FieldType == DbfType.Character || f.FieldType == DbfType.Logical))
                {
                    hasRunField = true;
                    runIsBool = f.FieldType == DbfType.Logical;
                }
                else if (f.Name == "visit" && (f.FieldType == DbfType.Character || f.FieldType == DbfType.Logical))
                {
                    hasVisitField = true;
                    visitIsBool = f.FieldType == DbfType.Logical;
                }
                else if (f.Name == "group" && f.FieldType == DbfType.Character)
                {
                    hasGroupField = true;
                }
            }
                         
            if (!hasTileXField) throw new Exception("missing numeric field tile_num_x in " + dbfFilePath);
            if (!hasTileYField) throw new Exception("missing numeric field tile_num_y in " + dbfFilePath);
            if (!hasRunField) throw new Exception("missing character field run in " + dbfFilePath);
            if (!hasVisitField) throw new Exception("missing character field visit in " + dbfFilePath);
            if (!hasGroupField) throw new Exception("missing character field group in " + dbfFilePath);

            int nt = TilesHorizontal * TilesVertical;
            if (dbf.RecordCount != nt)
            {
                throw new Exception(string.Format("expected {0} rows in {1} for {2}x{3} tiles, got {4} rows",
                                                  nt, dbfFilePath, TilesHorizontal, TilesVertical, dbf.RecordCount));
            }

            TileShapeData = new ShapeData[nt];
            var tilesToVisit = new HashSet<int>();
            int tilesToRun = 0;

            var groups = new HashSet<string>();
            int n = 0;
            foreach (var r in dbf)
            {
                if (r.Count != dbf.Fields.Count)
                {
                    throw new Exception(string.Format("expected {0} fields for record {1} in {2}, got {3} fields",
                                                      dbf.Fields.Count, n, dbfFilePath, r.Count));
                }
                try
                {
                    int x = Convert.ToInt32(r["tile_num_x"]);
                    int y = Convert.ToInt32(r["tile_num_y"]);
                    bool v = DbfParseBool(r, "visit", visitIsBool);
                    bool u = DbfParseBool(r, "run", runIsBool);
                    string g = (string)(r["group"]);
                    int idx = GetTileIndex(x, y);
                    TileShapeData[idx] = new ShapeData { tileX = x, tileY = y, visit = v, run = u, grp = g };
                    if (v) tilesToVisit.Add(idx);
                    if (u) tilesToRun++;
                    groups.Add(g);
                } catch (Exception ex) {
                    System.Windows.Forms.MessageBox.Show(
                        string.Format("Error parsing record {0} in {1}: {2}.", n, dbfFilePath, ex.Message),
                        "Shape File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                n++;
            }

            Console.WriteLine(string.Format("Loaded DBF file {0} with {1} tiles: " +
                                            "{2} groups, {3} tiles to visit, {4} tiles to run",
                                            dbfFilePath, n, groups.Count, tilesToVisit.Count, tilesToRun));

            remainingTilesToTune = remainingTilesToTune.Where(tile => tilesToVisit.Contains(tile)).ToList();
        }
        
        bool DbfParseBool(IAttributesTable record, string fieldName, bool isLogical)
        {
            if (isLogical) return (bool)(record[fieldName]);
            string str = (string)(record[fieldName]);
            if (bool.TryParse(str, out bool b)) return b;
            throw new Exception(string.Format("error parsing \"{0}\" as boolean in field {1}", str, fieldName));
        }
        
        void RockListToShapeFile(string path)
        {
            string shpPath = Path.ChangeExtension(path, ".shp");

            Console.WriteLine(string.Format("Converting rock list {0} to shape file {1}", path, shpPath));

            if (File.Exists(shpPath))
            {
                var result = MessageBox.Show(
                    string.Format("Shape file {0} already exists. Do you want to overwrite it?", shpPath),
                    "Overwrite Existing File", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.No)
                {
                    throw new Exception(string.Format("Not overwriting existing {0}", shpPath));
                }

                File.Delete(shpPath);

                string shxPath = Path.ChangeExtension(path, ".shx");
                if (File.Exists(shxPath))
                {
                    File.Delete(shxPath);
                }

                string dbfPath = Path.ChangeExtension(path, ".dbf");
                if (File.Exists(dbfPath))
                {
                    File.Delete(dbfPath);
                }
            }

            const string expectedColumns = "id, tileR, tileC, shaX, shaY, rockX, rockY, tileShaX, tileShaY, shaArea, shaLen, rockWidth, rockHeight, score, gradMean, Compact, Exent, Class, gamma";

            var rocks = new List<RockDetector.OUTROCK>();
            bool foundColumnHeader = false;
            bool foundGsd = false;
            float gsd = 0.0f;
            int lineNumber = 0;

            foreach (string line in File.ReadLines(path))
            {
                lineNumber++;

                if (string.IsNullOrWhiteSpace(line)) continue;

                if (!foundColumnHeader && line.TrimStart().StartsWith("version"))
                {
                    continue;
                }

                if (!foundColumnHeader && line.TrimStart().StartsWith("%"))
                {
                    if (!foundGsd && line.Contains("GSD_resolution"))
                    {
                        string[] parts = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length < 2)
                        {
                            throw new Exception(string.Format(
                                "Invalid GSD_resolution format at line {0}: expected whitespace followed by a number",
                                lineNumber));
                        }

                        string valueStr = parts[parts.Length - 1];
                        if (!float.TryParse(valueStr, out gsd))
                        {
                            throw new Exception(string.Format("Failed to parse GSD_resolution at line {0}: \"{1}\"",
                                                              lineNumber, valueStr));
                        }

                        foundGsd = true;
                    }
                    continue;
                }

                if (!foundColumnHeader)
                {
                    if (line != expectedColumns)
                    {
                        throw new Exception(string.Format("Column names mismatch at line {0}.\nExpected: {1}\nGot: {2}",
                                                          lineNumber, expectedColumns, line));
                    }

                    if (!foundGsd)
                    {
                        throw new Exception("GSD_resolution header not found in file");
                    }

                    foundColumnHeader = true;
                    continue;
                }

                string[] values = line.Split(',');
                if (values.Length != 19)
                {
                    throw new Exception(string.Format("Expected 19 values at line {0}, got {1}",
                                                      lineNumber, values.Length));
                }

                try
                {
                    var rock = new RockDetector.OUTROCK
                    {
                        id = int.Parse(values[0].Trim()),
                        tileR = int.Parse(values[1].Trim()),
                        tileC = int.Parse(values[2].Trim()),
                        shaX = float.Parse(values[3].Trim()),
                        shaY = float.Parse(values[4].Trim()),
                        rockX = float.Parse(values[5].Trim()),
                        rockY = float.Parse(values[6].Trim()),
                        tileShaX = float.Parse(values[7].Trim()),
                        tileShaY = float.Parse(values[8].Trim()),
                        shaArea = int.Parse(values[9].Trim()),
                        shaLen = float.Parse(values[10].Trim()),
                        rockWidth = float.Parse(values[11].Trim()),
                        rockHeight = float.Parse(values[12].Trim()),
                        score = float.Parse(values[13].Trim()),
                        gradMean = float.Parse(values[14].Trim()),
                        compact = float.Parse(values[15].Trim()),
                        extent = float.Parse(values[16].Trim()),
                        Class = int.Parse(values[17].Trim()),
                        gamma = float.Parse(values[18].Trim())
                    };
                    rocks.Add(rock);
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("Error parsing line {0}: {1}", lineNumber, ex.Message));
                }
            }

            Console.WriteLine(string.Format("Loaded rock list {0} with {1} rocks, gsd={2}", path, rocks.Count(), gsd));

            if (!foundColumnHeader)
            {
                throw new Exception("No column header found in file");
            }

            if (rocks.Count() == 0)
            {
                throw new Exception("Empty rocklist");
            }

            //now write an ESRI shape file containing all the rocks and their metadata
            //the specifics here mimic the functionality of original matlab script rocklist2shapefileNOMAP.m
            //by Marshall Trautman
            //the original script is attached to https://github.com/nasa-jpl/ROSCO/issues/7

            var fields = new List<DbfField>();
            var idField = fields.AddNumericInt32Field("id");
            var tileRField = fields.AddNumericInt32Field("tileR");
            var tileCField = fields.AddNumericInt32Field("tileC");
            var shaXField = fields.AddFloatField("shaX");
            var shaYField = fields.AddFloatField("shaY");
            var rockXField = fields.AddFloatField("rockX");
            var rockYField = fields.AddFloatField("rockY");
            var tileShaXField = fields.AddFloatField("tileShaX");
            var tileShaYField = fields.AddFloatField("tileShaY");
            var shaAreaField = fields.AddNumericInt32Field("shaArea");
            var shaLenField = fields.AddFloatField("shaLen");
            var rockWidthField = fields.AddFloatField("rockWidth");
            var rockHeightField = fields.AddFloatField("rockHeight");
            var scoreField = fields.AddFloatField("score");
            var gradMeanField = fields.AddFloatField("gradMean");
            var compactField = fields.AddFloatField("Compact");
            var extentField = fields.AddFloatField("Extent");
            var classField = fields.AddNumericInt32Field("Class");
            var gammaField = fields.AddFloatField("gamma");
            var diamMField = fields.AddFloatField("DiamM");
            var radiusField = fields.AddFloatField("Radius");
            var radiusMField = fields.AddFloatField("RadiusM");

            Console.WriteLine(string.Format("Saving shape file {0}...", shpPath));

            var options = new ShapefileWriterOptions(ShapeType.Polygon, fields.ToArray());
            using (var writer = Shapefile.OpenWrite(shpPath, options))
            {
                foreach (RockDetector.OUTROCK rock in rocks)
                {
                    const int numSides = 18;
                    var coords = new Coordinate[numSides + 1];
                    double radius = rock.rockWidth / 2.0;
                    for (int i = 0; i <= numSides; i++)
                    {
                        //the negative y coordinate here replicates the functionality of original matlab code
                        //in readrockList.m by Marshall Trautman
                        double angle = i < numSides ? (2.0 * Math.PI * i / numSides) : 0;
                        double x = rock.rockX + radius * Math.Cos(angle);
                        double y = -rock.rockY + radius * Math.Sin(angle);
                        coords[i] = new Coordinate(x, y);
                    }
                    writer.Geometry = new Polygon(new LinearRing(coords));

                    idField.NumericValue = rock.id;
                    tileRField.NumericValue = rock.tileR;
                    tileCField.NumericValue = rock.tileC;
                    shaXField.NumericValue = rock.shaX;
                    shaYField.NumericValue = rock.shaY;
                    rockXField.NumericValue = rock.rockX;
                    rockYField.NumericValue = rock.rockY;
                    tileShaXField.NumericValue = rock.tileShaX;
                    tileShaYField.NumericValue = rock.tileShaY;
                    shaAreaField.NumericValue = rock.shaArea;
                    shaLenField.NumericValue = rock.shaLen;
                    rockWidthField.NumericValue = rock.rockWidth;
                    rockHeightField.NumericValue = rock.rockHeight;
                    scoreField.NumericValue = rock.score;
                    gradMeanField.NumericValue = rock.gradMean;
                    compactField.NumericValue = rock.compact;
                    extentField.NumericValue = rock.extent;
                    classField.NumericValue = rock.Class;
                    gammaField.NumericValue = rock.gamma;
                    diamMField.NumericValue = gsd * rock.rockWidth;
                    radiusField.NumericValue = rock.rockWidth / 2;
                    radiusMField.NumericValue = gsd * rock.rockWidth / 2;

                    writer.Write();
                }
            }

            Console.WriteLine(string.Format("Saved {0} rocks to shape file {1}", rocks.Count(), shpPath));
        }
    }
}
