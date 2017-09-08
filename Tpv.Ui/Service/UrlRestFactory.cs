using System.Configuration;
using Tpv.Ui.Model;

namespace Tpv.Ui.Service
{
    public static class UrlRestFactory
    {
        public static string GetUrlLoyalty()
        {
            return string.Format(ConfigurationManager.AppSettings["UrlLoyalty"], ConfigurationManager.AppSettings["Domain"]);
        }
        public static string GetUrlResume(PosCheckModel model)
        {
            return string.Format(ConfigurationManager.AppSettings["UrlResume"], ConfigurationManager.AppSettings["Domain"], model.BarCode, model.Tpv, model.Shop);
        }
        public static string GetUrlValidate(PosCheckModel model)
        {
            return string.Format(ConfigurationManager.AppSettings["UrlValidate"], ConfigurationManager.AppSettings["Domain"], model.BarCode, model.Tpv, model.Shop);
        }
    }
}