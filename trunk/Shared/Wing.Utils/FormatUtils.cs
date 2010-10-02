using System;

namespace Wing.Utils
{
    public static class FormatUtils
    {
        public static String FormatCpf(String cpf)
        {
            return StuffChars(StringUtils.FilterNumbers(cpf), "xxx.xxx.xxx-xx", true);
        }

        public static String FormatFone(String fone)
        {
            return StuffChars(StringUtils.FilterNumbers(fone), "(xx)xxxx-xxxx");
        }

        public static String FormatCEP(String cep)
        {
            return StuffChars(StringUtils.FilterNumbers(cep), "xxxxx-xxx");
        }

        public static String StuffChars(String source, String picture)
        {
            return StuffChars(source, picture, false, "");
        }

        public static String StuffChars(String source, String picture, Boolean reverse)
        {
            return StuffChars(source, picture, reverse, "");
        }

        public static String StuffChars(String source, String picture, Boolean reverse, String removedChar)
        {
            String result = "";
            String nextChar = "";
            int sourceIndex = reverse ? source.Length - 1 : 0;
            int pictIndex = reverse ? picture.Length - 1 : 0;
            int inc = reverse ? -1 : 1;

            while (pictIndex > -1
                && pictIndex < picture.Length
                && sourceIndex < source.Length)
            {
                //ler a picture e determinar de onde tirar o caracter
                String pictChar = picture.Substring(pictIndex, 1);
                nextChar = "";
                if (pictChar.Equals("x", StringComparison.OrdinalIgnoreCase))
                {
                    if (sourceIndex > -1)
                    {
                        nextChar = source.Substring(sourceIndex, 1);
                        sourceIndex += inc;
                    }
                }
                else if (pictChar.Equals("<"))
                {
                    nextChar = removedChar;
                    sourceIndex += inc;
                }
                else if (sourceIndex > -1)
                    nextChar = pictChar;

                //adicionar no resultado
                if (reverse)
                    result = nextChar + result;
                else
                    result += nextChar;

                //incrementar os indices
                pictIndex += inc;
            }

            return result;
        }

        public static String CreateComponentName(String name)
        {
            var result = "";
            foreach (var ch in name)
            {
                if (StringUtils.FilterChars(ch.ToString()) == "" && StringUtils.FilterNumbers(ch.ToString()) == "")
                    result += "_" + Convert.ToByte(ch).ToString("X");
                else
                    result += ch;
            }
            return result;
        }
    }
}
