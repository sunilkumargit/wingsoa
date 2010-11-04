namespace Telerik.Windows.Controls
{
    using System;
    using System.Runtime.CompilerServices;
    using Telerik.Windows;

    public class FoldEventArgs : RadRoutedEventArgs
    {
        public FoldEventArgs(object source, FoldPosition position, RoutedEvent routedEvent) : base(routedEvent, source)
        {
            this.Position = position;
        }

        public FoldPosition Position { get; private set; }
    }
}

