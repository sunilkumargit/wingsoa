namespace Telerik.Windows.Controls.Common
{
    using System;
    using System.Windows.Input;

    internal class TestableMouseButtonEventArgs : TestableMouseEventArgs, IMouseButtonEventArgs, IMouseEventArgs
    {
        private MouseButtonEventArgs originalButtonArgs;

        internal TestableMouseButtonEventArgs(MouseButtonEventArgs e) : base(e)
        {
            this.originalButtonArgs = e;
        }

        public bool Handled
        {
            get
            {
                return this.originalButtonArgs.Handled;
            }
            set
            {
                this.originalButtonArgs.Handled = value;
            }
        }
    }
}

