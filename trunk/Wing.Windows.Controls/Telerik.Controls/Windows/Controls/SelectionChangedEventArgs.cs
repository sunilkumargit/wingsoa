namespace Telerik.Windows.Controls
{
    using System;
    using System.Collections;
    using Telerik.Windows;

    public class SelectionChangedEventArgs : RadRoutedEventArgs
    {
        private object[] addedItems;
        private object[] removedItems;

        public SelectionChangedEventArgs(RoutedEvent routedEvent, IList removedItems, IList addedItems) : base(routedEvent)
        {
            if (routedEvent == null)
            {
                throw new ArgumentNullException("routedEvent", Telerik.Windows.Controls.SR.GetString("RadSelectionChangedEventArgsIdArgumentNullException"));
            }
            if (removedItems == null)
            {
                throw new ArgumentNullException("removedItems", Telerik.Windows.Controls.SR.GetString("RadSelectionChangedEventArgsRemovedItemsArgumentNullException"));
            }
            if (addedItems == null)
            {
                throw new ArgumentNullException("addedItems", Telerik.Windows.Controls.SR.GetString("RadSelectionChangedEventArgsAddedItemsArgumentNullException"));
            }
            this.removedItems = new object[removedItems.Count];
            removedItems.CopyTo(this.removedItems, 0);
            this.addedItems = new object[addedItems.Count];
            addedItems.CopyTo(this.addedItems, 0);
        }

        public IList AddedItems
        {
            get
            {
                return this.addedItems;
            }
        }

        public IList RemovedItems
        {
            get
            {
                return this.removedItems;
            }
        }
    }
}

