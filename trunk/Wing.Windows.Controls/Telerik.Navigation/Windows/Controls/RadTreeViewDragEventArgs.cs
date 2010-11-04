namespace Telerik.Windows.Controls
{
    using System;
    using System.Collections.ObjectModel;
    using Wing.Windows.Controls.Telerik.Navigation.Windows.Controls;
    using Telerik.Windows;

    [ScriptableType]
    public class RadTreeViewDragEventArgs : RadRoutedEventArgs
    {
        private Collection<object> draggedItems;

        public RadTreeViewDragEventArgs(Collection<object> draggedItems, RoutedEvent routedEvent, object source) : base(routedEvent, source)
        {
            this.draggedItems = draggedItems;
        }

        [ScriptableMember]
        public Collection<object> DraggedItems
        {
            get
            {
                return this.draggedItems;
            }
        }
    }
}

