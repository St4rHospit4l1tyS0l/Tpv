using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tpv.Printer.Infrastructure.Extensions;

namespace Tpv.Printer.Model.Shared
{

    public static class MasterModel
    {
        public static int MaxDebugFileSize { get; set; }
        public static bool IsDebugMode { get; set; }
        public static bool IsValidModel { get; set; }
        public static int QrPixelPerModule { get; set; }
        public static string QrSizeOnTicket { get; set; }
        public static List<int> ExcludedOrderModes { get; set; }
        public static List<string> PrintLine1 { get; set; }
        public static List<string> PrintLine2 { get; set; }
        public static List<string> PrintLine3 { get; set; }
        public static List<string> PrintLine4 { get; set; }
        public static List<string> PrintLine5 { get; set; }
        public static List<string> PrintLine6 { get; set; }
        public static List<string> PrintLine7 { get; set; }
        public static List<string> PrintLine8 { get; set; }
        public static List<string> PrintLine9 { get; set; }
        public static List<string> PrintLine10 { get; set; }
        public static List<string> PrintLine11 { get; set; }
        public static List<string> PrintLine12 { get; set; }
        public static List<PrintItem> PrintLines { get; set; }



        public static Dictionary<string, MasterPropertyModel> Parameters = new Dictionary<string, MasterPropertyModel>
        {
           {"QR_PIXEL_PER_MODULE=", new MasterPropertyModel(SetIntValue, nameof(QrPixelPerModule), true, "Pixel per module on QR (QR_PIXEL_PER_MODULE)")}
            ,{"QR_SIZE_ON_TICKET=", new MasterPropertyModel(SetStringValue, nameof(QrSizeOnTicket), true, "Size on ticket (QR_SIZE_ON_TICKET)")}
            ,{"EXCLUDED_ORDER_MODES=", new MasterPropertyModel(SetListInt, nameof(ExcludedOrderModes), false, "Excluded order modes (EXCLUDED_ORDER_MODES)")}
            ,{"PRINT_LINE_1=", new MasterPropertyModel(SetListString, nameof(PrintLine1), false, "Printing line 1")}
            ,{"PRINT_LINE_2=", new MasterPropertyModel(SetListString, nameof(PrintLine2), false, "Printing line 2")}
            ,{"PRINT_LINE_3=", new MasterPropertyModel(SetListString, nameof(PrintLine3), false, "Printing line 3")}
            ,{"PRINT_LINE_4=", new MasterPropertyModel(SetListString, nameof(PrintLine4), false, "Printing line 4")}
            ,{"PRINT_LINE_5=", new MasterPropertyModel(SetListString, nameof(PrintLine5), false, "Printing line 5")}
            ,{"PRINT_LINE_6=", new MasterPropertyModel(SetListString, nameof(PrintLine6), false, "Printing line 6")}
            ,{"PRINT_LINE_7=", new MasterPropertyModel(SetListString, nameof(PrintLine7), false, "Printing line 7")}
            ,{"PRINT_LINE_8=", new MasterPropertyModel(SetListString, nameof(PrintLine8), false, "Printing line 8")}
            ,{"PRINT_LINE_9=", new MasterPropertyModel(SetListString, nameof(PrintLine9), false, "Printing line 9")}
            ,{"PRINT_LINE_10=", new MasterPropertyModel(SetListString, nameof(PrintLine10), false, "Printing line 10")}
            ,{"PRINT_LINE_11=", new MasterPropertyModel(SetListString, nameof(PrintLine11), false, "Printing line 11")}
            ,{"PRINT_LINE_12=", new MasterPropertyModel(SetListString, nameof(PrintLine12), false, "Printing line 12")}
            ,{"DEBUG_MODE=", new MasterPropertyModel(SetBoolValue, nameof(IsDebugMode), false, "Debug Mode (DEBUG_MODE), FALSE no, TRUE yes")}
            ,{"MAX_DEBUG_FILE_SIZE=", new MasterPropertyModel(SetIntValue, nameof(MaxDebugFileSize), true, "Max debug file size (MAX_DEBUG_FILE_SIZE) in MB")}
        };


        static MasterModel()
        {
            IsValidModel = false;
            foreach (var parameter in Parameters.Values)
            {
                var field = typeof(MasterModel).GetProperty(parameter.Field);

                if (field == null)
                    continue;

                if (field.PropertyType == typeof(int))
                    field.SetValue(null, Constants.NULL_ID, null);

                if (field.PropertyType == typeof(bool))
                    field.SetValue(null, false, null);
            }

            MaxDebugFileSize = 20;
        }


        private static void SetIntValue(string field, string value, Func<int, int> funcExtraTrans = null)
        {
            var iVal = int.Parse(value);

            if (funcExtraTrans != null)
                iVal = funcExtraTrans.Invoke(iVal);

            var propertyInfo = typeof(MasterModel).GetProperty(field);
            if (propertyInfo != null)
                propertyInfo.SetValue(null, iVal, null);
        }

