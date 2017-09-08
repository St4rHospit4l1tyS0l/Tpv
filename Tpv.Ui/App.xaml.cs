using log4net;
using log4net.Config;
using System;
using System.Windows;
using System.Windows.Threading;
using Tpv.Ui.Model;

namespace Tpv.Ui
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(App));
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            XmlConfigurator.Configure(); //only once

            GlobalParams.ProcessArguments(e.Args);

            //var response = new ResponseMessage();
            //Database.DicPromotions = DbReader.ReadDictionaryFromFile(ConfigurationManager.AppSettings["DatabaseFile"], response);

            //if (response.IsSuccess)
            //{
            //    _log.Info("Catálogo cargado de forma correcta");
            //    return;
            //}
            //_log.Error(response.Message);
            //MessageBox.Show(response.Message, "TPV", MessageBoxButton.OK, MessageBoxImage.Error);
            //Shutdown(0);
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs args)
        {
            _log.Fatal("An unexpected application exception occurred", args.Exception);

            MessageBox.Show("An unexpected exception has occurred. Shutting down the application. Please check the log file for more details. " + args.Exception);

            // Prevent default unhandled exception processing
            args.Handled = true;

            Environment.Exit(0);
        }
    }
}
