using log4net;
using log4net.Config;
using System;
using System.Diagnostics;
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

            KillOtherInstances();

            GlobalParams.ProcessArguments(e.Args);
        }

        private void KillOtherInstances()
        {
            try
            {
                var current = Process.GetCurrentProcess();
                var processes = Process.GetProcessesByName(current.ProcessName);
                foreach (var process in processes)
                {
                    if (process.Id != current.Id)
                    {
                        process.Kill();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
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
