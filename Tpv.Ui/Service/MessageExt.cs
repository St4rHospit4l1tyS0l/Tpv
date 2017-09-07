using System;
using System.Windows;
using Tpv.Ui.Model;

namespace Tpv.Ui.Service
{
    public static class MessageExt
    {
        public static void ShowErrorMessage(string message)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    if (Application.Current.MainWindow == null)
                        MessageBox.Show(message, Constants.APP_TITLE, MessageBoxButton.OK, MessageBoxImage.Error);
                    else
                        MessageBox.Show(Application.Current.MainWindow, message, Constants.APP_TITLE, MessageBoxButton.OK, MessageBoxImage.Error);

                }));
            }
            catch
            {
                MessageBox.Show(message, Constants.APP_TITLE, MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        public static void ShowSuccessMessage(string message)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                MessageBox.Show(Application.Current.MainWindow, message, Constants.APP_TITLE, MessageBoxButton.OK, MessageBoxImage.Information);
            }));
        }
    }
}
