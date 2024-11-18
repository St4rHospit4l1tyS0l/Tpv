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
            const string cline = "****";
            var child = (xDoc.Descendants("PRINTCENTERED").FirstOrDefault(e => e.Value.StartsWith(cline)) ??
                         xDoc.Descendants("STOPJOURNAL").LastOrDefault()) ??
                        xDoc.Descendants("POSTLINEFEEDS").FirstOrDefault();

            XElement element;
            foreach (var line in MasterModel.PrintLines)
            {
                if (child == null) continue;

                element = AddPrintingStyle(line.Size, line.Style);
                child.AddBeforeSelf(element);

                var text = line.Text.Replace(Constants.PrintingTag.TAG_CODE, code);
                switch (line.Align)
                {
                    case Constants.PrintingTag.PRINTLEFT:
                    case Constants.PrintingTag.PRINTRIGHT:
                        element = new XElement(Constants.PrintingTag.PRINTLEFTRIGHT);
                        element.Add(new XElement(line.Align, text));
                        child.AddBeforeSelf(element);
                        break;

                    default:
                        child.AddBeforeSelf(new XElement(Constants.PrintingTag.PRINTCENTERED, text));
                        break;
                }
            }

            child?.AddBeforeSelf(AddPrintingStyle(Constants.PrintingTag.PS_CW_MEDIUM, Constants.PrintingTag.PST_NORMAL));

            element = AddQrCode(code, checkId);

            if (child != null)
                child.AddBeforeSelf(element);
            else
                xDoc.AddAfterSelf(element);

            return xDoc.ToString();
        }

        public static string AddCodeInformation(XDocument xDoc, string code)
        {
            var child = xDoc.Descendants("PRINTCENTERED").FirstOrDefault(e => e.Value.StartsWith("****"));

            if (child == null)
                return "";

            foreach (var line in MasterModel.PrintLines)
            {
                child.AddBeforeSelf(AddPrintingStyle(line.Size, line.Style));

                var text = line.Text.Replace(Constants.PrintingTag.TAG_CODE, code);
                switch (line.Align)
                {
                    case Constants.PrintingTag.PRINTLEFT:
                    case Constants.PrintingTag.PRINTRIGHT:
                        var element = new XElement(Constants.PrintingTag.PRINTLEFTRIGHT);
                        element.Add(new XElement(line.Align, text));
                        child.AddBeforeSelf(element);
                        break;

                    default:
                        child.AddBeforeSelf(new XElement(Constants.PrintingTag.PRINTCENTERED, text));
                        break;
                }
            }

            child.AddBeforeSelf(AddPrintingStyle(Constants.PrintingTag.PS_CW_MEDIUM, Constants.PrintingTag.PST_NORMAL));
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
            var pathBmp = Path.Combine(GlobalParams.LocalDir, "BMP");

            if (!Directory.Exists(pathBmp))
                Directory.CreateDirectory(pathBmp);

            var bmpName = $"{checkId}.bmp";
            qrCodeImage.Save(Path.Combine(pathBmp, bmpName), ImageFormat.Bmp);

            return bmpName;
        }

        private static XElement AddPrintingStyle(int cpi, int style)
        {
            var font = new XElement(Constants.PrintingTag.PRINTSTYLE, new XElement(Constants.PrintingTag.CPI, cpi),
                new XElement(Constants.PrintingTag.STYLE, style));
            return font;
        }
    }
}