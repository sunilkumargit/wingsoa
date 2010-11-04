namespace Telerik.Windows.Controls.PanelBar
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Windows.Controls;
    using Telerik.Windows;

    public class OrientationChangedEventArgs : RadRoutedEventArgs
    {
        public OrientationChangedEventArgs(object source, System.Windows.Controls.Orientation orientation, RoutedEvent routedEvent) : base(routedEvent, source)
        {
            this.Orientation = orientation;
        }

        public System.Windows.Controls.Orientation Orientation { get; set; }
    }
}

