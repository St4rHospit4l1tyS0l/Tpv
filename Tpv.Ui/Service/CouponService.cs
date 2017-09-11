using log4net;
using System;
using System.Threading.Tasks;
using Tpv.Ui.Model;

namespace Tpv.Ui.Service
{
    public static class CouponService
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(CouponService));

        public static void ApplyCoupon(string barCode, MainAppOperations operations)
        {
            int iCode;
            if (int.TryParse(barCode.Substring(0, SharedConstants.BAR_CODE_LEN), out iCode) == false)
            {
                operations.ShowError("El código de barras no es válido");
                return;
            }

            Task.Factory.StartNew(() =>
            {
                try
                {
                    operations.DisableControls();
                    _log.Info("Validando el código de barras: " + barCode);


                    var model = new PosCheckModel { BarCode = barCode };
                    if (!IniEnvReaderService.GetTpvAndStoreInfo(model))
                        return;

                    var urlResume = UrlRestFactory.GetUrlResume(model);
                    if (!RestService.CallApplyCouponGetService(urlResume, model))
                        return;

                    if (model.Response.Estado == Constants.RESPONSE_ERROR)
                    {
                        MessageExt.ShowErrorMessage($"El cupón no es válido debido a: {model.Response.Status}");
                        return;
                    }

                    var urlValidate = UrlRestFactory.GetUrlValidate(model);
                    if (!RestService.CallApplyCouponGetService(urlValidate, model))
                        return;

                    if (!model.Response.GetCode())
                    {
                        MessageExt.ShowErrorMessage($"Código de promoción inválido: {model.Response.Code}");
                        return;
                    }

                    if (model.Response.Estado == Constants.RESPONSE_ERROR)
                    {
                        MessageExt.ShowErrorMessage($"El cupón no se pudo canjear debido a: {model.Response.Status}");
                        return;
                    }

                    operations.ShowResponse(model.Response, barCode);
                }
                catch (Exception ex)
                {
                    _log.Error($"Error al validar el código de barras: {barCode} | Error: {ex.Message}");
                    operations.ShowError(ex.Message);
                }
                finally
                {
                    operations.ClearAll();
                }
            });
        }
    }
}
