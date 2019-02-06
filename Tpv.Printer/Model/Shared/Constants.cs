using System.Collections.Generic;

namespace Tpv.Printer.Model.Shared
{
    public static class Constants
    {
        public const int NULL_ID = -1;
        public const string NOT_DEFINED = "ND";
        public const string APP_TITLE = "TPV";
        public const string POS_INI_FILE = "Aloha.ini";
        public const string INI_FILE = "TpvCfg.ini";

        public const char WILDCARD_CHAR = '%';
        public const char SPLIT_ARRAY_SEPARATOR = '|';
        public const char SPLIT_ARRAY_ARRAY_SEPARATOR = '`';

        public const string RESPONSE_ERROR = "0";

        public const string ZERO_VALUE = "0.00";

        public const string POS_BIN_FOLDER = "BIN";
        public const string POS_DATA_FOLDER = "DATA";

        public const string CONN_STR_TEMPLATE = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=dBASE IV;User ID=Admin;Password=;";

        public class InternalPos
        {
            public const string TAG_CHECKID = "CHECKID";
            public const string DEFAULT_CARD_ID = "4XXXXX";
            public const int FILE_CAT = 2;
            public const int FILE_ITM = 12;

            public const int INTERNAL_LOCALSTATE = 720;
            public const int INTERNAL_CATS_ITEMIDS = 751;
        }

        public class Database
        {
            public const string CAT_ITEM_QUERY = "SELECT CIT.CATEGORY, CIT.ITEMID FROM CIT";
        }

        public class CodeErrors
        {
            public const int NO_ERROR = 0;
            public const int TOKEN_ERROR = 35987;
            public const int GENERAL_ERROR = 50000;
            public const int VALIDATION_ERROR = 51000;
            public const int REFERENCE_ID_ALREADYUSED = 51001;
            public const int INSTANCE_POS_ERROR = 52000;
            public const int INSTANCE_FUNCTION_ERROR = 52001;
            public const int GET_TERMINAL_ERROR = 52002;
            public const int LOGIN_POS_ERROR = 52003;
            public const int ADD_TABLE_POS_ERROR = 52004;
            public const int ADD_CHECK_POS_ERROR = 52005;
            public const int REFRESH_FUNC_POS_ERROR = 52006;
            public const int ADD_ITEMS_POS_ERROR = 52007;
            public const int ADD_ITEM_POS_ERROR = 52008;
            public const int ORDER_ITEMS_POS_ERROR = 52009;
            public const int JOB_POS_ERROR = 52010;
            public const int QUEUE_TABLE_POS_ERROR = 52011;
            public const int APPLY_COMP_POS_ERROR = 52012;
            public const int CLOCKIN_POS_ERROR = 52013;
            public const int APPLY_GLOBAL_COMP_POS_ERROR = 52014;
            public const int EXTRACT_INFO_POS_ERROR = 52015;
            public const int INIT_LOCAL_STATE_ERROR = 52016;
            public const int READ_CHECK_INFO_ERROR = 52017;


            public const int ORDER_NOT_FOUND_ERROR = 70001;
            public const int ORDER_FAILED_FOUND_ERROR = 70002;
            public const int CHECK_ID_NULL_ERROR = 70003;
            public const int PAYMENT_NOT_EQUAL_BALANCE_ERROR = 70004;
            public const int PAYMENT_NOT_EQUAL_BALANCE_IN_POS_ERROR = 70005;
            public const int GET_CHECK_POS_ERROR = 70006;
            public const int APPLY_PAYMENT_POS_ERROR = 70007;
            public const int CANNOT_GET_BALANCE_POS_ERROR = 70009;
            public const int BALANCE_NOT_ZERO_POS_ERROR = 70010;
            public const int CLOSE_CHECK_POS_ERROR = 70011;
            public const int VOID_ORDER_POS_ERROR = 70012;
            public const int DELETE_PAYMENT = 70013;
            public const int VOID_PAYMENTS_NOT_FOUND_ERROR = 70014;
            public const int VOID_ITEM_ERROR = 70015;
            public const int DELETE_COMP = 70016;


            public const int PRINTING_CHECK_ID_NOT_FOUND = 80001;
            public const int PRINTING_NO_ENVIRONMENT_VARIABLE_ERROR = 80002;
            public const int PRINTING_CHECK_NOT_FOUND = 80003;

            public const int GET_INFO_ITEMS_PROMOS_POS_ERROR = 90001;

            public const int CALL_PRINT_POS_ERROR = 90101;
            public const int REFRESH_POS_ERROR = 90102;
            public const int WRITE_CODE_POS_ERROR = 90103;
            public const int READ_CODE_POS_ERROR = 90104;
        }

        public class PrintingTag
        {
            public const string LINEFEED = "LINEFEED";
            public const string CPI = "CPI";
            public const string STYLE = "STYLE";
            public const string PRINTSTYLE = "PRINTSTYLE";
            public const string STOPJOURNAL = "STOPJOURNAL";
            public const string TAG_CODE = "#CODE#";


            public const string PRINTCENTERED = "PRINTCENTERED";
            public const string PRINTLEFTRIGHT = "PRINTLEFTRIGHT";
            public const string PRINTLEFT = "LEFT";
            public const string PRINTRIGHT = "RIGHT";

            public static readonly Dictionary<string, string> CharAlign = new Dictionary<string, string>
            {
                {"LEFT", PRINTLEFT},
                {"CENTER", PRINTCENTERED},
                {"RIGHT", PRINTRIGHT}
            };


            public const int PS_CW_LARGE = 0;
            public const int PS_CW_MEDIUM = 1;
            public const int PS_CW_SMALL = 2;
            public static readonly Dictionary<string, int> CharSize = new Dictionary<string, int>
            {
                {"LARGE", PS_CW_LARGE},
                {"MEDIUM", PS_CW_MEDIUM},
                {"SMALL", PS_CW_SMALL}
            };

            public const int PST_NORMAL = 0;
            public const int PST_EXPANDED_WIDTH = 1;
            public const int PST_EXPANDED_HEIGHT = 2;
            public const int PST_EMPHASIZED = 4;

            public static readonly Dictionary<string, int> CharStyle = new Dictionary<string, int>
            {
                {"NORMAL", PST_NORMAL},
                {"EXPANDED_WIDTH", PST_EXPANDED_WIDTH},
                {"EXPANDED_HEIGHT", PST_EXPANDED_HEIGHT},
                {"EMPHASIZED", PST_EMPHASIZED}
            };

        }

        public class PosAttribute
        {
            public const string CODE_CHRISTMAS_PROMO = "CCP";
        }

        public class Pos
        {
            public const string UNITNUMBER = "UNITNUMBER";
        }
    }
}
