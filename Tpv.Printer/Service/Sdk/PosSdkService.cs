using LasaFOHLib;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Tpv.Printer.Infrastructure.Log;
using Tpv.Printer.Model.Sdk;
using Tpv.Printer.Model.Shared;
using Tpv.Printer.Service.Shared;

namespace Tpv.Printer.Service.Sdk
{
    public static class PosSdkService
    {
        private static PosSdkFuncModel _posSdkFunc;

        public const int INTERNAL_CHECKS = 540;
        public const int INTERNAL_CHECKS_ENTRIES = 542;
        public const int INTERNAL_ENTRIES = 560;
        public const int INTERNAL_ENTRIES_ITEM_DATA = 562;
        public const int INTERNAL_CHECKS_PROMOS = 544;
        public const int INTERNAL_PROMOS_ITEMS = 621;
        public const int INTERNAL_LOCALSTATE = 720;
        public const int INTERNAL_LOCALSTATE_ITEMINFOS = 731;


        public static PosSdkModel ReadCheckInfo(int checkId)
        {
            try
            {
                var model = new PosSdkModel();
                try
                {
                    model.TerminalId = _posSdkFunc.LocalState.GetLongVal("TERMINAL_NUM");
                    model.CheckId = _posSdkFunc.LocalState.GetLongVal("CURRENT_CHECK_ID");
                    model.CheckId = model.CheckId <= 0 ? checkId : model.CheckId;
                    model.ReadableCheckId = GetReadableCheckId(model.CheckId);
                    return model;
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                    MessageExt.ShowErrorMessage("Make sure POS is running.");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                MessageExt.ShowErrorMessage("Make sure POS is running or have a valid license SDK.");
                return null;
            }
        }

        public static bool HasExcludeByOrderMode(int iCheckId, ResponseMessage response)
        {
            try
            {
                if (MasterModel.ExcludedOrderModes == null || MasterModel.ExcludedOrderModes.Count == 0)
                    return false;


                var pDepot = new IberDepot();
                Logger.Log("HasExcludeByOrderMode");


                foreach (IIberObject chkObject in pDepot.FindObjectFromId(INTERNAL_CHECKS, iCheckId))
                {
                    var orderMode = chkObject.GetLongVal("ORDERMODE");
                    Logger.Log($"OM: {orderMode} - C: {iCheckId}");

                    if (MasterModel.ExcludedOrderModes.Any(e => e == orderMode))
                        return true;

                }

                return false;
            }
            catch (Exception ex)
            {
                ExtractException(ex, response, Constants.CodeErrors.EXTRACT_INFO_POS_ERROR);
                return false;
            }
        }

        public static int GetReadableCheckId(this int checkId)
        {
            long decodeTerm = checkId >> 20;
            long decodeRel = checkId & 0xFFFFF;
            return Convert.ToInt32(decodeTerm * 10000 + decodeRel);
        }

        public static ResponseMessage GetInstanceFunctionsAndCheck(int checkId, out PosSdkModel sdkModel)
        {
            var response = new ResponseMessage();
            sdkModel = null;
            if (_posSdkFunc == null)
            {
                try
                {
                    _posSdkFunc = new PosSdkFuncModel();
                }
                catch (Exception ex)
                {
                    ExtractException(ex, response, Constants.CodeErrors.INSTANCE_POS_ERROR);
                    _posSdkFunc = null;
                    return response;
                }

                try
                {
                    _posSdkFunc.InitFunction();
                }
                catch (Exception ex)
                {
                    ExtractException(ex, response, Constants.CodeErrors.INSTANCE_FUNCTION_ERROR);
                    _posSdkFunc = null;
                    return response;
                }

                try
                {
                    _posSdkFunc.InitLocalState();
                }
                catch (Exception ex)
                {
                    ExtractException(ex, response, Constants.CodeErrors.INIT_LOCAL_STATE_ERROR);
                    _posSdkFunc = null;
                    return response;
                }
            }

            if (HasExcludeByOrderMode(checkId, response))
            {
                response.SetErrorMessage("Excluded by OrderMode");
                return response;
            }

            try
            {
                sdkModel = ReadCheckInfo(checkId);
            }
            catch (Exception ex)
            {
                ExtractException(ex, response, Constants.CodeErrors.READ_CHECK_INFO_ERROR);
                _posSdkFunc = null;
                return response;
            }

            sdkModel.Balance = GetFullBalance(sdkModel.CheckId);
            response.IsSuccess = true;
            return response;
        }

        public static BalanceModel GetFullBalance(int checkId)
        {
            double subTotal = 0, tax = 0;

            try
            {
                _posSdkFunc.Funcs.GetCheckTotal(checkId, out subTotal, out tax);

                return new BalanceModel
                {
                    SubTotal = (decimal)subTotal,
                    Tax = (decimal)tax,
                };

            }
            catch (Exception e)
            {
                Logger.Write($"Error al obtener el balance completo: CheckId {checkId}");
                Logger.Log(e);
                return new BalanceModel
                {
                    SubTotal = (decimal)subTotal,
                    Tax = (decimal)tax,
                };
            }
        }

        private static void ExtractException(Exception ex, ResponseMessage response, int errorCode)
        {
            response.IsSuccess = false;
            response.Message = GetPosException(ex, errorCode);
        }

        private static string GetPosException(Exception ex, int errorCode)
        {
            try
            {
                Logger.Log(ex);

                PropertyInfo propInfo = ex.GetType().GetProperty("HResult");

                if (propInfo != null)
                {
                    Int32 hresult = Convert.ToInt32(propInfo.GetValue(ex, null));
                    if (((hresult >> 16) & 0x07ff) == 0x06)
                    {
                        var posErrorCode = hresult & 0xFFF;
                        return $"Error, code:{errorCode}, message: {ex.Message}, POS error code: {posErrorCode:X}";
                    }
                }

                return $"POS error COM: {errorCode}, {ex.Message}";
            }
            catch (Exception exIn)
            {
                return $"POS error COM extraction: {errorCode}, {ex.Message}, internal: {exIn.Message}";
            }
        }

        public static void ReadIniConf()
        {
            if (MasterModel.IsValidModel)
                return;

            GlobalParams.IberDir = Environment.GetEnvironmentVariable("IBERDIR", EnvironmentVariableTarget.Machine);
            if (string.IsNullOrEmpty(GlobalParams.IberDir))
                throw new ArgumentException("IBERDIR enviroment variable must be defined");

            GlobalParams.Tpv = Environment.GetEnvironmentVariable("TERM", EnvironmentVariableTarget.Machine);
            if (string.IsNullOrEmpty(GlobalParams.Tpv))
                throw new ArgumentException("TERM enviroment variable must be defined");

            GlobalParams.DebugPathTmpDir = Path.Combine(GlobalParams.IberDir, "TMP", Logger.FILE_PATH);
            GlobalParams.PosDirDbfFiles = Path.Combine(GlobalParams.IberDir, "DATA");
            var sFullPath = Path.Combine(GlobalParams.IberDir, Constants.POS_BIN_FOLDER, Constants.INI_FILE);

            if (File.Exists(sFullPath) == false)
                throw new ArgumentException($"'{sFullPath}' file must be defined");

            var response = new ResponseMessage();
            if (!MasterModel.ExtractInfo(sFullPath, response))
                throw new ArgumentException($"Error on reading file: {response.Message}");

            ReadPosIniFile();

            if (!MasterModel.ValidateModel(sFullPath, response))
                throw new ArgumentException($"Error on validate file: {response.Message}");

            Logger.ClearLogBySize();

        }

        private static void ReadPosIniFile()
        {
            var file = Path.Combine(GlobalParams.IberDir, Constants.POS_DATA_FOLDER, Constants.POS_INI_FILE);

            if (!File.Exists(file))
            {
                throw new Exception($"Archivo {Constants.POS_INI_FILE} no existe en la siguiente ruta: {file}");
            }

            var lines = File.ReadLines(file).ToArray();

            foreach (var line in lines)
            {
                if (!line.Contains(Constants.Pos.UNITNUMBER)) continue;

                GlobalParams.UnitNumber = GetStringValue(line);
                break;
            }

            if (string.IsNullOrWhiteSpace(GlobalParams.UnitNumber))
            {
                throw new Exception($"UNITNUMBER no fue encontrado en: {file}");
            }
        }

        private static string GetStringValue(string line)
        {
            var sValues = line.Split('=');
            return sValues[1].Trim();
        }
    }
}
