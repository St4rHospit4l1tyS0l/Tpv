using System.Collections.Generic;

namespace Tpv.Aloha.Model
{
    internal class PosCheck
    {
        public int CheckId { get; set; }
        public double SubTotal { get; set; }
        public double Tax { get; set; }
        public double Total { get; set; }
        public List<ItemModel> LstItems { get; set; }
    }
}