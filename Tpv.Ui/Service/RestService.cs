using System;
using System.Configuration;
using System.Net;
using System.Runtime.Serialization.Json;
using log4net;
using Tpv.Ui.Model;

namespace Tpv.Ui.Service
{
    public class RestService
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(RestService));

        public static string CreateRequestToValidate(string queryString)
        {
            var urlRequest = String.Format(ConfigurationManager.AppSettings["UrlToValidate"], queryString);
            return (urlRequest);
        }

        public static string CreateRequestToDelete(string queryString)
        {
            var urlRequest = String.Format(ConfigurationManager.AppSettings["UrlToDelete"], queryString);
            return (urlRequest);
        }

        public static ResponseValidationModel MakeRequest(string requestUrl)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(requestUrl);
                request.Method = "GET";
                request.KeepAlive = false;
                request.Accept = "application/json";

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        Log.Error(String.Format("Server error (HTTP {0}: {1}).", response.StatusCode, response.StatusDescription));
                        return null;
                    }

                    var jsonSerializer = new DataContractJsonSerializer(typeof(ResponseValidationModel));
                    var respStream = response.GetResponseStream();

                    if (respStream == null)
                        return null;

                    var objResponse = jsonSerializer.ReadObject(respStream);
                    var jsonResponse = objResponse as ResponseValidationModel;
                    return jsonResponse;
                }
            }
            catch (Exception e)
            {
                Log.Error("Make request: " + e.Message);
                return null;
            }
        }

    }
}
