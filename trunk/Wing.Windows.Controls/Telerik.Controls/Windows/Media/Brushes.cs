namespace Telerik.Windows.Media
{
    using System;
    using System.Windows.Media;

    internal static class Brushes
    {
        private static SolidColorBrush transparentBrush;

        public static SolidColorBrush Transparent
        {
            get
            {
                if (transparentBrush == null)
                {
                    transparentBrush = new SolidColorBrush(Colors.Transparent);
                }
                return transparentBrush;
            }
        }
    }
}

