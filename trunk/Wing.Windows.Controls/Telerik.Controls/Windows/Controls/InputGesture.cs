namespace Telerik.Windows.Controls
{
    using System;
    using System.Windows;

    public abstract class InputGesture
    {
        protected InputGesture()
        {
        }

        public abstract bool Matches(object targetElement, RoutedEventArgs inputEventArgs);
    }
}

