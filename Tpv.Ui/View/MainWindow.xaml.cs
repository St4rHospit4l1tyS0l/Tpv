using System;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Windows;
using log4net;
using Tpv.Ui.Model;
using Tpv.Ui.Repository;
using Tpv.Ui.Service;

namespace Tpv.Ui.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(MainWindow));

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            TxtBarCode.Focus();
        }

        private void SearchBtn_Click(object sender, RoutedEventArgs e)
        {
            var barCode = TxtBarCode.Text;
            TxtBarCode.Text = String.Empty;
            try
            {

                if (String.IsNullOrEmpty(barCode) || barCode.Length <= SharedConstants.BAR_CODE_LEN)
                {
                    ShowError("El código de barras es inválido");
                    return;
                }

                int iCode;
                if (int.TryParse(barCode.Substring(0, SharedConstants.BAR_CODE_LEN), out iCode) == false)
                {
                    ShowError("El código de barras no es válido");
                    return;
                }

                if (Database.DicPromotions.Keys.Any(i => i == iCode) == false)
                {
                    ShowError("No existe la promoción en la tienda");
                    return;
                }

                new Thread(() =>
                {
                    try
                    {
                        DisableControls();
                        Log.Info("Validando el código de barras: " + barCode);

                        var resp = RestService.MakeRequest(RestService.CreateRequestToValidate(barCode));

                        ShowResponse(resp, iCode, barCode);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(String.Format("Error al validar el código de barras: {0} | Error: {1}", barCode, ex.Message));
                        MainWnd.Dispatcher.Invoke(new Action(() =>
                        {
                            ErrorTxt.Text = ex.Message;
                            ErrorStPan.Visibility = Visibility.Visible;
                            ProgressStPan.Visibility = Visibility.Collapsed;
                            SearchBtn.IsEnabled = true;
                        }));
                    }
                }).Start();
            }
            catch (Exception ex)
            {
                Log.Error(String.Format("Error general al validar el código de barras: {0} | Error: {1}", barCode, ex.Message));
                ErrorTxt.Text = ex.Message;
                ErrorStPan.Visibility = Visibility.Visible;
            }
        }

        private void ShowError(string error)
        {
            ErrorTxt.Text = error;
            ErrorStPan.Visibility = Visibility.Visible;
            ProgressStPan.Visibility = Visibility.Collapsed;
            SearchBtn.IsEnabled = true;
            PromoStPan.Visibility = Visibility.Collapsed;
            PromoTxt.Text = String.Empty;
            TitlePromoTxt.Text = String.Empty;
        }

        private void DisableControls()
        {
            MainWnd.Dispatcher.Invoke(new Action(() =>
            {
                ErrorStPan.Visibility = Visibility.Collapsed;
                SuccessStPan.Visibility = Visibility.Collapsed;
                ProgressStPan.Visibility = Visibility.Visible;
                SearchBtn.IsEnabled = false;
                PromoStPan.Visibility = Visibility.Collapsed;
                PromoTxt.Text = String.Empty;
                TitlePromoTxt.Text = String.Empty;
            }));
        }

        private void ShowResponse(ResponseValidationModel resp, int iCode, string barCode)
        {
            MainWnd.Dispatcher.Invoke(new Action(() =>
            {
                if (resp != null && String.IsNullOrEmpty(resp.Status) == false)
                {
                    if (resp.Status.Contains(ConfigurationManager.AppSettings["ValidationOkString"]))
                    {
                        SuccessStPan.Visibility = Visibility.Visible;
                        SuccessTxt.Text = resp.Status;
                        PromoStPan.Visibility = Visibility.Visible;
                        PromoTxt.Text = Database.DicPromotions[iCode];
                        TitlePromoTxt.Text = String.Format("Promoción aplicable al código {0}:", barCode);
                        Log.Info(String.Format("Promoción aplicable para el código de barras: {0}. | Respuesta: {1}", barCode, resp.Status));
                    }
                    else
                    {
                        ErrorStPan.Visibility = Visibility.Visible;
                        ErrorTxt.Text = resp.Status;
                        Log.Info(String.Format("Código de barras: {0} no válido. | Respuesta: {1}", barCode, resp.Status));
                    }

                }
                else
                {
                    ErrorStPan.Visibility = Visibility.Visible;
                    ErrorTxt.Text = "No hubo respuesta por parte del servidor";
                    Log.Info(String.Format("Código de barras: {0} no válido. | Respuesta: {1}", barCode, ErrorTxt.Text));
                }

                ProgressStPan.Visibility = Visibility.Collapsed;
                SearchBtn.IsEnabled = true;
            }));
        }

        

    }
}
