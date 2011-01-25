using System;
using System.Text.RegularExpressions;

namespace Wing
{
    [System.Diagnostics.DebuggerStepThrough]
    public static class StringHelper
    {
        public static readonly string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        public static readonly string Numbers = "0123456789";
        public static readonly string Alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        public static readonly string UppercaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public static readonly char[] UppercaseCharsArray = new char[] {
            'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z'};

        public static String FilterNumbers(String str)
        {
            return Filter(str, Numbers);
        }

        public static String FilterChars(String str)
        {
            return Filter(str, Chars);
        }

        public static String FirstWord(String phrase)
        {
            if (phrase != null)
            {
                int index = phrase.IndexOf(" ");
                if (index >= 0)
                {
                    return phrase.Substring(0, index);
                }
                else
                    return phrase;
            }
            else
                return "";
        }

        public static String FirstUpper(String phrase)
        {
            if (!phrase.IsEmpty())
                return phrase.Substring(0, 1).ToUpper() + phrase.Substring(1);
            else
                return "";
        }

        public static String NextWord(int startIndex, String phrase)
        {
            if (phrase != null && startIndex < phrase.Length)
            {
                int index = phrase.IndexOf(" ", startIndex);
                if (index >= startIndex)
                {
                    return phrase.Substring(index, (phrase.Length - index)).Trim();
                }
                else
                    return phrase;
            }
            else
                return "";
        }

        public static String Parse(Object obj)
        {
            if (obj != null)
                return obj.ToString();
            return "";
        }

        public static String Copy(String str, int startIndex, int count)
        {
            if (String.IsNullOrEmpty(str))
                return "";
            else
            {
                int copyCount = Math.Min(count, str.Length - startIndex);
                return str.Substring(startIndex, copyCount);
            }
        }

        public static String Copy(String str, int count)
        {
            return Copy(str, 0, count);
        }

        public static String Filter(String str, String allowedChars)
        {
            String result = "";
            foreach (Char c in str.AsString())
            {
                if (allowedChars.Contains(c.ToString()))
                    result += c;
            }

            return result;
        }

        public static String FilterFriendlyChars(String str)
        {
            return Filter(str, "123456789ABCDEFGHIJKLMPRSTWXYZ");
        }

        public static String RemoveAccents(String phrase)
        {
            string accentChars = "·‡„‚‰ÈËÍÎÌÏÓÔÛÚıÙˆ˙˘˚¸Á¡¿√¬ƒ…» ÀÕÃŒœ”“’÷‘⁄Ÿ€‹«";
            string resultChars = "aaaaaeeeeiiiiooooouuuucAAAAAEEEEIIIIOOOOOUUUUC";
            for (int i = 0; i < accentChars.Length; i++)
                phrase = phrase.Replace(accentChars[i], resultChars[i]);
            return phrase;
        }

        public static String Clamp(String text, int maxSize, String signals)
        {
            text = text ?? "";
            signals = signals ?? "";
            if (text.Length > maxSize)
                return Copy(text, 0, maxSize - signals.Length) + signals;
            return text;
        }

        public static String Clamp(String text, int maxSize)
        {
            return Clamp(text, maxSize, " ...");
        }

        public static String Quoted(String str)
        {
            return "\"" + (str ?? "") + "\"";
        }

        public static String[] SplitLines(String str)
        {
            if (String.IsNullOrEmpty(str))
                return new String[0];
            return Regex.Split(str, "\r\n");
        }
    }

    [System.Diagnostics.DebuggerStepThrough]
    public static class StringHelperExtensions
    {
        public static bool IsEmpty(this String str)
        {
            return String.IsNullOrEmpty(str);
        }

        public static bool HasValue(this String str)
        {
            return !String.IsNullOrEmpty(str);
        }

        public static String Default(this String str, String ifEmpty)
        {
            return String.IsNullOrEmpty(str) ? ifEmpty : str;
        }

        public static bool EqualsIgnoreCase(this String str, String str2)
        {
            return str == str2
                || (str != null && str.Equals(str2, StringComparison.OrdinalIgnoreCase));
        }

        public static String FirstUpper(this String str)
        {
            return StringHelper.FirstUpper(str);
        }

        public static String Quoted(this String str)
        {
            return StringHelper.Quoted(str);
        }

        public static string RemoveAccents(this String str)
        {
            return StringHelper.RemoveAccents(str ?? "");
        }

        public static String Copy(this String str, int startIndex, int count)
        {
            return StringHelper.Copy(str, startIndex, count);
        }

        public static String Copy(this String str, int count)
        {
            return StringHelper.Copy(str, 0, count);
        }

        public static String FirstWord(this String str)
        {
            return StringHelper.FirstWord(str);
        }

        public static String Templ(this String str, params Object[] args)
        {
            return String.Format(str ?? "", args);
        }

        public static String FilterNumbers(this String str)
        {
            return StringHelper.FilterNumbers(str);
        }

        public static String FilterChars(this String str)
        {
            return StringHelper.FilterChars(str);
        }

        public static String Filter(this String str, String allowedChars)
        {
            return StringHelper.Filter(str, allowedChars);
        }

        public static String Clamp(this String str, int maxSize, String signals)
        {
            return StringHelper.Clamp(str ?? "", maxSize, signals);
        }

        public static String Clamp(this String str, int maxSize)
        {
            return StringHelper.Clamp(str ?? "", maxSize);
        }
    }
}
