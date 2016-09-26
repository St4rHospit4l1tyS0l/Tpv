using System;
using System.Collections.Generic;
using System.IO;
using Tpv.Aloha.BsLogic;
using Tpv.Aloha.Model;

namespace Tpv.Aloha.Infrastructure.Connector
{
    public static class DbReader
    {
        public static Dictionary<int, string> DicPromos = new Dictionary<int, string>();
        public static Dictionary<string, string> DicConfig = new Dictionary<string, string>();

        public static string PromoFile
        {
            get
            {
                string file;
                return DicConfig.TryGetValue(Constants.PROMO_FILE, out file) ? file : "MsgPromoFile.tpv";
            }
        }

        public static string UrlToDelete
        {
            get
            {
                string value;
                return DicConfig.TryGetValue(Constants.URL_TO_DELETE, out value) ? value : "http://dunkin.rkpeople.com/developer/dunkin_ws/public/validar-codigo-barra/{0}&1";
            }
        }


        public static bool ReadDictionaryFromFile(string file)
        {
            if (DicConfig.Count > 0)
                return true;

            DicPromos = new Dictionary<int, string>();
            DicConfig = new Dictionary<string, string>();

            if (!File.Exists(file)){
                Logger.Write(String.Format("El archivo '{0}' con las promociones no existe", file));
                return false;
            }

            var lines = File.ReadAllLines(file);

            foreach (var line in lines)
            {
                if(String.IsNullOrEmpty(line))
                    continue;

                var keyVal = line.Split('=');

                if (keyVal.Length != 2)
                {
                    Logger.Write(String.Format("El archivo '{0}' no tiene el formato correcto, debe ser llave=valor", file));
                    return false;
                }

                try
                {
                    int iVal;

                    if (int.TryParse(keyVal[0], out iVal))
                    {
                        DicPromos.Add(iVal, keyVal[1]);
                    }
                    else
                    {
                        DicConfig.Add(keyVal[0], keyVal[1]);                        
                    }

                }
                catch (Exception ex)
                {
                    Logger.Write(String.Format("La cadena '{0}' no tiene el formato correcto, debe ser llave(entero)=valor. Error: {1}", line, ex.Message));
                    return false;
                }
            }

            if (DicPromos.Count != 0)
                return true;
            
            Logger.Write("No se encontró alguna promoción en el archivo");
            return false;
        }
    }
}
