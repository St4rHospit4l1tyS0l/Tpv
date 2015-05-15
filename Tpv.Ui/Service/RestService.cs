using System;
using System.Configuration;
using System.Net;
using System.Runtime.Serialization.Json;
using Tpv.Ui.Model;

namespace Tpv.Ui.Service
{
    public class RestService
    {
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
                        throw new Exception(String.Format(
                        "Server error (HTTP {0}: {1}).",
                        response.StatusCode,
                        response.StatusDescription));

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
                Console.WriteLine(e.Message);
                return null;
            }
        }

    }
}
