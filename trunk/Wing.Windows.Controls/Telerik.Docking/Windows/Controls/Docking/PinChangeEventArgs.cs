namespace Telerik.Windows.Controls.Docking
{
    using System;
    using System.Runtime.CompilerServices;
    using Telerik.Windows;

    internal class PinChangeEventArgs : StateChangeCommandEventArgs
    {
        public PinChangeEventArgs(RoutedEvent routedEvent, object source, bool isPinned, bool isPinPerGroup, bool suppressEvents) : base(routedEvent, source, suppressEvents)
        {
            this.IsPinned = isPinned;
            this.IsPinPerGroup = isPinPerGroup;
        }

        public bool IsPinned { get; set; }

        public bool IsPinPerGroup { get; set; }
    }
}

