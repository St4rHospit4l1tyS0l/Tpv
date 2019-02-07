namespace Tpv.Printer.Model.Sdk
{
    public class BalanceModel
    {
        public decimal SubTotal { get; set; }
        public decimal Tax { get; set; }
        public decimal Pvp => SubTotal + Tax;
    }
}
