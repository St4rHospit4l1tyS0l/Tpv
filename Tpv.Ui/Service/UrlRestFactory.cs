using System.Configuration;

namespace Tpv.Ui.Service
{
    public static class UrlRestFactory
    {
        public static string GetUrlLoyalty()
        {
            return BuildUrl("UrlLoyalty");
        }

        private static string BuildUrl(string urlKey, params string[] urlParams)
        {
            return string.Format(ConfigurationManager.AppSettings[urlKey], ConfigurationManager.AppSettings["Domain"], urlParams);
        }
    }
}