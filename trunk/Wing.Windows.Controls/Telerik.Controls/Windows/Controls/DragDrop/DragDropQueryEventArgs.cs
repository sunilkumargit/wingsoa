namespace Telerik.Windows.Controls.DragDrop
{
    using System;
    using System.Runtime.CompilerServices;
    using Telerik.Windows;

    public class DragDropQueryEventArgs : DragDropEventArgs
    {
        public DragDropQueryEventArgs(RoutedEvent routedEvent, object source, DragDropOptions options) : base(routedEvent, source, options)
        {
            base.Options = options;
        }

        protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget)
        {
            (genericHandler as EventHandler<DragDropQueryEventArgs>)(genericTarget, this);
        }

        public bool? QueryResult { get; set; }
    }
}

