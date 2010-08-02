using System;
using System.Security.Cryptography;
using System.Text;

namespace Wing.Utils
{
    public static class CryptoUtils
    {
        private static AesManaged cryptoProvider = new AesManaged();
        private static byte[] RCS_IV_BYTES = { 28, 130, 112, 64, 48, 92, 16, 8, 72, 126, 16, 22, 58, 96, 114, 32 };
        private static String cryptoKey = "mDzNRuLe";

        private static byte[] FormatKey(String key)
        {
            //a chave tem que ter entre 3 e 8 caracteres
            if (key.Length > 8) { key = key.Substring(0, 8); }
            key = key.PadRight(8, 'X');
            return new UnicodeEncoding().GetBytes(key);
        }

        public static byte[] Encode(String text, String key)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            byte[] UnicodeKey = FormatKey(key);
            byte[] Text = encoding.GetBytes(text);
            ICryptoTransform transf = cryptoProvider.CreateEncryptor(UnicodeKey, RCS_IV_BYTES);
            return transf.TransformFinalBlock(Text, 0, Text.Length);
        }

        public static string Decode(byte[] bytes, String key)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            byte[] UnicodeKey = FormatKey(key);
            ICryptoTransform transf = cryptoProvider.CreateDecryptor(UnicodeKey, RCS_IV_BYTES);
            byte[] result = transf.TransformFinalBlock(bytes, 0, bytes.Length);
            return encoding.GetString(result, 0, result.Length);
        }

        public static String EncodeToBase64(String text)
        {
            return Convert.ToBase64String(Encode(text, cryptoKey));
        }

        public static String DecodeFromBase64(String text)
        {
            return Decode(Convert.FromBase64String(text), cryptoKey);
        }

        public static String EncodeToHex(string str, string key)
        {
            byte[] bytes = Encode(str, key);
            StringBuilder res = new StringBuilder(bytes.Length * 2);

            foreach (byte b in bytes)
                res.Append(b.ToString("X2"));

            return res.ToString().ToLower();
        }

        public static String EncodeToHex(string str)
        {
            return EncodeToHex(str, cryptoKey);
        }

        public static String DecodeFromHex(String str, string key)
        {
            byte[] bytes = new byte[str.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
                bytes[i] = byte.Parse(str.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber);
            return Decode(bytes, key);
        }

        public static String DecodeFromHex(String str)
        {
            return DecodeFromHex(str, cryptoKey);
        }
    }
}
