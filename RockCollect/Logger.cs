using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RockCollect
{
    public class Logger
    {
        StreamWriter sw;

        public Logger(string path)
        {
            sw = new StreamWriter(path);
            sw.AutoFlush = true;
        }

        public void Error(string msg)
        {
            lock (sw)
            {
                sw.WriteLine(string.Format("{0}: {1}: {2}", DateTime.Now, "ERROR", string.IsNullOrEmpty(msg)?  "" : msg));
            }
        }

        public void Warn(string msg)
        {
            lock (sw)
            {
                sw.WriteLine(string.Format("{0}: {1}: {2}", DateTime.Now, "WARN", string.IsNullOrEmpty(msg) ? "" : msg));
            }
        }

        public void Info(string msg)
        {
            lock (sw)
            {
                sw.WriteLine(string.Format("{0}: {1}: {2}", DateTime.Now, "INFO", string.IsNullOrEmpty(msg) ? "" : msg));
            }
        }

        public void Flush()
        {
            sw.Flush();
        }
    }
}
