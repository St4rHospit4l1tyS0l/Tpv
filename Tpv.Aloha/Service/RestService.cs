using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using Tpv.Aloha.BsLogic;
using Tpv.Aloha.Infrastructure.Connector;
using Tpv.Aloha.Model;

namespace Tpv.Aloha.Service
{
    public class RestService
    {

        public static string CreateRequestToDelete(string queryString)
        {
            var urlRequest = String.Format(DbReader.UrlToDelete, queryString);
            return (urlRequest);
        }

        public static ResponseValidationModel MakeRequest(string requestUrl)
        {
            try
            {
                Logger.Write("INFO: Consultando a: " + requestUrl);

                var request = (HttpWebRequest)WebRequest.Create(requestUrl);
                request.Method = "GET";
                request.KeepAlive = false;
                request.Accept = "application/json";

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        Logger.Write(String.Format("Server error (HTTP {0}: {1}).", response.StatusCode, response.StatusDescription));
                        return null;
                    }

                    var jsonSerializer = new DataContractJsonSerializer(typeof(ResponseValidationModel));
                    var respStream = response.GetResponseStream();

                    if (respStream == null)
                        return null;

                    /*using (var reader = new StreamReader(respStream))
                    {
                        var info = reader.ReadToEnd();
                        Logger.Write("INFO: Al eliminar la promoción, el resultado del servicio es: " + info);
                    }*/

                    var objResponse = jsonSerializer.ReadObject(respStream);
                    var jsonResponse = objResponse as ResponseValidationModel;

                    if (jsonResponse == null)
                    {
                        Logger.Write("INFO: Al eliminar la promoción, no se recibió una respuesta del servicio, para mostrar.");
                    }
                    else
                    {
                        Logger.Write("INFO: Al eliminar la promoción, el resultado del servicio es: " + jsonResponse.status);
                    }

                    //return new ResponseValidationModel();
                    return jsonResponse;
                }
            }
            catch (Exception e)
            {
                Logger.Write("ERROR: Make request: " + e.Message);
                return null;
            }
        }
    }
}
