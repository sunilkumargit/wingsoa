namespace Telerik.Windows.Controls.Docking
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using Telerik.Windows;

    public class ResizeEventArgs : RadRoutedEventArgs
    {
        public ResizeEventArgs(Telerik.Windows.RoutedEvent routedEvent, object source) : base(routedEvent, source)
        {
            this.AvailableSize = Size.Empty;
            this.MinSize = Size.Empty;
        }

        protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget)
        {
            EventHandler<ResizeEventArgs> handler = (EventHandler<ResizeEventArgs>) genericHandler;
            handler(genericTarget, this);
        }

        internal FrameworkElement AffectedElement { get; set; }

        internal Size AvailableSize { get; set; }

        internal Size MinSize { get; set; }

        internal FrameworkElement ResizedElement { get; set; }

        internal bool ShowsPreview { get; set; }
    }
}

