namespace Telerik.Windows.Controls
{
    using System;
    using System.Runtime.CompilerServices;
    using Telerik.Windows;

    public class RadTreeViewCheckEventArgs : RadRoutedEventArgs
    {
        internal RadTreeViewCheckEventArgs(RoutedEvent routedEvent, object source) : base(routedEvent, source)
        {
        }

        public bool IsUserInitiated { get; internal set; }
    }
}

