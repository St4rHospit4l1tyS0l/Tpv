using System;
using Tpv.Printer.Infrastructure.Crypto;

namespace Tpv.TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //var code = AdvancedEncryptionStandard.EncryptString("12345");
            //Console.WriteLine(code);
            //var decode = AdvancedEncryptionStandard.DecryptString(code);
            //Console.WriteLine(decode);
            var code = AdvancedEncryptionStandard.DecryptString("q+dlPFT7AkY15ocsX5qs5MlV/x1bB/2gojPhyQCqZts=");
            Console.WriteLine(code);
            var decode = AdvancedEncryptionStandard.EncryptString(code);
            Console.WriteLine(decode);
        }
    }
}
