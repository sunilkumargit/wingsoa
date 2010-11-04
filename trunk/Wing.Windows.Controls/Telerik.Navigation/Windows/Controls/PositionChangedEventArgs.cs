namespace Telerik.Windows.Controls
{
    using System;
    using System.Runtime.CompilerServices;
    using Telerik.Windows;

    public class PositionChangedEventArgs : RadRoutedEventArgs
    {
        public PositionChangedEventArgs(object source, OutlookBarItemPosition oldPosition, OutlookBarItemPosition newPosition) : base(RadOutlookBarItem.PositionChangedEvent, source)
        {
            this.NewPosition = newPosition;
            this.OldPosition = oldPosition;
        }

        protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget)
        {
            (genericHandler as PositionChangedEventHandler)(genericTarget, this);
        }

        public OutlookBarItemPosition NewPosition { get; set; }

        public OutlookBarItemPosition OldPosition { get; set; }
    }
}

