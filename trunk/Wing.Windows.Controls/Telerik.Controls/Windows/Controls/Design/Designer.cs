namespace Telerik.Windows.Controls.Design
{
    using System;
    using System.ComponentModel;
    using System.Windows.Controls;

    internal static class Designer
    {
        private static bool? isInDesignMode = null;

        public static void SimulateDesignMode(bool simulate)
        {
            isInDesignMode = new bool?(simulate);
        }

        public static bool IsInDesignMode
        {
            get
            {
                if (!isInDesignMode.HasValue)
                {
                    isInDesignMode = new bool?(DesignerProperties.GetIsInDesignMode(new ContentControl()));
                }
                return isInDesignMode.Value;
            }
        }
    }
}

