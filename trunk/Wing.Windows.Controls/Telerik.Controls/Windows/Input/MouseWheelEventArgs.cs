namespace Telerik.Windows.Input
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using Telerik.Windows;

    public class MouseWheelEventArgs : RadRoutedEventArgs
    {
        internal MouseWheelEventArgs(Telerik.Windows.RoutedEvent routedEvent, Point mousePosition, int delta) : base(routedEvent)
        {
            this.Delta = delta;
            this.MousePosition = mousePosition;
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

        public int Delta { get; private set; }

        internal Point MousePosition { get; private set; }
    }
}

