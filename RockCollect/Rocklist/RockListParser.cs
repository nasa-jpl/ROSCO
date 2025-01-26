using System.Collections.Generic;

namespace RockCollect
{
    public interface RockListParser
    {
        int GetColumnHeaderLineIndex(); // the line of column headings before real data
        string GetColumnHeadersString(); // the contents of the column labels

        string FileExtension();

        ParamList ReadHeader(string[] fileContents);
        string WriteHeader(ParamList paramList);

        void ParseRocks(string[] fileContents, out Dictionary<int, List<Rock>> rocksByHash,
            out Dictionary<int, Rock> rocksById, out List<string> invalidRocks);

        Rock ReadRock(string line);
        string WriteRock(Rock rock);
    }
}
