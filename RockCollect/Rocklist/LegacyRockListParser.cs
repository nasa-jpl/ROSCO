using System;
using System.Collections.Generic;

namespace RockCollect
{
    public class LegacyRockListParser : RockListParser
    {
        static private int idxLineGamma = 1;
        private float gamma;

        public int GetColumnHeaderLineIndex() { return 10; }
        public string GetColumnHeadersString()
        {
            throw new SystemException("Column header for legacy file types not implemented yet");
        }

        public string FileExtension() { return ".txt"; }

        public ParamList ReadHeader(string[] fileContents)
        {
            char[] separator = new char[] { ' ', '%' };
            ParamList p = new ParamList();
            try
            {
                p.GSD_resolution = float.Parse(fileContents[0].Split(separator, StringSplitOptions.RemoveEmptyEntries)[1]);
                p.gamma = float.Parse(fileContents[1].Split(separator, StringSplitOptions.RemoveEmptyEntries)[1]);
                p.sun_azimuth_angle = float.Parse(fileContents[2].Split(separator, StringSplitOptions.RemoveEmptyEntries)[1]);
                p.sun_incidence_angle = float.Parse(fileContents[3].Split(separator, StringSplitOptions.RemoveEmptyEntries)[1]);
                p.min_shadow_size = float.Parse(fileContents[4].Split(separator, StringSplitOptions.RemoveEmptyEntries)[1]);
                p.max_shadow_size = float.Parse(fileContents[5].Split(separator, StringSplitOptions.RemoveEmptyEntries)[1]);
                p.confidence_threshold = float.Parse(fileContents[6].Split(separator, StringSplitOptions.RemoveEmptyEntries)[1]);
                p.min_split_shadow_size = float.Parse(fileContents[7].Split(separator, StringSplitOptions.RemoveEmptyEntries)[1]);
                p.mean_gradient = float.Parse(fileContents[8].Split(separator, StringSplitOptions.RemoveEmptyEntries)[1]);
                p.shadow_excentricity = float.Parse(fileContents[9].Split(separator, StringSplitOptions.RemoveEmptyEntries)[2]);
            }
            catch
            {
                throw;
            }
            return p;
        }

        public string WriteHeader(ParamList paramList)
        {
            throw new SystemException("Writing param list for legacy file types not implemented yet");
        }

        public void ParseRocks(string[] fileContents, out Dictionary<int, List<Rock>> rocksByHash,
            out Dictionary<int, Rock> rocksById, out List<string> invalidRocks)
        {
            gamma = float.Parse(fileContents[idxLineGamma].Split(separator)[1]);

            //skip header
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

                    if (!rocksByHash.ContainsKey(hash) || rocksByHash[hash] == null)
                        rocksByHash.Add(hash, new List<Rock>());

                    rocksByHash[hash].Add(curRock);
                    rocksById.Add(curRock.Id, curRock);
                }
                catch (System.FormatException)
                {
                    invalidRocks.Add(curLine);
                }
            }
        }

        private static readonly char[] separator = new char[] { ' ' };

        public Rock ReadRock(string line)
        {
            string[] vars = line.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            Rock rock = new Rock();

            rock.Id = int.Parse(vars[0]);
            rock.TileR = int.Parse(vars[1]);
            rock.TileC = int.Parse(vars[2]);
            rock.ShaX = float.Parse(vars[3]);
            rock.ShaY = float.Parse(vars[4]);
            rock.RockX = float.Parse(vars[5]);
            rock.RockY = float.Parse(vars[6]);
            rock.TileShaX = float.Parse(vars[7]);
            rock.TileShaY = float.Parse(vars[8]);
            rock.ShaArea = int.Parse(vars[9]);
            rock.ShaLen = float.Parse(vars[10]);
            rock.RockWidth = float.Parse(vars[11]);
            rock.RockHeight = float.Parse(vars[12]);
            rock.Score = float.Parse(vars[13]);
            rock.GradMean = float.Parse(vars[14]);
            rock.Compact = float.Parse(vars[15]);
            rock.Extent = float.Parse(vars[16]);
            rock.Class = int.Parse(vars[17]);

            if (vars.Length == 19)
            {
                rock.Gamma = gamma;
            }

            return rock;
        }
        public string WriteRock(Rock rock)
        {
            throw new System.Exception("Writing legacy rocklists not implemented yet.");
        }
    }
}
