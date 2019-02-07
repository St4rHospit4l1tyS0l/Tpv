using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Tpv.Printer.Model.Shared;

namespace Tpv.Printer.Infrastructure.Crypto
{
    public static class AdvancedEncryptionStandard
    {
        public static string EncryptString(string plainText)
        {
            byte[] key;
            using (var sha256 = SHA256.Create())
            {
                key = sha256.ComputeHash(Encoding.ASCII.GetBytes(GlobalParams.AES_KEY));
            }

            var iv = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            return EncryptStringToBytesAes(plainText, key, iv);
        }

        public static string DecryptString(string cipherText)
        {
            byte[] key;
            using (var sha256 = SHA256.Create())
            {
                key = sha256.ComputeHash(Encoding.ASCII.GetBytes(GlobalParams.AES_KEY));
            }
            var iv = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            return DecryptStringFromBytesAes(cipherText, key, iv);
        }

        private static string EncryptStringToBytesAes(string plainText, byte[] key, byte[] iv)
        {
            if (plainText == null || plainText.Length <= 0)
                throw new Exception("Test empty to encrypt");
            byte[] encrypted;


            using (var aesAlg = new AesManaged())
            {
                var aesKey = new byte[32];
                Array.Copy(key, 0, aesKey, 0, 32);

                aesAlg.Key = aesKey;
                aesAlg.KeySize = 256;
                aesAlg.BlockSize = 128;
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.IV = iv;

                // Create a decrytor to perform the stream transform.
                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }


            return Convert.ToBase64String(encrypted);
        }

        private static string DecryptStringFromBytesAes(string cipherText, byte[] key, byte[] iv)
        {
            if (cipherText == null || cipherText.Length <= 0)
                throw new Exception("Test empty to decrypt");


            var encryptedbytes = Convert.FromBase64String(cipherText);

            string plaintext;


            using (var aesAlg = new AesManaged())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                // Create a decrytor to perform the stream transform.
                var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (var msDecrypt = new MemoryStream(encryptedbytes))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            return plaintext;
        }
    }
}