        private static void SetBoolValue<T>(string field, string value, Func<T, T> funcExtraTrans = null)
        {
            var bVal = bool.Parse(value);
            var propertyInfo = typeof(MasterModel).GetProperty(field);
            if (propertyInfo != null)
                propertyInfo.SetValue(null, bVal, null);
        }

        private static void SetStringValue<T>(string field, string value, Func<T, T> funcExtraTrans = null)
        {
            value = string.IsNullOrEmpty(value) ? string.Empty : value.ReplaceWildCardByEnvironmentVariables(Constants.WILDCARD_CHAR);
            var propertyInfo = typeof(MasterModel).GetProperty(field);
            if (propertyInfo != null)
                propertyInfo.SetValue(null, value, null);
        }

        private static void SetListInt<T>(string field, string value, Func<T, T> funcExtraTrans = null)
        {
            var lstValues = string.IsNullOrWhiteSpace(value) ? new List<int>() : value.Split(Constants.SPLIT_ARRAY_SEPARATOR).Select(int.Parse).ToList();
            var propertyInfo = typeof(MasterModel).GetProperty(field);
            if (propertyInfo != null)
                propertyInfo.SetValue(null, lstValues, null);
        }

        private static void SetListListInt<T>(string field, string value, Func<T, T> funcExtraTrans = null)
        {
            var lstValues = string.IsNullOrWhiteSpace(value) ? new List<List<int>>() :
                value.Split(Constants.SPLIT_ARRAY_ARRAY_SEPARATOR).Select(e => e.Split(Constants.SPLIT_ARRAY_SEPARATOR).Select(int.Parse).ToList()).ToList();
            var propertyInfo = typeof(MasterModel).GetProperty(field);
            if (propertyInfo != null)
                propertyInfo.SetValue(null, lstValues, null);
        }

        private static void SetListString<T>(string field, string value, Func<T, T> funcExtraTrans = null)
        {
            var lstValues = string.IsNullOrEmpty(value) ? new List<string>() : value.Split(Constants.SPLIT_ARRAY_SEPARATOR).ToList();
            var propertyInfo = typeof(MasterModel).GetProperty(field);
            if (propertyInfo != null)
                propertyInfo.SetValue(null, lstValues, null);
        }

        private static void SetListTime<T>(string field, string value, Func<T, T> funcExtraTrans = null)
        {
            List<TimeSpan> lstValues;
            if (string.IsNullOrWhiteSpace(value))
            {
                lstValues = new List<TimeSpan>();
            }
            else
            {
                lstValues = value.Split(Constants.SPLIT_ARRAY_SEPARATOR).Select(e => e.ToTimeSpan()).OrderBy(e => e).ToList();
            }


            //var lstValues = string.IsNullOrWhiteSpace(value) ? new List<TimeSpan>() : value.Split(Constants.SPLIT_ARRAY_SEPARATOR).Select(int.Parse).ToList();
            var propertyInfo = typeof(MasterModel).GetProperty(field);
            if (propertyInfo != null)
                propertyInfo.SetValue(null, lstValues, null);
        }

        public static bool ExtractInfo(string sPath, ResponseMessage response)
        {
            try
            {
                using (var file = new StreamReader(sPath))
                {
                    string line;
                    while ((line = file.ReadLine()) != null)
                    {
                        var flatLine = line.TrimStart();
                        try
                        {
                            var token = Parameters.Where(e => flatLine.StartsWith(e.Key)).Select(e => new { e.Key, e.Value }).FirstOrDefault();

                            if (token == null)
                                continue;

                            flatLine = line.Replace(token.Key, string.Empty);
                            token.Value.Transform.Invoke(token.Value.Field, flatLine, token.Value.ExtraTransform);
                        }
                        catch (Exception ex)
                        {
                            response.Message = $"An error ocurred when information was extracted from line '{flatLine}' of file {sPath}: {ex.Message}";
                            return false;
                        }

                    }
                    file.Close();
                }

                return true;
            }
            catch (Exception ex)
            {
                response.Message = $"An error ocurred when information was read from file {sPath}: {ex.Message}";
            }
            return false;
        }

