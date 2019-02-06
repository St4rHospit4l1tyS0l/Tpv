using System.IO;
using System.Threading;
using System.Web.Script.Serialization;

namespace Tpv.Printer.Infrastructure.Extensions
{
    public static class SerializeExtension
    {
        private static readonly JavaScriptSerializer _serializer;

        static SerializeExtension()
        {
            _serializer = new JavaScriptSerializer();
        }

        public static void Write(this string filename, object value)
        {
            try
            {
                using (var file = File.CreateText(filename))
                {
                    file.WriteLine(_serializer.Serialize(value));
                }
            }
            catch
            {
                //
            }
        }


        public static T Read<T>(this string filename, T defValue)
        {
            try
            {
                using (var file = File.OpenText(filename))
                {
                    return (T)_serializer.Deserialize(file.ReadToEnd(), typeof(T));
                }
            }
            catch
            {
                return defValue;
            }
        }

        public static bool ExistsFile(this string filename)
        {
            return File.Exists(filename);
        }

        public static void SafeForceRemoveFile(this string filename)
        {
            var tries = 0;
            while (tries++ < 5)
            {
                try
                {
                    File.Delete(filename);
                    break;
                }
                catch
                {
                    Thread.Sleep(100);
                }
            }
        }
    }
}
