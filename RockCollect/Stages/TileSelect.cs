using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RockCollect.Stages
{
    public class TileSelect : Stage
    {
        public const int TILESIZE = 500;
        public const int TILEOVERLAP = 50;

        protected List<int> remainingTilesToTune;
        protected List<int> skippedTiles;

        static public readonly float DATA_VERSION = 0.1f;

        string ImagePath;
        int HeightPixels = 0;
        int WidthPixels = 0;
        int TilesHorizontal = 0;
        int TilesVertical = 0;
        int ActiveTile = -1;
        Image ActiveImage;
        int Skips;

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
            bool result = base.LoadInput(directory);
            if (result == false)
                return false;

            if (!this.inData.Data.ContainsKey("IMAGE_PATH"))
                return false;

            ImagePath = inData.Data["IMAGE_PATH"];
            if (!File.Exists(ImagePath))
                throw new Exception(string.Format("Input image {0} doesn't exist", ImagePath));

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
            
            remainingTilesToTune = Enumerable.Range(0, TilesHorizontal * TilesVertical).ToList();
            
            //cull out already tuned tiles
            var alreadyTuned = new HashSet<int>();
            for (int y = 0; y < TilesVertical; y++)
            {
                for (int x = 0; x < TilesHorizontal; x++)
                {
                    if (File.Exists(GetTileJSON(x, y)))
                    {
                        alreadyTuned.Add(GetTileIndex(x, y));
                    }
                }
            }
            
            remainingTilesToTune = remainingTilesToTune.Where(tile => !alreadyTuned.Contains(tile)).ToList();

            if (skippedTiles != null)
            {
                skippedTiles = skippedTiles.Where(tile => !alreadyTuned.Contains(tile)).ToList();
            }
            
            if (inData.Data.ContainsKey("SHAPE_FILE") && !string.IsNullOrEmpty(inData.Data["SHAPE_FILE"]) &&
                TileShapeData == null)
            {
                ParseShapeFile(inData.Data["SHAPE_FILE"]);
            }

            return true;
        }

        public override bool SaveOutput()
        {
            if (base.SaveOutput())
            {
                if (ActiveImage == null)
                {
                    return false;
                }
                
                if (string.IsNullOrEmpty(ImagePath))
                {
                    return false;
                }

                if (ActiveTile == -1)
                {
                    return false;
                }

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

                if (!WriteOutputJSON())
                    return false;

                return true;
            }

            return false;
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
                    if (File.Exists(GetTileJSON(x, y))) n++;
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
                    if (File.Exists(file))
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

        public void CopySettings(int fromIndex, int toIndex, string subdir = null)
        {
            GetTileAddress(fromIndex, out int fromX, out int fromY);
            GetTileAddress(toIndex, out int toX, out int toY);

            var tsd = TileShapeData;
            if (tsd != null && (tsd[fromIndex] != null || tsd[toIndex] != null))
            {
                string fromGrp = tsd[fromIndex] != null ? tsd[fromIndex].grp : null;
                string toGrp = tsd[toIndex] != null ? tsd[toIndex].grp : null;
                if (fromGrp != toGrp)
                {
                    var result = MessageBox.Show(
                        string.Format("Tile ({0}, {1}) is in shape file group {2}, are you sure you want to copy its " +
                                      "settings to tile ({3}, {4}) in shape file group {5}?",
                                      fromX, fromY, fromGrp, toX, toY, toGrp),
                        "Confirmation", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                    if (result == DialogResult.Cancel) return;
                }
            }

            string srcName = GetTileOutputName(fromIndex) + ".json";
            string srcJSON = GetTileJSON(fromIndex);
            string dstJSON = GetTileJSON(toIndex);

            var data = JsonSerializer.Deserialize<StageData>(File.ReadAllText(srcJSON)).Data;

            data.Remove("TILE_PATH");

            data["COPIED_FROM"] = srcName;
            data["TILE_INDEX"] = toIndex.ToString();
            data["TILE_COL"] = toX.ToString();
            data["TILE_ROW"] = toY.ToString();

            if (!string.IsNullOrEmpty(subdir))
            {
                string dir = Path.Combine(GetDirectory(Dir.FinalOutput), subdir);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                string dstName = GetTileOutputName(toIndex) + ".json";
                dstJSON = Path.Combine(dir, dstName);
            }

            var writeJSONOpts = new JsonSerializerOptions { WriteIndented = true };
            File.WriteAllText(dstJSON, JsonSerializer.Serialize(data, data.GetType(), writeJSONOpts));
        }

        public int GetClosestTunedTile(int tileIndex, Func<int, bool> loaded)
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
                        if (!loaded(ni)) ni = -1;
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
                        if (!loaded(ni)) ni = -1;
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
                                            GetDirectory(Dir.FinalOutput)));
            int numLoaded = 0;
            for (int y = 0; y < TilesVertical; y++)
            {
                for (int x = 0; x < TilesHorizontal; x++)
                {
                    string tileJSON = GetTileJSON(x, y);
                    if (File.Exists(tileJSON))
                    {
                        Console.WriteLine(string.Format("loading settings for tile at col {0}, row {1} from {2}",
                                                        x, y, tileJSON));
                        var data = JsonSerializer.Deserialize<StageData>(File.ReadAllText(tileJSON)).Data;
                        var settings = new RockDetector.Settings(data);
                        int idx = GetTileIndex(x, y);
                        inSettings[idx] = RockDetector.CreateInSettings(settings);
                        loaded[idx] = true;
                        numLoaded++;
                        string was = data.ContainsKey("TILE_GROUP") ? data["TILE_GROUP"] : null;
                        if (tsd != null && tsd[idx] != null && was != tsd[idx].grp)
                        {
                            Console.WriteLine(string.Format("updating TILE_GROUP from \"{0}\" to \"{1}\" in {2}",
                                                            was, tsd[idx].grp, tileJSON));
                            data["TILE_GROUP"] = tsd[idx].grp;
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
                            "Shape File Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
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
                            CopySettings(ni, idx, "copied_settings");
                            nc++;
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
            Console.WriteLine(string.Format("failed to copy settings for {0} un-tuned tiles from any tuned neighbor{1}",
                                            nf, tsd != null ? " in same shape file group" : ""));

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

            Console.WriteLine(string.Format("running rock detector on {0} runnable of {1} total tiles, " +
                                            "saving resulting rocklist to \"{2}\"", nr, numTiles, fileName));

            RockDetector.detect_per_tile_settings(ImagePath, fileName, numTiles, inSettings);
            //TODO: warn
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
            if (skippedTiles == null)
            {
                return 0;
            }

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
            if (forward)
            {
                ClearTile();
            }

            if (!base.Activate(workArea, statusForm, forward))
            {
                return false;
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
            if (ActiveTile >= 0 && !File.Exists(GetTileJSON(ActiveTile)))
            {
                if (skippedTiles == null)
                {
                    skippedTiles = new List<int>();
                }

                skippedTiles.Add(ActiveTile);
            }

            ClearTile();

            if (remainingTilesToTune == null || remainingTilesToTune.Count == 0)
            {
                return false;
            }
            
            for (int counter = 0; counter < Skips; counter++)
            {
                if (skippedTiles == null)
                {
                    skippedTiles = new List<int>();
                }
                
                skippedTiles.Add(remainingTilesToTune.First());
                
                remainingTilesToTune.RemoveAt(0);
                if (remainingTilesToTune.Count == 0)
                {
                    return false;
                }
            }
            
            ActiveTile = remainingTilesToTune.First();
            remainingTilesToTune.RemoveAt(0);
            
            GetTileAddress(ActiveTile, out int tileCol, out int tileRow);

            ChooseTile(tileCol, tileRow);

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
        }

        public void ParseShapeFile(string shapeFilePath)
        {
            string dbfFilePath = Path.ChangeExtension(shapeFilePath, ".dbf");

            if (!File.Exists(dbfFilePath))
                throw new Exception(string.Format("Shape file {0} doesn't exist", dbfFilePath));

            var dbf = Kaitai.Dbf.FromFile(dbfFilePath);
            //Console.WriteLine("num fields: " + dbf.Header2.Fields.Count);
            //Console.WriteLine("num records: " + dbf.Records.Count);

            int tileXField = -1;
            int tileYField = -1;
            int runField = -1;
            int visitField = -1;
            int groupField = -1;

            for (int i = 0; i < dbf.Header2.Fields.Count; i++)
            {
                var f = dbf.Header2.Fields[i];
                //Console.WriteLine(f.Name + " " + (char)f.Datatype);
                //tile_num_x N
                //tile_num_y N
                //tl_px_col N
                //tl_px_row N
                //run C
                //visit C
                //group C
                if (f.Name == "tile_num_x" && f.Datatype == 'N') tileXField = i;
                else if (f.Name == "tile_num_y" && f.Datatype == 'N') tileYField = i;
                else if (f.Name == "run" && f.Datatype == 'C') runField = i;
                else if (f.Name == "visit" && f.Datatype == 'C') visitField = i;
                else if (f.Name == "group" && f.Datatype == 'C') groupField = i;
            }

            if (tileXField < 0) throw new Exception("missing numeric field tile_num_x in " + dbfFilePath);
            if (tileYField < 0) throw new Exception("missing numeric field tile_num_y in " + dbfFilePath);
            if (runField < 0) throw new Exception("missing character field run in " + dbfFilePath);
            if (visitField < 0) throw new Exception("missing character field visit in " + dbfFilePath);
            if (groupField < 0) throw new Exception("missing character field group in " + dbfFilePath);

            int nt = TilesHorizontal * TilesVertical;
            if (dbf.Records.Count != nt)
            {
                throw new Exception(string.Format("expected {0} rows in {1} for {2}x{3} tiles, got {4} rows",
                                                  nt, dbfFilePath, TilesHorizontal, TilesVertical, dbf.Records.Count));
            }

            TileShapeData = new ShapeData[nt];
            var tilesToVisit = new HashSet<int>();

            for (int i = 0; i < nt; i++)
            {
                var r = dbf.Records[i];
                if (r.RecordFields.Count != dbf.Header2.Fields.Count)
                {
                    throw new Exception(string.Format("expected {0} fields for record {1} in {2}, got {3} fields",
                                                      dbf.Header2.Fields.Count, i, dbfFilePath, r.RecordFields.Count));
                }
                try
                {
                    int x = DbfParseInt(r.RecordFields[tileXField], "tile_num_x", tileXField);
                    int y = DbfParseInt(r.RecordFields[tileYField], "tile_num_y", tileYField);
                    bool v = DbfParseBool(r.RecordFields[visitField], "visit", visitField);
                    bool u = DbfParseBool(r.RecordFields[runField], "run", runField);
                    string g = DbfParseString(r.RecordFields[groupField]);
                    int idx = GetTileIndex(x, y);
                    TileShapeData[idx] = new ShapeData { tileX = x, tileY = y, visit = v, run = u, grp = g };
                    if (v) tilesToVisit.Add(i);
                } catch (Exception ex) {
                    System.Windows.Forms.MessageBox.Show(
                        string.Format("Error parsing record {0} in {1}: {2}.", i, dbfFilePath, ex.Message),
                        "Shape File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            remainingTilesToTune = remainingTilesToTune.Where(tile => tilesToVisit.Contains(tile)).ToList();
        }
        
        string DbfParseString(byte[] data)
        {
            return System.Text.Encoding.UTF8.GetString(data, 0, data.Length).Trim();
        }
        
        bool DbfParseBool(byte[] data, string fieldName, int fieldNum)
        {
            string str = DbfParseString(data);
            if (bool.TryParse(str, out bool result)) return result;
            throw new Exception(string.Format("error parsing \"{0}\" as bool in field {1} \"{2}\"",
                                              str, fieldNum, fieldName));
        }
        
        int DbfParseInt(byte[] data, string fieldName, int fieldNum)
        {
            string str = DbfParseString(data);
            if (int.TryParse(str, out int result)) return result;
            throw new Exception(string.Format("error parsing \"{0}\" as int in field {1} \"{2}\"",
                                              str, fieldNum, fieldName));
        }
    }
}
