namespace Telerik.Windows.Controls
{
    using System;
    using System.Collections.ObjectModel;
    using Wing.Windows.Controls.Telerik.Navigation.Windows.Controls;
    using Telerik.Windows;

    [ScriptableType]
    public class RadTreeViewDragEndedEventArgs : RadTreeViewDragEventArgs
    {
        private Telerik.Windows.Controls.DropPosition dropPosition;
        private bool isCanceled;
        private RadTreeViewItem targetDropItem;

        public RadTreeViewDragEndedEventArgs(bool isCanceled, Collection<object> draggedItems, RadTreeViewItem targetDropItem, RoutedEvent routedEvent, object source) : base(draggedItems, routedEvent, source)
        {
            this.isCanceled = isCanceled;
            this.dropPosition = Telerik.Windows.Controls.DropPosition.Inside;
            if (targetDropItem != null)
            {
                this.dropPosition = targetDropItem.DropPosition;
            }
            this.targetDropItem = targetDropItem;
        }

        [ScriptableMember]
        public Telerik.Windows.Controls.DropPosition DropPosition
        {
            get
            {
                return this.dropPosition;
            }
        }

        [ScriptableMember]
        public bool IsCanceled
        {
            get
            {
                return this.isCanceled;
            }
        }

        [ScriptableMember]
        public RadTreeViewItem TargetDropItem
        {
            get
            {
                return this.targetDropItem;
            }
        }
    }
}

