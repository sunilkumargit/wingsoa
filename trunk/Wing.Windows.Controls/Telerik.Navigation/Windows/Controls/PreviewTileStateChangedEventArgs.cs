namespace Telerik.Windows.Controls
{
    using System;
    using System.Runtime.CompilerServices;
    using Telerik.Windows;

    public class PreviewTileStateChangedEventArgs : RadRoutedEventArgs
    {
        public PreviewTileStateChangedEventArgs()
        {
        }

        public PreviewTileStateChangedEventArgs(TileViewItemState tileState)
        {
            this.TileState = tileState;
        }

        public PreviewTileStateChangedEventArgs(RoutedEvent routedEvent, object source)
        {
            base.RoutedEvent = routedEvent;
            base.Source = source;
        }

        public TileViewItemState TileState { get; internal set; }
    }
}

