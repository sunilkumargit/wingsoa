namespace Telerik.Windows.Controls
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Telerik.Windows.Input;

    [DefaultEvent("SelectionChanged"), DefaultProperty("SelectedValue"), TemplatePart(Name="PART_ScrollViewer", Type=typeof(ScrollViewer))]
    public class ListBox : Selector
    {
        private WeakReference anchorItem;
        private ScrollViewer elementScrollViewer;
        private int focusedIndex;
        private WeakReference lastActionItem;
        public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.Register("SelectedItems", typeof(IList), typeof(Telerik.Windows.Controls.ListBox), null);
        public static readonly DependencyProperty SelectionModeProperty = DependencyProperty.Register("SelectionMode", typeof(Telerik.Windows.Controls.SelectionMode), typeof(Telerik.Windows.Controls.ListBox), new PropertyMetadata(Telerik.Windows.Controls.SelectionMode.Single, new PropertyChangedCallback(Telerik.Windows.Controls.ListBox.OnSelectionModeChanged)));

        public ListBox()
        {
            base.DefaultStyleKey = typeof(Telerik.Windows.Controls.ListBox);
            this.SelectedItems = base.SelectionChange;
            base.KeyDown += new KeyEventHandler(this.OnKeyDown);
        }

        internal Telerik.Windows.Controls.ListBoxItem ContainerFromItemOrContainer(object item)
        {
            return ((item as Telerik.Windows.Controls.ListBoxItem) ?? (base.ItemContainerGenerator.ContainerFromItem(item) as Telerik.Windows.Controls.ListBoxItem));
        }

        private int ElementIndex(DependencyObject listItem)
        {
            return base.ItemContainerGenerator.IndexFromContainer(listItem);
        }

        private void ElementScrollViewerScrollInDirection(Key key)
        {
            if (this.elementScrollViewer != null)
            {
                switch (key)
                {
                    case Key.Left:
                        this.NavigatePrev();
                        return;

                    case Key.Up:
                        this.NavigatePrev();
                        return;

                    case Key.Right:
                        this.NavigateNext();
                        return;

                    case Key.Down:
                        this.NavigateNext();
                        return;

                    default:
                        return;
                }
            }
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return base.GetContainerForItemOverride();
        }

        private double GetItemHeight(object item)
        {
            Telerik.Windows.Controls.ListBoxItem listBoxItem = this.ContainerFromItemOrContainer(item);
            if ((listBoxItem != null) && (listBoxItem.Visibility == Visibility.Visible))
            {
                return listBoxItem.ActualHeight;
            }
            return 0.0;
        }

        private double GetItemWidth(object item)
        {
            Telerik.Windows.Controls.ListBoxItem listBoxItem = this.ContainerFromItemOrContainer(item);
            if ((listBoxItem != null) && (listBoxItem.Visibility == Visibility.Visible))
            {
                return listBoxItem.ActualWidth;
            }
            return 0.0;
        }

        private int GetNextSelectableItem(int startIndex)
        {
            for (int index = startIndex + 1; index < base.Items.Count; index++)
            {
                if (IsSelectableHelper(base.ItemContainerGenerator.ContainerFromIndex(index)))
                {
                    return index;
                }
            }
            return startIndex;
        }

        private int GetPreviousSelectableItem(int startIndex)
        {
            for (int index = startIndex - 1; index > -1; index--)
            {
                if (IsSelectableHelper(base.ItemContainerGenerator.ContainerFromIndex(index)))
                {
                    return index;
                }
            }
            return startIndex;
        }

        private double GetPreviuousItemsHeight(object item)
        {
            double previousItemsHeight = 0.0;
            foreach (object i in base.Items)
            {
                if (i == item)
                {
                    return previousItemsHeight;
                }
                previousItemsHeight += this.GetItemHeight(i);
            }
            return previousItemsHeight;
        }

        private double GetPreviuousItemsWidth(object item)
        {
            double previousItemsWidth = 0.0;
            foreach (object i in base.Items)
            {
                if (i == item)
                {
                    return previousItemsWidth;
                }
                previousItemsWidth += this.GetItemWidth(i);
            }
            return previousItemsWidth;
        }

        private static object GetWeakReferenceTarget(ref WeakReference weakReference)
        {
            if (weakReference != null)
            {
                return weakReference.Target;
            }
            return null;
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        internal bool HandleKeyDown(Key key, Telerik.Windows.Controls.ListBoxItem originalSource)
        {
            bool isHandled = false;
            int focusIndex = this.focusedIndex;
            switch (key)
            {
                case Key.Enter:
                case Key.Space:
                    if (((key != Key.Enter) && ((System.Windows.Input.Keyboard.Modifiers & (ModifierKeys.Control | ModifierKeys.Alt)) != ModifierKeys.Alt)) && ((originalSource != null) && (Telerik.Windows.Controls.ItemsControl.ItemsControlFromItemContainer(originalSource) == this)))
                    {
                        switch (this.SelectionMode)
                        {
                            case Telerik.Windows.Controls.SelectionMode.Single:
                                if ((System.Windows.Input.Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.Control)
                                {
                                    this.MakeSingleSelection(originalSource);
                                }
                                else
                                {
                                    this.MakeToggleSelection(originalSource);
                                }
                                break;

                            case Telerik.Windows.Controls.SelectionMode.Multiple:
                                this.MakeToggleSelection(originalSource);
                                isHandled = true;
                                break;

                            case Telerik.Windows.Controls.SelectionMode.Extended:
                                if ((System.Windows.Input.Keyboard.Modifiers & (ModifierKeys.Shift | ModifierKeys.Control)) != ModifierKeys.Control)
                                {
                                    if ((System.Windows.Input.Keyboard.Modifiers & (ModifierKeys.Shift | ModifierKeys.Control)) == ModifierKeys.Shift)
                                    {
                                        this.MakeAnchorSelection(originalSource);
                                    }
                                    else if ((System.Windows.Input.Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.None)
                                    {
                                        this.MakeSingleSelection(originalSource);
                                    }
                                }
                                else
                                {
                                    this.MakeToggleSelection(originalSource);
                                }
                                break;
                        }
                    }
                    break;

                case Key.End:
                    focusIndex = base.Items.Count - 1;
                    break;

                case Key.Home:
                    focusIndex = 0;
                    break;

                case Key.Left:
                    if (!this.IsVerticalOrientation())
                    {
                        focusIndex = this.focusedIndex - 1;
                    }
                    this.ElementScrollViewerScrollInDirection(Key.Left);
                    break;

                case Key.Up:
                    if (this.IsVerticalOrientation())
                    {
                        focusIndex = this.focusedIndex - 1;
                    }
                    this.ElementScrollViewerScrollInDirection(Key.Up);
                    break;

                case Key.Right:
                    if (!this.IsVerticalOrientation())
                    {
                        focusIndex = this.focusedIndex + 1;
                    }
                    this.ElementScrollViewerScrollInDirection(Key.Right);
                    break;

                case Key.Down:
                    if (this.IsVerticalOrientation())
                    {
                        focusIndex = this.focusedIndex + 1;
                    }
                    this.ElementScrollViewerScrollInDirection(Key.Down);
                    break;

                case Key.A:
                case Key.Divide:
                    if ((((System.Windows.Input.Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control) || this.IsCtrlDown) && (this.SelectionMode == Telerik.Windows.Controls.SelectionMode.Extended))
                    {
                        this.SelectAll();
                    }
                    break;
            }
            if (focusIndex != -1)
            {
                isHandled = true;
                focusIndex = Math.Min(focusIndex, base.Items.Count - 1);
                if (0 > focusIndex)
                {
                    return isHandled;
                }
                if (((System.Windows.Input.Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.None) || this.IsCtrlDown)
                {
                    this.SetFocusedItem(focusIndex, true);
                    return isHandled;
                }
                this.SetFocusedItem(focusIndex, true);
                if (this.SelectionMode != Telerik.Windows.Controls.SelectionMode.Multiple)
                {
                    base.SelectedIndex = focusIndex;
                }
            }
            return isHandled;
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return (item is Telerik.Windows.Controls.ListBoxItem);
        }

        private static bool IsSelectableHelper(object o)
        {
            ContentControl contentControl = o as ContentControl;
            return ((contentControl == null) || (contentControl.IsEnabled && (contentControl.Visibility == Visibility.Visible)));
        }

        internal bool IsVerticalOrientation()
        {
            StackPanel itemsHost = this.ItemsHost as StackPanel;
            if (itemsHost != null)
            {
                return (itemsHost.Orientation == Orientation.Vertical);
            }
            return true;
        }

        private void MakeAnchorSelection(Telerik.Windows.Controls.ListBoxItem actionItem)
        {
            if (this.AnchorItem == null)
            {
                if (this.SelectedItems.Count > 0)
                {
                    this.AnchorItem = this.SelectedItems[this.SelectedItems.Count - 1];
                }
                else
                {
                    this.AnchorItem = base.Items[0];
                }
                if (this.AnchorItem == null)
                {
                    return;
                }
            }
            int actionItemIndex = this.ElementIndex(actionItem);
            int index = base.Items.IndexOf(this.AnchorItem);
            if (actionItemIndex > index)
            {
                int oldActionItemIndex = actionItemIndex;
                actionItemIndex = index;
                index = oldActionItemIndex;
            }
            bool shouldEndSelection = false;
            if (!base.SelectionChange.IsActive)
            {
                shouldEndSelection = true;
                base.SelectionChange.Begin();
            }
            try
            {
                for (int j = 0; j < this.SelectedItems.Count; j++)
                {
                    object item = this.SelectedItems[j];
                    int itemIndex = base.Items.IndexOf(item);
                    if ((itemIndex < actionItemIndex) || (index < itemIndex))
                    {
                        this.Unselect(item);
                    }
                }
                if (this.AnchorItem == null)
                {
                    if (this.SelectedItems.Count > 0)
                    {
                        this.AnchorItem = this.SelectedItems[this.SelectedItems.Count - 1];
                    }
                    else
                    {
                        this.AnchorItem = base.Items[0];
                    }
                    if (this.AnchorItem == null)
                    {
                        return;
                    }
                }
                IEnumerator enumerator = base.Items.GetEnumerator();
                for (int i = 0; i <= index; i++)
                {
                    enumerator.MoveNext();
                    if (i >= actionItemIndex)
                    {
                        this.Select(enumerator.Current);
                    }
                }
            }
            finally
            {
                if (shouldEndSelection)
                {
                    base.SelectionChange.End();
                }
            }
        }

        private void MakeAnchorSelection(Telerik.Windows.Controls.ListBoxItem actionItem, bool clearCurrent)
        {
            if (this.AnchorItem == null)
            {
                if (base.SelectionChange.Count > 0)
                {
                    this.AnchorItem = base.SelectionChange[base.SelectionChange.Count - 1];
                }
                else
                {
                    this.AnchorItem = base.Items[0];
                }
                if (this.AnchorItem == null)
                {
                    return;
                }
            }
            int actionItemIndex = this.ElementIndex(actionItem);
            int index = base.Items.IndexOf(this.AnchorItem);
            if (actionItemIndex > index)
            {
                int oldActionItemIndex = actionItemIndex;
                actionItemIndex = index;
                index = oldActionItemIndex;
            }
            bool shouldEndSelection = false;
            if (!base.SelectionChange.IsActive)
            {
                shouldEndSelection = true;
                base.SelectionChange.Begin();
            }
            try
            {
                if (clearCurrent)
                {
                    for (int j = 0; j < base.SelectionChange.Count; j++)
                    {
                        object item = base.SelectionChange[j];
                        int itemIndex = base.Items.IndexOf(item);
                        if ((itemIndex < actionItemIndex) || (index < itemIndex))
                        {
                            base.SelectionChange.Remove(item);
                        }
                    }
                }
                IEnumerator enumerator = base.Items.GetEnumerator();
                for (int i = 0; i <= index; i++)
                {
                    enumerator.MoveNext();
                    if (i >= actionItemIndex)
                    {
                        base.SelectionChange.Add(enumerator.Current);
                    }
                }
            }
            finally
            {
                if (shouldEndSelection)
                {
                    base.SelectionChange.End();
                }
            }
            this.LastActionItem = actionItem;
        }

        private void MakeSingleSelection(Telerik.Windows.Controls.ListBoxItem listItem)
        {
            if (Telerik.Windows.Controls.ItemsControl.ItemsControlFromItemContainer(listItem) == this)
            {
                object item = base.ItemContainerGenerator.ItemFromContainer(listItem);
                if (this.SelectionMode == Telerik.Windows.Controls.SelectionMode.Extended)
                {
                    base.SelectionChange.Begin();
                    try
                    {
                        if (!this.SelectedItems.Contains(item))
                        {
                            this.SelectedItems.Clear();
                        }
                        else
                        {
                            (from i in base.Items
                                where i != item
                                select i).ToList<object>().ForEach(delegate (object itemToUnselect) {
                                this.Unselect(itemToUnselect);
                            });
                        }
                    }
                    finally
                    {
                        base.SelectionChange.End();
                    }
                }
                base.SelectionChange.SelectJustThisItem(item);
                if (!this.SelectedItems.Contains(item))
                {
                    this.SelectedItems.Add(item);
                }
                listItem.Focus();
                this.UpdateAnchorAndActionItem(listItem);
            }
        }

        private void MakeToggleSelection(Telerik.Windows.Controls.ListBoxItem item)
        {
            item.IsSelected = !item.IsSelected;
            this.UpdateAnchorAndActionItem(item);
        }

        internal void NavigateNext()
        {
            if (base.Items.Count > 0)
            {
                this.focusedIndex = this.GetNextSelectableItem(this.focusedIndex);
                if (this.focusedIndex >= 0)
                {
                    this.NavigateToItem(base.Items[this.focusedIndex]);
                }
            }
        }

        internal void NavigatePrev()
        {
            if (base.Items.Count > 0)
            {
                if (this.focusedIndex == -1)
                {
                    this.focusedIndex = base.Items.Count - 1;
                }
                else
                {
                    this.focusedIndex = this.GetPreviousSelectableItem(this.focusedIndex);
                }
                this.NavigateToItem(base.Items[this.focusedIndex]);
            }
        }

        private void NavigateToItem(object item)
        {
            if (item != null)
            {
                this.ScrollIntoView(item);
            }
        }

        internal void NotifyListItemClicked(Telerik.Windows.Controls.ListBoxItem item, MouseButton mouseButton)
        {
            this.focusedIndex = base.Items.IndexOf(item);
            switch (this.SelectionMode)
            {
                case Telerik.Windows.Controls.SelectionMode.Single:
                    if (!item.IsSelected)
                    {
                        item.IsSelected = true;
                    }
                    else if (((System.Windows.Input.Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control) || this.IsCtrlDown)
                    {
                        item.IsSelected = false;
                    }
                    this.UpdateAnchorAndActionItem(item);
                    return;

                case Telerik.Windows.Controls.SelectionMode.Multiple:
                    this.MakeToggleSelection(item);
                    return;

                case Telerik.Windows.Controls.SelectionMode.Extended:
                    if (mouseButton == MouseButton.Left)
                    {
                        if (((System.Windows.Input.Keyboard.Modifiers & (ModifierKeys.Shift | ModifierKeys.Control)) != (ModifierKeys.Shift | ModifierKeys.Control)) || this.IsCtrlDown)
                        {
                            if (((System.Windows.Input.Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control) || this.IsCtrlDown)
                            {
                                this.MakeToggleSelection(item);
                                return;
                            }
                            if ((System.Windows.Input.Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                            {
                                this.MakeAnchorSelection(item, true);
                                return;
                            }
                            this.MakeSingleSelection(item);
                            return;
                        }
                        this.MakeAnchorSelection(item, false);
                        return;
                    }
                    if ((mouseButton != MouseButton.Right) || ((System.Windows.Input.Keyboard.Modifiers & (ModifierKeys.Shift | ModifierKeys.Control)) != ModifierKeys.None))
                    {
                        break;
                    }
                    if (!item.IsSelected)
                    {
                        this.MakeSingleSelection(item);
                        break;
                    }
                    this.UpdateAnchorAndActionItem(item);
                    return;

                default:
                    return;
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (base.Template != null)
            {
                this.elementScrollViewer = base.GetTemplateChild("PART_ScrollViewer") as ScrollViewer;
                if (this.elementScrollViewer != null)
                {
                    FrameworkElement scrollViewerContent = this.elementScrollViewer.Content as FrameworkElement;
                    if (scrollViewerContent != null)
                    {
                        scrollViewerContent.KeyDown += new KeyEventHandler(this.OnKeyDown);
                    }
                }
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            Telerik.Windows.Controls.ListBoxItem originalSource = e.OriginalSource as Telerik.Windows.Controls.ListBoxItem;
            Key key = e.Key;
            bool isHandled = false;
            if (!e.Handled)
            {
                isHandled = this.HandleKeyDown(key, originalSource);
            }
            if (isHandled)
            {
                e.Handled = true;
            }
        }

        protected override void OnSelectionChanged(System.Windows.Controls.SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);
            if ((this.SelectionMode == Telerik.Windows.Controls.SelectionMode.Single) && (base.SelectedItem != null))
            {
                Telerik.Windows.Controls.ListBoxItem selectedItem = base.SelectedItem as Telerik.Windows.Controls.ListBoxItem;
                if (selectedItem == null)
                {
                    selectedItem = base.ItemContainerGenerator.ContainerFromItem(base.SelectedItem) as Telerik.Windows.Controls.ListBoxItem;
                }
                if (selectedItem != null)
                {
                    this.UpdateAnchorAndActionItem(selectedItem);
                }
            }
        }

        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId="e")]
        private static void OnSelectionModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Telerik.Windows.Controls.ListBox box = (Telerik.Windows.Controls.ListBox) d;
            box.ValidateSelectionMode(box.SelectionMode);
        }

        internal void ScrollIntoView(object item)
        {
            if (this.elementScrollViewer != null)
            {
                if (this.IsVerticalOrientation())
                {
                    double verticalOffset = this.elementScrollViewer.VerticalOffset;
                    double previousItemsHeight = this.GetPreviuousItemsHeight(item);
                    if (previousItemsHeight < verticalOffset)
                    {
                        verticalOffset = previousItemsHeight;
                    }
                    else
                    {
                        double itemOffset = previousItemsHeight + this.GetItemHeight(item);
                        if ((itemOffset - this.elementScrollViewer.ViewportHeight) > verticalOffset)
                        {
                            verticalOffset = itemOffset - this.elementScrollViewer.ViewportHeight;
                        }
                    }
                    this.elementScrollViewer.ScrollToVerticalOffset(Math.Max(0.0, Math.Min(this.elementScrollViewer.ScrollableHeight, verticalOffset)));
                }
                else
                {
                    double horizontalOffset = this.elementScrollViewer.HorizontalOffset;
                    double previousItemsWidth = this.GetPreviuousItemsWidth(item);
                    if (previousItemsWidth < horizontalOffset)
                    {
                        horizontalOffset = previousItemsWidth;
                    }
                    else
                    {
                        double itemOffset = previousItemsWidth + this.GetItemWidth(item);
                        if ((itemOffset - this.elementScrollViewer.ViewportWidth) > horizontalOffset)
                        {
                            horizontalOffset = itemOffset - this.elementScrollViewer.ViewportWidth;
                        }
                    }
                    this.elementScrollViewer.ScrollToHorizontalOffset(Math.Max(0.0, Math.Min(this.elementScrollViewer.ScrollableWidth, horizontalOffset)));
                }
            }
        }

        private void Select(object item)
        {
            this.SelectedItems.Add(item);
        }

        public void SelectAll()
        {
            if (!base.CanSelectMultiple)
            {
                throw new NotSupportedException(Telerik.Windows.Controls.SR.Get("ListBoxSelectAllSelectionMode"));
            }
            this.SelectAllImpl();
        }

        internal void SelectAllImpl()
        {
            base.SelectionChange.Begin();
            base.SelectionChange.Clear();
            try
            {
                foreach (object o in base.Items)
                {
                    this.Select(o);
                }
            }
            finally
            {
                base.SelectionChange.End();
            }
        }

        private void SelectItems(IList itemsToSelect)
        {
            foreach (object value in itemsToSelect)
            {
                this.Select(value);
            }
        }

        internal void SetFocusedItem(int index, bool scrollIntoView)
        {
            if ((index < 0) || (base.Items.Count <= index))
            {
                index = -1;
            }
            this.focusedIndex = index;
            if (this.focusedIndex == -1)
            {
                base.Focus();
            }
            else
            {
                Telerik.Windows.Controls.ListBoxItem visualListBoxItem = base.Items[index] as Telerik.Windows.Controls.ListBoxItem;
                if (visualListBoxItem != null)
                {
                    if (scrollIntoView)
                    {
                        this.ScrollIntoView(base.Items[this.focusedIndex]);
                    }
                    visualListBoxItem.Focus();
                }
            }
        }

        private void Unselect(object item)
        {
            Telerik.Windows.Controls.ListBoxItem listBoxItem = this.ContainerFromItemOrContainer(item);
            if (listBoxItem != null)
            {
                listBoxItem.IsSelected = false;
            }
        }

        internal void UnselectAll()
        {
            base.SelectionChange.Begin();
            base.SelectionChange.Clear();
            try
            {
                foreach (object o in base.Items)
                {
                    this.Unselect(o);
                }
            }
            finally
            {
                base.SelectionChange.End();
            }
        }

        private void UnselectItems(IList itemsToUnselect)
        {
            foreach (object value in itemsToUnselect)
            {
                if (base.Items.IndexOf(value) != -1)
                {
                    this.Unselect(value);
                }
            }
        }

        private void UpdateAnchorAndActionItem(Telerik.Windows.Controls.ListBoxItem listItem)
        {
            object obj2 = base.ItemContainerGenerator.ItemFromContainer(listItem);
            if (obj2 == null)
            {
                this.AnchorItem = null;
                this.LastActionItem = null;
            }
            else
            {
                this.AnchorItem = obj2;
                this.LastActionItem = listItem;
            }
        }

        private void ValidateSelectionMode(Telerik.Windows.Controls.SelectionMode mode)
        {
            base.CanSelectMultiple = mode != Telerik.Windows.Controls.SelectionMode.Single;
        }

        internal object AnchorItem
        {
            get
            {
                return GetWeakReferenceTarget(ref this.anchorItem);
            }
            set
            {
                this.anchorItem = new WeakReference(value);
            }
        }

        public Telerik.Windows.Controls.ListBoxItem FocusedItem
        {
            get
            {
                return (base.Items[this.focusedIndex] as Telerik.Windows.Controls.ListBoxItem);
            }
        }

        internal bool IsCtrlDown { get; set; }

        internal Panel ItemsHost
        {
            get
            {
                return this.GetItemsPanel();
            }
        }

        internal Telerik.Windows.Controls.ListBoxItem LastActionItem
        {
            get
            {
                return (GetWeakReferenceTarget(ref this.lastActionItem) as Telerik.Windows.Controls.ListBoxItem);
            }
            set
            {
                this.lastActionItem = new WeakReference(value);
            }
        }

        [Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IList SelectedItems
        {
            get
            {
                return (IList) base.GetValue(SelectedItemsProperty);
            }
            private set
            {
                base.SetValue(SelectedItemsProperty, value);
            }
        }

        public Telerik.Windows.Controls.SelectionMode SelectionMode
        {
            get
            {
                return (Telerik.Windows.Controls.SelectionMode) base.GetValue(SelectionModeProperty);
            }
            set
            {
                base.SetValue(SelectionModeProperty, value);
            }
        }
    }
}

