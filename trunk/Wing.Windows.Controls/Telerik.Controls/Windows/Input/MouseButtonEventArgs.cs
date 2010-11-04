namespace Telerik.Windows.Input
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Input;
    using Telerik.Windows;

    public class MouseButtonEventArgs : RadRoutedEventArgs
    {
        private System.Windows.Input.MouseButtonEventArgs originalMouseButtonEventArgs;

        internal MouseButtonEventArgs(Telerik.Windows.RoutedEvent routedEvent, Point mousePosition, MouseButton changedButton, MouseButtonState buttonState, int clickCount, System.Windows.Input.MouseButtonEventArgs args) : base(routedEvent)
        {
            this.MousePosition = mousePosition;
            this.ChangedButton = changedButton;
            this.ButtonState = buttonState;
            this.ClickCount = clickCount;
            this.originalMouseButtonEventArgs = args;
        }

        public Point GetPosition(UIElement relativeTo)
        {
            if (relativeTo == null)
            {
                return this.MousePosition;
            }
            Point p = relativeTo.TransformToVisual(Application.Current.RootVisual).Transform(new Point(0.0, 0.0));
            return new Point(this.MousePosition.X - p.X, this.MousePosition.Y - p.Y);
        }

        public MouseButtonState ButtonState { get; private set; }

        public MouseButton ChangedButton { get; private set; }

        public int ClickCount { get; private set; }

        public bool Handled
        {
            get
            {
                return ((this.originalMouseButtonEventArgs != null) && this.originalMouseButtonEventArgs.Handled);
            }
            set
            {
                if (this.originalMouseButtonEventArgs != null)
                {
                    this.originalMouseButtonEventArgs.Handled = value;
                }
            }
        }

        internal Point MousePosition { get; private set; }
    }
}

