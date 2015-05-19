using System;
using System.Collections.Generic;
using System.IO;
using Tpv.Ui.Model;

namespace Tpv.Ui.Infrastructure.Connector
{
    public static class DbReader
    {
        public static Dictionary<int, string> ReadDictionaryFromFile(string file, ResponseMessage response)
        {
            var dic = new Dictionary<int, string>();

            if (!File.Exists(file)){
                response.Message = String.Format("El archivo '{0}' con las promociones no existe", file);
                response.IsSuccess = false;
                return null;
            }

            var lines = File.ReadAllLines(file);

            foreach (var line in lines)
            {
                if(String.IsNullOrEmpty(line))
                    continue;

                var keyVal = line.Split('=');

                if (keyVal.Length != 2)
                {
                    response.Message = String.Format("El archivo '{0}' no tiene el formato correcto, debe ser llave=valor", file);
                    response.IsSuccess = false;
                    return null;
                }

                try
                {
                    dic.Add(int.Parse(keyVal[0]), keyVal[1]);
                }
                catch (Exception ex)
                {
                    response.Message = String.Format("La cadena '{0}' no tiene el formato correcto, debe ser llave(entero)=valor. Error: {1}", line, ex.Message);
                    response.IsSuccess = false;
                    return null;
                }
            }

            if (dic.Count != 0)
            {
                response.IsSuccess = true;
                return dic;
            }
            
            response.Message = "No se encontró alguna promoción en el archivo";
            response.IsSuccess = false;
            return null;
        }
    }
}
