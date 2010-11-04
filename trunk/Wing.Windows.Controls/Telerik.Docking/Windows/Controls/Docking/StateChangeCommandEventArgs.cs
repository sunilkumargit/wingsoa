namespace Telerik.Windows.Controls.Docking
{
    using System;
    using System.Runtime.CompilerServices;
    using Telerik.Windows;

    internal class StateChangeCommandEventArgs : RadRoutedEventArgs
    {
        public StateChangeCommandEventArgs(RoutedEvent routedEvent, object source, bool suppressEvents) : base(routedEvent, source)
        {
            this.SuppressEvents = suppressEvents;
        }

        public bool Canceled { get; set; }

        public bool SuppressEvents { get; set; }
    }
}

