using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RockCollect
{
    class Files
    {
       static public void CopyDirectory(string srcDir, string destDir, bool deleteExisting = true)
        {
            if (deleteExisting && Directory.Exists(destDir))
            {
                Directory.Delete(destDir,true);
            }

            if (!Directory.Exists(destDir))
            {
                Directory.CreateDirectory(destDir);
            }

            if (!Directory.Exists(srcDir))
                return;

            DirectoryInfo srcDirInfo = new DirectoryInfo(srcDir);
            FileInfo[] files = srcDirInfo.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDir, file.Name);
                file.CopyTo(temppath, true);
            }

            foreach (DirectoryInfo subdir in srcDirInfo.GetDirectories())
            {
                string temppath = Path.Combine(destDir, subdir.Name);
                CopyDirectory(subdir.FullName, temppath, deleteExisting);
            }
        }
    }
}
