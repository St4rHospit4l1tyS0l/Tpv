using System;
using System.Windows;
using Tpv.Printer.Infrastructure.Log;
using Tpv.Printer.Model.Shared;

namespace Tpv.Printer.Service.Shared
{
    public static class MessageExt
    {
        public static void ShowErrorMessage(string message)
        {
            try
            {
                MessageBox.Show(message, Constants.APP_TITLE, MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.Log(message);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                Logger.Log(message);
                //MessageBox.Show(message, Constants.APP_TITLE, MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        public static void ShowSuccessMessage(string message)
        {
            Logger.Log(message);
        }
    }
}
