using System;

namespace Tpv.Ui.Model
{
    public class ItemTicket
    {
        public string Name { get; set; }
        public bool IsMain { get; set; }
        public double PriceVal { get; set; }
        public string PriceTxt
        {
            get
            {
                return Math.Abs(PriceVal) < 0.00000000001 ? "" : PriceVal.ToString("0.00");
            }
        }

        public ItemModifier ItemModifier { get; set; }
    }
}