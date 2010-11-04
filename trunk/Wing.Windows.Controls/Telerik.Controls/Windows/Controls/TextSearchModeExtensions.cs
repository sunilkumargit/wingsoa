namespace Telerik.Windows.Controls
{
    using System;
    using System.Runtime.CompilerServices;

    internal static class TextSearchModeExtensions
    {
        public static bool IsCaseSensitive(this TextSearchMode mode)
        {
            if (mode != TextSearchMode.ContainsCaseSensitive)
            {
                return (mode == TextSearchMode.StartsWithCaseSensitive);
            }
            return true;
        }

        public static bool IsContains(this TextSearchMode mode)
        {
            if (mode != TextSearchMode.Contains)
            {
                return (mode == TextSearchMode.ContainsCaseSensitive);
            }
            return true;
        }

        public static bool IsStartsWith(this TextSearchMode mode)
        {
            if (mode != TextSearchMode.StartsWith)
            {
                return (mode == TextSearchMode.StartsWithCaseSensitive);
            }
            return true;
        }
    }
}

