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

        [DataMember(Name = "code")]
        public string Code { get; set; }

    }
}