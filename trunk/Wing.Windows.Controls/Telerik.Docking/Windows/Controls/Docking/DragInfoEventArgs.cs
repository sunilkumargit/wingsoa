namespace Telerik.Windows.Controls.Docking
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using Telerik.Windows;

    internal class DragInfoEventArgs : RadRoutedEventArgs
    {
        public DragInfoEventArgs()
        {
        }

        public DragInfoEventArgs(Telerik.Windows.RoutedEvent routedEvent) : base(routedEvent)
        {
        }

        public DragInfoEventArgs(Telerik.Windows.RoutedEvent routedEvent, Point globalMousePosition) : this(routedEvent)
        {
            this.GlobalMousePosition = globalMousePosition;
        }

        public DragInfoEventArgs(Telerik.Windows.RoutedEvent routedEvent, Point globalMousePosition, bool canceled) : this(routedEvent)
        {
            this.GlobalMousePosition = globalMousePosition;
            this.Canceled = canceled;
        }

        protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget)
        {
            DragInfoEventHandler handler = (DragInfoEventHandler) genericHandler;
            handler(genericTarget, this);
        }

        public bool Canceled { get; private set; }

        public Point GlobalMousePosition { get; private set; }
    }
}

