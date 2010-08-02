
using System;

namespace Wing.Utils
{
    public static class RandomUtils
    {

        public static int Random(int digits)
        {
            Random _random = new Random(DateTime.Now.Millisecond);
            String initial = "1".PadRight(Math.Max(1, digits), '0');
            String final = "9".PadRight(Math.Max(1, digits), '9');
            return _random.Next(Int32.Parse(initial), Int32.Parse(final));
        }

        public static string RandomChars(int digits)
        {
            //calcular um numero randomico
            int rn = Random(digits + 3);
            //converter pra base 64
            String rc = ConvertUtils.ToBase64String(rn.ToString());
            //pegar apenas o tamanho desejado
            return StringUtils.Copy(rc, digits);
        }

        public static string RandomCharsFriendly(int digits)
        {
            //calcular um numero randomico
            int rn = Random(digits + 3);
            //converter pra base 64
            String rc = CryptoUtils.EncodeToBase64(ConvertUtils.ToBase64String(rn.ToString()));
            //pegar apenas o tamanho desejado
            return StringUtils.Copy(StringUtils.FilterFriendlyChars(rc.ToUpper()), digits);
        }

        static int _rnc = 0;
        public static String RandomName()
        {
            var r = ConvertUtils.ToBase64String(DateTime.Now.Ticks.ToString() + (++_rnc).ToString());
            return StringUtils.FilterChars(r) + StringUtils.FilterNumbers(r) + _rnc.ToString();
        }

        public static string RandomCode()
        {
            return Guid.NewGuid().ToString("N").ToLower();
        }
    }
}
