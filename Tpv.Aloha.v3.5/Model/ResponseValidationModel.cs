namespace Tpv.Aloha.Model
{
    public class ResponseValidationModel
    {
        public string status { get; set; }

        //public Links Links { get; set; }
        
    }

    public class Links
    {

        public Self Self { get; set; }
    }

    public class Self
    {
        public string Href { get; set; }
    }
}