        public static bool ValidateModel(string sFullPath, ResponseMessage response)
        {
            foreach (var parameter in Parameters.Values.Where(e => e.IsRequired))
            {
                var isValid = true;
                var field = typeof(MasterModel).GetProperty(parameter.Field);

                if (field != null)
                {
                    if (field.PropertyType == typeof(int))
                    {
                        var propertyInfo = typeof(MasterModel).GetProperty(parameter.Field);
                        if (propertyInfo != null)
                        {
                            var value = (int)propertyInfo.GetValue(null, null);
                            if (value <= Constants.NULL_ID)
                                isValid = false;
                        }
                    }
                    else if (field.PropertyType == typeof(List<int>))
                    {
                        var propertyInfo = typeof(MasterModel).GetProperty(parameter.Field);
                        if (propertyInfo != null)
                        {
                            var value = (List<int>)propertyInfo.GetValue(null, null);
                            if (!value.Any())
                                isValid = false;
                        }
                    }
                }
                else
                {
                    var propertyInfo = typeof(MasterModel).GetProperty(parameter.Field);
                    if (propertyInfo != null)
                    {
                        var value = (string)propertyInfo.GetValue(null, null);
                        if (string.IsNullOrWhiteSpace(value))
                            isValid = false;
                    }
                }

                if (isValid)
                    continue;

                response.IsSuccess = false;
                response.Message = $"{parameter.FieldMsg} is a required field on configuration file. (Numeric values must be equal or greater than zero) {sFullPath}";
                return false;
            }

            if (!AddAndValidatePrintLines(response))
                return false;

            IsValidModel = true;
            response.IsSuccess = true;
            return true;
        }

        private static bool AddAndValidatePrintLines(ResponseMessage response)
        {
            try
            {
                var printLines = new List<List<string>> { PrintLine1, PrintLine2, PrintLine3, PrintLine4, PrintLine5, PrintLine6, PrintLine7, PrintLine8, PrintLine9, PrintLine10, PrintLine11, PrintLine12 };
                PrintLines = new List<PrintItem>();

                var count = 0;
                foreach (var line in printLines)
                {
                    count++;
                    if (line == null || line.Count == 0) continue;

                    var printItem = new PrintItem();
                    switch (line.Count)
                    {
                        case 4:
                            if (!ExtractTextSizeStyle(response, line, printItem)) return false;

                            var key = line[3];
                            string value;
                            if (!Constants.PrintingTag.CharAlign.TryGetValue(key, out value))
                            {
                                response.SetErrorMessage(AddErrorAlign(key));
                                return false;
                            }
                            printItem.Align = value;

                            break;
                        case 3:
                            if (!ExtractTextSizeStyle(response, line, printItem)) return false;
                            printItem.Align = Constants.PrintingTag.PRINTCENTERED;
                            break;
                        case 2:
                            response.Message = $"La línea {count} sólo tiene 2 parámetros ({line[0]}, {line[1]}) y se necesitan 1, 3 o 4, por ejemplo: 'Felicidades código #code#' , 'Felicidades código #code#|LARGE|NORMAL' o 'Felicidades código #code#|LARGE|NORMAL|LEFT' ";
                            return false;
                        default:
                            printItem.Text = string.IsNullOrWhiteSpace(line[0]) ? string.Empty : line[0];
                            printItem.Size = Constants.PrintingTag.PS_CW_MEDIUM;
                            printItem.Style = Constants.PrintingTag.PST_NORMAL;
                            printItem.Align = Constants.PrintingTag.PRINTCENTERED;
                            break;
                    }

                    PrintLines.Add(printItem);
                }
            }
            catch (Exception ex)
            {
                response.SetErrorMessage(ex);
                return false;
            }


            return true;
        }

        private static bool ExtractTextSizeStyle(ResponseMessage response, List<string> line, PrintItem printItem)
        {
            var key = line[1];
            if (string.IsNullOrWhiteSpace(key))
            {
                response.SetErrorMessage(AddErrorSize(string.Empty));
                return false;
            }

            int value;
            if (!Constants.PrintingTag.CharSize.TryGetValue(key, out value))
            {
                response.SetErrorMessage(AddErrorSize(key));
                return false;
            }
            printItem.Size = value;

            key = line[2];
            if (string.IsNullOrWhiteSpace(key))
            {
                response.SetErrorMessage(AddErrorStyle(string.Empty));
                return false;
            }

            if (!Constants.PrintingTag.CharStyle.TryGetValue(key, out value))
            {
                response.SetErrorMessage(AddErrorStyle(key));
                return false;
            }
            printItem.Style = value;

            printItem.Text = string.IsNullOrWhiteSpace(line[0]) ? string.Empty : line[0];
            return true;
        }

        private static string AddErrorStyle(string value)
        {
            return $"La opción de estilo '{value}' no es válida, las opciones válidas son: {string.Join(", ", Constants.PrintingTag.CharStyle.Keys.ToArray())}";
        }

        private static string AddErrorSize(string value)
        {
            return $"La opción de tamaño '{value}' no es válida, las opciones válidas son: {string.Join(", ", Constants.PrintingTag.CharSize.Keys.ToArray())}";
        }

        private static string AddErrorAlign(string value)
        {
            return $"La alineación '{value}' no es válida, las opciones válidas son: {string.Join(", ", Constants.PrintingTag.CharAlign.Keys.ToArray())}";
        }
    }
}
