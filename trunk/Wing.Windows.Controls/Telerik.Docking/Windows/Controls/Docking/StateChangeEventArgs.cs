namespace Telerik.Windows.Controls.Docking
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using Telerik.Windows;

    public class StateChangeEventArgs : RadRoutedEventArgs
    {
        public StateChangeEventArgs(RoutedEvent routedEvent, object source, IEnumerable<RadPane> panes) : base(routedEvent, source)
        {
            this.Panes = panes;
        }

        public IEnumerable<RadPane> Panes { get; private set; }
    }
}

