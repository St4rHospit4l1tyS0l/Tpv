using log4net;
using System;
using System.IO;
using Tpv.Ui.Model;

namespace Tpv.Ui.Service
{
    public static class IniEnvReaderService
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(PosService));
        private static string _iberDir;

        public static bool GetTpvAndStoreInfo(PosCheckModel model)
        {
            if (!GetEnvInfo(model))
                return false;

            return GetPosIniInfo(model);
        }

        private static bool GetEnvInfo(PosCheckModel model)
        {
            try
            {
                _iberDir = Environment.GetEnvironmentVariable("IBERDIR", EnvironmentVariableTarget.Machine);
                if (string.IsNullOrEmpty(_iberDir))
                {
                    MessageExt.ShowErrorMessage("Es necesario tener definido la variable de entorno IBERDIR, con el valor correcto");
                    return false;
                }

                model.Tpv = Environment.GetEnvironmentVariable("TERM", EnvironmentVariableTarget.Machine);
                if (string.IsNullOrEmpty(model.Tpv))
                {
                    MessageExt.ShowErrorMessage("Es necesario tener definido la variable de entorno TERM, con el valor correcto");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message + " | " + ex.StackTrace);
                MessageExt.ShowErrorMessage("No fue posible leer las variables de entorno, revise que tenga los permisos necesarios.");
                return false;
            }
        }

        private static bool GetPosIniInfo(PosCheckModel model)
        {
            try
            {
                var sFullPath = Path.Combine(Path.Combine(_iberDir, Constants.POS_DATA_PATH), Constants.POS_INI_FILE);

                if (!File.Exists(sFullPath))
                {
                    MessageExt.ShowErrorMessage($"El archivo {sFullPath} no se encontró");
                    return false;
                }

                foreach (var line in File.ReadAllLines(sFullPath))
                {
                    var split = line.Split('=');

                    if (split.Length < 2) continue;

                    var key = split[0].Trim().ToUpper();
                    if (key == "UNITNUMBER")
                    {
                        model.Shop = split[1].Trim();
                        return true;
                    }
                }

                MessageExt.ShowErrorMessage($"No hay definido un número de la tienda en el archivo {sFullPath}");
                return false;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message + " | " + ex.StackTrace);
                MessageExt.ShowErrorMessage("No fue posible leer el archivo INI.");
                return false;
            }
        }
    }
}