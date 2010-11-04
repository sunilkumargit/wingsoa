namespace Telerik.Windows.Controls
{
    using System;
    using System.Runtime.CompilerServices;
    using Telerik.Windows;

    public class CancelRoutedEventArgs : RadRoutedEventArgs
    {
        public CancelRoutedEventArgs()
        {
        }

        public CancelRoutedEventArgs(RoutedEvent routedEvent) : base(routedEvent)
        {
        }

        public CancelRoutedEventArgs(RoutedEvent routedEvent, object source) : base(routedEvent, source)
        {
        }

        public bool Cancel { get; set; }
    }
}

