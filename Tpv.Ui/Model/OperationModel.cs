using System;

namespace Tpv.Ui.Model
{
    public class OperationModel
    {
        public string Title { get; set; }
        public Action<string, MainAppOperations> Operation { get; set; }
    }
}