using System.Collections.Generic;
using System.IO;
using Tpv.Printer.Model.Sdk;

namespace Tpv.Printer.Model.Shared
{
    public static class GlobalParams
    {
        public const string AES_KEY = "EeornBJ4EdLClyYmLWmpkEt46QDTdmIV";

        private static string _pathDatabase;
        public static string IberDir { get; set; }
        public static string DebugPathTmpDir { get; set; }
        public static PosSdkFuncModel PosSdkFunc { get; set; }
        public static string PosDirDbfFiles { get; set; }
        public static List<int> ItemsIds { get; set; }
        public static string Tpv { get; set; }

        public static string ConnStr
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_pathDatabase))
                    return _pathDatabase;

                _pathDatabase = string.Format(Constants.CONN_STR_TEMPLATE, Path.Combine(PosDirDbfFiles, string.Empty));
                return _pathDatabase;
            }
            set { _pathDatabase = value; }
        }

        public static string UnitNumber { get; set; }
        public static Dictionary<int, string> DicItems { get; set; }

    }
}
