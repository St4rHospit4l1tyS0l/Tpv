using System;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
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
        private string _lastBarCode;

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
            _lastBarCode = TxtBarCode.Text;
            TxtBarCode.Text = String.Empty;
            try
            {

                if (String.IsNullOrEmpty(_lastBarCode) || _lastBarCode.Length <= SharedConstants.BAR_CODE_LEN)
                {
                    ShowError("El código de barras es inválido");
                    return;
                }

                int iCode;
                if (int.TryParse(_lastBarCode.Substring(0, SharedConstants.BAR_CODE_LEN), out iCode) == false)
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
                        Log.Info("Validando el código de barras: " + _lastBarCode);

                        var resp = RestService.MakeRequest(RestService.CreateRequestToValidate(_lastBarCode));

                        ShowResponse(resp, iCode, _lastBarCode);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(String.Format("Error al validar el código de barras: {0} | Error: {1}", _lastBarCode, ex.Message));
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
                Log.Error(String.Format("Error general al validar el código de barras: {0} | Error: {1}", _lastBarCode, ex.Message));
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
                        SearchBtn.IsEnabled = false;
                    }
                    else
                    {
                        ErrorStPan.Visibility = Visibility.Visible;
                        ErrorTxt.Text = resp.Status;
                        Log.Info(String.Format("Código de barras: {0} no válido. | Respuesta: {1}", barCode, resp.Status));
                        SearchBtn.IsEnabled = true;
                    }
                }
                else
                {
                    ErrorStPan.Visibility = Visibility.Visible;
                    ErrorTxt.Text = "No hubo respuesta por parte del servidor";
                    SearchBtn.IsEnabled = true;
                    Log.Info(String.Format("Código de barras: {0} no válido. | Respuesta: {1}", barCode, ErrorTxt.Text));
                }

                ProgressStPan.Visibility = Visibility.Collapsed;
            }));
        }


        private void ApplyBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Log.Info("INICIA baja de código de barras: " + _lastBarCode);

                new Thread(() =>
                {
                    try
                    {
                        MainWnd.Dispatcher.Invoke(new Action(() =>
                        {
                            ApplyBtn.IsEnabled = false;
                            ApplyStPan.Visibility = Visibility.Visible;
                        }));

                        var resp = RestService.MakeRequest(RestService.CreateRequestToDelete(_lastBarCode));

                        if (resp == null)
                            throw new Exception("No se pudo establecer conexión con el servidor");

                        var msg = String.Format("La eliminación del código de barras '{0}' se realizó de forma exitosa. Estatus: {1}", _lastBarCode, resp.Status);

                        Log.Info(msg);
                        Log.Info(" ******" + resp.Status + "******");
                        MainWnd.Dispatcher.Invoke(new Action(() => MessageBox.Show(msg, "TPV", MessageBoxButton.OK, MessageBoxImage.Information)));

                        ClearAll();
                    }
                    catch (Exception ex)
                    {
                        Log.Error(String.Format("Error al aplicar el código de barras: {0} | Error: {1}", _lastBarCode, ex.Message));
                        MainWnd.Dispatcher.Invoke(new Action(() =>
                        {
                            MessageBox.Show("No fue posible conectarse al servidor. Por favor presione el botón 'Aplicar promoción' para intentar de nuevo.", "TPV", MessageBoxButton.OK, MessageBoxImage.Error);
                            ApplyBtn.IsEnabled = true;
                            ApplyStPan.Visibility = Visibility.Collapsed;
                        }));
                    }
                }).Start();
            }
            catch (Exception ex)
            {
                Log.Error(String.Format("Error general al aplicar el código de barras: {0} | Error: {1}", _lastBarCode, ex.Message));
                ErrorTxt.Text = ex.Message;
                ErrorStPan.Visibility = Visibility.Visible;
            }
        }

        private void ClearAll()
        {
            MainWnd.Dispatcher.Invoke(new Action(() =>
            {
                ErrorStPan.Visibility = Visibility.Collapsed;
                SuccessStPan.Visibility = Visibility.Collapsed;
                ProgressStPan.Visibility = Visibility.Collapsed;
                SearchBtn.IsEnabled = true;
                PromoStPan.Visibility = Visibility.Collapsed;
                PromoTxt.Text = String.Empty;
                TitlePromoTxt.Text = String.Empty;
                ErrorTxt.Text = String.Empty;
                ErrorStPan.Visibility = Visibility.Visible;
                ApplyStPan.Visibility = Visibility.Collapsed;
                TxtBarCode.Focus();
            }));

        }

        private void Close(object sender, MouseEventArgs e)
        {
            Close();
        }

        private void Minimize(object sender, MouseButtonEventArgs e)
        {
            MainWnd.Width = 13;
            MainWnd.Height = 20;
            LblMax.Visibility = Visibility.Visible;
        }

        private void Maximize(object sender, MouseButtonEventArgs e)
        {

            MainWnd.Width = 600;
            MainWnd.Height = 420;
            LblMax.Visibility = Visibility.Hidden;
        }
    }
}
