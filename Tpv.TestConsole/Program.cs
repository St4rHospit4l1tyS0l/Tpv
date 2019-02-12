using System;
using Tpv.Printer.Infrastructure.Crypto;

namespace Tpv.TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var code = AdvancedEncryptionStandard.EncryptString("12345");
            Console.WriteLine(code);
            var decode = AdvancedEncryptionStandard.DecryptString(code);
            Console.WriteLine(decode);
        }
    }
}
