namespace Telerik.Windows.Controls
{
    using System;

    internal static class BooleanBoxes
    {
        private static object falseBox = false;
        private static object trueBox = true;

        internal static object Box(bool value)
        {
            if (value)
            {
                return TrueBox;
            }
            return FalseBox;
        }

        public static object FalseBox
        {
            get
            {
                return falseBox;
            }
            set
            {
                falseBox = value;
            }
        }

        public static object TrueBox
        {
            get
            {
                return trueBox;
            }
            set
            {
                trueBox = value;
            }
        }
    }
}

