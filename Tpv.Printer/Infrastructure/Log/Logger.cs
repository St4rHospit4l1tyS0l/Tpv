using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web.Script.Serialization;
using Tpv.Printer.Infrastructure.Extensions;
using Tpv.Printer.Model.Shared;

namespace Tpv.Printer.Infrastructure.Log
{
    public class Logger
    {
        private static readonly object _lock = new object();
        private static readonly JavaScriptSerializer _serializer;
        public const string FILE_PATH = "TpvPrinter.log";

        //private static string FilePath => string.Format(FILE_PATH, GlobalParams.DayLog.Day);

        static Logger()
        {
            _serializer = new JavaScriptSerializer();
        }

        public static void Log(string message)
        {
            try
            {
                message = message.Trim();
                string appDir;
                if (!Directory.Exists((Environment.GetEnvironmentVariable("LOCALDIR") + "\\TMP")))
                {
                    appDir = Assembly.GetExecutingAssembly().Location;
                    if (appDir.Contains("\\BIN\\"))
                    {
                        if (Directory.Exists((appDir.Substring(0, appDir.LastIndexOf("\\BIN\\", StringComparison.Ordinal)) + "\\TMP")))
                        {
                            appDir = (appDir.Substring(0, appDir.LastIndexOf("\\BIN\\", StringComparison.Ordinal)) + "\\TMP");
                        }
                        else
                        {
                            appDir = appDir.Substring(0, appDir.LastIndexOf("\\", StringComparison.Ordinal));
                        }
                    }
                    else
                    {
                        appDir = appDir.Substring(0, appDir.LastIndexOf("\\", StringComparison.Ordinal));
                    }
                }
                else
                {
                    appDir = (Environment.GetEnvironmentVariable("LOCALDIR") + "\\TMP");
                }

                var xFileName = appDir + $"\\{FILE_PATH}";
                var textFile = new StreamWriter(xFileName, true, Encoding.Unicode);
                textFile.WriteLine(DateTime.Now.ToString(CultureInfo.InvariantCulture) + "   " + message);
                textFile.Close();
            }
            catch (Exception)
            {
                //MessageBox.Show(ex.Message, "Printing intercept", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void Log(Exception ex)
        {
            Log($"{ex.Message} | {ex.StackTrace} | {ex.InnerException?.Message ?? ""}");
        }


        public static void Write(string sMsg)
        {
            lock (_lock)
            {
                try
                {
                    File.AppendAllText(GlobalParams.DebugPathTmpDir, $"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} - {sMsg}" + Environment.NewLine, Encoding.UTF8);
                }
                catch (Exception)
                {
                    //MessageBox.Show(ex.Message + " | " + ex.StackTrace);
                }
            }
        }

        public static void SerializeAndWrite(string url, dynamic contentResponse)
        {
            string json;
            try
            {
                json = _serializer.Serialize(contentResponse);
            }
            catch (Exception e)
            {
                json = e.Message;
            }

            Write($"Operation: {url} | Response: {json}");
        }

        public static void ClearLogBySize()
        {
            DeleteLog(GlobalParams.DebugPathTmpDir);
        }

        private static void DeleteLog(string filename)
        {
            try
            {
                if (!File.Exists(filename)) return;

                if (new FileInfo(filename).Length / (1024 * 1024) < MasterModel.MaxDebugFileSize) return;

                filename.SafeForceRemoveFile();
            }
            catch
            {
                //
            }
        }
    }

}
