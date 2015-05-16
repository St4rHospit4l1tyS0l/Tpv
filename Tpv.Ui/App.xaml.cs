using System;
using System.Configuration;
using System.Windows;
using System.Windows.Threading;
using log4net;
using log4net.Config;
using Tpv.Ui.Infrastructure.Connector;
using Tpv.Ui.Model;
using Tpv.Ui.Repository;

namespace Tpv.Ui
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(App));
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            XmlConfigurator.Configure(); //only once

            var response = new ResponseMessage();
            Database.DicPromotions = DbReader.ReadDictionaryFromFile(ConfigurationManager.AppSettings["DatabaseFile"], response);

            if (response.IsSuccess)
            {
                Log.Info("Catálogo cargado de forma correcta");
                return;
            }

            Log.Error(response.Message);
            MessageBox.Show(response.Message, "TPV", MessageBoxButton.OK, MessageBoxImage.Error);   
            Shutdown(0);
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs args)
        {
            Log.Fatal("An unexpected application exception occurred", args.Exception);

            MessageBox.Show("An unexpected exception has occurred. Shutting down the application. Please check the log file for more details. " + args.Exception);

            // Prevent default unhandled exception processing
            args.Handled = true;

            Environment.Exit(0);
        }
    }
}
