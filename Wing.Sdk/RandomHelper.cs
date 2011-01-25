using System;

namespace Wing
{
    [System.Diagnostics.DebuggerStepThrough]
    public static class RandomHelper
    {
        private static Random _random = new Random(DateTime.Now.Millisecond);

        public static int Next(int minValue, int maxValue)
        {
            return _random.Next(minValue, maxValue);
        }

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
            String rc = ConversionHelper.ToBase64String(rn.ToString());
            //pegar apenas o tamanho desejado
            return StringHelper.Copy(rc, digits);
        }

        public static string RandomCharsFriendly(int digits)
        {
            //calcular um numero randomico
            int rn = Random(digits + 3);
            //converter pra base 64
            String rc = CryptographyHelper.EncodeToBase64(ConversionHelper.ToBase64String(rn.ToString()));
            //pegar apenas o tamanho desejado
            return StringHelper.Copy(StringHelper.FilterFriendlyChars(rc.ToUpper()), digits);
        }

        static int _rnc = 0;
        public static String RandomName()
        {
            var r = ConversionHelper.ToBase64String(DateTime.Now.Ticks.ToString() + (++_rnc).ToString());
            return StringHelper.FilterChars(r) + StringHelper.FilterNumbers(r) + _rnc.ToString();
        }

        public static string RandomCode()
        {
            return Guid.NewGuid().ToString("N").ToLower();
        }
    }
}
