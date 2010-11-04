namespace Telerik.Windows
{
    using System;

    internal static class SystemParameters
    {
        private const double minimumHorizontalDragDistance = 4.0;
        private const double minimumVerticalDragDistance = 4.0;

        internal static double MinimumHorizontalDragDistance
        {
            get
            {
                return 4.0;
            }
        }

        internal static double MinimumVerticalDragDistance
        {
            get
            {
                return 4.0;
            }
        }
    }
}

