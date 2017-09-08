using System.Runtime.Serialization;

namespace Tpv.Ui.Model
{
    [DataContract]
    public class ResponseValidationModel
    {
        [DataMember(Name = "status")]
        public string Status { get; set; }

        [DataMember(Name = "_links")]
        public Links Links { get; set; }

    }


    [DataContract]
    public class Links
    {

        [DataMember(Name = "self")]
        public Self Self { get; set; }
    }

    [DataContract]
    public class Self
    {
        [DataMember(Name = "href")]
        public string Href { get; set; }
    }
}

//{"status":"Codigo de barras validado exitosamente","_links":{"self":{"href":"http:\/\/dunkin.rkpeople.com\/developer\/dunkin_ws\/public\/validar-codigo-barra\/890120861879"}}}

