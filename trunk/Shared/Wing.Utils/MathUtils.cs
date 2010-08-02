using System;

namespace Wing.Utils
{
    public static class MathUtils
    {
        private static long _seq = 1000;

        public static String Seq(String format)
        {
            return Seq().ToString(format);
        }

        public static long Seq()
        {
            return (++_seq);
        }

        public static Int32 GetDecimalIntegerFloor(Int32 value)
        {
            return Convert.ToInt32(Math.Floor(Convert.ToDouble(value / 10)) * 10);
        }

        public static Int32 GetDecimalIntegerFloor(Decimal value)
        {
            return GetDecimalIntegerFloor(Convert.ToInt32(Math.Floor(Convert.ToDouble(value))));
        }

        public static byte GenerateMod11CheckDigit(Decimal number)
        {
            return GenerateMod11CheckDigit(number.ToString());
        }

        public static byte GenerateMod11CheckDigit(String number)
        {
            int sum = 0;
            int rest;
            number = number.Trim();

            for (int i = 0; i < number.Length; i++)
                sum += Convert.ToInt32(number.Substring(number.Length - i - 1, 1)) * (i + 2);

            rest = sum % 11;
            if (rest > 9)
                rest -= 9;

            return Convert.ToByte(rest);
        }

        public static byte GenerateCheckDigit(String str)
        {
            //gerar uma string com os valores decimais dos caracteres
            String seq = str.Length.ToString();
            for (int i = 0; i < str.Length; i++)
                seq += Convert.ToByte(str[i]).ToString();
            return GenerateMod11CheckDigit(seq);
        }

        public static bool IsOdd(int n)
        {
            return (n % 2) == 1;
        }
    }
}
