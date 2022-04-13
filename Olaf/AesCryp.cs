using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace AuraWave.WinApi.Olaf
{
    public class AesCryp
    {

   

        public static string Iv = "@l4km47++s3l0mi@";  // 16 chars = 128 bytes
        public static string Key = "ow7dxys8glfor9tnc2ansdfo1etkfjca";   // 32 chars = 256 bytes

        public static string Encrypt(string decrypted)
        {
            var textbytes = Encoding.ASCII.GetBytes(decrypted);
            var encdec = new AesCryptoServiceProvider
            {
                BlockSize = 128,
                KeySize = 256,
                Key = Encoding.ASCII.GetBytes(Key),
                IV = Encoding.ASCII.GetBytes(Iv),
                Padding = PaddingMode.PKCS7,
                Mode = CipherMode.CBC
            };

            var icrypt = encdec.CreateEncryptor(encdec.Key, encdec.IV);

            var enc = icrypt.TransformFinalBlock(textbytes, 0, textbytes.Length);
            icrypt.Dispose();

            return Convert.ToBase64String(enc);
        }

        public static string Decrypt(string encrypted)
        {
            var encbytes = Convert.FromBase64String(encrypted);
            var encdec = new AesCryptoServiceProvider
            {
                BlockSize = 128,
                KeySize = 256,
                Key = Encoding.ASCII.GetBytes(Key),
                IV = Encoding.ASCII.GetBytes(Iv),
                Padding = PaddingMode.PKCS7,
                Mode = CipherMode.CBC
            };

            var icrypt = encdec.CreateDecryptor(encdec.Key, encdec.IV);

            var dec = icrypt.TransformFinalBlock(encbytes, 0, encbytes.Length);
            icrypt.Dispose();

            return Encoding.ASCII.GetString(dec);
        }
    }
}
