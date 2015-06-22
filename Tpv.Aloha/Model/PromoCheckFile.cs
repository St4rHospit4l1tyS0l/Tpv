using System.Collections.Generic;

namespace Tpv.Aloha.Model
{
    public class PromoCheckFile
    {
        public int CheckId { get; set; }
        public List<PromoItemFile> LstPromo { get; set; }
    }
}