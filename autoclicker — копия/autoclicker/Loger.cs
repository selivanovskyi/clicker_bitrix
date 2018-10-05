using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace autoclicker
{
    class Loger
    {
        private static string path_logs = @"c:\autoclick_logs";
        private static string date = DateTime.Today.Date.ToString().Substring(0, 10);
        private static StreamWriter sw;

        private static void CheckPath()
        {
            if (!Directory.Exists(path_logs + @"\bot"))
                Directory.CreateDirectory(path_logs + @"\bot");
        }

        private static void WriteLog(string str)
        {
            sw = File.AppendText(path_logs + @"\bot\" + date + ".log");
            sw.WriteLine(str);
            sw.Close();
        }

        public static void StartTime()
        {
            CheckPath();
            string str = DateTime.Now.TimeOfDay.ToString().Substring(0, 8) + "  ==========  Session started  ==========";
            WriteLog("");
            WriteLog(str);
            WriteLog("");
        }

        public static void EndTime()
        {
            CheckPath();
            string str = DateTime.Now.TimeOfDay.ToString().Substring(0, 8) + "  ==========  Session stoped   ==========";
            WriteLog("");
            WriteLog(str);
            WriteLog("");
        }

        public static void SetLog(string str)
        {
            CheckPath();
            str = DateTime.Now.TimeOfDay.ToString().Substring(0, 8) + "  " + str;
            WriteLog(str);

        }
    }
}
