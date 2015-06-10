using System;
using System.Collections.Generic;

namespace Tpv.Ui.Model
{
    public class ItemTicket
    {

        public static Dictionary<int, ModCode> DicModCode = new Dictionary<int, ModCode>
        {
            {1, new ModCode{Name = "Normal", ShortName = ""}},
            {2, new ModCode{Name = "No", ShortName = ""}},
            {3, new ModCode{Name = "Extra", ShortName = "XT"}},
            {4, new ModCode{Name = "Side", ShortName = "SD"}},
            {14, new ModCode{Name = "Light", ShortName = "LT"}},
            {15, new ModCode{Name = "Heavy", ShortName = "HV"}},
            {16, new ModCode{Name = "Only", ShortName = "ONLY"}},
            {17, new ModCode{Name = "Half", ShortName = "1/2"}},
            {18, new ModCode{Name = "Quater", ShortName = "1/4"}},
            {19, new ModCode{Name = "No", ShortName = ""}},
        };

        public string Name { get; set; }

        public string NameFull
        {
            get
            {
                ModCode name;
                return DicModCode.TryGetValue(ModCode, out name) ? name.ShortName + " " + Name : Name;
            }
        }

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
        public int ModCode { get; set; }

        public ItemTicket()
        {
            ModCode = 1;
        }
    }
}