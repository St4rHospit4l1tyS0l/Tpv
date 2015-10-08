using System;
using System.IO;
using System.Windows.Forms;
using Tpv.Aloha.Infrastructure.Connector;
using Tpv.Aloha.Model;

namespace Tpv.Aloha.BsLogic
{
    public static class Logger
    {
        private const string FILE_PATH = "LoggerTpv.txt";

        private static string FilePath
        {
            get
            {
                string value;
                if (DbReader.DicConfig.TryGetValue(Constants.PATH_FILE_LOGGER, out value))
                    return value;
                return FILE_PATH;
            }
        }

        public static void Write(string sMsg)
        {
            try
            {
                File.AppendAllText(FilePath, String.Format("{0} - {1}", DateTime.Now.ToString("G"), sMsg) + Environment.NewLine);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " | " + ex.StackTrace);
            }
        }
    }
}
