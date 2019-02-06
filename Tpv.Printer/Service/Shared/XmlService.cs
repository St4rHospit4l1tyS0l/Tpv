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

        //public static string AddCodeInformation(XDocument xDoc, ParticipantModel participant, RewardModel reward, SpendModel spend)
        //{
        //    var child = xDoc.Descendants(Constants.PrintingTag.PRINTCENTERED).FirstOrDefault(e => e.Value.StartsWith("****"));

        //    if (child == null)
        //    {
        //        child = xDoc.Descendants(Constants.PrintingTag.STOPJOURNAL).FirstOrDefault();

        //        if (child == null)
        //        {
        //            return xDoc.ToString();
        //        }
        //    }


        //    child.AddBeforeSelf(new XElement(Constants.PrintingTag.PRINTCENTERED, "****************"));
        //    child.AddBeforeSelf(AddPrintingStyle(Constants.PrintingTag.PS_CW_MEDIUM, Constants.PrintingTag.PST_EXPANDED_HEIGHT));
        //    child.AddBeforeSelf(new XElement(Constants.PrintingTag.PRINTCENTERED, "Loyalty award details:"));
        //    child.AddBeforeSelf(AddPrintingStyle(Constants.PrintingTag.PS_CW_MEDIUM, Constants.PrintingTag.PST_NORMAL));
        //    child.AddBeforeSelf(new XElement(Constants.PrintingTag.LINEFEED, "1"));
        //    child.AddBeforeSelf(new XElement(Constants.PrintingTag.PRINTCENTERED, participant.Name));

        //    if (participant.Type != Model.Loyalty.Identification.PhoneNumber)
        //    {
        //        child.AddBeforeSelf(new XElement(Constants.PrintingTag.PRINTCENTERED, Constants.DicIdDescription[participant.Type]));
        //        child.AddBeforeSelf(new XElement(Constants.PrintingTag.PRINTCENTERED, participant.Value));
        //    }

        //    child.AddBeforeSelf(new XElement(Constants.PrintingTag.PRINTCENTERED, "Award amount"));
        //    child.AddBeforeSelf(new XElement(Constants.PrintingTag.PRINTCENTERED, $"EUR {reward.TotalAwardedAmount}"));
        //    child.AddBeforeSelf(new XElement(Constants.PrintingTag.PRINTCENTERED, $"Invoice Number - {reward.ReferenceNumber}"));

        //    if (reward.InstantDiscount > 0)
        //    {
        //        child.AddBeforeSelf(new XElement(Constants.PrintingTag.PRINTCENTERED, "Instant discount"));
        //        child.AddBeforeSelf(new XElement(Constants.PrintingTag.PRINTCENTERED, $"{reward.InstantDiscount}"));
        //    }

        //    child.AddBeforeSelf(new XElement(Constants.PrintingTag.PRINTCENTERED, "Wallet balance"));
        //    child.AddBeforeSelf(new XElement(Constants.PrintingTag.PRINTCENTERED, $"EUR {reward.Balance}"));
        //    child.AddBeforeSelf(new XElement(Constants.PrintingTag.LINEFEED, "1"));


        //    if (spend == null) return xDoc.ToString();


        //    child.AddBeforeSelf(new XElement(Constants.PrintingTag.PRINTCENTERED, "****************"));
        //    child.AddBeforeSelf(AddPrintingStyle(Constants.PrintingTag.PS_CW_MEDIUM, Constants.PrintingTag.PST_EXPANDED_HEIGHT));
        //    child.AddBeforeSelf(new XElement(Constants.PrintingTag.PRINTCENTERED, "Loyalty spend details:"));
        //    child.AddBeforeSelf(AddPrintingStyle(Constants.PrintingTag.PS_CW_MEDIUM, Constants.PrintingTag.PST_NORMAL));
        //    child.AddBeforeSelf(new XElement(Constants.PrintingTag.LINEFEED, "1"));
        //    child.AddBeforeSelf(new XElement(Constants.PrintingTag.PRINTCENTERED, "Spend amount"));
        //    child.AddBeforeSelf(new XElement(Constants.PrintingTag.PRINTCENTERED, $"EUR {spend.SpendAmount}"));
        //    child.AddBeforeSelf(new XElement(Constants.PrintingTag.LINEFEED, "1"));
        //    //*/

        //    return xDoc.ToString();
        //}

        private static XElement AddPrintingStyle(int cpi, int style)
        {
            var font = new XElement(Constants.PrintingTag.PRINTSTYLE, new XElement(Constants.PrintingTag.CPI, cpi), new XElement(Constants.PrintingTag.STYLE, style));
            return font;
        }
    }
}
