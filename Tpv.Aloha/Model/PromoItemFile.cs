using System;
using System.Globalization;

namespace Tpv.Aloha.Model
{
    public class PromoItemFile
    {
        public int PromoId { get; set; }
        public string BarCode { get; set; }
        public string DateTx { get; set; }

        public DateTime Date
        {
            get
            {
                if (String.IsNullOrEmpty(DateTx))
                    return DateTime.Today.AddDays(-100);
                DateTime dt;
                return DateTime.TryParseExact(DateTx, "yy/MM/dd", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out dt) ? dt : DateTime.Today.AddDays(-100);
            }
        }

    }
}