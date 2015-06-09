using System;

namespace Tpv.Ui.Model
{
    public class ItemModifier
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Price { get; set; }
        public double PriceVal {
            get
            {
                if (String.IsNullOrEmpty(Price))
                    return 0;

                double price = 0;
                return double.TryParse(Price, out price) ? price : 0;
            }
        }
        public string PriceTxt {
            get
            {
                if (Math.Abs(PriceVal) < 0.00000000001)
                    return "";

                return PriceVal.ToString("0.00");
            }
        }

        public ItemGroupModifier GroupModifier { get; set; }
    }
}