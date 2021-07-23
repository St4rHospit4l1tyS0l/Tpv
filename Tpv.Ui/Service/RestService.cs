using log4net;
using System;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using Tpv.Ui.Model;

namespace Tpv.Ui.Service
{
    public static class RestService
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(RestService));

        static RestService()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
        }

        public static bool CallLoyaltyPostService(string requestUrl, PosCheckModel model)
        {
            try
            {
                var postData = model.PostData;
                var data = Encoding.UTF8.GetBytes(postData);

                var request = (HttpWebRequest)WebRequest.Create(requestUrl);
                request.Method = "POST";
                request.KeepAlive = false;
                request.Accept = "application/json";
                request.ContentType = " application/json";
                request.ContentLength = data.Length;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                try
                {
                    using (var response = (HttpWebResponse)request.GetResponse())
                    {
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            MessageExt.ShowSuccessMessage("Los puntos de lealtad fueron aplicados de forma exitosa");
                            return true;
                        }

                        _log.Error($"Parámetros empleados que generan el error: {postData}. Url: {requestUrl}");
                        MessageExt.ShowErrorMessage($"No fue posible aplicar puntos de lealtad al cliente debido a: {GetReadableMessageByCode(response.StatusCode)}");
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    _log.Error(ex.Message + " | " + ex.StackTrace);
                    _log.Error($"Parámetros empleados que generan el error: {postData}. Url: {requestUrl}");
                    MessageExt.ShowErrorMessage($"No fue posible aplicar puntos de lealtad. Revise que el código del cliente sea correcto, que tenga un pedido activo, o que tenga conexión a Internet. ERROR: {ex.Message}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message + " | " + ex.StackTrace);
                MessageExt.ShowErrorMessage($"No fue posible hacer la petición al servidor, revise que tenga Internet, que el servicio esté activo. ERROR: {ex.Message}");
                return false;
            }
        }

        private static string GetReadableMessageByCode(HttpStatusCode responseStatusCode)
        {
            switch (responseStatusCode)
            {
                case HttpStatusCode.InternalServerError:
                    return "Error interno del servidor";
                case HttpStatusCode.BadRequest:
                    return "Parámetros incorrectos";
                case HttpStatusCode.NotFound:
                    return "Cliente no encontrado";
                case HttpStatusCode.NotAcceptable:
                    return "Total de la cuenta menor al promedio del pedido";
                default:
                    return $"Error no definido ({responseStatusCode})";
            }
        }


        public static bool CallApplyCouponGetService(string requestUrl, PosCheckModel model)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(requestUrl);
                request.Method = "GET";
                request.KeepAlive = false;
                request.Accept = "application/json";
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                try
                {
                    using (var response = (HttpWebResponse)request.GetResponse())
                    {
                        if (response.StatusCode != HttpStatusCode.OK)
                        {
                            _log.Error($"Parámetros empleados que generan el error: {requestUrl}");
                            MessageExt.ShowErrorMessage($"No fue posible aplicar esta operación: {GetReadableMessageByCode(response.StatusCode)}");
                            return false;
                        }

                        var jsonSerializer = new DataContractJsonSerializer(typeof(ResponseCouponModel));
                        var respStream = response.GetResponseStream();

                        if (respStream == null)
                        {
                            _log.Error($"Parámetros empleados que generan el error: {requestUrl}");
                            MessageExt.ShowErrorMessage($"No se recibió respuesta a la operación.");
                            return false;
                        }

                        var objResponse = jsonSerializer.ReadObject(respStream);
                        model.Response = objResponse as ResponseCouponModel;
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    _log.Error(ex.Message + " | " + ex.StackTrace);
                    _log.Error($"Parámetros empleados que generan el error: {requestUrl}");
                    MessageExt.ShowErrorMessage($"No fue posible aplicar puntos de lealtad. Revise que el código del cliente sea correcto, que tenga un pedido activo, o que tenga conexión a Internet. ERROR: {ex.Message}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message + " | " + ex.StackTrace);
                MessageExt.ShowErrorMessage($"No fue posible hacer la petición al servidor, revise que tenga Internet, que el servicio esté activo. ERROR: {ex.Message}");
                return false;
            }
        }

    }



    /*
    public static string CreateRequestToValidate(string queryString)
    {
        var urlRequest = String.Format(ConfigurationManager.AppSettings["UrlToValidate"], queryString);
        return (urlRequest);
    }

    public static string CreateRequestApplyLoyalty(string queryString)
    {
        var urlRequest = String.Format(ConfigurationManager.AppSettings["UrlToApplyLoyalty"], queryString);
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
                    _log.Error(String.Format("Server error (HTTP {0}: {1}).", response.StatusCode, response.StatusDescription));
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
            _log.Error("Make request: " + e.Message);
            return null;
        }
    }
    */

}
