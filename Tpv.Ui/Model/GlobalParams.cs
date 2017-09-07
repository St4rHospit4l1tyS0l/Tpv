namespace Tpv.Ui.Model
{
    public static class GlobalParams
    {
        public static OperationMode Mode { get; set; }

        public static void ProcessArguments(string[] eArgs)
        {
            if (eArgs == null || eArgs.Length == 0)
            {
                Mode = OperationMode.ApplyCoupon;
                return;
            }

            var option = eArgs[0].ToLower();

            switch (option)
            {
                case "-l":
                    Mode = OperationMode.Loyalty;
                    break;
                case "-t":
                    Mode = OperationMode.Transaction;
                    break;
                default:
                    Mode = OperationMode.ApplyCoupon;
                    break;
            }
        }
    }
}
