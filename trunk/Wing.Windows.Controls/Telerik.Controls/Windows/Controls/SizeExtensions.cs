namespace Telerik.Windows.Controls
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Windows;

    public static class SizeExtensions
    {
        internal static Size Round(this Size size)
        {
            return new Size(Math.Round(size.Width), Math.Round(size.Height));
        }

        internal static Size Swap(this Size size)
        {
            return new Size(size.Height, size.Width);
        }
    }
}

