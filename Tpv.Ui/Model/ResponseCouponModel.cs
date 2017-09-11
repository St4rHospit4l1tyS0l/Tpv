using System.Runtime.Serialization;

namespace Tpv.Ui.Model
{
    [DataContract]
    public class ResponseCouponModel
    {
        [DataMember(Name = "status")]
        public string Status { get; set; }

        [DataMember(Name = "estado")]
        public int Estado { get; set; }

        [DataMember(Name = "codigo")]
        public string Code { get; set; }

        [DataMember(Name = "nombre")]
        public string Name { get; set; }

        [DataMember(Name = "importe")]
        public string Amount { get; set; }

        public int PromotionCode { get; private set; }

        public bool GetCode()
        {
            int promotionCode;
            if (int.TryParse(Code, out promotionCode))
            {
                PromotionCode = promotionCode;
                return true;
            }

            return false;
        }
    }
}