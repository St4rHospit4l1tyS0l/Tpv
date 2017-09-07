using System.Threading.Tasks;
using Tpv.Ui.Model;

namespace Tpv.Ui.Service
{
    public static class LoyaltyService
    {
        public static void ApplyLoyalty(string barCode, MainAppOperations operations)
        {
            operations.DisableControls();

            Task.Factory.StartNew(() =>
            {
                var model = new PosCheckModel { ClientCode = barCode };
                if (!PosService.ReadCheckInfo(model))
                    return;

                if (!IniEnvReaderService.GetTpvAndStoreInfo(model))
                    return;

                var urlLoyalty = UrlRestFactory.GetUrlLoyalty();

                if (!RestService.CallLoyaltyPostService(urlLoyalty, model))
                    operations.ClearAll();
                else
                    operations.CloseAll();

            });
        }
    }
}
