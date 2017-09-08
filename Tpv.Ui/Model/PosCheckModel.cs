using System;
using System.Web.Script.Serialization;

namespace Tpv.Ui.Model
{
    public class PosCheckModel
    {
        public string ClientCode { get; set; }
        public int TermId { get; set; }
        public int CheckId { get; set; }
        public decimal Pvp { get; set; }
        public decimal PvpPromo { get; set; }
        public string Shop { get; set; }
        public string Tpv { get; set; }
        public int ReadableCheckId { get; set; }
        public string BarCode { get; set; }
        public ResponseCouponModel Response { get; set; }
        public string PostData => new JavaScriptSerializer().Serialize(new
        {
            code = ClientCode,
            ticket = ReadableCheckId,
            tpv = Tpv,
            shop = Shop,
            pvp = Pvp,
            pvpPromo = PvpPromo,
            fxOper = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond
        });

    }
}