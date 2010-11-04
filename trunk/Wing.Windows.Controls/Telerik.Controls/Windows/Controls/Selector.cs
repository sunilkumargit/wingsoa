namespace Telerik.Windows.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using Telerik.Windows;

    public abstract class Selector : Telerik.Windows.Controls.ItemsControl
    {
        private int deferredSelectedIndex = -1;
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.RegisterAttached("IsSelected", typeof(bool), typeof(Selector), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(Selector.OnIsSelectedChanged)));
        public static readonly Telerik.Windows.RoutedEvent SelectedEvent = EventManager.RegisterRoutedEvent("Selected", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Selector));
        public static readonly DependencyProperty SelectedIndexProperty = DependencyPropertyExtensions.Register("SelectedIndex", typeof(int), typeof(Selector), new Telerik.Windows.PropertyMetadata(-1, new PropertyChangedCallback(Selector.OnSelectedIndexChanged), new CoerceValueCallback(Selector.CoerceSelectedIndex)));
        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(object), typeof(Selector), new Telerik.Windows.PropertyMetadata(null, new PropertyChangedCallback(Selector.OnSelectedItemChanged), new CoerceValueCallback(Selector.CoerceSelectedItem)));
        private SelectionChanger<object> selectedItems;
        public static readonly DependencyProperty SelectedValuePathProperty = DependencyProperty.Register("SelectedValuePath", typeof(string), typeof(Selector), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(Selector.OnSelectedValuePathChanged)));
        public static readonly DependencyProperty SelectedValueProperty = DependencyPropertyExtensions.Register("SelectedValue", typeof(object), typeof(Selector), new Telerik.Windows.PropertyMetadata(null, new PropertyChangedCallback(Selector.OnSelectedValueChanged), new CoerceValueCallback(Selector.OnCoerceSelectedValue)));
        private bool selectingItemWithValue;
        public static readonly Telerik.Windows.RoutedEvent SelectionChangedEvent = EventManager.RegisterRoutedEvent("SelectionChanged", RoutingStrategy.Bubble, typeof(Telerik.Windows.Controls.SelectionChangedEventHandler), typeof(Selector));
        private bool skipCoerceSelectedItemCheck;
        public static readonly Telerik.Windows.RoutedEvent UnselectedEvent = EventManager.RegisterRoutedEvent("Unselected", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Selector));

        [Telerik.Windows.Controls.SRCategory("Behavior")]
        public event Telerik.Windows.Controls.SelectionChangedEventHandler SelectionChanged
        {
            add
            {
                this.AddHandler(SelectionChangedEvent, value);
            }
            remove
            {
                this.RemoveHandler(SelectionChangedEvent, value);
            }
        }

        static Selector()
        {
            EventManager.RegisterClassHandler(typeof(Selector), SelectedEvent, new RoutedEventHandler(Selector.OnSelected));
            EventManager.RegisterClassHandler(typeof(Selector), UnselectedEvent, new RoutedEventHandler(Selector.OnUnselected));
        }

        internal Selector()
        {
            this.SelectedIndex = -1;
            this.selectedItems = new SelectionChanger<object>(this);
            this.selectedItems.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.OnSelectionChanged);
        }

        public static void AddSelectedHandler(DependencyObject element, RoutedEventHandler handler)
        {
            element.AddHandler(SelectedEvent, handler);
        }

        public static void AddUnselectedHandler(DependencyObject element, RoutedEventHandler handler)
        {
            element.AddHandler(UnselectedEvent, handler);
        }

        private static object CoerceSelectedIndex(DependencyObject d, object value)
        {
            Selector selector = (Selector)d;
            if ((value is int) && (((int)value) >= selector.Items.Count))
            {
                selector.deferredSelectedIndex = (int)value;
                return -1;
            }
            selector.deferredSelectedIndex = -1;
            return value;
        }

        private static object CoerceSelectedItem(DependencyObject d, object value)
        {
            Selector selector = (Selector)d;
            if ((value != null) && !selector.skipCoerceSelectedItemCheck)
            {
                int selectedIndex = selector.SelectedIndex;
                if (((selectedIndex > -1) && (selectedIndex < selector.Items.Count)) && (selector.Items[selectedIndex] == value))
                {
                    return value;
                }
                if (!selector.Items.Contains(value))
                {
                    return null;
                }
            }
            return value;
        }

        private object CoerceSelectedValue(object value)
        {
            if (this.SelectionChange.IsActive)
            {
                this.selectingItemWithValue = false;
                return value;
            }
            if ((this.SelectItemWithValue(value) == null) && base.HasItems)
            {
                value = null;
            }
            return value;
        }

        private static bool ContainerGetIsSelected(DependencyObject container, object item)
        {
            if (container != null)
            {
                return (bool)container.GetValue(IsSelectedProperty);
            }
            DependencyObject obj2 = item as DependencyObject;
            return ((obj2 != null) && ((bool)obj2.GetValue(IsSelectedProperty)));
        }

        private object FindItemWithValue(object value)
        {
            Func<object, object> getValueFunc = null;
            ItemCollection items = base.Items;
            for (int i = 0; i < items.Count; i++)
            {
                object item = items[i];
                if ((getValueFunc == null) && (item != null))
                {
                    getValueFunc = BindingExpressionHelper.CreateGetValueFunc(item.GetType(), this.SelectedValuePath);
                }
                object itemValue = getValueFunc(item);
                if ((itemValue != null) && object.Equals(itemValue, value))
                {
                    return item;
                }
            }
            return null;
        }

        public static bool GetIsSelected(DependencyObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return (bool)element.GetValue(IsSelectedProperty);
        }

        private static object GetItemOrContainerFromContainer(DependencyObject container)
        {
            object itemOrContainer = container;
            Telerik.Windows.Controls.ItemsControl owner = Telerik.Windows.Controls.ItemsControl.ItemsControlFromItemContainer(container);
            if (owner != null)
            {
                itemOrContainer = owner.ItemContainerGenerator.ItemFromContainer(container);
            }
            return itemOrContainer;
        }

        private bool IndexGetIsSelected(int index, object item)
        {
            return ContainerGetIsSelected(base.ItemContainerGenerator.ContainerFromIndex(index), item);
        }

        internal static bool ItemGetIsSelectable(object item)
        {
            return (item != null);
        }

        private bool ItemGetIsSelected(object item)
        {
            if (item == null)
            {
                return false;
            }
            return ContainerGetIsSelected(base.ItemContainerGenerator.ContainerFromItem(item), item);
        }

        private void ItemSetIsSelected(object item, bool value)
        {
            if (item != null)
            {
                DependencyObject element = base.ItemContainerGenerator.ContainerFromItem(item);
                if (element == null)
                {
                    element = item as DependencyObject;
                }
                if ((element != null) && (GetIsSelected(element) != value))
                {
                    element.SetValue(IsSelectedProperty, value);
                }
            }
        }

        internal void NotifyIsSelectedChanged(FrameworkElement container, bool selected, RadRoutedEventArgs e)
        {
            if (!this.SelectionChange.IsActive)
            {
                if (container != null)
                {
                    object itemOrContainerFromContainer = GetItemOrContainerFromContainer(container);
                    if (itemOrContainerFromContainer != DependencyProperty.UnsetValue)
                    {
                        this.SetSelectedHelper(itemOrContainerFromContainer, selected);
                        e.Handled = true;
                    }
                }
            }
            else
            {
                e.Handled = true;
            }
        }

        private static object OnCoerceSelectedValue(DependencyObject sender, object newValue)
        {
            return (sender as Selector).CoerceSelectedValue(newValue);
        }

        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ISelectable selectable = d as ISelectable;
            if (selectable != null)
            {
                selectable.IsSelected = (bool)e.NewValue;
                if ((bool)e.NewValue)
                {
                    selectable.OnSelected(new RadRoutedEventArgs(SelectedEvent, d));
                }
                else
                {
                    selectable.OnUnselected(new RadRoutedEventArgs(UnselectedEvent, d));
                }
            }
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            if (this.deferredSelectedIndex != -1)
            {
                this.SelectedIndex = this.deferredSelectedIndex;
            }
            if (((this.SelectedValue != null) && (this.SelectedIndex == -1)) && !object.Equals(this.SelectedValue, this.InternalSelectedValue))
            {
                this.SelectItemWithValue(this.SelectedValue);
            }
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    this.SelectionChange.Begin();
                    try
                    {
                        object item = e.NewItems[0];
                        if (this.ItemGetIsSelected(item))
                        {
                            this.SelectionChange.Add(item);
                        }
                        return;
                    }
                    finally
                    {
                        this.SelectionChange.End();
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    break;

                case NotifyCollectionChangedAction.Replace:
                    return;

                case NotifyCollectionChangedAction.Reset:
                    if (base.Items.Count == 0)
                    {
                        this.SelectionChange.Clear();
                    }
                    this.SelectionChange.Begin();
                    try
                    {
                        for (int i = 0; i < this.selectedItems.Count; i++)
                        {
                            object item = this.selectedItems[i];
                            if (!base.Items.Contains(item))
                            {
                                this.SelectionChange.Remove(item);
                            }
                        }
                        if (base.ItemsSource == null)
                        {
                            for (int j = 0; j < base.Items.Count; j++)
                            {
                                object item2 = base.Items[j];
                                if (this.IndexGetIsSelected(j, item2) && !this.selectedItems.Contains(item2))
                                {
                                    this.SelectionChange.Add(item2);
                                }
                            }
                        }
                        return;
                    }
                    finally
                    {
                        this.SelectionChange.End();
                    }

                default:
                    throw new NotSupportedException(Telerik.Windows.Controls.SR.Get("UnexpectedCollectionChangeAction", new object[] { e.Action }));
            }
            this.SelectionChange.Begin();
            try
            {
                object t = e.OldItems[0];
                if (this.selectedItems.Contains(t))
                {
                    this.SelectionChange.Remove(t);
                }
                return;
            }
            finally
            {
                this.SelectionChange.End();
            }
        }

        private static void OnSelected(object sender, RoutedEventArgs e)
        {
            RadRoutedEventArgs args = e as RadRoutedEventArgs;
            if (args != null)
            {
                ((Selector)sender).NotifyIsSelectedChanged(args.OriginalSource as FrameworkElement, true, args);
            }
        }

        private static void OnSelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Selector selector = (Selector)d;
            if ((selector.SelectionChange != null) && !selector.SelectionChange.IsActive)
            {
                int newValue = (int)e.NewValue;
                object item = (newValue == -1) ? null : selector.Items[newValue];
                selector.SelectionChange.SelectJustThisItem(item);
            }
        }

        private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Selector selector = (Selector)d;
            if (!selector.SelectionChange.IsActive)
            {
                selector.SelectionChange.SelectJustThisItem(e.NewValue);
            }
        }

        protected virtual void OnSelectedValueChanged(object oldValue, object newValue)
        {
        }

        private static void OnSelectedValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as Selector).OnSelectedValueChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnSelectedValuePathChanged(string oldValue, string newValue)
        {
            if (this.SelectedValue != null)
            {
                this.SelectItemWithValue(this.SelectedValue);
            }
        }

        private static void OnSelectedValuePathChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as Selector).OnSelectedValuePathChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual void OnSelectionChanged(System.Windows.Controls.SelectionChangedEventArgs e)
        {
            this.RaiseEvent(new Telerik.Windows.Controls.SelectionChangedEventArgs(SelectionChangedEvent, e.RemovedItems, e.AddedItems));
        }

        private void OnSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            this.OnSelectionChanged(e);
        }

        private static void OnUnselected(object sender, RoutedEventArgs e)
        {
            RadRoutedEventArgs args = e as RadRoutedEventArgs;
            if (args != null)
            {
                ((Selector)sender).NotifyIsSelectedChanged(args.OriginalSource as FrameworkElement, false, args);
            }
        }

        public static void RemoveSelectedHandler(DependencyObject element, RoutedEventHandler handler)
        {
            element.RemoveHandler(SelectedEvent, handler);
        }

        public static void RemoveUnselectedHandler(DependencyObject element, RoutedEventHandler handler)
        {
            element.RemoveHandler(UnselectedEvent, handler);
        }

        private object SelectItemWithValue(object value)
        {
            this.selectingItemWithValue = true;
            object unsetValue = null;
            if (value != null)
            {
                unsetValue = this.FindItemWithValue(value);
            }
            if (this.SelectedItem != unsetValue)
            {
                this.SelectionChange.SelectJustThisItem(unsetValue);
            }
            if ((unsetValue != null) || base.HasItems)
            {
                this.selectingItemWithValue = false;
            }
            return unsetValue;
        }

        public static void SetIsSelected(DependencyObject element, bool isSelected)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            element.SetValue(IsSelectedProperty, isSelected);
        }

        private void SetSelectedHelper(object item, bool selected)
        {
            if (!ItemGetIsSelectable(item) && selected)
            {
                throw new InvalidOperationException(Telerik.Windows.Controls.SR.Get("CannotSelectNotSelectableItem", new object[0]));
            }
            this.SelectionChange.Begin();
            try
            {
                if (selected)
                {
                    this.SelectionChange.Add(item);
                }
                else
                {
                    this.SelectionChange.Remove(item);
                }
            }
            finally
            {
                this.SelectionChange.End();
            }
        }

        internal static bool UIGetIsSelectable(DependencyObject item)
        {
            if (item != null)
            {
                if (!ItemGetIsSelectable(item))
                {
                    return false;
                }
                Telerik.Windows.Controls.ItemsControl control = Telerik.Windows.Controls.ItemsControl.ItemsControlFromItemContainer(item);
                if (control != null)
                {
                    object itemFromContainer = control.ItemContainerGenerator.ItemFromContainer(item);
                    if (itemFromContainer != item)
                    {
                        return ItemGetIsSelectable(itemFromContainer);
                    }
                    return true;
                }
            }
            return false;
        }

        internal virtual void UpdatePublicSelectionProperties()
        {
            int oldSelectedIndex = this.SelectedIndex;
            if (((oldSelectedIndex > (base.Items.Count - 1)) || ((oldSelectedIndex == -1) && (this.selectedItems.Count > 0))) || ((oldSelectedIndex > -1) && (this.selectedItems.Count == 0)))
            {
                this.deferredSelectedIndex = oldSelectedIndex;
            }
            if (this.SelectedIndex != this.InternalSelectedIndex)
            {
                base.SetValue(SelectedIndexProperty, this.InternalSelectedIndex);
            }
            if (this.SelectedItem != this.InternalSelectedItem)
            {
                try
                {
                    this.skipCoerceSelectedItemCheck = true;
                    base.SetValue(SelectedItemProperty, this.InternalSelectedItem);
                }
                finally
                {
                    this.skipCoerceSelectedItemCheck = false;
                }
            }
            if (!this.selectingItemWithValue && (this.SelectedValue != this.InternalSelectedValue))
            {
                base.SetValue(SelectedValueProperty, this.InternalSelectedValue);
            }
        }

        internal bool CanSelectMultiple { get; set; }

        internal int InternalSelectedIndex
        {
            get
            {
                if (this.selectedItems.Count != 0)
                {
                    return base.Items.IndexOf(this.selectedItems[0]);
                }
                return -1;
            }
        }

        internal object InternalSelectedItem
        {
            get
            {
                if (this.selectedItems.Count != 0)
                {
                    return this.selectedItems[0];
                }
                return null;
            }
        }

        private object InternalSelectedValue
        {
            get
            {
                object internalSelectedItem = this.InternalSelectedItem;
                if (internalSelectedItem == null)
                {
                    return null;
                }
                if (this.SelectedValuePath == null)
                {
                    return internalSelectedItem;
                }
                return BindingExpressionHelper.GetValue(internalSelectedItem, this.SelectedValuePath);
            }
        }

        [Telerik.Windows.Controls.SRCategory("Appearance")]
        public int SelectedIndex
        {
            get
            {
                return (int)base.GetValue(SelectedIndexProperty);
            }
            set
            {
                base.SetValue(SelectedIndexProperty, value);
            }
        }

        [Telerik.Windows.Controls.SRCategory("Appearance")]
        public object SelectedItem
        {
            get
            {
                return base.GetValue(SelectedItemProperty);
            }
            set
            {
                base.SetValue(SelectedItemProperty, value);
            }
        }

        public object SelectedValue
        {
            get
            {
                return base.GetValue(SelectedValueProperty);
            }
            set
            {
                base.SetValue(SelectedValueProperty, value);
            }
        }

        public string SelectedValuePath
        {
            get
            {
                return (string)base.GetValue(SelectedValuePathProperty);
            }
            set
            {
                base.SetValue(SelectedValuePathProperty, value);
            }
        }

        internal SelectionChanger<object> SelectionChange
        {
            get
            {
                return this.selectedItems;
            }
        }

        internal class SelectionChanger<T> : ObservableCollection<T>
        {
            private List<T> internalSelection;
            private bool isActive;
            private Func<T, bool> isItemValidForSelection;
            private List<T> itemsToSelect;
            private List<T> itemsToUnselect;
            private Selector selector;

            public event System.Windows.Controls.SelectionChangedEventHandler SelectionChanged;

            public SelectionChanger(Selector owner)
            {
                this.selector = owner;
                this.itemsToSelect = new List<T>(1);
                this.itemsToUnselect = new List<T>(1);
                this.internalSelection = new List<T>(1);
                this.InitFlags();
            }

            public SelectionChanger(Selector owner, Func<T, bool> isItemValidForSelection)
                : this(owner)
            {
                this.isItemValidForSelection = isItemValidForSelection;
            }

            internal void AddJustThis(T item)
            {
                this.Begin();
                int count = base.Items.Count;
                for (int i = 0; i < count; i++)
                {
                    T itemInCollection = base.Items[i];
                    if (!itemInCollection.Equals(item))
                    {
                        base.Remove(itemInCollection);
                    }
                }
                if (!base.Items.Contains(item))
                {
                    base.Add(item);
                }
                this.End();
            }

            private void AddToSelection(T item)
            {
                bool result = true;
                if (this.isItemValidForSelection != null)
                {
                    result = this.isItemValidForSelection(item);
                }
                if (result)
                {
                    this.itemsToSelect.Add(item);
                }
            }

            internal void Begin()
            {
                this.isActive = true;
                this.itemsToSelect.Clear();
                this.itemsToUnselect.Clear();
            }

            internal void Cancel()
            {
                this.InitFlags();
                if (this.itemsToSelect.Count > 0)
                {
                    this.itemsToSelect.Clear();
                }
                if (this.itemsToUnselect.Count > 0)
                {
                    this.itemsToUnselect.Clear();
                }
            }

            protected override void ClearItems()
            {
                if (this.isActive)
                {
                    this.InternalClear();
                }
                else
                {
                    this.Begin();
                    this.InternalClear();
                    this.End();
                }
            }

            internal void End()
            {
                this.SynchronizeInternalSelection();
                this.selector.UpdatePublicSelectionProperties();
                this.InitFlags();
                if ((this.itemsToUnselect.Count > 0) || (this.itemsToSelect.Count > 0))
                {
                    this.InvokeSelectionChangedEvent();
                }
            }

            private void InitFlags()
            {
                this.isActive = false;
            }

            protected override void InsertItem(int index, T item)
            {
                if (this.isActive)
                {
                    this.Select(item);
                }
                else
                {
                    this.Begin();
                    this.Select(item);
                    this.End();
                }
            }

            protected void InternalClear()
            {
                base.ClearItems();
                this.itemsToSelect.Clear();
                this.itemsToUnselect = new List<T>(this.internalSelection);
                this.internalSelection.Clear();
            }

            private void InvokeSelectionChangedEvent()
            {
                if (this.SelectionChanged != null)
                {
                    System.Windows.Controls.SelectionChangedEventArgs e = new System.Windows.Controls.SelectionChangedEventArgs(this.itemsToUnselect, this.itemsToSelect);
                    this.SelectionChanged(this, e);
                }
            }

            protected override void RemoveItem(int index)
            {
                T changedItem = base[index];
                if (this.isActive)
                {
                    this.Unselect(changedItem);
                }
                else
                {
                    this.Begin();
                    this.Unselect(changedItem);
                    this.End();
                }
            }

            private void Select(T item)
            {
                if (!this.itemsToSelect.Contains(item))
                {
                    if (!this.internalSelection.Contains(item))
                    {
                        this.AddToSelection(item);
                    }
                    if (!this.selector.CanSelectMultiple && (this.itemsToSelect.Count > 0))
                    {
                        foreach (T forUnselecting in this.internalSelection)
                        {
                            this.itemsToUnselect.Add(forUnselecting);
                        }
                    }
                }
            }

            internal void SelectJustThisItem(T item)
            {
                this.Begin();
                if (base.Items.Count > 0)
                {
                    base.Clear();
                }
                try
                {
                    if (this.selector.Items.Contains(item))
                    {
                        if (this.itemsToUnselect.Contains(item))
                        {
                            this.itemsToUnselect.Remove(item);
                        }
                        else
                        {
                            this.Select(item);
                        }
                    }
                }
                finally
                {
                    this.End();
                }
            }

            protected override void SetItem(int index, T item)
            {
                this.Begin();
                T oldItem = base[index];
                this.itemsToUnselect.Add(oldItem);
                this.itemsToSelect.Add(item);
                base.SetItem(index, item);
                this.InvokeSelectionChangedEvent();
                this.Cancel();
            }

            private void SynchronizeInternalSelection()
            {
                foreach (T item in this.itemsToUnselect)
                {
                    int index = base.IndexOf(item);
                    if (index > -1)
                    {
                        base.RemoveItem(index);
                    }
                    this.selector.ItemSetIsSelected(item, false);
                    this.internalSelection.Remove(item);
                }
                foreach (T item in this.itemsToSelect)
                {
                    base.InsertItem(base.Items.Count, item);
                    this.internalSelection.Add(item);
                    this.selector.ItemSetIsSelected(item, true);
                }
            }

            private void Unselect(T item)
            {
                if ((!this.itemsToSelect.Remove(item) && !this.itemsToUnselect.Contains(item)) && this.internalSelection.Contains(item))
                {
                    this.itemsToUnselect.Add(item);
                }
            }

            internal bool IsActive
            {
                get
                {
                    return this.isActive;
                }
            }
        }
    }
}

