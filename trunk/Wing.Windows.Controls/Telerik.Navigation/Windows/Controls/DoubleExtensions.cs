namespace Telerik.Windows.Controls
{
    using System;
    using System.Runtime.CompilerServices;

    internal static class DoubleExtensions
    {
        public static bool HasValue(this double value)
        {
            return (!double.IsNaN(value) && !double.IsInfinity(value));
        }
    }
}

