namespace Telerik.Windows.Controls.Common
{
    using System;
    using System.Windows;
    using System.Windows.Input;

    internal class TestableMouseEventArgs : IMouseEventArgs
    {
        private MouseEventArgs originalArgs;

        internal TestableMouseEventArgs(MouseEventArgs e)
        {
            this.originalArgs = e;
        }

        public Point GetPosition(UIElement relativeTo)
        {
            return this.originalArgs.GetPosition(relativeTo);
        }

        public object OriginalSource
        {
            get
            {
                return this.originalArgs.OriginalSource;
            }
        }
    }
}

