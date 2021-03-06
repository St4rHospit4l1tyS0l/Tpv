﻿using log4net;
using System;
using System.Threading.Tasks;
using Tpv.Ui.Model;

namespace Tpv.Ui.Service
{
    public static class LoyaltyService
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(LoyaltyService));

        public static void ApplyLoyalty(string barCode, MainAppOperations operations)
        {
            operations.DisableControls();

            Task.Factory.StartNew(() =>
            {
                try
                {
                    var model = new PosCheckModel { ClientCode = barCode };
                    if (!PosService.ReadCheckInfo(model))
                        return;

                    if (!IniEnvReaderService.GetTpvAndStoreInfo(model))
                        return;


                    if (model.Pvp <= 0)
                    {
                        MessageExt.ShowErrorMessage("Para poder sumar los puntos, el ticket debe ser cobrado, ya que actualmente tiene un importe de cero.");
                        return;
                    }

                    if (model.Pvp < 2.5m)
                    {
                        MessageExt.ShowErrorMessage("Para poder sumar los puntos, el ticket debe tener un importe mayor o igual a € 2.5.");
                        return;
                    }

                    var urlLoyalty = UrlRestFactory.GetUrlLoyalty();

                    if (RestService.CallLoyaltyPostService(urlLoyalty, model))
                        operations.CloseAll();

                }
                catch (Exception ex)
                {
                    _log.Error(ex.Message + " | " + ex.StackTrace);
                    MessageExt.ShowErrorMessage("No fue posible aplicar puntos de lealtad. Error interno.");
                }
                finally
                {
                    operations.ClearAll();
                }
            });
        }
    }
}
