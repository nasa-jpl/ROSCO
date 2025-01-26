using System;
using System.Collections.Generic;

namespace RockCollect
{

    public class CombinedRockListParser : RockListParser
    {
        public int GetColumnHeaderLineIndex() { return 7; }

        static private string columnHeader = "id,rockXAve,rockYAve,rockWidthAve,rockHeightAve,shaXAve,shaYAve,shaAreaAve,shaLenAve,scoreAve,gradMeanAve,CompactAve,ExentAve,rockXStd,rockYStd,rockWidthStd,rockHeightStd,shaXStd,shaYStd,shaAreaStd,shaLenStd,scoreStd,gradMeanStd,CompactStd,ExentStd";
        public string GetColumnHeadersString()
        {
            return columnHeader;
        }

        public string FileExtension() { return ".csv"; }

        public ParamList ReadHeader(string[] fileContents)
        {
            Console.WriteLine("Warning: unable to build param list for combined rocklist filetype.");
            return null;
        }

        public string WriteHeader(ParamList paramList)
        {
            return "%\n%\n%\n%\n%\n%\n%\n";
        }

        public void ParseRocks(string[] fileContents, out Dictionary<int, List<Rock>> rocksByHash,
            out Dictionary<int, Rock> rocksById, out List<string> invalidRocks)
        {
            rocksByHash = new Dictionary<int, List<Rock>>();
            rocksById = new Dictionary<int, Rock>();
            invalidRocks = new List<string>();

            for (int idxCurLine = GetColumnHeaderLineIndex() + 1; idxCurLine < fileContents.Length; idxCurLine++)
            {
                string curLine = fileContents[idxCurLine];

                try
                {
                    Rock curRock = ReadRock(curLine);

                    int hash = curRock.SimpleHash();
                    if (!rocksByHash.ContainsKey(hash))
                        rocksByHash[hash] = new List<Rock>();

                    rocksByHash[hash].Add(curRock);
                    rocksById.Add(curRock.Id, curRock);
                }
                catch (System.FormatException)
                {
                    invalidRocks.Add(curLine);
                }
            }
        }

        private static readonly char[] separator = new char[] { ',' };

        public Rock ReadRock(string line)
        {
            string[] vars = line.Split(separator);
            Rock rock = new Rock();
            rock.Id = int.Parse(vars[0]);
            rock.RockX = float.Parse(vars[1]);
            rock.RockY = float.Parse(vars[2]);
            rock.RockWidth = float.Parse(vars[3]);
            rock.RockHeight = float.Parse(vars[4]);
            rock.ShaX = float.Parse(vars[5]);
            rock.ShaY = float.Parse(vars[6]);
            rock.ShaArea = float.Parse(vars[7]);
            rock.ShaLen = float.Parse(vars[8]);
            rock.Score = float.Parse(vars[9]);
            rock.GradMean = float.Parse(vars[10]);
            rock.Compact = float.Parse(vars[11]);
            rock.Extent = float.Parse(vars[12]);
            rock.rockXStd = float.Parse(vars[13]);
            rock.rockYStd = float.Parse(vars[14]);
            rock.rockWidthStd = float.Parse(vars[15]);
            rock.rockHeightStd = float.Parse(vars[16]);
            rock.shaXStd = float.Parse(vars[17]);
            rock.shaYStd = float.Parse(vars[18]);
            rock.shaAreaStd = float.Parse(vars[19]);
            rock.shaLenStd = float.Parse(vars[20]);
            rock.scoreStd = float.Parse(vars[21]);
            rock.gradMeanStd = float.Parse(vars[22]);
            rock.CompactStd = float.Parse(vars[23]);
            rock.ExentStd = float.Parse(vars[24]);

            //calculated
            const int tileSize = 500;
            rock.TileC = (int)(rock.RockX / tileSize);
            rock.TileR = (int)(rock.RockY / tileSize);
            rock.TileShaX = rock.RockX - (rock.TileC * tileSize);
            rock.TileShaY = rock.RockY - (rock.TileR * tileSize);

            //not defined
            rock.Class = 0;
            rock.Gamma = 0;

            return rock;
        }

        public string WriteRock(Rock rock)
        {
            string sep = new string(separator);
            return "   " + rock.Id + sep +
                "   " + rock.RockX + sep +
                "   " + rock.RockY + sep +
                "   " + rock.RockWidth + sep +
                "   " + rock.RockHeight + sep +
                "   " + rock.ShaX + sep +
                "   " + rock.ShaY + sep +
                "   " + rock.ShaArea + sep +
                "   " + rock.ShaLen + sep +
                "   " + rock.Score + sep +
                "   " + rock.GradMean + sep +
                "   " + rock.Compact + sep +
                "   " + rock.Extent + sep +
                "   " + rock.rockXStd + sep +
                "   " + rock.rockYStd + sep +
                "   " + rock.rockWidthStd + sep +
                "   " + rock.rockHeightStd + sep +
                "   " + rock.shaXStd + sep +
                "   " + rock.shaYStd + sep +
                "   " + rock.shaAreaStd + sep +
                "   " + rock.shaLenStd + sep +
                "   " + rock.scoreStd + sep +
                "   " + rock.gradMeanStd + sep +
                "   " + rock.CompactStd + sep +
                "   " + rock.ExentStd;
        }
    }
}