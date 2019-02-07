using LasaFOHLib;
using System;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using Tpv.Printer.Infrastructure.Crypto;
using Tpv.Printer.Infrastructure.Log;
using Tpv.Printer.Model.Sdk;
using Tpv.Printer.Model.Shared;
using Tpv.Printer.Service.Sdk;
using Tpv.Printer.Service.Shared;

namespace Tpv.Printer.Com
{

    [Guid("F708AE50-7D43-44ab-8F44-BBCF8A350366")]
    public class InterceptPrinting : IInterceptAlohaPrinting2
    {

        public InterceptPrinting()
        {
            Logger.Log("Constructor Tpv InterceptPrinting");
        }

        [ComRegisterFunction]
        private static void Reg(string regKey)
        {
            Microsoft.Win32.Registry.ClassesRoot.CreateSubKey((regKey.Substring(18) + "\\Implemented Categories\\{D0579B21-8915-4851-B99F-798F51E2A3BB}"));
        }

        [ComUnregisterFunction]
        private static void Unreg(string regKey)
        {
            try
            {
                Microsoft.Win32.Registry.ClassesRoot.DeleteSubKey((regKey.Substring(18) + "\\Implemented Categories\\{D0579B21-8915-4851-B99F-798F51E2A3BB}"));
            }
            catch
            {
                // ignored
            }
        }

        public string AddQrCode(string xmlIn)
        {
            try
            {
                Logger.Log("Inicia AddQrCode");

                var xDoc = XDocument.Parse(xmlIn);
                var checkId = XmlService.GetCheckId(xDoc);

                if (checkId == Constants.NULL_ID)
                {
                    Logger.Log($"Check does not have ID.{Environment.NewLine}{xmlIn}");
                    return xmlIn;
                }

                GlobalParams.SdkModel = new PosSdkModel { CheckId = checkId };

                PosSdkService.ReadIniConf();

                PosSdkModel model;
                var response = PosSdkService.GetInstanceFunctionsAndCheck(checkId, out model);

                if (!response.IsSuccess)
                {
                    Logger.Log($"GetInstanceFunctionsAndCheck Error: {response.Message}");
                    return xmlIn;
                }

                var code = GenerateCode(model);
                Logger.Log("Code E: " + code);


                var xmlOut = XmlService.AddCodeInformation(xDoc, code, model.CheckId);
                return xmlOut;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                MessageExt.ShowErrorMessage("Error on execute app: " + ex.Message);
            }

            return xmlIn;
        }

        private string GenerateCode(PosSdkModel model)
        {
            var code = $"{model.ReadableCheckId}#{GlobalParams.Tpv}#{GlobalParams.UnitNumber}#{model.Balance.Pvp}#{DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond}";
            Logger.Log("Code: " + code);
            /*
             El tpv obtendrá la siguiente información del ticket de compra:
            NumTicket#tpv#shop#pvp#fxOper
            Por ejemplo: 001#1#504#10#1529481484708

                         */
            /*
             AES-256 y en base 64 para evitar caracteres no
permitidos en peticiones http. La clave para encriptar y desencriptar será:
Zxc0122W_THYiopdwes345UUgh
             */
            return AdvancedEncryptionStandard.EncryptString(code);
        }

        string IInterceptAlohaPrinting.PrintXML(int fohDocumentType, string xmlIn)
        {
            try
            {
                if (fohDocumentType != (int)DOCTYPES.FOHDOC_CHECK)
                    return xmlIn;

                return AddQrCode(xmlIn);
            }
            catch (Exception)
            {
                return xmlIn;
            }
        }

        public DOCTYPES[] GetSubscribeOptions()
        {
            return new[] { DOCTYPES.FOHDOC_CHECK };
        }

        public NOTIFY_OPTIONS[] GetNotifyOptions()
        {
            return new[] { NOTIFY_OPTIONS.NTFY_PREPRINT, NOTIFY_OPTIONS.NTFY_PRINT };
        }

        public void PrePrint(DOCTYPES docType, string xmlPrintStream)
        {
        }

        public void PostPrint(DOCTYPES docType, string xmlPrintStream)
        {
        }

        string IInterceptAlohaPrinting2.PrintXML(int fohDocumentType, string xmlIn)
        {
            try
            {
                if (fohDocumentType != (int)DOCTYPES.FOHDOC_CHECK)
                    return xmlIn;

                return AddQrCode(xmlIn);
            }
            catch (Exception)
            {
                return xmlIn;
            }
        }
    }
}
