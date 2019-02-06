using System;

namespace Tpv.Printer.Model.Shared
{
    public class ResponseMessage
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }

        public void SetErrorMessage(string msg)
        {
            IsSuccess = false;
            Message = msg;
        }

        public void SetOkMessage(string msg)
        {
            IsSuccess = true;
            Message = msg;
        }

        public void SetErrorMessage(Exception ex)
        {
            SetErrorMessage(ex.Message + " - " + ex.StackTrace);
        }
    }
}
