namespace Telerik.Windows.Controls
{
    using System;
    using System.Runtime.CompilerServices;
    using Telerik.Windows;

    public class TileViewDragEventArgs : RadRoutedEventArgs
    {
        public TileViewDragEventArgs(object draggedItem, RoutedEvent routedEvent, object source) : base(routedEvent, source)
        {
            this.DraggedItem = draggedItem;
        }

        public object DraggedItem { get; set; }
    }
}

