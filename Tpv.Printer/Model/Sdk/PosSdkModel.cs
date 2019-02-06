namespace Tpv.Printer.Model.Sdk
{
    public class PosSdkModel
    {
        public int TerminalId { get; set; }
        public int CheckId { get; set; }
        public int ReadableCheckId { get; set; }
        public BalanceModel Balance { get; set; }
    }
}
