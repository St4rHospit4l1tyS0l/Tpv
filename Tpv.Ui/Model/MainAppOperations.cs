using System;

namespace Tpv.Ui.Model
{
    public class MainAppOperations
    {
        public Action<string> ShowError { get; set; }
        public Action DisableControls { get; set; }
        public Action<ResponseValidationModel, int, string> ShowResponse { get; set; }
        public Action ClearAll { get; set; }
        public Action CloseAll { get; set; }
    }
}