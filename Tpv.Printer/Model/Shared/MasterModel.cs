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
        public static int BarCodeHeight { get; set; }
        public static List<int> ExcludedOrderModes { get; set; }



        public static Dictionary<string, MasterPropertyModel> Parameters = new Dictionary<string, MasterPropertyModel>
        {
           {"QR_PIXEL_PER_MODULE=", new MasterPropertyModel(SetIntValue, nameof(QrPixelPerModule), true, "Pixel por module on QR (QR_PIXEL_PER_MODULE)")}
            ,{"QR_SIZE_ON_TICKET=", new MasterPropertyModel(SetStringValue, nameof(QrSizeOnTicket), true, "Size on ticket (QR_SIZE_ON_TICKET)")}
            ,{"BARCODE_HEIGHT=", new MasterPropertyModel(SetIntValue, nameof(BarCodeHeight), true, "Barcode height in milimiters (BARCODE_HEIGHT)")}
            ,{"EXCLUDED_ORDER_MODES=", new MasterPropertyModel(SetListInt, nameof(ExcludedOrderModes), false, "Excluded order modes (EXCLUDED_ORDER_MODES)")}
            ,{"DEBUG_MODE=", new MasterPropertyModel(SetBoolValue, nameof(IsDebugMode), false, "Debug Mode (DEBUG_MODE), FALSE no, TRUE yes")}
            ,{"MAX_DEBUG_FILE_SIZE=", new MasterPropertyModel(SetBoolValue, nameof(MaxDebugFileSize), true, "Max debug file size (MAX_DEBUG_FILE_SIZE) in MB")}
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

            IsValidModel = true;
            response.IsSuccess = true;
            return true;
        }
    }
}
