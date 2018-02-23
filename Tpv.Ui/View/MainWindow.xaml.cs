using LasaFOHLib67;
using log4net;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using Tpv.Ui.Model;
using Tpv.Ui.Service;

namespace Tpv.Ui.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(MainWindow));
        private string _lastBarCode;

        private readonly Dictionary<OperationMode, OperationModel> _dicOperations
            = new Dictionary<OperationMode, OperationModel>
        {
            {OperationMode.ApplyCoupon, new OperationModel {Title = "Aplicar canjeo de cupón", Operation = CouponService.ApplyCoupon}},
            {OperationMode.Loyalty, new OperationModel {Title = "Aplicar puntos de lealtad", Operation = LoyaltyService.ApplyLoyalty}},
            {OperationMode.Transaction, new OperationModel {Title = "Aplicar canjeo de cupón", Operation = CouponService.ApplyCoupon}}
        };

        private readonly OperationModel _operation;

        public MainWindow()
        {
            InitializeComponent();
            _operation = _dicOperations[GlobalParams.Mode];
            LblTitle.Content = _operation.Title;
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            TxtBarCode.Focus();
        }

        private void SearchBtn_Click(object sender, RoutedEventArgs e)
        {
            _lastBarCode = TxtBarCode.Text;
            TxtBarCode.Text = string.Empty;

            try
            {
                if (string.IsNullOrEmpty(_lastBarCode) || _lastBarCode.Length <= SharedConstants.BAR_CODE_LEN)
                {
                    ShowError("El código de barras es inválido");
                    return;
                }

                _operation.Operation(_lastBarCode, new MainAppOperations
                {
                    ShowError = ShowError,
                    DisableControls = DisableControls,
                    ShowResponse = ShowResponse,
                    ClearAll = ClearAll,
                    CloseAll = CloseAll
                });
            }
            catch (Exception ex)
            {
                _log.Error($"Error general al validar el código de barras: {_lastBarCode} | Error: {ex.Message}");
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
                PromoTxt.Text = string.Empty;
                TitlePromoTxt.Text = string.Empty;
            }));
        }

        private void ShowResponse(ResponseCouponModel resp, string barCode)
        {
            if (resp != null && string.IsNullOrEmpty(resp.Status) == false)
            {
                if (resp.Estado == Constants.RESPONSE_OK)
                {
                    MainWnd.Dispatcher.Invoke(new Action(() => SelectModifiers(resp)));
                    _log.Info($"Promoción aplicable para el código de barras: {barCode}. | Respuesta: {resp.Status}");
                }
                else
                {
                    MainWnd.Dispatcher.Invoke(new Action(() =>
                    {
                        ErrorStPan.Visibility = Visibility.Visible;
                        ErrorTxt.Text = resp.Status;
                        SearchBtn.IsEnabled = true;
                    }));
                    _log.Info($"Código de barras: {barCode} no válido. | Respuesta: {resp.Status}");
                }
            }
            else
            {
                MainWnd.Dispatcher.Invoke(new Action(() =>
                {
                    ErrorStPan.Visibility = Visibility.Visible;
                    ErrorTxt.Text = "No hubo respuesta por parte del servidor";
                    SearchBtn.IsEnabled = true;
                    _log.Info($"Código de barras: {barCode} no válido. | Respuesta: {ErrorTxt.Text}");
                }));
            }

            MainWnd.Dispatcher.Invoke(new Action(() =>
            {
                ProgressStPan.Visibility = Visibility.Collapsed;
                SearchBtn.IsEnabled = true;
            }));
        }

        private void SelectModifiers(ResponseCouponModel resp)
        {
            var dlg = new PromoModWnd
            (
                this,
                resp.PromotionCode,
                $"{resp.Name} {resp.Amount}"//Database.DicPromotions[iCode]
            );

            if (!dlg.IsReady)
                return;

            var response = dlg.ShowDialog();

            if (response.HasValue == false || response.Value == false)
                return;

            //Ingresar los items a Aloha
            TryToAddItemsToAloha(dlg);
        }

        private void TryToAddItemsToAloha(PromoModWnd dlg)
        {
            var iTries = 3;
            do
            {
                if (AddItemsToAloha(dlg))
                {
                    //AddPromoToFile(dlg, barCode);
                    Close();
                    return;
                }

                Thread.Sleep(1000);
            } while (iTries-- > 0);
        }

        //private void AddPromoToFile(PromoModWnd dlg, string barCode)
        //{
        //    var promo = new PromoCheckFile
        //    {
        //        CheckId = dlg.CheckId,
        //        LstPromo = new List<PromoItemFile>{
        //            new PromoItemFile
        //            {
        //                PromoId = dlg.CodeGroupModifier,
        //                BarCode = barCode,
        //                DateTx = DateTime.Now.ToString("yy/MM/dd")
        //            }
        //        }
        //    };

        //    SaveFile(promo);

        //}

        //private void SaveFile(PromoCheckFile promo)
        //{
        //    var iTries = 3;

        //    while (iTries-- >= 0)
        //    {
        //        try
        //        {
        //            var lstPromos = new List<PromoCheckFile>();
        //            var file = ConfigurationManager.AppSettings["MsgPromoFile"];
        //            if (File.Exists(file))
        //            {
        //                try
        //                {
        //                    var fileInfo = File.ReadAllText(file);
        //                    var des = new JavaScriptSerializer().Deserialize<List<PromoCheckFile>>(fileInfo);

        //                    var check = des.FirstOrDefault(e => e.CheckId == promo.CheckId);

        //                    if (check != null)
        //                    {
        //                        if (check.LstPromo.Any(e => e.BarCode == promo.LstPromo[0].BarCode) == false)
        //                            check.LstPromo.Add(promo.LstPromo[0]);
        //                    }
        //                    else
        //                    {
        //                        lstPromos.Add(promo);
        //                    }
        //                }
        //                catch (Exception ex1)
        //                {
        //                    _log.Error($"{ex1.Message} | {ex1.StackTrace}");
        //                    if (lstPromos.Count == 0)
        //                        lstPromos.Add(promo);
        //                }
        //            }
        //            else
        //            {
        //                lstPromos.Add(promo);
        //            }

        //            var ser = new JavaScriptSerializer().Serialize(lstPromos);
        //            File.WriteAllText(file, ser);
        //            break;
        //        }
        //        catch (Exception ex2)
        //        {
        //            _log.Error($"{ex2.Message} | {ex2.StackTrace}");
        //        }
        //    }
        //}


        private bool AddItemsToAloha(PromoModWnd dlg)
        {
            try
            {

                IberFuncs funcs;

                try
                {
                    funcs = new IberFuncsClass();
                }
                catch (Exception ex)
                {
                    ExtractLogError(ex);
                    funcs = (IberFuncs)SdkFactory.GetIberFuncsInstance();
                }

                var parentEntry = funcs.BeginItem(dlg.TermId, dlg.CheckId, dlg.CodeGroupModifier, "", -999999999);

                var ticketItems = dlg.GetTicketItems();
                foreach (var mod in ticketItems)
                {
                    funcs.ModItem(dlg.TermId, parentEntry, mod.ItemModifier.Id, "", -999999999, mod.ModCode);
                }

                funcs.EndItem(dlg.TermId);
                funcs.RefreshCheckDisplay();
                return true;
            }
            catch (Exception ex)
            {
                return ExtractLogError(ex);
            }
        }

        private static bool ExtractLogError(Exception ex)
        {
            try
            {
                PropertyInfo propInfo = ex.GetType().GetProperty("HResult", BindingFlags.Instance | BindingFlags.NonPublic);

                if (propInfo == null)
                {
                    _log.Error(ex.ToString());
                    return false;
                }

                Int32 hresult = Convert.ToInt32(propInfo.GetValue(ex, null));

                if (((hresult >> 16) & 0x07ff) != 0x06) // this is not an Aloha COM Error
                {
                    _log.Error(ex.ToString());
                    return false;
                }

                var erroCode = hresult & 0xFFF;
                _log.Error($"Aloha returned error code {erroCode}");
                return false;
            }
            catch (Exception exIn)
            {
                _log.Error(exIn.Message + " | " + exIn.StackTrace);
                return false;
            }
        }


        private void ApplyBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _log.Info("INICIA baja de código de barras: " + _lastBarCode);

                new Thread(() =>
                {
                    try
                    {
                        MainWnd.Dispatcher.Invoke(new Action(() =>
                        {
                            ApplyBtn.IsEnabled = false;
                            ApplyStPan.Visibility = Visibility.Visible;
                        }));

                        //var resp = RestService.MakeRequest(RestService.CreateRequestToDelete(_lastBarCode));

                        //if (resp == null)
                        //    throw new Exception("No se pudo establecer conexión con el servidor");

                        //var msg = $"La eliminación del código de barras '{_lastBarCode}' se realizó de forma exitosa. Estatus: {resp.Status}";

                        //_log.Info(msg);
                        //_log.Info(" ******" + resp.Status + "******");
                        //MainWnd.Dispatcher.Invoke(new Action(() =>
                        //{
                        //    MessageBox.Show(msg, "TPV", MessageBoxButton.OK, MessageBoxImage.Information);
                        //    Application.Current.Shutdown();
                        //}));

                        ClearAll();
                    }
                    catch (Exception ex)
                    {
                        _log.Error($"Error al aplicar el código de barras: {_lastBarCode} | Error: {ex.Message}");
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
                _log.Error($"Error general al aplicar el código de barras: {_lastBarCode} | Error: {ex.Message}");
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
                ApplyBtn.IsEnabled = true;
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
            CloseAll();
        }

        private void CloseAll()
        {
            MainWnd.Dispatcher.Invoke(new Action(Close));
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
