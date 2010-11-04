namespace Telerik.Windows.Controls.DragDrop
{
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using Telerik.Windows;
    using Telerik.Windows.Controls;

    public class DragDropEventArgs : RadRoutedEventArgs
    {
        public DragDropEventArgs(Telerik.Windows.RoutedEvent routedEvent, object source, DragDropOptions options) : base(routedEvent, source)
        {
            this.Options = options;
        }

        public T GetElement<T>(Point dragPoint) where T: FrameworkElement
        {
            return VisualTreeHelperExtensions.GetElementsInScreenCoordinates<T>(dragPoint).FirstOrDefault<T>();
        }

        protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget)
        {
            (genericHandler as EventHandler<DragDropEventArgs>)(genericTarget, this);
        }

        public DragDropOptions Options { get; internal set; }
    }
}

