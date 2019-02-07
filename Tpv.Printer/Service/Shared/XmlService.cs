using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Tpv.Printer.Model.Shared;

namespace Tpv.Printer.Service.Shared
{
    public static class XmlService
    {
        public static int GetCheckId(XDocument xDoc)
        {
            var check = xDoc.Descendants(Constants.InternalPos.TAG_CHECKID).FirstOrDefault();
            if (check != null)
                return int.Parse(check.Value);

            return Constants.NULL_ID;
        }
        public static string AddCodeInformation(XDocument xDoc, string code, int checkId)
        {
            var line = "****";
            var child = (xDoc.Descendants("PRINTCENTERED").FirstOrDefault(e => e.Value.StartsWith(line)) ?? xDoc.Descendants("STOPJOURNAL").LastOrDefault()) ?? xDoc.Descendants("POSTLINEFEEDS").FirstOrDefault();

            var element = AddQrCode(code, checkId);

            if (child != null)
                child.AddBeforeSelf(element);
            else
                xDoc.AddAfterSelf(element);

            return xDoc.ToString();
        }

        private static XElement AddQrCode(string code, int checkId)
        {
            var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(code, QRCodeGenerator.ECCLevel.M);
            var qrCode = new QRCode(qrCodeData);
            var qrCodeImage = qrCode.GetGraphic(MasterModel.QrPixelPerModule);

            var bmpName = SaveBmpFile(qrCodeImage, checkId);

            var element = new XElement(Constants.PrintingTag.BTIMAP);
            element.Add(new XElement("PATH", bmpName));
            element.Add(new XElement("SIZE", MasterModel.QrSizeOnTicket));
            element.Add(new XElement("JUSTIFY", Constants.PrintingTag.PLJUSTIFY_CENTER));
            return element;
        }

        private static string SaveBmpFile(Bitmap qrCodeImage, int checkId)
        {
            var pathBmp = Path.Combine(GlobalParams.IberDir, "BMP");

            if (!Directory.Exists(pathBmp))
                Directory.CreateDirectory(pathBmp);

            var bmpName = $"{checkId}.bmp";
            qrCodeImage.Save(Path.Combine(pathBmp, bmpName), ImageFormat.Bmp);

            return bmpName;
        }
    }
}
