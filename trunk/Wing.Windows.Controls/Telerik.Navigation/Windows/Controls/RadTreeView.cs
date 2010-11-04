using Wing.Windows.Controls.Telerik.Navigation.Windows.Controls;
namespace Telerik.Windows.Controls
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Windows;
    using System.Windows.Automation;
    using System.Windows.Automation.Peers;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Media;
    using Telerik.Windows;
    using Telerik.Windows.Controls.Animation;
    using Telerik.Windows.Controls.DragDrop;
    using Telerik.Windows.Controls.Primitives;
    using Telerik.Windows.Controls.TreeView;

    [DefaultEvent("SelectionChanged"), TemplateVisualState(GroupName = "DropStates", Name = "DropImpossible"), StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(RadTreeViewItem)), StyleTypedProperty(Property = "ExpanderStyle", StyleTargetType = typeof(ToggleButton)), ScriptableType, DefaultProperty("SelectedValue"), TemplateVisualState(GroupName = "DropStates", Name = "DropPossible"), TemplateVisualState(GroupName = "DropStates", Name = "DropRootPossible")]
    public class RadTreeView : Telerik.Windows.Controls.ItemsControl
    {
        public static readonly DependencyProperty BringIntoViewModeProperty = DependencyProperty.Register("BringIntoViewMode", typeof(Telerik.Windows.Controls.BringIntoViewMode), typeof(RadTreeView), new Telerik.Windows.PropertyMetadata(Telerik.Windows.Controls.BringIntoViewMode.Header));
        private bool cancelDragging;
        public static readonly DependencyProperty CheckedItemsProperty = DependencyProperty.Register("CheckedItems", typeof(ICollection<object>), typeof(RadTreeView), null);
        private FrameworkElement dragBetweenItemsFeedback;
        public static readonly Telerik.Windows.RoutedEvent DragEndedEvent = EventManager.RegisterRoutedEvent("DragEnded", RoutingStrategy.Bubble, typeof(RadTreeViewDragEndedEventHandler), typeof(RadTreeView));
        public static readonly Telerik.Windows.RoutedEvent DragStartedEvent = EventManager.RegisterRoutedEvent("DragStarted", RoutingStrategy.Bubble, typeof(RadTreeViewDragEventHandler), typeof(RadTreeView));
        public static readonly DependencyProperty DropExpandDelayProperty = DependencyProperty.Register("DropExpandDelay", typeof(TimeSpan), typeof(RadTreeView), new Telerik.Windows.PropertyMetadata(TimeSpan.FromSeconds(1.0)));
        private string[] expandByPathpathParts;
        public static readonly DependencyProperty ExpanderStyleProperty = DependencyProperty.Register("ExpanderStyle", typeof(Style), typeof(RadTreeView), null);
        private Dictionary<object, WeakReference> generatedItemCache = new Dictionary<object, WeakReference>(0x200);
        public static readonly DependencyProperty ImagesBaseDirProperty = DependencyProperty.Register("ImagesBaseDir", typeof(string), typeof(RadTreeView), null);
        public static readonly DependencyProperty IsDragDropEnabledProperty = DependencyProperty.Register("IsDragDropEnabled", typeof(bool), typeof(RadTreeView), null);
        public static readonly DependencyProperty IsDragPreviewEnabledProperty = DependencyProperty.Register("IsDragPreviewEnabled", typeof(bool), typeof(RadTreeView), new Telerik.Windows.PropertyMetadata(true));
        public static readonly DependencyProperty IsDragTooltipEnabledProperty = DependencyProperty.Register("IsDragTooltipEnabled", typeof(bool), typeof(RadTreeView), new Telerik.Windows.PropertyMetadata(true));
        private bool isDropPossible;
        public static readonly DependencyProperty IsDropPreviewLineEnabledProperty = DependencyProperty.Register("IsDropPreviewLineEnabled", typeof(bool), typeof(RadTreeView), new Telerik.Windows.PropertyMetadata(true));
        public static readonly DependencyProperty IsEditableProperty = DependencyProperty.Register("IsEditable", typeof(bool), typeof(RadTreeView), null);
        public static readonly DependencyProperty IsExpandOnDblClickEnabledProperty = DependencyProperty.Register("IsExpandOnDblClickEnabled", typeof(bool), typeof(RadTreeView), new Telerik.Windows.PropertyMetadata(true));
        public static readonly DependencyProperty IsExpandOnSingleClickEnabledProperty = DependencyProperty.Register("IsExpandOnSingleClickEnabled", typeof(bool), typeof(RadTreeView), null);
        [Obsolete("Incremental search is not yet available, using this property will have no effect.", true)]
        public static readonly DependencyProperty IsIncrementalSearchEnabledProperty = DependencyProperty.Register("IsIncrementalSearchEnabled", typeof(bool), typeof(RadTreeView), null);
        public static readonly DependencyProperty IsLineEnabledProperty = DependencyProperty.Register("IsLineEnabled", typeof(bool), typeof(RadTreeView), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadTreeView.OnIsLineEnabledChanged)));
        public static readonly DependencyProperty IsLoadOnDemandEnabledProperty = DependencyProperty.Register("IsLoadOnDemandEnabled", typeof(bool), typeof(RadTreeView), null);
        public static readonly DependencyProperty IsOptionElementsEnabledProperty = DependencyProperty.Register("IsOptionElementsEnabled", typeof(bool), typeof(RadTreeView), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadTreeView.OnIsOptionElementsEnabledChanged)));
        public static readonly DependencyProperty IsRootLinesEnabledProperty = DependencyProperty.Register("IsRootLinesEnabled", typeof(bool), typeof(RadTreeView), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadTreeView.OnIsRootLinesEnabledChanged)));
        private bool isSelectionActive;
        public static readonly DependencyProperty IsSingleExpandPathProperty = DependencyProperty.Register("IsSingleExpandPath", typeof(bool), typeof(RadTreeView), null);
        public static readonly DependencyProperty IsTriStateModeProperty = DependencyProperty.Register("IsTriStateMode", typeof(bool), typeof(RadTreeView), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadTreeView.OnIsTriStateModeChanged)));
        public static readonly DependencyProperty ItemEditTemplateProperty = DependencyProperty.Register("ItemEditTemplate", typeof(DataTemplate), typeof(RadTreeView), null);
        public static readonly DependencyProperty ItemEditTemplateSelectorProperty = DependencyProperty.Register("ItemEditTemplateSelector", typeof(DataTemplateSelector), typeof(RadTreeView), null);
        private TreeViewPanel itemsHost;
        public static readonly DependencyProperty ItemsIndentProperty = DependencyProperty.Register("ItemsIndent", typeof(int), typeof(RadTreeView), new Telerik.Windows.PropertyMetadata(0x10));
        public static readonly DependencyProperty ItemsOptionListTypeProperty = DependencyProperty.Register("ItemsOptionListType", typeof(OptionListType), typeof(RadTreeView), new Telerik.Windows.PropertyMetadata(OptionListType.Default, new PropertyChangedCallback(RadTreeView.OnItemsOptionListTypePropertyChanged)));
        private ItemAttachedStorage itemStorage;
        private Dictionary<WeakReferenceKey<object>, ToggleState> itemToggleStateStorage;
        private bool justChangeSelectedItem;
        public static readonly DependencyProperty PathSeparatorProperty = DependencyProperty.Register("PathSeparator", typeof(string), typeof(RadTreeView), new Telerik.Windows.PropertyMetadata(@"\"));
        public static readonly Telerik.Windows.RoutedEvent PreviewDragEndedEvent = EventManager.RegisterRoutedEvent("PreviewDragEnded", RoutingStrategy.Tunnel, typeof(RadTreeViewDragEndedEventHandler), typeof(RadTreeView));
        public static readonly Telerik.Windows.RoutedEvent PreviewDragStartedEvent = EventManager.RegisterRoutedEvent("PreviewDragStarted", RoutingStrategy.Tunnel, typeof(RadTreeViewDragEventHandler), typeof(RadTreeView));
        public static readonly Telerik.Windows.RoutedEvent PreviewSelectionChangedEvent = EventManager.RegisterRoutedEvent("PreviewSelectionChanged", RoutingStrategy.Tunnel, typeof(Telerik.Windows.Controls.SelectionChangedEventHandler), typeof(RadTreeView));
        private static ContainerRepository<RadTreeViewItem> repository = new ContainerRepository<RadTreeViewItem>();
        private System.Windows.Controls.ScrollViewer scroller;
        public static readonly DependencyProperty SelectedContainerProperty;
        private static readonly DependencyPropertyKey SelectedContainerPropertyKey = DependencyPropertyExtensions.RegisterReadOnly("SelectedContainer", typeof(RadTreeViewItem), typeof(RadTreeView), null);
        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(object), typeof(RadTreeView), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadTreeView.OnSelectedItemChanged)));
        public static readonly DependencyProperty SelectedItemsProperty;
        private static readonly DependencyPropertyKey SelectedItemsPropertyKey = DependencyPropertyExtensions.RegisterReadOnly("SelectedItems", typeof(ObservableCollection<object>), typeof(RadTreeView), null);
        public static readonly DependencyProperty SelectedValuePathProperty = DependencyProperty.Register("SelectedValuePath", typeof(string), typeof(RadTreeView), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadTreeView.OnSelectedValuePathChanged)));
        public static readonly DependencyProperty SelectedValueProperty = DependencyProperty.Register("SelectedValue", typeof(object), typeof(RadTreeView), null);
        public static readonly Telerik.Windows.RoutedEvent SelectionChangedEvent = Telerik.Windows.Controls.Selector.SelectionChangedEvent.AddOwner(typeof(RadTreeView));
        private Telerik.Windows.Controls.SelectionChanger<Object> selectionChanger;
        public static readonly DependencyProperty SelectionModeProperty = DependencyProperty.Register("SelectionMode", typeof(Telerik.Windows.Controls.SelectionMode), typeof(RadTreeView), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadTreeView.OnSelectionModeChanged)));

        [ScriptableMember]
        public event EventHandler<RadRoutedEventArgs> Checked
        {
            add
            {
                this.AddHandler(RadTreeViewItem.CheckedEvent, value);
            }
            remove
            {
                this.RemoveHandler(RadTreeViewItem.CheckedEvent, value);
            }
        }

        [ScriptableMember]
        public event EventHandler<RadRoutedEventArgs> Collapsed
        {
            add
            {
                this.AddHandler(RadTreeViewItem.CollapsedEvent, value);
            }
            remove
            {
                this.RemoveHandler(RadTreeViewItem.CollapsedEvent, value);
            }
        }

        [ScriptableMember, Category("Behavior"), Description("Occurs when the drag has ended.")]
        public event RadTreeViewDragEndedEventHandler DragEnded
        {
            add
            {
                this.AddHandler(DragEndedEvent, value);
            }
            remove
            {
                this.RemoveHandler(DragEndedEvent, value);
            }
        }

        [ScriptableMember, Description("Occurs when the drag has started."), Category("Behavior")]
        public event RadTreeViewDragEventHandler DragStarted
        {
            add
            {
                this.AddHandler(DragStartedEvent, value);
            }
            remove
            {
                this.RemoveHandler(DragStartedEvent, value);
            }
        }

        [ScriptableMember]
        public event RadTreeViewItemEditedEventHandler EditCanceled
        {
            add
            {
                this.AddHandler(EditableHeaderedItemsControl.EditCanceledEvent, value);
            }
            remove
            {
                this.RemoveHandler(EditableHeaderedItemsControl.EditCanceledEvent, value);
            }
        }

        [ScriptableMember]
        public event RadTreeViewItemEditedEventHandler Edited
        {
            add
            {
                this.AddHandler(EditableHeaderedItemsControl.EditedEvent, value);
            }
            remove
            {
                this.RemoveHandler(EditableHeaderedItemsControl.EditedEvent, value);
            }
        }

        [ScriptableMember]
        public event RadTreeViewItemEditedEventHandler EditStarted
        {
            add
            {
                this.AddHandler(EditableHeaderedItemsControl.EditStartedEvent, value);
            }
            remove
            {
                this.RemoveHandler(EditableHeaderedItemsControl.EditStartedEvent, value);
            }
        }

        [ScriptableMember]
        public event EventHandler<RadRoutedEventArgs> Expanded
        {
            add
            {
                this.AddHandler(RadTreeViewItem.ExpandedEvent, value);
            }
            remove
            {
                this.RemoveHandler(RadTreeViewItem.ExpandedEvent, value);
            }
        }

        public event EventHandler<RadTreeViewItemPreparedEventArgs> ItemPrepared;

        [ScriptableMember]
        public event EventHandler<RadRoutedEventArgs> LoadOnDemand
        {
            add
            {
                this.AddHandler(RadTreeViewItem.LoadOnDemandEvent, value);
            }
            remove
            {
                this.RemoveHandler(RadTreeViewItem.LoadOnDemandEvent, value);
            }
        }

        [ScriptableMember]
        public event EventHandler<RadRoutedEventArgs> PreviewChecked
        {
            add
            {
                this.AddHandler(RadTreeViewItem.PreviewCheckedEvent, value);
            }
            remove
            {
                this.RemoveHandler(RadTreeViewItem.PreviewCheckedEvent, value);
            }
        }

        [ScriptableMember]
        public event EventHandler<RadRoutedEventArgs> PreviewCollapsed
        {
            add
            {
                this.AddHandler(RadTreeViewItem.PreviewCollapsedEvent, value);
            }
            remove
            {
                this.RemoveHandler(RadTreeViewItem.PreviewCollapsedEvent, value);
            }
        }

        [ScriptableMember, Description("Occurs before the drag has been ended."), Category("Behavior")]
        public event RadTreeViewDragEndedEventHandler PreviewDragEnded
        {
            add
            {
                this.AddHandler(PreviewDragEndedEvent, value);
            }
            remove
            {
                this.RemoveHandler(PreviewDragEndedEvent, value);
            }
        }

        [Category("Behavior"), Description("Occurs before the drag is started."), ScriptableMember]
        public event RadTreeViewDragEventHandler PreviewDragStarted
        {
            add
            {
                this.AddHandler(PreviewDragStartedEvent, value);
            }
            remove
            {
                this.RemoveHandler(PreviewDragStartedEvent, value);
            }
        }

        [ScriptableMember]
        public event RadTreeViewItemEditedEventHandler PreviewEditCanceled
        {
            add
            {
                this.AddHandler(EditableHeaderedItemsControl.PreviewEditCanceledEvent, value);
            }
            remove
            {
                this.RemoveHandler(EditableHeaderedItemsControl.PreviewEditCanceledEvent, value);
            }
        }

        [ScriptableMember]
        public event RadTreeViewItemEditedEventHandler PreviewEdited
        {
            add
            {
                this.AddHandler(EditableHeaderedItemsControl.PreviewEditedEvent, value);
            }
            remove
            {
                this.RemoveHandler(EditableHeaderedItemsControl.PreviewEditedEvent, value);
            }
        }

        [ScriptableMember]
        public event RadTreeViewItemEditedEventHandler PreviewEditStarted
        {
            add
            {
                this.AddHandler(EditableHeaderedItemsControl.PreviewEditStartedEvent, value);
            }
            remove
            {
                this.RemoveHandler(EditableHeaderedItemsControl.PreviewEditStartedEvent, value);
            }
        }

        [ScriptableMember]
        public event EventHandler<RadRoutedEventArgs> PreviewExpanded
        {
            add
            {
                this.AddHandler(RadTreeViewItem.PreviewExpandedEvent, value);
            }
            remove
            {
                this.RemoveHandler(RadTreeViewItem.PreviewExpandedEvent, value);
            }
        }

        [ScriptableMember]
        public event EventHandler<RadRoutedEventArgs> PreviewSelected
        {
            add
            {
                this.AddHandler(RadTreeViewItem.PreviewSelectedEvent, value);
            }
            remove
            {
                this.RemoveHandler(RadTreeViewItem.PreviewSelectedEvent, value);
            }
        }

        [Description("Occurs before the value of the SelectedItems property of a tree is changed."), ScriptableMember]
        public event Telerik.Windows.Controls.SelectionChangedEventHandler PreviewSelectionChanged
        {
            add
            {
                this.AddHandler(PreviewSelectionChangedEvent, value);
            }
            remove
            {
                this.RemoveHandler(PreviewSelectionChangedEvent, value);
            }
        }

        [ScriptableMember]
        public event EventHandler<RadRoutedEventArgs> PreviewUnchecked
        {
            add
            {
                this.AddHandler(RadTreeViewItem.PreviewUncheckedEvent, value);
            }
            remove
            {
                this.RemoveHandler(RadTreeViewItem.PreviewUncheckedEvent, value);
            }
        }

        [ScriptableMember]
        public event EventHandler<RadRoutedEventArgs> PreviewUnselected
        {
            add
            {
                this.AddHandler(RadTreeViewItem.PreviewUnselectedEvent, value);
            }
            remove
            {
                this.RemoveHandler(RadTreeViewItem.PreviewUnselectedEvent, value);
            }
        }

        [ScriptableMember]
        public event EventHandler<RadRoutedEventArgs> Selected
        {
            add
            {
                this.AddHandler(RadTreeViewItem.SelectedEvent, value);
            }
            remove
            {
                this.RemoveHandler(RadTreeViewItem.SelectedEvent, value);
            }
        }

        [Description("Occurs after the value of the SelectedItems collection of a tree is changed."), ScriptableMember, Category("Behavior")]
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

        [ScriptableMember]
        public event EventHandler<RadRoutedEventArgs> Unchecked
        {
            add
            {
                this.AddHandler(RadTreeViewItem.UncheckedEvent, value);
            }
            remove
            {
                this.RemoveHandler(RadTreeViewItem.UncheckedEvent, value);
            }
        }

        [ScriptableMember]
        public event EventHandler<RadRoutedEventArgs> Unselected
        {
            add
            {
                this.AddHandler(RadTreeViewItem.UnselectedEvent, value);
            }
            remove
            {
                this.RemoveHandler(RadTreeViewItem.UnselectedEvent, value);
            }
        }

        static RadTreeView()
        {
            EventManager.RegisterClassHandler(typeof(RadTreeView), RadDragAndDropManager.DragQueryEvent, new EventHandler<DragDropQueryEventArgs>(RadTreeView.OnTreeViewDragQuery));
            EventManager.RegisterClassHandler(typeof(RadTreeView), RadDragAndDropManager.DragInfoEvent, new EventHandler<DragDropEventArgs>(RadTreeView.OnTreeViewDragInfo));
            EventManager.RegisterClassHandler(typeof(RadTreeView), RadDragAndDropManager.DropInfoEvent, new EventHandler<DragDropEventArgs>(RadTreeView.OnTreeViewDropInfo));
            EventManager.RegisterClassHandler(typeof(RadTreeView), RadDragAndDropManager.DropQueryEvent, new EventHandler<DragDropQueryEventArgs>(RadTreeView.OnTreeViewDropQuery));
            EventManager.RegisterClassHandler(typeof(RadTreeViewItem), RadDragAndDropManager.DropQueryEvent, new EventHandler<DragDropQueryEventArgs>(RadTreeView.OnTreeViewItemDropQuery));
            SelectedItemsProperty = SelectedItemsPropertyKey.DependencyProperty;
            SelectedContainerProperty = SelectedContainerPropertyKey.DependencyProperty;
        }

        public RadTreeView()
        {
            
            base.DefaultStyleKey = typeof(RadTreeView);
            base.TabNavigation = KeyboardNavigationMode.Once;
            System.Windows.Controls.ScrollViewer.SetHorizontalScrollBarVisibility(this, ScrollBarVisibility.Auto);
            System.Windows.Controls.ScrollViewer.SetVerticalScrollBarVisibility(this, ScrollBarVisibility.Auto);
            this.selectionChanger = new Telerik.Windows.Controls.SelectionChanger<Object>(new Func<object, bool>(this.IsItemValidForSelection), new Func<object, bool>(this.IsItemValidForDeselection), new Func<System.Windows.Controls.SelectionChangedEventArgs, bool>(this.IsSelectionChangePossible));
            this.selectionChanger.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.OnSelectionChangerSelectionChanged);
            this.SelectedItems = this.selectionChanger;
            this.itemStorage = new ItemAttachedStorage();
            this.itemToggleStateStorage = new Dictionary<WeakReferenceKey<object>, ToggleState>(0x20);
            this.CheckedItems = new CheckedItemsCollection(this.itemToggleStateStorage.Keys, this.itemToggleStateStorage);
            base.GotFocus += new RoutedEventHandler(this.OnTreeViewGotFocus);
            base.LostFocus += new RoutedEventHandler(this.OnLostFocus);
            base.Loaded += new RoutedEventHandler(this.OnLoaded);
            base.Unloaded += new RoutedEventHandler(this.OnUnloaded);
            this.AddHandler(RadContextMenu.OpenedEvent, new RoutedEventHandler(this.ContextMenuOpened));
            this.AddHandler(RadContextMenu.ClosedEvent, new RoutedEventHandler(this.ContextMenuClosed));
            this.SetDefaultValues();
        }

        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "ByPath"), ScriptableMember]
        public RadTreeViewItem AddItemByPath(string path)
        {
            return this.AddItemByPath(path, this.PathSeparator);
        }

        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "ByPath"), ScriptableMember]
        public RadTreeViewItem AddItemByPath(string path, string separator)
        {
            if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(separator))
            {
                return null;
            }
            int lastSepIndex = path.LastIndexOf(separator, StringComparison.OrdinalIgnoreCase);
            IList targetCollection = base.Items;
            if (lastSepIndex != -1)
            {
                RadTreeViewItem parent = this.GetItemByPath(path.Substring(0, lastSepIndex), separator);
                if (parent != null)
                {
                    targetCollection = parent.Items;
                }
            }
            lastSepIndex++;
            string newItemText = path.Substring(lastSepIndex, path.Length - lastSepIndex);
            RadTreeViewItem item = new RadTreeViewItem
            {
                Header = newItemText
            };
            targetCollection.Add(item);
            return item;
        }

        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "ByPath")]
        public RadTreeViewItem AddItemsByPath(string path, string separator)
        {
            if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(separator))
            {
                return null;
            }
            RadTreeViewItem item = null;
            string[] itemsArray = path.Split(separator.ToCharArray());
            int i = 0;
            string currentpath = null;
            IList targetCollection = base.Items;
            while (itemsArray.Length != i)
            {
                if (currentpath == null)
                {
                    currentpath = itemsArray[i];
                }
                else
                {
                    currentpath = currentpath + separator + itemsArray[i];
                }
                RadTreeViewItem parent = this.GetItemByPath(currentpath, separator);
                if (parent != null)
                {
                    targetCollection = parent.Items;
                    string newItemText = itemsArray[i];
                    item = new RadTreeViewItem
                    {
                        Header = newItemText
                    };
                    targetCollection.Add(item);
                }
                else
                {
                    string newItemText = itemsArray[i];
                    item = new RadTreeViewItem
                    {
                        Header = newItemText
                    };
                    targetCollection = (targetCollection[targetCollection.Count - 1] as RadTreeViewItem).Items;
                    targetCollection.Add(item);
                }
                i++;
            }
            return item;
        }

        private void AddNonHierarchicalSelectedItems(Collection<object> draggedItems)
        {
            foreach (object validSelectedItem in this.selectionChanger.Where<object>(delegate(object selected)
            {
                RadTreeViewItem container = this.ContainerFromItemRecursive(selected);
                return (container == null) || (from parent in container.GetAncestors<RadTreeViewItem>() select parent.Item).All<object>(parentItem => !this.selectionChanger.Contains(parentItem));
            }))
            {
                draggedItems.Add(validSelectedItem);
            }
        }

        private void BindXAMLElements()
        {
            this.dragBetweenItemsFeedback = (FrameworkElement)base.GetTemplateChild("DragBetweenItemsFeedback");
            this.scroller = (System.Windows.Controls.ScrollViewer)base.GetTemplateChild("ScrollViewer");
            if (this.scroller != null)
            {
                this.scroller.HorizontalScrollBarVisibility = System.Windows.Controls.ScrollViewer.GetHorizontalScrollBarVisibility(this);
                this.scroller.VerticalScrollBarVisibility = System.Windows.Controls.ScrollViewer.GetVerticalScrollBarVisibility(this);
            }
        }

        public void BringIndexIntoView(int index)
        {
            if (((index > -1) && (index < base.Items.Count)) && (this.itemsHost != null))
            {
                this.itemsHost.BringIndexIntoViewInternal(index);
            }
        }

        public void BringItemIntoView(object item)
        {
            int itemIndex = base.Items.IndexOf(item);
            this.BringIndexIntoView(itemIndex);
        }

        private void CalculateInitialCheckState()
        {
            if (this.IsOptionElementsEnabled && this.IsTriStateMode)
            {
                foreach (object item in base.Items)
                {
                    RadTreeViewItem itemContainer = item as RadTreeViewItem;
                    if (itemContainer != null)
                    {
                        if ((itemContainer.CheckState != ToggleState.On) && (itemContainer.OptionType != OptionListType.OptionList))
                        {
                            itemContainer.CheckState = RadTreeViewItem.CalculateInitialItemCheckState(itemContainer);
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        internal void CancelDrag()
        {
            this.cancelDragging = true;
            this.EndDrag(null, null);
        }

        protected internal override void ChangeVisualState(bool useTransitions)
        {
            base.ChangeVisualState(useTransitions);
            if (base.IsEnabled)
            {
                if (this.isDropPossible)
                {
                    if (!base.HasItems)
                    {
                        VisualStateManager.GoToState(this, "DropRootPossible", useTransitions);
                    }
                    else
                    {
                        VisualStateManager.GoToState(this, "DropPossible", useTransitions);
                    }
                }
                else
                {
                    VisualStateManager.GoToState(this, "DropImpossible", useTransitions);
                }
            }
        }

        internal virtual void ClearContainerForDescendant(DependencyObject element, object item)
        {
            RadTreeViewItem treeViewItem = element as RadTreeViewItem;
            if (item == null)
            {
                item = treeViewItem.Item;
            }
            if (treeViewItem == this.SelectedContainer)
            {
                this.SelectedContainer = null;
            }
            treeViewItem.ParentItem = null;
            treeViewItem.ParentTreeView = null;
            if (this.IsVirtualizing && this.generatedItemCache.ContainsKey(item))
            {
                this.generatedItemCache.Remove(item);
            }
            if ((treeViewItem.Item != null) && (treeViewItem.GetBindingExpression(RadTreeViewItem.IsSelectedProperty) == null))
            {
                try
                {
                    treeViewItem.SupressEventRaising = true;
                    treeViewItem.ClearValue(RadTreeViewItem.IsSelectedProperty);
                }
                finally
                {
                    treeViewItem.SupressEventRaising = false;
                }
            }
            treeViewItem.SupressEventRaising = true;
            try
            {
                ContainerBinding.RemoveContaienrBindings(treeViewItem, treeViewItem.HeaderTemplate);
            }
            finally
            {
                treeViewItem.SupressEventRaising = false;
            }
            if (this.IsVirtualizing)
            {
                if (TreeViewPanel.GetVirtualizationMode(this) == Telerik.Windows.Controls.TreeView.VirtualizationMode.Hierarchical)
                {
                    treeViewItem.ParentItem = null;
                    treeViewItem.IsRenderPending = true;
                    if (treeViewItem.ItemsPresenterElement != null)
                    {
                        treeViewItem.ItemsPresenterElement.Visibility = Visibility.Collapsed;
                    }
                    if (repository.RecycleContainer(treeViewItem))
                    {
                        ((IItemContainerGenerator)treeViewItem.ItemContainerGenerator).RemoveAll();
                    }
                }
                this.ClearDependencyProperties(treeViewItem, item);
            }
        }

        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            this.ClearContainerForDescendant(element, item);
            base.ClearContainerForItemOverride(element, item);
        }

        private void ClearDependencyProperties(RadTreeViewItem treeViewItem, object item)
        {
            treeViewItem.SupressEventRaising = true;
            try
            {
                treeViewItem.ClearValue(HeaderedItemsControl.HeaderProperty);
                treeViewItem.ClearValue(HeaderedItemsControl.HeaderTemplateProperty);
                treeViewItem.ClearValue(HeaderedItemsControl.HeaderTemplateSelectorProperty);
                treeViewItem.ClearValue(EditableHeaderedItemsControl.HeaderEditTemplateProperty);
                treeViewItem.ClearValue(EditableHeaderedItemsControl.HeaderEditTemplateSelectorProperty);
                treeViewItem.ClearValue(Telerik.Windows.Controls.ItemsControl.ItemContainerStyleProperty);
                treeViewItem.ClearValue(Telerik.Windows.Controls.ItemsControl.ItemContainerStyleSelectorProperty);
                treeViewItem.ClearValue(System.Windows.Controls.ItemsControl.ItemsSourceProperty);
                this.ItemStorage.StoreValue(treeViewItem, RadTreeViewItem.ExpandedImageSrcProperty, item, null);
                this.ItemStorage.StoreValue(treeViewItem, RadTreeViewItem.DefaultImageSrcProperty, item, null);
                this.ItemStorage.StoreValue(treeViewItem, RadTreeViewItem.SelectedImageSrcProperty, item, null);
                if (this.IsLoadOnDemandEnabled)
                {
                    this.ItemStorage.StoreValue(treeViewItem, RadTreeViewItem.IsLoadingOnDemandProperty, item, false);
                    this.ItemStorage.StoreValue(treeViewItem, RadTreeViewItem.IsLoadOnDemandEnabledProperty, item, false);
                }
                if (this.IsOptionElementsEnabled)
                {
                    treeViewItem.ClearValue(RadTreeViewItem.OptionTypeProperty);
                }
                treeViewItem.ClearValue(RadTreeViewItem.IsExpandedProperty);
            }
            finally
            {
                treeViewItem.SupressEventRaising = false;
            }
        }

        private void ClearItemIndeterminatedState(RadTreeViewItem itemContainer)
        {
            if ((itemContainer.OptionElement != null) && (itemContainer.CheckState == ToggleState.Indeterminate))
            {
                itemContainer.CheckState = ToggleState.Off;
            }
            itemContainer.ForEachContainerItem<RadTreeViewItem>(delegate(RadTreeViewItem childContainer)
            {
                this.ClearItemIndeterminatedState(childContainer);
            });
        }

        private void ClearSelectionChanger(object itemToKeep)
        {
            for (int i = 0; i < this.selectionChanger.Count; i++)
            {
                if (itemToKeep != this.selectionChanger[i])
                {
                    this.selectionChanger.Remove(this.selectionChanger[i]);
                }
            }
        }

        [ScriptableMember]
        public void CollapseAll()
        {
            bool oldAnimationValue = AnimationManager.IsGlobalAnimationEnabled;
            AnimationManager.IsGlobalAnimationEnabled = false;
            this.SetExpandState(base.Items, false, true);
            AnimationManager.IsGlobalAnimationEnabled = oldAnimationValue;
        }

        public RadTreeViewItem ContainerFromItemRecursive(object item)
        {
            if (!this.IsVirtualizing)
            {
                return this.ContainerFromItemRecursive(item, this);
            }
            if (this.generatedItemCache.ContainsKey(item))
            {
                return (this.generatedItemCache[item].Target as RadTreeViewItem);
            }
            return null;
        }

        private RadTreeViewItem ContainerFromItemRecursive(object item, Telerik.Windows.Controls.ItemsControl itemContainer)
        {
            if (item == null)
            {
                return null;
            }
            if (this.IsVirtualizing)
            {
                if (this.generatedItemCache.ContainsKey(item))
                {
                    return (this.generatedItemCache[item].Target as RadTreeViewItem);
                }
                return null;
            }
            RadTreeViewItem container = itemContainer.ItemContainerGenerator.ContainerFromItem(item) as RadTreeViewItem;
            if (container == null)
            {
                for (int i = 0; i < itemContainer.Items.Count; i++)
                {
                    RadTreeViewItem childContainer = itemContainer.ItemContainerGenerator.ContainerFromItem(itemContainer.Items[i]) as RadTreeViewItem;
                    if (childContainer != null)
                    {
                        container = this.ContainerFromItemRecursive(item, childContainer);
                        if (container != null)
                        {
                            return container;
                        }
                    }
                }
            }
            return container;
        }

        private void ContextMenuClosed(object sender, RoutedEventArgs e)
        {
            RadRoutedEventArgs radArgs = e as RadRoutedEventArgs;
            RadContextMenu contextMenu = radArgs.Source as RadContextMenu;
            if (contextMenu != null)
            {
                RadTreeViewItem itemToHilite = contextMenu.GetClickedElement<RadTreeViewItem>();
                if (itemToHilite != null)
                {
                    itemToHilite.IsDragOver = false;
                }
            }
        }

        private void ContextMenuOpened(object sender, RoutedEventArgs e)
        {
            RadRoutedEventArgs radArgs = e as RadRoutedEventArgs;
            RadContextMenu contextMenu = radArgs.Source as RadContextMenu;
            if (contextMenu != null)
            {
                RadTreeViewItem itemToHilite = contextMenu.GetClickedElement<RadTreeViewItem>();
                if (itemToHilite != null)
                {
                    itemToHilite.IsDragOver = true;
                }
            }
        }

        private void CopyVirtualizingStackPanelPropertiesIfSet()
        {
            object virtualizationModeVSP = base.ReadLocalValue(VirtualizingStackPanel.VirtualizationModeProperty);
            if ((virtualizationModeVSP != null) && (virtualizationModeVSP != DependencyProperty.UnsetValue))
            {
                object virtualizationModeTVP = base.ReadLocalValue(TreeViewPanel.VirtualizationModeProperty);
                if ((virtualizationModeTVP == null) || (virtualizationModeTVP == DependencyProperty.UnsetValue))
                {
                    if (((System.Windows.Controls.VirtualizationMode)virtualizationModeVSP) == System.Windows.Controls.VirtualizationMode.Recycling)
                    {
                        TreeViewPanel.SetVirtualizationMode(this, Telerik.Windows.Controls.TreeView.VirtualizationMode.Recycling);
                    }
                    else if (((System.Windows.Controls.VirtualizationMode)virtualizationModeVSP) == System.Windows.Controls.VirtualizationMode.Standard)
                    {
                        TreeViewPanel.SetVirtualizationMode(this, Telerik.Windows.Controls.TreeView.VirtualizationMode.Standard);
                    }
                }
            }
        }

        private TreeViewDragCue CreateDragCue()
        {
            if (!this.IsDragPreviewEnabled && !this.IsDragTooltipEnabled)
            {
                return null;
            }
            TreeViewDragCue visualCue = new TreeViewDragCue
            {
                DragPreviewVisibility = this.IsDragPreviewEnabled ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed,
                DragTooltipVisibility = this.IsDragTooltipEnabled ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed,
                IsDropPossible = false
            };
            StyleManager.SetTheme(visualCue, StyleManager.GetTheme(this));
            return visualCue;
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity"), SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        internal void EndDrag(FrameworkElement destinationItem, IEnumerable draggedItemsEnumerable)
        {
            if (RadDragAndDropManager.IsDragging)
            {
                Collection<object> draggedItems = new Collection<object>();
                if (draggedItemsEnumerable != null)
                {
                    foreach (object item in draggedItemsEnumerable)
                    {
                        draggedItems.Add(item);
                    }
                }
                this.HideBetweenItemsDragCue();
                RadTreeViewItem destinationTreeViewItem = destinationItem as RadTreeViewItem;
                if (this.cancelDragging || (destinationItem == null))
                {
                    this.OnDragEnded(new RadTreeViewDragEndedEventArgs(true, draggedItems, destinationTreeViewItem, DragEndedEvent, this));
                }
                else
                {
                    this.PreviewDragEndResult = new bool?(this.OnPreviewDragEnded(new RadTreeViewDragEndedEventArgs(false, draggedItems, destinationTreeViewItem, PreviewDragEndedEvent, this)));
                    if (!this.PreviewDragEndResult.Value)
                    {
                        foreach (object draggedTreeItem in draggedItems)
                        {
                            RadTreeViewItem draggedItemContainer = this.ContainerFromItemRecursive(draggedTreeItem);
                            if (draggedItemContainer != null)
                            {
                                Telerik.Windows.Controls.ItemsControl draggedItemOwner = draggedItemContainer.Owner;
                                if (draggedItemOwner.ItemsSource != null)
                                {
                                    IList itemsSourceAsIList = draggedItemOwner.ItemsSource as IList;
                                    if (itemsSourceAsIList != null)
                                    {
                                        itemsSourceAsIList.Remove(draggedTreeItem);
                                    }
                                }
                                else
                                {
                                    Telerik.Windows.Controls.ItemsControl.ItemsControlFromItemContainer(draggedItemContainer).Items.Remove(draggedTreeItem);
                                }
                            }
                        }
                    }
                }
            }
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        internal void EndDrop(FrameworkElement dropTarget, IEnumerable draggedItemsEnumerable)
        {
            object firstItem;
            if ((dropTarget != null) && (draggedItemsEnumerable != null))
            {
                Collection<object> draggedItemsCollection = new Collection<object>();
                foreach (object draggedItem in draggedItemsEnumerable)
                {
                    draggedItemsCollection.Add(draggedItem);
                }
                Telerik.Windows.Controls.ItemsControl itemsControlDestination = dropTarget as Telerik.Windows.Controls.ItemsControl;
                IList collection = null;
                if (itemsControlDestination.ItemsSource != null)
                {
                    collection = itemsControlDestination.ItemsSource as IList;
                }
                else
                {
                    collection = itemsControlDestination.Items;
                }
                int targetIndex = 0;
                RadTreeViewItem treeViewItemDestination = itemsControlDestination as RadTreeViewItem;
                if ((!this.PreviewDragEndResult.HasValue || !this.PreviewDragEndResult.Value) && (this.PreviewDragEndResult.HasValue || !this.OnPreviewDragEnded(new RadTreeViewDragEndedEventArgs(false, draggedItemsCollection, treeViewItemDestination, PreviewDragEndedEvent, this))))
                {
                    if ((treeViewItemDestination != null) && (treeViewItemDestination.DropPosition != DropPosition.Inside))
                    {
                        Telerik.Windows.Controls.ItemsControl parentContainer = Telerik.Windows.Controls.ItemsControl.ItemsControlFromItemContainer(treeViewItemDestination);
                        if (parentContainer.ItemsSource != null)
                        {
                            collection = parentContainer.ItemsSource as IList;
                        }
                        else
                        {
                            collection = parentContainer.Items;
                        }
                        int destinationItemIndex = collection.IndexOf(treeViewItemDestination.Item);
                        targetIndex = (treeViewItemDestination.DropPosition == DropPosition.Before) ? destinationItemIndex : (destinationItemIndex + 1);
                        if (targetIndex < 0)
                        {
                            targetIndex = 0;
                        }
                    }
                    else if (collection != null)
                    {
                        targetIndex = collection.Count;
                    }
                    foreach (object draggedItem in draggedItemsCollection.Reverse<object>())
                    {
                        RadTreeViewItem draggedItemContainer = this.ContainerFromItemRecursive(draggedItem);
                        if (draggedItemContainer != null)
                        {
                            Telerik.Windows.Controls.ItemsControl draggedItemOwner = draggedItemContainer.Owner;
                            if (((collection != null) && collection.Contains(draggedItem)) && (targetIndex > draggedItemOwner.ItemContainerGenerator.IndexFromContainer(draggedItemContainer)))
                            {
                                targetIndex--;
                            }
                        }
                        try
                        {
                            collection.Insert(targetIndex, draggedItem);
                            continue;
                        }
                        catch (InvalidCastException)
                        {
                            continue;
                        }
                        catch (ArgumentException)
                        {
                            continue;
                        }
                    }
                    this.OnDragEnded(new RadTreeViewDragEndedEventArgs(false, draggedItemsCollection, treeViewItemDestination, DragEndedEvent, this));
                    if ((treeViewItemDestination != null) && (treeViewItemDestination.DropPosition == DropPosition.Inside))
                    {
                        treeViewItemDestination.IsExpanded = true;
                    }
                    firstItem = draggedItemsCollection.FirstOrDefault<object>();
                    if (firstItem != null)
                    {
                        base.Dispatcher.BeginInvoke(delegate
                        {
                            RadTreeViewItem newContainer = this.ContainerFromItemRecursive(firstItem);
                            if (newContainer != null)
                            {
                                newContainer.Focus();
                            }
                        });
                    }
                }
            }
        }

        [ScriptableMember]
        public void ExpandAll()
        {
            bool oldAnimationValue = AnimationManager.IsGlobalAnimationEnabled;
            AnimationManager.IsGlobalAnimationEnabled = false;
            this.SetExpandState(base.Items, true, true);
            AnimationManager.IsGlobalAnimationEnabled = oldAnimationValue;
        }

        private void ExpandChildItem(string searchString, Telerik.Windows.Controls.ItemsControl parent)
        {
            foreach (object item in parent.Items)
            {
                RadTreeViewItem treeItem = parent.ItemContainerGenerator.ContainerFromItem(item) as RadTreeViewItem;
                string itemString = TextSearch.GetPrimaryTextFromItem(this, item);
                if (((treeItem != null) && (itemString != null)) && itemString.Equals(searchString))
                {
                    treeItem.IsExpanded = true;
                    if (this.expandByPathpathParts[this.expandByPathpathParts.Length - 1].Equals(searchString))
                    {
                        this.expandByPathpathParts = null;
                        this.ItemPrepared -= new EventHandler<RadTreeViewItemPreparedEventArgs>(this.RadTreeView_ItemPrepared);
                    }
                    break;
                }
            }
        }

        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "ByPath")]
        public void ExpandItemByPath(string path)
        {
            this.ExpandItemByPath(path, this.PathSeparator);
        }

        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "ByPath")]
        public void ExpandItemByPath(string path, string separator)
        {
            if (!string.IsNullOrEmpty(path) && !string.IsNullOrEmpty(separator))
            {
                string[] pathParts = path.Split(separator.ToCharArray());
                this.expandByPathpathParts = pathParts;
                this.ItemPrepared += new EventHandler<RadTreeViewItemPreparedEventArgs>(this.RadTreeView_ItemPrepared);
                RadTreeViewItem retrievedItem = this.GetItemByPath(path, separator);
                if ((retrievedItem != null) && (retrievedItem.HasItems || retrievedItem.IsLoadOnDemandEnabled))
                {
                    retrievedItem.IsExpanded = true;
                    retrievedItem.UpdateLayout();
                }
            }
        }

        private static object FindFirstItem(RadTreeViewItem treeViewItem)
        {
            if (((treeViewItem != null) && (treeViewItem.Items.Count > 0)) && treeViewItem.IsExpanded)
            {
                return treeViewItem.Items[0];
            }
            return null;
        }

        private bool FindNextItem(ref RadTreeViewItem treeViewItem, ref object item, ref Telerik.Windows.Controls.ItemsControl itemOwner)
        {
            if ((treeViewItem != null) && !this.IsVirtualizing)
            {
                RadTreeViewItem nextContainer = treeViewItem.NextItem;
                if (nextContainer != null)
                {
                    treeViewItem = nextContainer;
                    item = treeViewItem.Item;
                    itemOwner = treeViewItem.Owner;
                    return true;
                }
            }
            object firstVisibleChildItem = FindFirstItem(treeViewItem);
            if (firstVisibleChildItem != null)
            {
                itemOwner = treeViewItem;
                treeViewItem = treeViewItem.ItemContainerGenerator.ContainerFromIndex(0) as RadTreeViewItem;
                item = firstVisibleChildItem;
                return true;
            }
            Telerik.Windows.Controls.ItemsControl currentItemOwner = itemOwner;
            object currentItem = item;
            do
            {
                int nextSiblingIndex = currentItemOwner.Items.IndexOf(currentItem) + 1;
                int siblingsCount = currentItemOwner.Items.Count;
                if (nextSiblingIndex < siblingsCount)
                {
                    item = currentItemOwner.Items[nextSiblingIndex];
                    treeViewItem = currentItemOwner.ItemContainerGenerator.ContainerFromIndex(nextSiblingIndex) as RadTreeViewItem;
                    itemOwner = currentItemOwner;
                    return true;
                }
                RadTreeViewItem treeViewItemOwner = currentItemOwner as RadTreeViewItem;
                if ((currentItemOwner != null) && (treeViewItemOwner != null))
                {
                    currentItem = treeViewItemOwner.Item;
                }
                if (currentItemOwner != null)
                {
                    currentItemOwner = Telerik.Windows.Controls.ItemsControl.ItemsControlFromItemContainer(currentItemOwner);
                }
            }
            while ((currentItem != null) && (currentItemOwner != null));
            item = null;
            itemOwner = null;
            treeViewItem = null;
            return false;
        }

        private bool FindPreviousItem(ref RadTreeViewItem treeViewItem, ref object item, ref Telerik.Windows.Controls.ItemsControl itemOwner)
        {
            if ((treeViewItem != null) && !this.IsVirtualizing)
            {
                RadTreeViewItem prevContainer = treeViewItem.PreviousItem;
                if (prevContainer != null)
                {
                    treeViewItem = prevContainer;
                    item = treeViewItem.Item;
                    itemOwner = treeViewItem.Owner;
                    return true;
                }
            }
            int prevIndex = itemOwner.Items.IndexOf(item) - 1;
            object prevSinglingItem = null;
            RadTreeViewItem prevSiblingContainer = null;
            if (prevIndex > -1)
            {
                prevSinglingItem = itemOwner.Items[prevIndex];
                prevSiblingContainer = itemOwner.ItemContainerGenerator.ContainerFromIndex(prevIndex) as RadTreeViewItem;
            }
            if (prevSinglingItem != null)
            {
                while (((prevSinglingItem != null) && (prevSiblingContainer != null)) && (prevSiblingContainer.IsExpanded && (prevSiblingContainer.Items.Count > 0)))
                {
                    int lastItemIndex = prevSiblingContainer.Items.Count - 1;
                    itemOwner = prevSiblingContainer;
                    prevSinglingItem = prevSiblingContainer.Items[lastItemIndex];
                    prevSiblingContainer = prevSiblingContainer.ItemContainerGenerator.ContainerFromIndex(lastItemIndex) as RadTreeViewItem;
                }
                item = prevSinglingItem;
                treeViewItem = prevSiblingContainer;
                return true;
            }
            if (itemOwner is RadTreeViewItem)
            {
                treeViewItem = itemOwner as RadTreeViewItem;
                itemOwner = treeViewItem.Owner;
                item = treeViewItem.Item;
                return true;
            }
            item = null;
            itemOwner = null;
            treeViewItem = null;
            return false;
        }

        private void FindString(string searchString)
        {
            RadTreeViewItem currentItem = this.SelectedContainer;
            if (currentItem != null)
            {
                currentItem = currentItem.NextItem;
            }
            if (currentItem == null)
            {
                currentItem = base.ItemContainerGenerator.ContainerFromIndex(0) as RadTreeViewItem;
            }
            RadTreeViewItem startItem = currentItem;
            while (currentItem != null)
            {
                string text1 = currentItem.ToString();
                if ((text1 != null) && text1.StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase))
                {
                    this.SetSelectedItem(currentItem);
                    return;
                }
                currentItem = currentItem.NextItem;
                if (currentItem == null)
                {
                    currentItem = base.ItemContainerGenerator.ContainerFromIndex(0) as RadTreeViewItem;
                }
                if (currentItem == startItem)
                {
                    return;
                }
            }
        }

        internal IEnumerable<RadTreeViewItem> GetAllItemContainers()
        {
            return this.GetAllItemContainers(this);
        }

        internal IEnumerable<RadTreeViewItem> GetAllItemContainers(Telerik.Windows.Controls.ItemsControl itemsControl)
        {
            if (itemsControl != this || !this.IsVirtualizing || generatedItemCache == null)
            {
                for (var i = 0; i < itemsControl.Items.Count; i++)
                {
                    var childItemContainer = itemsControl.ItemContainerGenerator.ContainerFromIndex(i) as RadTreeViewItem;
                    if (childItemContainer != null)
                    {
                        yield return childItemContainer;
                        foreach (var item in this.GetAllItemContainers(childItemContainer))
                            yield return item;
                    }
                }
            }
            else
            {
                foreach (var item in generatedItemCache.Values)
                    yield return item.Target as RadTreeViewItem;
            }
        }

        internal ToggleState GetCheckStateValue(object item)
        {
            ToggleState result;
            WeakReferenceKey<object> key = new WeakReferenceKey<object>(item);
            if (this.itemToggleStateStorage.TryGetValue(key, out result))
            {
                return result;
            }
            return ToggleState.Off;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return this.GetContainerForTreeViewItem();
        }

        internal DependencyObject GetContainerForTreeViewItem()
        {
            if (this.IsVirtualizing && (TreeViewPanel.GetVirtualizationMode(this) == Telerik.Windows.Controls.TreeView.VirtualizationMode.Hierarchical))
            {
                return repository.GetContainer();
            }
            return new RadTreeViewItem();
        }

        private object GetDragActionText(RadTreeViewItem destinationItem)
        {
            switch (destinationItem.DropPosition)
            {
                case DropPosition.Before:
                    return this.TextDropBefore;

                case DropPosition.Inside:
                    return this.TextDropIn;

                case DropPosition.After:
                    return this.TextDropAfter;
            }
            return string.Empty;
        }

        private static DataTemplate GetDragTooltipTemplateFromItem(RadTreeViewItem destinationItem)
        {
            DataTemplate result = destinationItem.HeaderTemplate;
            if ((destinationItem.HeaderTemplate == null) && (destinationItem.HeaderTemplateSelector != null))
            {
                result = destinationItem.HeaderTemplateSelector.SelectTemplate(destinationItem.Item, destinationItem);
            }
            return result;
        }

        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "ByPath"), ScriptableMember]
        public RadTreeViewItem GetItemByPath(string path)
        {
            return this.GetItemByPath(path, this.PathSeparator);
        }

        [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily"), SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "ByPath"), ScriptableMember]
        public RadTreeViewItem GetItemByPath(string path, string separator)
        {
            if (!string.IsNullOrEmpty(path) && !string.IsNullOrEmpty(separator))
            {
                string[] searchTexts = path.Split(separator.ToCharArray());
                Telerik.Windows.Controls.ItemsControl itemsControl = this;
                bool itemFound = false;
                for (int pathIndex = 0; pathIndex < searchTexts.Length; pathIndex++)
                {
                    itemFound = false;
                    RadTreeViewItem childContainer = null;
                    string searchText = searchTexts[pathIndex];
                    for (int childIndex = 0; childIndex < itemsControl.Items.Count; childIndex++)
                    {
                        if (!TextSearch.GetPrimaryTextFromItem(this, itemsControl.Items[childIndex]).Equals(searchText))
                        {
                            continue;
                        }
                        for (int containerQueryCount = 0; containerQueryCount < 10; containerQueryCount++)
                        {
                            RadTreeViewItem treeViewItem = itemsControl as RadTreeViewItem;
                            if (((treeViewItem != null) && !treeViewItem.IsExpanded) && treeViewItem.IsEnabled)
                            {
                                bool oldAnimationEnabled = AnimationManager.GetIsAnimationEnabled(treeViewItem);
                                if (oldAnimationEnabled)
                                {
                                    AnimationManager.SetIsAnimationEnabled(treeViewItem, false);
                                }
                                treeViewItem.IsExpanded = true;
                                treeViewItem.UpdateLayout();
                                if (oldAnimationEnabled)
                                {
                                    AnimationManager.SetIsAnimationEnabled(treeViewItem, true);
                                }
                            }
                            childContainer = itemsControl.ItemContainerGenerator.ContainerFromIndex(childIndex) as RadTreeViewItem;
                            if (childContainer != null)
                            {
                                break;
                            }
                            if (treeViewItem != null)
                            {
                                treeViewItem.BringIndexIntoView(childIndex);
                                treeViewItem.InvalidateMeasure();
                                treeViewItem.UpdateLayout();
                            }
                            RadTreeView treeView = itemsControl as RadTreeView;
                            if (treeView != null)
                            {
                                treeView.BringIndexIntoView(childIndex);
                                treeView.UpdateLayout();
                            }
                            childContainer = itemsControl.ItemContainerGenerator.ContainerFromIndex(childIndex) as RadTreeViewItem;
                        }
                        itemsControl = childContainer;
                        itemFound = true;
                        break;
                    }
                    if (itemsControl == null)
                    {
                        break;
                    }
                }
                if (itemFound)
                {
                    return (itemsControl as RadTreeViewItem);
                }
            }
            return null;
        }

        private Binding GetSelectedValuePathBinding()
        {
            if ((this.SelectedItem == null) || string.IsNullOrEmpty(this.SelectedValuePath))
            {
                return null;
            }
            Binding binding = new Binding();
            RadTreeViewItem itemContainer = this.SelectedItem as RadTreeViewItem;
            if (itemContainer != null)
            {
                binding.Source = itemContainer;
            }
            else
            {
                binding.Source = this.SelectedItem;
            }
            binding.Path = new PropertyPath(this.SelectedValuePath, new object[0]);
            return binding;
        }

        private RadTreeViewItem GetTreeViewItemFromDropDestination(FrameworkElement dropDestination)
        {
            RadTreeViewItem result = dropDestination as RadTreeViewItem;
            if ((dropDestination == this) && base.HasItems)
            {
                result = base.ItemContainerGenerator.ContainerFromIndex(base.Items.Count - 1) as RadTreeViewItem;
            }
            return result;
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic"), SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "useTransitions"), SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "stateNames")]
        internal void GoToState(bool useTransitions, params string[] stateNames)
        {
            if (stateNames != null)
            {
                foreach (string str in stateNames)
                {
                    if (VisualStateManager.GoToState(this, str, useTransitions))
                    {
                        break;
                    }
                }
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        internal void HandleAddKey(KeyEventArgs e)
        {
            RadTreeViewItem sourceItem = e.OriginalSource as RadTreeViewItem;
            if ((sourceItem != null) && sourceItem.IsEnabled)
            {
                sourceItem.IsExpanded = true;
                e.Handled = true;
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        internal void HandleDivideKey(KeyEventArgs e)
        {
            RadTreeViewItem sourceItem = e.OriginalSource as RadTreeViewItem;
            if ((sourceItem != null) && sourceItem.IsEnabled)
            {
                sourceItem.CollapseAll();
                e.Handled = true;
            }
        }

        internal void HandleDownArrowKey(KeyEventArgs e)
        {
            if (this.SelectedContainer == null)
            {
                RadTreeViewItem focusedItem = null;
                focusedItem = FocusManager.GetFocusedElement() as RadTreeViewItem;
                if (focusedItem != null)
                {
                    RadTreeViewItem nextItem = focusedItem.NextItem;
                    this.SelectNextItem(nextItem, e);
                }
                else
                {
                    RadTreeViewItem firstItem = base.ItemContainerGenerator.ContainerFromIndex(0) as RadTreeViewItem;
                    if ((firstItem != null) && firstItem.IsEnabled)
                    {
                        if (!this.IsCtrlPressed)
                        {
                            this.SetSelectedItem(firstItem);
                        }
                        firstItem.Focus();
                        e.Handled = true;
                    }
                }
            }
            else
            {
                RadTreeViewItem sourceItem = e.OriginalSource as RadTreeViewItem;
                if (sourceItem == null)
                {
                    sourceItem = this.SelectedContainer;
                }
                RadTreeViewItem nextItem = sourceItem.NextItem;
                this.SelectNextItem(nextItem, e);
            }
        }

        internal bool HandleEndKey()
        {
            if (!base.HasItems)
            {
                return false;
            }
            int lastItemIndex = base.Items.Count - 1;
            this.BringIndexIntoView(lastItemIndex);
            base.UpdateLayout();
            RadTreeViewItem lastItem = base.ItemContainerGenerator.ContainerFromIndex(lastItemIndex) as RadTreeViewItem;
            if (lastItem != null)
            {
                while (((lastItem != null) && lastItem.HasItems) && (lastItem.IsExpanded && lastItem.IsEnabled))
                {
                    int lastChildIndex = lastItem.Items.Count - 1;
                    lastItem.BringIndexIntoView(lastChildIndex);
                    lastItem.UpdateLayout();
                    lastItem = lastItem.ItemContainerGenerator.ContainerFromIndex(lastChildIndex) as RadTreeViewItem;
                }
                if ((lastItem != null) && !lastItem.IsEnabled)
                {
                    lastItem = lastItem.PreviousItem;
                    while ((lastItem != null) && !lastItem.IsEnabled)
                    {
                        lastItem = lastItem.PreviousItem;
                    }
                }
            }
            if (lastItem == null)
            {
                return false;
            }
            if (!this.IsCtrlPressed)
            {
                this.HandleItemSelectionFromUI(lastItem);
            }
            lastItem.Focus();
            return true;
        }

        internal static void HandleEnterKey(KeyEventArgs e)
        {
            RadTreeViewItem sourceItem = e.OriginalSource as RadTreeViewItem;
            if ((sourceItem != null) && sourceItem.IsEnabled)
            {
                sourceItem.IsExpanded = !sourceItem.IsExpanded;
                e.Handled = true;
            }
        }

        internal virtual void HandleF2Key(KeyEventArgs e)
        {
            RadTreeViewItem selectedContainer = this.SelectedContainer;
            if (((selectedContainer != null) && !this.IsEditing) && (selectedContainer.IsEnabled && (e.OriginalSource == selectedContainer)))
            {
                e.Handled = selectedContainer.BeginEdit();
            }
        }

        internal bool HandleHomeKey()
        {
            if (base.HasItems)
            {
                RadTreeViewItem firstItem = null;
                for (int retryCount = 0; retryCount < 10; retryCount++)
                {
                    if (this.ScrollViewer != null)
                    {
                        this.BringIndexIntoView(0);
                    }
                    base.UpdateLayout();
                    firstItem = base.ItemContainerGenerator.ContainerFromIndex(0) as RadTreeViewItem;
                    if (firstItem != null)
                    {
                        break;
                    }
                }
                if (firstItem != null)
                {
                    if (!firstItem.IsEnabled)
                    {
                        firstItem = firstItem.NextItem;
                        while ((firstItem != null) && !firstItem.IsEnabled)
                        {
                            firstItem = firstItem.NextItem;
                        }
                    }
                    if (firstItem != null)
                    {
                        if (!this.IsCtrlPressed)
                        {
                            this.HandleItemSelectionFromUI(firstItem);
                        }
                        firstItem.Focus();
                        return true;
                    }
                }
            }
            return false;
        }

        internal void HandleItemSelectionFromUI(RadTreeViewItem treeViewItemToSelect)
        {
            if (treeViewItemToSelect.IsEnabled)
            {
                if ((this.CurrentEditedItem != null) && !this.CurrentEditedItem.CommitEdit())
                {
                    this.CurrentEditedItem.CancelEdit();
                }
                bool shiftKeyPressed = (System.Windows.Input.Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;
                if (((this.SelectionMode != Telerik.Windows.Controls.SelectionMode.Single) && (treeViewItemToSelect != null)) && (this.IsCtrlPressed || shiftKeyPressed))
                {
                    if (this.IsCtrlPressed)
                    {
                        object dataItem = treeViewItemToSelect.Item;
                        if (!this.selectionChanger.Contains(dataItem))
                        {
                            this.selectionChanger.Insert(0, dataItem);
                        }
                        else
                        {
                            this.selectionChanger.Remove(dataItem);
                        }
                    }
                    else if (shiftKeyPressed)
                    {
                        RadTreeViewItem startTreeViewItem;
                        this.selectionChanger.Begin();
                        if (this.selectionChanger.Count == 0)
                        {
                            startTreeViewItem = this.SelectedContainer;
                        }
                        else
                        {
                            startTreeViewItem = this.selectionChanger[0] as RadTreeViewItem;
                            if (startTreeViewItem == null)
                            {
                                startTreeViewItem = this.ContainerFromItemRecursive(this.selectionChanger[0]);
                            }
                            this.ClearSelectionChanger(null);
                        }
                        if (startTreeViewItem == null)
                        {
                            this.selectionChanger.End();
                        }
                        else
                        {
                            object startItem = startTreeViewItem.Item;
                            Telerik.Windows.Controls.ItemsControl startItemOwner = startTreeViewItem.Owner;
                            object itemToSelect = treeViewItemToSelect.Item;
                            ItemTraversal traversalMethod = startTreeViewItem.IsBefore(treeViewItemToSelect) ? new ItemTraversal(this.FindNextItem) : new ItemTraversal(this.FindPreviousItem);
                            this.selectionChanger.Add(startItem);
                            while ((startItem != null) && (startItem != itemToSelect))
                            {
                                traversalMethod(ref startTreeViewItem, ref startItem, ref startItemOwner);
                                if ((startItem != null) && this.IsItemEnabled(startTreeViewItem, startItem))
                                {
                                    this.selectionChanger.Add(startItem);
                                }
                            }
                            this.selectionChanger.End();
                        }
                    }
                }
                else
                {
                    this.SetSelectedItem(treeViewItemToSelect);
                }
            }
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        internal void HandleKeyDown(KeyEventArgs e)
        {
            if (this.CurrentEditedItem == null)
            {
                switch (e.Key)
                {
                    case Key.Enter:
                        HandleEnterKey(e);
                        return;

                    case Key.Shift:
                    case Key.Ctrl:
                    case Key.Alt:
                    case Key.CapsLock:
                    case Key.Escape:
                    case Key.Decimal:
                        return;

                    case Key.Space:
                        this.HandleSpaceKey(e);
                        return;

                    case Key.PageUp:
                        e.Handled = this.HandlePageUp(e.OriginalSource);
                        return;

                    case Key.PageDown:
                        e.Handled = this.HandlePageDown(e.OriginalSource);
                        return;

                    case Key.End:
                        e.Handled = this.HandleEndKey();
                        return;

                    case Key.Home:
                        e.Handled = this.HandleHomeKey();
                        return;

                    case Key.Left:
                        this.HandleLeftArrowKey(e);
                        return;

                    case Key.Up:
                        this.HandleUpArrowKey(e);
                        return;

                    case Key.Right:
                        this.HandleRightArrowKey(e);
                        return;

                    case Key.Down:
                        this.HandleDownArrowKey(e);
                        return;

                    case Key.F2:
                        this.HandleF2Key(e);
                        return;

                    case Key.Multiply:
                        this.HandleMultiplyKey(e);
                        return;

                    case Key.Add:
                        this.HandleAddKey(e);
                        return;

                    case Key.Subtract:
                        this.HandleSubtractKey(e);
                        return;

                    case Key.Divide:
                        this.HandleDivideKey(e);
                        return;
                }
            }
        }

        internal void HandleLeftArrowKey(KeyEventArgs e)
        {
            RadTreeViewItem sourceItem = e.OriginalSource as RadTreeViewItem;
            if (sourceItem != null)
            {
                if (!sourceItem.IsExpanded)
                {
                    if ((sourceItem.ParentItem != null) && sourceItem.ParentItem.IsEnabled)
                    {
                        if (!this.IsCtrlPressed)
                        {
                            this.HandleItemSelectionFromUI(sourceItem.ParentItem);
                        }
                        sourceItem.ParentItem.Focus();
                        e.Handled = true;
                    }
                }
                else if (sourceItem.Items.Count > 0)
                {
                    sourceItem.IsExpanded = false;
                    e.Handled = true;
                }
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        internal void HandleMultiplyKey(KeyEventArgs e)
        {
            RadTreeViewItem sourceItem = e.OriginalSource as RadTreeViewItem;
            if ((sourceItem != null) && sourceItem.IsEnabled)
            {
                sourceItem.ExpandAll();
                e.Handled = true;
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "TransformToVisual throws a general exceotion")]
        internal bool HandlePageDown(object originalSource)
        {
            if (this.ScrollViewer != null)
            {
                double availHeight = this.ScrollViewer.ViewportHeight;
                RadTreeViewItem item1 = originalSource as RadTreeViewItem;
                if (item1 != null)
                {
                    if ((this.ItemsHost != null) && this.IsVirtualizing)
                    {
                        try
                        {
                            double topOffset = item1.TransformToVisual(this.ItemsHost).Transform(new Point()).Y + this.ScrollViewer.VerticalOffset;
                            this.ScrollViewer.ScrollToVerticalOffset(topOffset);
                            this.ScrollViewer.UpdateLayout();
                        }
                        catch
                        {
                            return false;
                        }
                    }
                    RadTreeViewItem item2 = item1.NextItem;
                    while ((item2 != null) && (availHeight > (2.0 * item2.HeaderHeight)))
                    {
                        if (item2.NextItem == null)
                        {
                            break;
                        }
                        availHeight -= item2.HeaderHeight;
                        item2 = item2.NextItem;
                    }
                    if (item2 != null)
                    {
                        if (!item2.IsEnabled)
                        {
                            while ((item2 != null) && !item2.IsEnabled)
                            {
                                item2 = item2.NextItem;
                            }
                        }
                        if (item2 != null)
                        {
                            if (!this.IsCtrlPressed)
                            {
                                this.HandleItemSelectionFromUI(item2);
                            }
                            item2.Focus();
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "TransformToVisual throws a general exception")]
        internal bool HandlePageUp(object originalSource)
        {
            if (this.ScrollViewer != null)
            {
                double availHeight = this.ScrollViewer.ViewportHeight;
                RadTreeViewItem item1 = originalSource as RadTreeViewItem;
                if (item1 != null)
                {
                    if ((this.ItemsHost != null) && this.IsVirtualizing)
                    {
                        try
                        {
                            double topOffset = item1.TransformToVisual(this.ItemsHost).Transform(new Point()).Y + ((this.ScrollViewer.VerticalOffset - this.ScrollViewer.ViewportHeight) + item1.HeaderHeight);
                            this.ScrollViewer.ScrollToVerticalOffset(topOffset);
                            this.ScrollViewer.UpdateLayout();
                        }
                        catch
                        {
                            return false;
                        }
                    }
                    RadTreeViewItem item2 = item1;
                    while ((item2 != null) && (availHeight > item2.HeaderHeight))
                    {
                        if (item2.PreviousItem == null)
                        {
                            break;
                        }
                        availHeight -= item2.HeaderHeight;
                        item2 = item2.PreviousItem;
                    }
                    if (item2 != null)
                    {
                        if (!item2.IsEnabled)
                        {
                            while ((item2 != null) && !item2.IsEnabled)
                            {
                                item2 = item2.PreviousItem;
                            }
                        }
                        if (item2 != null)
                        {
                            if (!this.IsCtrlPressed)
                            {
                                this.HandleItemSelectionFromUI(item2);
                            }
                            item2.Focus();
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        internal void HandleRightArrowKey(KeyEventArgs e)
        {
            RadTreeViewItem sourceItem = e.OriginalSource as RadTreeViewItem;
            if (sourceItem != null)
            {
                if (sourceItem.IsExpanded)
                {
                    RadTreeViewItem nextItem = sourceItem.NextItem;
                    while ((nextItem != null) && !nextItem.IsEnabled)
                    {
                        nextItem = nextItem.NextItem;
                    }
                    if (nextItem != null)
                    {
                        if (!this.IsCtrlPressed)
                        {
                            this.HandleItemSelectionFromUI(nextItem);
                        }
                        nextItem.Focus();
                        e.Handled = true;
                    }
                }
                else if (sourceItem.IsEnabled && ((sourceItem.Items.Count > 0) || sourceItem.IsLoadOnDemandEnabled))
                {
                    sourceItem.IsExpanded = true;
                    e.Handled = true;
                }
            }
        }

        private void HandleSpaceKey(KeyEventArgs e)
        {
            RadTreeViewItem sourceItem = e.OriginalSource as RadTreeViewItem;
            if (sourceItem != null)
            {
                if (this.IsOptionElementsEnabled)
                {
                    if (sourceItem.IsCheckBoxEnabled)
                    {
                        sourceItem.CheckState = (sourceItem.CheckState != ToggleState.Off) ? ToggleState.Off : ToggleState.On;
                        e.Handled = true;
                    }
                    else if (sourceItem.IsRadioButtonEnabled)
                    {
                        sourceItem.CheckState = ToggleState.On;
                        e.Handled = true;
                    }
                }
                else
                {
                    sourceItem.IsSelected = !sourceItem.IsSelected;
                    e.Handled = true;
                }
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        internal void HandleSubtractKey(KeyEventArgs e)
        {
            RadTreeViewItem sourceItem = e.OriginalSource as RadTreeViewItem;
            if ((sourceItem != null) && sourceItem.IsEnabled)
            {
                sourceItem.IsExpanded = false;
                e.Handled = true;
            }
        }

        internal void HandleUpArrowKey(KeyEventArgs e)
        {
            if (this.SelectedContainer == null)
            {
                RadTreeViewItem focusedItem = null;
                focusedItem = FocusManager.GetFocusedElement() as RadTreeViewItem;
                if (focusedItem != null)
                {
                    RadTreeViewItem prevItem = focusedItem.PreviousItem;
                    this.SelectPreviousItem(prevItem, e);
                }
                else
                {
                    RadTreeViewItem lastItem = base.ItemContainerGenerator.ContainerFromIndex(base.Items.Count - 1) as RadTreeViewItem;
                    if ((lastItem != null) && lastItem.IsEnabled)
                    {
                        if (!this.IsCtrlPressed)
                        {
                            this.SetSelectedItem(lastItem);
                        }
                        lastItem.Focus();
                        e.Handled = true;
                    }
                }
            }
            else
            {
                RadTreeViewItem sourceItem = e.OriginalSource as RadTreeViewItem;
                if (sourceItem == null)
                {
                    sourceItem = this.SelectedContainer;
                }
                RadTreeViewItem prevItem = sourceItem.PreviousItem;
                this.SelectPreviousItem(prevItem, e);
            }
        }

        public void HideBetweenItemsDragCue()
        {
            if (this.dragBetweenItemsFeedback != null)
            {
                this.dragBetweenItemsFeedback.Visibility = Visibility.Collapsed;
            }
        }

        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "d")]
        private void ImagesBaseDirChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            string s = (string)e.NewValue;
            if ((!s.EndsWith("/", StringComparison.OrdinalIgnoreCase) && !s.EndsWith(@"\", StringComparison.OrdinalIgnoreCase)) && !string.IsNullOrEmpty(s))
            {
                s = s + "/";
            }
            this.ImagesBaseDir = s;
        }

        internal ICollection<object> InitDrag(object item)
        {
            if (!this.IsDragDropEnabled)
            {
                return null;
            }
            this.cancelDragging = false;
            Collection<object> draggedItems = new Collection<object>();
            if (this.selectionChanger.Contains(item))
            {
                this.AddNonHierarchicalSelectedItems(draggedItems);
            }
            else
            {
                draggedItems.Add(item);
            }
            if (this.OnPreviewDragStarted(new RadTreeViewDragEventArgs(draggedItems, PreviewDragStartedEvent, this)))
            {
                draggedItems.Clear();
                return null;
            }
            this.OnDragStarted(new RadTreeViewDragEventArgs(draggedItems, DragStartedEvent, this));
            return draggedItems;
        }

        private void InitializeComponent()
        {
            this.BindXAMLElements();
            this.ChangeVisualState(false);
        }

        private bool IsItemEnabled(RadTreeViewItem treeViewItem, object item)
        {
            if (treeViewItem != null)
            {
                return treeViewItem.IsEnabled;
            }
            return ((item == null) || this.itemStorage.GetValue<bool>(item, Control.IsEnabledProperty, true));
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return IsItemTreeviewItemContainer(item);
        }

        internal static bool IsItemTreeviewItemContainer(object item)
        {
            return (item is RadTreeViewItem);
        }

        private bool IsItemValidForDeselection(object item)
        {
            RadRoutedEventArgs routedArgs = new RadRoutedEventArgs(RadTreeViewItem.PreviewUnselectedEvent, item);
            RadTreeViewItem container = (item as RadTreeViewItem) ?? this.ContainerFromItemRecursive(item);
            if (container != null)
            {
                return !container.OnPreviewUnselected(routedArgs);
            }
            this.RaiseEvent(routedArgs);
            return !routedArgs.Handled;
        }

        private bool IsItemValidForSelection(object item)
        {
            RadRoutedEventArgs routedArgs = new RadRoutedEventArgs(RadTreeViewItem.PreviewSelectedEvent, item);
            RadTreeViewItem container = (item as RadTreeViewItem) ?? this.ContainerFromItemRecursive(item);
            if (container != null)
            {
                return !container.OnPreviewSelected(routedArgs);
            }
            this.RaiseEvent(routedArgs);
            return !routedArgs.Handled;
        }

        private bool IsSelectionChangePossible(System.Windows.Controls.SelectionChangedEventArgs e)
        {
            Telerik.Windows.Controls.SelectionChangedEventArgs routedArgs = new Telerik.Windows.Controls.SelectionChangedEventArgs(PreviewSelectionChangedEvent, e.RemovedItems, e.AddedItems);
            return this.OnPreviewSelectionChanged(routedArgs);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.InitializeComponent();
            this.CalculateInitialCheckState();
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new RadTreeViewAutomationPeer(this);
        }

        internal void OnDescendantRemoved(object descendant)
        {
            this.StoreCheckState(null, descendant, ToggleState.Off);
        }

        protected internal virtual void OnDragEnded(RadTreeViewDragEndedEventArgs e)
        {
            this.RaiseEvent(e);
        }

        protected internal virtual void OnDragStarted(RadTreeViewDragEventArgs e)
        {
            this.RaiseEvent(e);
        }

        private void OnDropComplete(FrameworkElement dropDestination, IEnumerable draggedItems)
        {
            this.HideBetweenItemsDragCue();
            this.isDropPossible = false;
            this.ChangeVisualState();
            RadTreeView treeViewDestination = dropDestination as RadTreeView;
            if ((treeViewDestination != null) && treeViewDestination.HasItems)
            {
                RadTreeViewItem firstItemContainer = treeViewDestination.ItemContainerGenerator.ContainerFromIndex(treeViewDestination.Items.Count - 1) as RadTreeViewItem;
                this.EndDrop(firstItemContainer, draggedItems);
            }
            else
            {
                this.EndDrop(dropDestination, draggedItems);
            }
        }

        private void OnDropImpossible(DragDropOptions options)
        {
            TreeViewDragCue treeCue = options.DragCue as TreeViewDragCue;
            if (treeCue != null)
            {
                treeCue.IsDropPossible = false;
                treeCue.DragActionContent = null;
                treeCue.DragActionContentTemplate = null;
                treeCue.DragTooltipContent = null;
                treeCue.DragTooltipContentTemplate = null;
            }
            this.HideBetweenItemsDragCue();
            this.isDropPossible = false;
            this.UpdateExpandTimer(options.Destination, false);
            this.ChangeVisualState();
        }

        private void OnDropPossible(FrameworkElement dropDestination, TreeViewDragCue treeCue)
        {
            this.isDropPossible = true;
            this.ChangeVisualState();
            this.UpdateExpandTimer(dropDestination, true);
            RadTreeViewItem destinationItem = this.GetTreeViewItemFromDropDestination(dropDestination);
            if (destinationItem != null)
            {
                this.UpdateBetweenItemsDragCue(destinationItem);
            }
            if (treeCue != null)
            {
                treeCue.IsDropPossible = true;
                this.UpdateTreeCueForDestinationItem(treeCue, destinationItem);
            }
        }

        private static void OnIsLineEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadTreeView treeView = d as RadTreeView;
            treeView.UpdateChildItemsLine(treeView);
        }

        internal virtual void OnIsOptionElementsEnabledChanged(bool oldValue, bool newValue)
        {
            if (this.ItemsOptionListType == OptionListType.Default)
            {
                this.ItemsOptionListType = OptionListType.CheckList;
            }
            foreach (RadTreeViewItem item in this.GetAllItemContainers(this))
            {
                switch (((item.ParentItem != null) ? item.ParentItem.ItemsOptionListType : item.ParentTreeView.ItemsOptionListType))
                {
                    case OptionListType.CheckList:
                        {
                            item.IsCheckBoxEnabled = newValue;
                            continue;
                        }
                    case OptionListType.OptionList:
                        {
                            item.IsRadioButtonEnabled = newValue;
                            continue;
                        }
                }
                item.IsCheckBoxEnabled = newValue;
            }
        }

        private static void OnIsOptionElementsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as RadTreeView).OnIsOptionElementsEnabledChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        private static void OnIsRootLinesEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadTreeView treeView = d as RadTreeView;
            if (treeView != null)
            {
                treeView.UpdateChildItemsLine(treeView);
            }
        }

        private static void OnIsTriStateModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadTreeView treeView = d as RadTreeView;
            if (treeView.IsOptionElementsEnabled)
            {
                if (!treeView.IsTriStateMode)
                {
                    treeView.ForEachContainerItem<RadTreeViewItem>(delegate(RadTreeViewItem itemContainer)
                    {
                        treeView.ClearItemIndeterminatedState(itemContainer);
                    });
                }
                else
                {
                    treeView.ForEachContainerItem<RadTreeViewItem>(delegate(RadTreeViewItem itemContainer)
                    {
                        treeView.UpdateItemIndeterminatedState(itemContainer);
                    });
                }
            }
        }

        protected internal virtual void OnItemPrepared(RadTreeViewItemPreparedEventArgs e)
        {
            if (this.ItemPrepared != null)
            {
                this.ItemPrepared(this, e);
            }
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            RadTreeViewItem container;
            base.OnItemsChanged(e);
            if (this.IsLineEnabled)
            {
                this.UpdateChildItemsLine(this);
            }
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    container = e.NewItems[0] as RadTreeViewItem;
                    if (container == null)
                    {
                        break;
                    }
                    RadTreeViewItem.CalculateInitialItemCheckState(container);
                    return;

                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Replace:
                    if (this.SelectedItems.Contains(e.OldItems[0]))
                    {
                        this.SelectedItems.Remove(e.OldItems[0]);
                    }
                    container = e.OldItems[0] as RadTreeViewItem;
                    if (container != null)
                    {
                        RadTreeViewItem.InvalidateRender(container);
                    }
                    this.OnDescendantRemoved(e.OldItems[0]);
                    return;

                case (NotifyCollectionChangedAction.Replace | NotifyCollectionChangedAction.Remove):
                    break;

                case NotifyCollectionChangedAction.Reset:
                    this.OnItemsReset();
                    break;

                default:
                    return;
            }
        }

        private static void OnItemsOptionListTypePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            (d as RadTreeView).ForEachContainerItem<RadTreeViewItem>(delegate(RadTreeViewItem itemContainer)
            {
                if (itemContainer.OptionType == ((OptionListType)args.OldValue))
                {
                    itemContainer.OptionType = (OptionListType)args.NewValue;
                }
                itemContainer.ParentTreeView.SetItemOptionElementType(itemContainer);
            });
        }

        private void OnItemsReset()
        {
            if (this.IsEditing)
            {
                this.CurrentEditedItem.CancelEdit();
                this.CurrentEditedItem = null;
            }
            if (base.ItemsSource == null)
            {
                this.SelectedItems.Clear();
                if (this.itemToggleStateStorage != null)
                {
                    this.itemToggleStateStorage.Clear();
                }
                if (this.CheckedItems != null)
                {
                    (this.CheckedItems as CheckedItemsCollection).NotifyCountChanged();
                }
                if (this.IsVirtualizing)
                {
                    this.generatedItemCache.Clear();
                    this.ItemStorage.Clear();
                }
                else
                {
                    this.CalculateInitialCheckState();
                }
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.CopyVirtualizingStackPanelPropertiesIfSet();
        }

        [SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers", Justification = "This a pattern for events and event handlers.")]
        protected virtual void OnLostFocus(object sender, RoutedEventArgs e)
        {
            if (Application.Current.RootVisual != null)
            {
                Application.Current.RootVisual.MouseWheel -= new MouseWheelEventHandler(this.OnRootVisualMouseWheel);
            }
            DependencyObject focusedElement = FocusManager.GetFocusedElement() as DependencyObject;
            if ((focusedElement == null) || (focusedElement.GetParent<RadTreeView>() != this))
            {
                this.IsSelectionActive = false;
                if ((this.CurrentEditedItem != null) && !this.CurrentEditedItem.CommitEdit())
                {
                    this.CurrentEditedItem.CancelEdit();
                }
            }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (base.IsEnabled)
            {
                DependencyObject focused = FocusManager.GetFocusedElement() as DependencyObject;
                if ((focused != null) && !this.IsAncestorOf(focused))
                {
                    base.Focus();
                }
                if (this.CurrentEditedItem != null)
                {
                    this.CurrentEditedItem.CommitEdit();
                    base.Focus();
                }
                e.Handled = true;
            }
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            this.OnRootVisualMouseWheel(null, e);
        }

        protected internal virtual bool OnPreviewDragEnded(RadTreeViewDragEndedEventArgs e)
        {
            this.RaiseEvent(e);
            return e.Handled;
        }

        protected internal virtual bool OnPreviewDragStarted(RadTreeViewDragEventArgs e)
        {
            this.RaiseEvent(e);
            return e.Handled;
        }

        protected virtual bool OnPreviewSelectionChanged(Telerik.Windows.Controls.SelectionChangedEventArgs e)
        {
            this.RaiseEvent(e);
            return e.Handled;
        }

        private void OnRootVisualMouseWheel(object sender, MouseWheelEventArgs e)
        {
            int scrollDelta = Math.Abs(e.Delta) / 3;
            if (this.ScrollViewer != null)
            {
                double previousVerticalOffset = this.ScrollViewer.VerticalOffset;
                if (e.Delta > 0)
                {
                    this.ScrollViewer.ScrollToVerticalOffset(this.ScrollViewer.VerticalOffset - scrollDelta);
                }
                else
                {
                    this.ScrollViewer.ScrollToVerticalOffset(this.ScrollViewer.VerticalOffset + scrollDelta);
                }
                if (previousVerticalOffset != this.ScrollViewer.VerticalOffset)
                {
                    e.Handled = true;
                }
            }
        }

        private static void OnSelectedItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            RadTreeView treeView = sender as RadTreeView;
            if (!treeView.justChangeSelectedItem)
            {
                object newSelection = e.NewValue;
                if (newSelection == null)
                {
                    treeView.selectionChanger.Clear();
                    treeView.SelectedContainer = null;
                }
                else
                {
                    if (!treeView.selectionChanger.Contains(newSelection))
                    {
                        treeView.selectionChanger.Insert(0, newSelection);
                    }
                    if (treeView.selectionChanger.Contains(newSelection))
                    {
                        treeView.SelectedContainer = treeView.ContainerFromItemRecursive(newSelection);
                    }
                    else
                    {
                        treeView.justChangeSelectedItem = true;
                        try
                        {
                            treeView.SelectedItem = null;
                        }
                        finally
                        {
                            treeView.justChangeSelectedItem = false;
                        }
                    }
                }
                treeView.UpdateSelectedValue();
            }
        }

        private static void OnSelectedValuePathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadTreeView treeview = (RadTreeView)d;
            treeview.SelectedValue = null;
            treeview.UpdateSelectedValue();
        }

        protected virtual void OnSelectionChanged(Telerik.Windows.Controls.SelectionChangedEventArgs e)
        {
            this.RaiseEvent(e);
        }

        private void OnSelectionChangerSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            foreach (object item in e.RemovedItems)
            {
                this.UnselectItem(item);
            }
            if (e.AddedItems.Count > 0)
            {
                if (this.SelectionMode == Telerik.Windows.Controls.SelectionMode.Single)
                {
                    Collection<object> itemsToRemove = new Collection<object>();
                    foreach (object item in this.selectionChanger)
                    {
                        if (item != e.AddedItems[0])
                        {
                            itemsToRemove.Add(item);
                        }
                    }
                    foreach (object item in itemsToRemove)
                    {
                        this.selectionChanger.Remove(item);
                    }
                    this.UpdateSelectedItemProperty();
                    RadTreeViewItem containerItem = e.AddedItems[0] as RadTreeViewItem;
                    if (containerItem == null)
                    {
                        containerItem = this.ContainerFromItemRecursive(e.AddedItems[0]);
                    }
                    if (containerItem != null)
                    {
                        containerItem.IsSelected = true;
                    }
                    else if (this.IsVirtualizing)
                    {
                        this.ItemStorage.SetValue(e.AddedItems[0], RadTreeViewItem.IsSelectedProperty, true);
                    }
                }
                else
                {
                    this.UpdateSelectedItemProperty();
                    foreach (object item in e.AddedItems)
                    {
                        this.SelectItem(item);
                    }
                }
            }
            else
            {
                this.UpdateSelectedItemProperty();
            }
            if ((this.SelectedContainer != null) && AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementSelected))
            {
                AutomationPeer peer = FrameworkElementAutomationPeer.CreatePeerForElement(this.SelectedContainer);
                if (peer != null)
                {
                    peer.RaiseAutomationEvent(AutomationEvents.SelectionItemPatternOnElementSelected);
                }
            }
            if ((this.SelectedContainer != null) && AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementRemovedFromSelection))
            {
                AutomationPeer peer2 = FrameworkElementAutomationPeer.CreatePeerForElement(this.SelectedContainer);
                if (peer2 != null)
                {
                    peer2.RaiseAutomationEvent(AutomationEvents.SelectionItemPatternOnElementRemovedFromSelection);
                }
            }
            if ((e.AddedItems.Count > 0) || (e.RemovedItems.Count > 0))
            {
                this.OnSelectionChanged(new Telerik.Windows.Controls.SelectionChangedEventArgs(SelectionChangedEvent, e.RemovedItems, e.AddedItems));
            }
        }

        private static void OnSelectionModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadTreeView treeView = d as RadTreeView;
            if ((((Telerik.Windows.Controls.SelectionMode)e.NewValue) == Telerik.Windows.Controls.SelectionMode.Single) && (treeView.SelectedItems.Count > 1))
            {
                treeView.SetSelectedItem(treeView.SelectedContainer);
            }
        }

        private static void OnTreeViewDragInfo(object sender, DragDropEventArgs e)
        {
            RadTreeView treeView = sender as RadTreeView;
            RadTreeViewItem treeViewItem = e.Options.Source as RadTreeViewItem;
            if (treeView.IsDragDropEnabled && (treeViewItem != null))
            {
                if (e.Options.Status == DragStatus.DragInProgress)
                {
                    treeView.StartDrag(e.Options.Payload as IEnumerable, e.Options);
                }
                else if (e.Options.Status == DragStatus.DragCancel)
                {
                    treeView.CancelDrag();
                }
                else if (e.Options.Status == DragStatus.DragComplete)
                {
                    treeView.EndDrag(e.Options.Destination, e.Options.Payload as IEnumerable);
                    treeView.Dispatcher.BeginInvoke(delegate
                    {
                        treeView.PreviewDragEndResult = null;
                    });
                }
                else if (e.Options.Status == DragStatus.DropImpossible)
                {
                    treeView.HideBetweenItemsDragCue();
                }
            }
        }

        private static void OnTreeViewDragQuery(object sender, DragDropQueryEventArgs e)
        {
            RadTreeView treeView = sender as RadTreeView;
            RadTreeViewItem treeViewItem = e.Options.Source as RadTreeViewItem;
            if ((treeView.IsDragDropEnabled && (treeViewItem != null)) && !treeViewItem.IsInEditMode)
            {
                if (e.Options.Status == DragStatus.DragQuery)
                {
                    ICollection<object> result = treeView.InitDrag(treeViewItem.Item);
                    e.Options.Payload = result;
                    e.QueryResult = new bool?(result != null);
                    e.Handled = true;
                }
                else if (e.Options.Status == DragStatus.DropSourceQuery)
                {
                    e.QueryResult = true;
                    e.Handled = true;
                }
            }
        }

        private static void OnTreeViewDropInfo(object sender, DragDropEventArgs e)
        {
            RadTreeView treeView = sender as RadTreeView;
            RadTreeViewItem treeViewItem = e.Options.Destination as RadTreeViewItem;
            if (treeView.IsDragDropEnabled)
            {
                if (e.Options.Status == DragStatus.DropComplete)
                {
                    treeView.OnDropComplete(e.Options.Destination, e.Options.Payload as IEnumerable);
                }
                else if (e.Options.Status == DragStatus.DropImpossible)
                {
                    treeView.OnDropImpossible(e.Options);
                }
                else if (e.Options.Status == DragStatus.DropPossible)
                {
                    treeView.OnDropPossible(e.Options.Destination, e.Options.DragCue as TreeViewDragCue);
                }
                if (treeViewItem != null)
                {
                    treeViewItem.IsDragOver = e.Options.Status == DragStatus.DropPossible;
                }
            }
        }

        private static void OnTreeViewDropQuery(object sender, DragDropQueryEventArgs e)
        {
            RadTreeView treeView = sender as RadTreeView;
            if (((treeView != null) && treeView.IsDragDropEnabled) && (e.Options.Status == DragStatus.DropDestinationQuery))
            {
                if (treeView.HasItems)
                {
                    RadTreeViewItem treeViewItem = treeView.ItemContainerGenerator.ContainerFromIndex(treeView.Items.Count - 1) as RadTreeViewItem;
                    IEnumerable dropCollection = e.Options.Payload as IEnumerable;
                    if (treeViewItem != null)
                    {
                        treeViewItem.DropPosition = DropPosition.After;
                        e.QueryResult = new bool?(treeViewItem.IsDropPossible(dropCollection));
                    }
                    e.Handled = true;
                }
                else
                {
                    e.QueryResult = true;
                    e.Handled = true;
                }
            }
        }

        [SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers", Justification = "This a pattern for events and event handlers.")]
        protected virtual void OnTreeViewGotFocus(object sender, RoutedEventArgs e)
        {
            if (Application.Current.RootVisual != null)
            {
                Application.Current.RootVisual.MouseWheel += new MouseWheelEventHandler(this.OnRootVisualMouseWheel);
            }
            if (e.OriginalSource == this)
            {
                if (this.SelectedContainer != null)
                {
                    this.SelectedContainer.Focus();
                }
                else
                {
                    RadTreeViewItem firstItem = base.ItemContainerGenerator.ContainerFromIndex(0) as RadTreeViewItem;
                    if (firstItem != null)
                    {
                        firstItem.Focus();
                    }
                }
            }
            this.IsSelectionActive = true;
        }

        private static void OnTreeViewItemDropQuery(object sender, DragDropQueryEventArgs e)
        {
            RadTreeViewItem treeViewItem = sender as RadTreeViewItem;
            IEnumerable dropCollection = e.Options.Payload as IEnumerable;
            if (((treeViewItem.ParentTreeView != null) && treeViewItem.ParentTreeView.IsDragDropEnabled) && (e.Options.Status == DragStatus.DropDestinationQuery))
            {
                DropPosition calculatedDropPosition = treeViewItem.GetDropPositionFromPoint(e.Options.CurrentDragPoint);
                treeViewItem.DropPosition = calculatedDropPosition;
                e.QueryResult = new bool?(treeViewItem.IsDropPossible(dropCollection));
                e.Handled = true;
            }
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            this.RemoveHandler(RadContextMenu.OpenedEvent, new RoutedEventHandler(this.ContextMenuOpened));
            this.RemoveHandler(RadContextMenu.ClosedEvent, new RoutedEventHandler(this.ContextMenuClosed));
        }

        internal virtual void PrepareContainerForDescendant(DependencyObject element, object item, Telerik.Windows.Controls.ItemsControl owner)
        {
            RadTreeViewItem treeViewItem = element as RadTreeViewItem;
            treeViewItem.ParentTreeView = this;
            treeViewItem.Owner = owner;
            RadTreeViewItem ownerAsTreeViewItem = owner as RadTreeViewItem;
            if (ownerAsTreeViewItem != null)
            {
                treeViewItem.ParentItem = ownerAsTreeViewItem;
                treeViewItem.Level = treeViewItem.ParentItem.Level + 1;
            }
            treeViewItem.Item = item;
            if (treeViewItem.IsSelected)
            {
                this.selectionChanger.Add(item);
            }
            if (this.SelectedItem == item)
            {
                this.SelectedContainer = treeViewItem;
            }
            if (this.selectionChanger.Contains(item))
            {
                try
                {
                    treeViewItem.SupressEventRaising = true;
                    treeViewItem.IsSelected = true;
                }
                finally
                {
                    treeViewItem.SupressEventRaising = false;
                }
            }
            if (!treeViewItem.IsLoadOnDemandEnabled)
            {
                treeViewItem.SetIsLoadOnDemandBinding();
            }
            this.PropagateEditProperties(treeViewItem);
            this.PropagateDragDropProperties(element);
            this.PrepareItemsFromAnotherTreeView(treeViewItem);
            StyleManager.SetThemeFromParent(treeViewItem, this);
            this.RestoreContainerProperties(item, treeViewItem);
            if (this.IsOptionElementsEnabled && (treeViewItem.OptionType == OptionListType.Default))
            {
                this.SetItemOptionElementType(treeViewItem);
            }
            this.RestoreAndPropagateCheckStateProperties(item, treeViewItem);
            treeViewItem.Render();
            if (this.IsVirtualizing)
            {
                this.generatedItemCache[item] = new WeakReference(treeViewItem);
            }
            TreeViewPanel.SetVirtualizationMode(element, TreeViewPanel.GetVirtualizationMode(this));
            TreeViewPanel.SetIsVirtualizing(element, TreeViewPanel.GetIsVirtualizing(this));
            treeViewItem.ChangeVisualState(false);
            if (treeViewItem.ItemsHost != null)
            {
                treeViewItem.IsRenderPending = true;
                treeViewItem.ItemsHost.InvalidateMeasure();
                treeViewItem.ItemsHost.InvalidateArrange();
            }
            this.OnItemPrepared(new RadTreeViewItemPreparedEventArgs(treeViewItem));
            this.PropagateDelayedExpansion(treeViewItem);
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            if (this.itemsHost == null)
            {
                this.itemsHost = VisualTreeHelper.GetParent(element) as TreeViewPanel;
            }
            base.PrepareContainerForItemOverride(element, item);
            this.PrepareContainerForDescendant(element, item, this);
        }

        private void PrepareItemsFromAnotherTreeView(RadTreeViewItem treeViewItem)
        {
            if (treeViewItem.HasItems)
            {
                foreach (object subItem in treeViewItem.Items)
                {
                    RadTreeViewItem treeViewSubItem = subItem as RadTreeViewItem;
                    if (((treeViewSubItem != null) && (treeViewSubItem.ParentTreeView != null)) && (treeViewSubItem.ParentTreeView != this))
                    {
                        this.PrepareContainerForDescendant(treeViewSubItem, treeViewSubItem.Item, treeViewItem);
                    }
                }
            }
        }

        private void PropagateDelayedExpansion(RadTreeViewItem treeViewItem)
        {
            if (((treeViewItem != null) && (treeViewItem.Item != null)) && (((treeViewItem.ParentItem != null) && treeViewItem.ParentItem.IsExpandAllPending) || this.itemStorage.GetValue<bool>(treeViewItem.Item, RadTreeViewItem.IsExpandAllPendingProperty, false)))
            {
                treeViewItem.ExpandAll();
                this.itemStorage.ClearValue(treeViewItem.Item, RadTreeViewItem.IsExpandAllPendingProperty);
            }
        }

        private void PropagateDragDropProperties(DependencyObject element)
        {
            if (this.IsDragDropEnabled)
            {
                RadDragAndDropManager.SetAllowDrag(element, true);
                RadDragAndDropManager.SetAllowDrop(element, true);
            }
        }

        private void PropagateEditProperties(RadTreeViewItem treeViewItem)
        {
            if ((treeViewItem.HeaderEditTemplate == null) && (treeViewItem.HeaderEditTemplateSelector == null))
            {
                treeViewItem.HeaderEditTemplate = this.ItemEditTemplate;
                treeViewItem.HeaderEditTemplateSelector = this.ItemEditTemplateSelector;
            }
        }

        private void RadTreeView_ItemPrepared(object sender, RadTreeViewItemPreparedEventArgs e)
        {
            if (e.PreparedItem.Level == 0)
            {
                if ((base.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated) && (this.expandByPathpathParts != null))
                {
                    this.ExpandChildItem(this.expandByPathpathParts[e.PreparedItem.Level], this);
                }
            }
            else if (((e.PreparedItem.ParentItem.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated) && (this.expandByPathpathParts != null)) && (this.expandByPathpathParts.Length > e.PreparedItem.Level))
            {
                this.ExpandChildItem(this.expandByPathpathParts[e.PreparedItem.Level], e.PreparedItem.ParentItem);
            }
        }

        private static bool RaiseUnselectedForContainer(RadTreeViewItem treeViewItem)
        {
            RadRoutedEventArgs routedArgs = new RadRoutedEventArgs(RadTreeViewItem.PreviewUnselectedEvent, treeViewItem.Item);
            return treeViewItem.OnPreviewUnselected(routedArgs);
        }

        private void RestoreAndPropagateCheckStateProperties(object item, RadTreeViewItem treeViewItem)
        {
            if (this.IsOptionElementsEnabled && (treeViewItem.GetBindingExpression(RadTreeViewItem.CheckStateProperty) == null))
            {
                if (this.IsVirtualizing)
                {
                    ToggleState checkState = this.GetCheckStateValue(item);
                    if ((this.IsTriStateMode && (treeViewItem.ParentItem != null)) && ((treeViewItem.ParentItem.ItemsOptionListType == OptionListType.Default) && (treeViewItem.ParentItem.CheckState != ToggleState.Indeterminate)))
                    {
                        treeViewItem.SetCheckStateWithNoPropagation(treeViewItem.ParentItem.CheckState);
                    }
                    else
                    {
                        try
                        {
                            treeViewItem.SupressEventRaising = true;
                            treeViewItem.SetCheckStateWithNoPropagation(checkState);
                        }
                        finally
                        {
                            treeViewItem.SupressEventRaising = false;
                        }
                    }
                }
                else
                {
                    if ((this.IsTriStateMode && (treeViewItem.ParentItem != null)) && ((treeViewItem.ParentItem.ItemsOptionListType == OptionListType.Default) && (treeViewItem.ParentItem.CheckState == ToggleState.On)))
                    {
                        treeViewItem.SetCheckStateWithNoPropagation(ToggleState.On);
                    }
                    this.StoreCheckState(null, item, treeViewItem.CheckState);
                }
                treeViewItem.Render();
            }
        }

        private void RestoreContainerProperties(object item, RadTreeViewItem treeViewItem)
        {
            bool oldAnimationValue = AnimationManager.IsGlobalAnimationEnabled;
            if (this.IsVirtualizing && (treeViewItem.Item != null))
            {
                AnimationManager.IsGlobalAnimationEnabled = false;
                try
                {
                    treeViewItem.SupressEventRaising = true;
                    bool clearedIsExpanded = treeViewItem.IsExpanded;
                    this.itemStorage.RestoreValue(treeViewItem, RadTreeViewItem.IsExpandedProperty, item, clearedIsExpanded);
                    if (clearedIsExpanded && treeViewItem.IsExpanded)
                    {
                        AnimationManager.IsGlobalAnimationEnabled = true;
                        bool oldAnimation = true;
                        AnimationManager.IsGlobalAnimationEnabled = false;
                        treeViewItem.OnIsExpandedChanged(false, true);
                        AnimationManager.IsGlobalAnimationEnabled = oldAnimation;
                    }
                    this.itemStorage.RestoreValue(treeViewItem, Control.IsEnabledProperty, item, true);
                    this.itemStorage.RestoreValue(treeViewItem, RadTreeViewItem.IsSelectedProperty, item, false);
                    this.itemStorage.RestoreValue(treeViewItem, RadTreeViewItem.DefaultImageSrcProperty, item, null);
                    this.itemStorage.RestoreValue(treeViewItem, RadTreeViewItem.ExpandedImageSrcProperty, item, null);
                    this.itemStorage.RestoreValue(treeViewItem, RadTreeViewItem.SelectedImageSrcProperty, item, null);
                    if (this.IsOptionElementsEnabled)
                    {
                        if ((treeViewItem.Style == null) || ((treeViewItem.ReadLocalValue(RadTreeViewItem.OptionTypeProperty) != DependencyProperty.UnsetValue) && (treeViewItem.OptionType != OptionListType.Default)))
                        {
                            this.itemStorage.RestoreValue(treeViewItem, RadTreeViewItem.OptionTypeProperty, item, OptionListType.Default);
                        }
                        if ((treeViewItem.Style == null) || ((treeViewItem.ReadLocalValue(RadTreeViewItem.ItemsOptionListTypeProperty) != DependencyProperty.UnsetValue) && (treeViewItem.ItemsOptionListType != OptionListType.Default)))
                        {
                            this.itemStorage.RestoreValue(treeViewItem, RadTreeViewItem.ItemsOptionListTypeProperty, item, OptionListType.Default);
                        }
                    }
                    if (this.IsLoadOnDemandEnabled)
                    {
                        this.itemStorage.RestoreValue(treeViewItem, RadTreeViewItem.IsLoadOnDemandEnabledProperty, item, false);
                        if (treeViewItem.Items.Count == 0)
                        {
                            this.itemStorage.RestoreValue(treeViewItem, RadTreeViewItem.IsLoadingOnDemandProperty, item, false);
                        }
                        else
                        {
                            this.itemStorage.ClearValue(item, RadTreeViewItem.IsLoadingOnDemandProperty);
                        }
                    }
                }
                finally
                {
                    AnimationManager.IsGlobalAnimationEnabled = oldAnimationValue;
                    treeViewItem.SupressEventRaising = false;
                }
                treeViewItem.IsMouseOverState = false;
            }
        }

        internal void ScrollBy(double deltaY, double deltaX)
        {
            if (this.ScrollViewer != null)
            {
                if (deltaY != 0.0)
                {
                    this.ScrollViewer.ScrollToVerticalOffset(this.ScrollViewer.VerticalOffset + deltaY);
                }
                if (deltaX != 0.0)
                {
                    this.ScrollViewer.ScrollToHorizontalOffset(this.ScrollViewer.HorizontalOffset + deltaX);
                }
            }
        }

        private void SelectItem(object item)
        {
            RadTreeViewItem containerItem = item as RadTreeViewItem;
            if (containerItem == null)
            {
                containerItem = this.ContainerFromItemRecursive(item);
            }
            if (containerItem != null)
            {
                containerItem.IsSelected = true;
            }
            else if (this.IsVirtualizing)
            {
                this.ItemStorage.SetValue(item, RadTreeViewItem.IsSelectedProperty, true);
            }
        }

        public void SelectItemByPath(string path)
        {
            this.SelectItemByPath(path, this.PathSeparator);
        }

        public void SelectItemByPath(string path, string separator)
        {
            RadTreeViewItem treeItem = this.GetItemByPath(path, separator);
            if (treeItem != null)
            {
                this.SelectedItems.Add(treeItem.Item);
            }
        }

        private void SelectNextItem(RadTreeViewItem nextItem, KeyEventArgs e)
        {
            while ((nextItem != null) && !nextItem.IsEnabled)
            {
                nextItem = nextItem.NextItem;
            }
            if (nextItem != null)
            {
                if (!this.IsCtrlPressed)
                {
                    this.HandleItemSelectionFromUI(nextItem);
                }
                nextItem.Focus();
                e.Handled = true;
            }
        }

        private void SelectPreviousItem(RadTreeViewItem prevItem, KeyEventArgs e)
        {
            while ((prevItem != null) && !prevItem.IsEnabled)
            {
                prevItem = prevItem.PreviousItem;
            }
            if (prevItem != null)
            {
                if (!this.IsCtrlPressed)
                {
                    this.HandleItemSelectionFromUI(prevItem);
                }
                prevItem.Focus();
                e.Handled = true;
            }
        }

        private void SetDefaultValues()
        {
            this.TextDropIn = LocalizationManager.GetString("TreeViewDropIn");
            this.TextDropAfter = LocalizationManager.GetString("TreeViewDropAfter");
            this.TextDropBefore = LocalizationManager.GetString("TreeViewDropBefore");
            this.TextDropRoot = LocalizationManager.GetString("TreeViewDropRoot");
        }

        internal void SetExpandState(ItemCollection items, bool expand, bool recursive)
        {
            foreach (object item in items)
            {
                RadTreeViewItem itemContainer = item as RadTreeViewItem;
                if (itemContainer == null)
                {
                    itemContainer = this.ContainerFromItemRecursive(item);
                }
                if (itemContainer != null)
                {
                    if (recursive)
                    {
                        if (expand)
                        {
                            itemContainer.ExpandAll();
                        }
                        else
                        {
                            itemContainer.CollapseAll();
                        }
                    }
                    else
                    {
                        itemContainer.IsExpanded = expand;
                    }
                    continue;
                }
                this.ItemStorage.SetValue(item, RadTreeViewItem.IsExpandedProperty, expand);
                this.ItemStorage.SetValue(item, RadTreeViewItem.IsExpandAllPendingProperty, expand);
            }
        }

        internal void SetItemOptionElementType(RadTreeViewItem itemContainer)
        {
            if (!this.IsOptionElementsEnabled)
            {
                if (itemContainer.OptionType != OptionListType.Default)
                {
                    itemContainer.OptionType = OptionListType.None;
                }
            }
            else
            {
                OptionListType listType = this.ItemsOptionListType;
                OptionListType parentItemType = (itemContainer.ParentItem != null) ? itemContainer.ParentItem.ItemsOptionListType : OptionListType.Default;
                if (parentItemType != OptionListType.Default)
                {
                    listType = parentItemType;
                }
                switch (listType)
                {
                    case OptionListType.CheckList:
                        if (itemContainer.OptionType != OptionListType.Default)
                        {
                            break;
                        }
                        itemContainer.OptionType = OptionListType.CheckList;
                        return;

                    case OptionListType.OptionList:
                        if (itemContainer.OptionType != OptionListType.Default)
                        {
                            break;
                        }
                        itemContainer.OptionType = OptionListType.OptionList;
                        return;

                    default:
                        if (itemContainer.OptionType == OptionListType.Default)
                        {
                            itemContainer.OptionType = OptionListType.None;
                        }
                        break;
                }
            }
        }

        internal void SetItemsOptionListType(RadTreeViewItem itemContainer)
        {
            itemContainer.ForEachContainerItem<RadTreeViewItem>(delegate(RadTreeViewItem childContainerItem)
            {
                this.SetItemOptionElementType(childContainerItem);
            });
        }

        internal void SetSelectedItem(RadTreeViewItem itemContainer)
        {
            if ((this.SelectedItem != itemContainer) || (this.SelectedItems.Count > 1))
            {
                this.selectionChanger.AddJustThis(itemContainer.Item);
            }
        }

        internal bool ShallLineBePrinted(RadTreeViewItem item)
        {
            if (!this.IsLineEnabled)
            {
                return false;
            }
            return ((item.Level != 0) || ((item.Level == 0) && this.IsRootLinesEnabled));
        }

        public void ShowBetweenItemsDragCue(RadTreeViewItem dropDestination)
        {
            if (this.dragBetweenItemsFeedback != null)
            {
                if (!this.IsDropPreviewLineEnabled)
                {
                    this.HideBetweenItemsDragCue();
                }
                else
                {
                    MatrixTransform mt;
                    DropPosition dropPosition = dropDestination.DropPosition;
                    this.dragBetweenItemsFeedback.Visibility = Visibility.Visible;
                    if (this.ScrollViewer != null)
                    {
                        mt = dropDestination.TransformToVisual(this.ScrollViewer) as MatrixTransform;
                    }
                    else
                    {
                        mt = dropDestination.TransformToVisual(this) as MatrixTransform;
                    }
                    double topPosition = mt.Matrix.OffsetY;
                    if (dropPosition == DropPosition.After)
                    {
                        topPosition += dropDestination.HeaderHeight;
                    }
                    if (this.ScrollViewer != null)
                    {
                        topPosition -= this.ScrollViewer.VerticalOffset;
                    }
                    double leftPosition = dropDestination.Level * this.ItemsIndent;
                    double viewPortWidth = base.ActualWidth;
                    if (this.ScrollViewer != null)
                    {
                        topPosition += this.ScrollViewer.VerticalOffset;
                        if (double.IsInfinity(this.ScrollViewer.ViewportWidth) && (this.ScrollViewer.ComputedVerticalScrollBarVisibility == Visibility.Visible))
                        {
                            viewPortWidth -= 16.0;
                        }
                    }
                    this.dragBetweenItemsFeedback.Width = Math.Max((double)1.0, (double)(viewPortWidth - leftPosition));
                    if (!(this.dragBetweenItemsFeedback.RenderTransform is TranslateTransform))
                    {
                        this.dragBetweenItemsFeedback.RenderTransform = new TranslateTransform();
                    }
                    TranslateTransform translateTransform = this.dragBetweenItemsFeedback.RenderTransform as TranslateTransform;
                    if (translateTransform != null)
                    {
                        translateTransform.X = leftPosition;
                        translateTransform.Y = topPosition - (this.dragBetweenItemsFeedback.Height / 2.0);
                        if (this.ScrollViewer != null)
                        {
                            translateTransform.X += this.ScrollViewer.Padding.Left;
                            translateTransform.Y -= this.ScrollViewer.Padding.Top;
                            if ((translateTransform.Y < 0.0) || (translateTransform.Y > this.ScrollViewer.ActualHeight))
                            {
                                this.dragBetweenItemsFeedback.Opacity = 0.0;
                            }
                            else
                            {
                                this.dragBetweenItemsFeedback.Opacity = 1.0;
                            }
                        }
                    }
                }
            }
        }

        internal virtual void StartDrag(IEnumerable draggedItems, DragDropOptions options)
        {
            TreeViewDragCue treeCue = this.CreateDragCue();
            options.DragCue = treeCue;
            this.OnDragStarted(new RadTreeViewDragEventArgs(draggedItems as Collection<object>, DragStartedEvent, this));
            if (treeCue != null)
            {
                treeCue.ItemTemplate = base.ItemTemplate;
                treeCue.ItemTemplateSelector = base.ItemTemplateSelector;
                treeCue.SetSafeItemsSource(draggedItems);
            }
        }

        internal void StoreCheckState(RadTreeViewItem container, object item, ToggleState newCheckState)
        {
            if (((item == null) && (container != null)) && !container.HasTemplate)
            {
                if (container.GetBindingExpression(RadTreeViewItem.CheckStateProperty) == null)
                {
                    item = container;
                }
                else
                {
                    item = container.Header;
                }
            }
            WeakReferenceKey<object> key = new WeakReferenceKey<object>(item);
            if (newCheckState == ToggleState.Off)
            {
                if (this.itemToggleStateStorage.Remove(key))
                {
                    (this.CheckedItems as CheckedItemsCollection).NotifyCountChanged();
                }
            }
            else
            {
                this.itemToggleStateStorage[key] = newCheckState;
                (this.CheckedItems as CheckedItemsCollection).NotifyCountChanged();
            }
        }

        private void UnselectItem(object item)
        {
            RadTreeViewItem containerItem = item as RadTreeViewItem;
            if (containerItem == null)
            {
                containerItem = this.ContainerFromItemRecursive(item);
            }
            if (containerItem != null)
            {
                containerItem.IsSelected = false;
            }
            else if (this.IsVirtualizing)
            {
                this.ItemStorage.ClearValue(item, RadTreeViewItem.IsSelectedProperty);
            }
        }

        private void UpdateBetweenItemsDragCue(RadTreeViewItem destinationItem)
        {
            if (destinationItem.DropPosition != DropPosition.Inside)
            {
                this.ShowBetweenItemsDragCue(destinationItem);
            }
            else
            {
                this.HideBetweenItemsDragCue();
            }
        }

        internal void UpdateChildItemsLine(Telerik.Windows.Controls.ItemsControl treeViewItemsContainer)
        {
            foreach (RadTreeViewItem childItemContainer in this.GetAllItemContainers(treeViewItemsContainer))
            {
                if (childItemContainer != null)
                {
                    childItemContainer.ChangeVisualState();
                    childItemContainer.RenderListRoot();
                    childItemContainer.RenderIndent();
                }
            }
        }

        private void UpdateExpandTimer(FrameworkElement dropDestination, bool dropPossible)
        {
            RadTreeViewItem treeViewItem = dropDestination as RadTreeViewItem;
            if ((treeViewItem != null) && (!treeViewItem.IsExpanded && (treeViewItem.ParentTreeView == this)))
            {
                if (dropPossible)
                {
                    if (!treeViewItem.DropExpandStartTime.HasValue)
                    {
                        if (this.DropExpandDelay.TotalMilliseconds == 0.0)
                        {
                            treeViewItem.IsExpanded = true;
                        }
                        else
                        {
                            treeViewItem.DropExpandStartTime = new DateTime?(DateTime.Now);
                        }
                    }
                    else
                    {
                        DateTime now = DateTime.Now;
                        TimeSpan? timeDifference = (TimeSpan?)(now - treeViewItem.DropExpandStartTime);
                        if ((timeDifference.HasValue ? (timeDifference.GetValueOrDefault() >= this.DropExpandDelay) : false) && (treeViewItem.HasItems || treeViewItem.IsLoadOnDemandEnabled))
                        {
                            treeViewItem.IsExpanded = true;
                            treeViewItem.DropExpandStartTime = null;
                        }
                    }
                }
                else
                {
                    treeViewItem.DropExpandStartTime = null;
                }
            }
        }

        private void UpdateItemIndeterminatedState(RadTreeViewItem itemContainer)
        {
            itemContainer.ForEachContainerItem<RadTreeViewItem>(delegate(RadTreeViewItem childContainer)
            {
                this.UpdateItemIndeterminatedState(childContainer);
            });
            itemContainer.SetCheckStateWithNoPropagation(RadTreeViewItem.CalculateItemCheckState(itemContainer));
        }

        private void UpdateSelectedItemProperty()
        {
            if (this.SelectedItems.Count > 0)
            {
                this.SelectedItem = this.SelectedItems[0];
            }
            else
            {
                this.SelectedItem = null;
            }
        }

        internal void UpdateSelectedValue()
        {
            if (string.IsNullOrEmpty(this.SelectedValuePath))
            {
                this.SelectedValue = this.SelectedItem;
            }
            else if (this.GetSelectedValuePathBinding() != null)
            {
                base.SetBinding(SelectedValueProperty, this.GetSelectedValuePathBinding());
            }
            else
            {
                base.ClearValue(SelectedValueProperty);
            }
        }

        internal virtual void UpdateTreeCueForDestinationItem(TreeViewDragCue treeCue, RadTreeViewItem destinationItem)
        {
            if (destinationItem != null)
            {
                treeCue.DragTooltipContent = TreeViewDragCue.GetNonVisualRepresentation(destinationItem);
                treeCue.DragTooltipContentTemplate = GetDragTooltipTemplateFromItem(destinationItem);
                treeCue.DragActionContent = this.GetDragActionText(destinationItem);
                treeCue.DragActionContentTemplate = null;
            }
            else
            {
                treeCue.DragActionContent = this.TextDropRoot;
            }
        }

        public Telerik.Windows.Controls.BringIntoViewMode BringIntoViewMode
        {
            get
            {
                return (Telerik.Windows.Controls.BringIntoViewMode)base.GetValue(BringIntoViewModeProperty);
            }
            set
            {
                base.SetValue(BringIntoViewModeProperty, value);
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), ScriptableMember]
        public ICollection<object> CheckedItems
        {
            get
            {
                return (ICollection<object>)base.GetValue(CheckedItemsProperty);
            }
            internal set
            {
                base.SetValue(CheckedItemsProperty, value);
            }
        }

        internal RadTreeViewItem CurrentEditedItem { get; set; }

        public TimeSpan DropExpandDelay
        {
            get
            {
                return (TimeSpan)base.GetValue(DropExpandDelayProperty);
            }
            set
            {
                base.SetValue(DropExpandDelayProperty, value);
            }
        }

        [Browsable(false)]
        public Style ExpanderStyle
        {
            get
            {
                return (Style)base.GetValue(ExpanderStyleProperty);
            }
            set
            {
                base.SetValue(ExpanderStyleProperty, value);
            }
        }

        [Description("The directory where image files used in the treeview reside"), DefaultValue(""), ScriptableMember]
        public string ImagesBaseDir
        {
            get
            {
                return (string)base.GetValue(ImagesBaseDirProperty);
            }
            set
            {
                base.SetValue(ImagesBaseDirProperty, value);
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        private bool IsCtrlPressed
        {
            get
            {
                return ((System.Windows.Input.Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control);
            }
        }

        [ScriptableMember]
        public bool IsDragDropEnabled
        {
            get
            {
                return (bool)base.GetValue(IsDragDropEnabledProperty);
            }
            set
            {
                base.SetValue(IsDragDropEnabledProperty, value);
            }
        }

        [ScriptableMember]
        public bool IsDragPreviewEnabled
        {
            get
            {
                return (bool)base.GetValue(IsDragPreviewEnabledProperty);
            }
            set
            {
                base.SetValue(IsDragPreviewEnabledProperty, value);
            }
        }

        [ScriptableMember]
        public bool IsDragTooltipEnabled
        {
            get
            {
                return (bool)base.GetValue(IsDragTooltipEnabledProperty);
            }
            set
            {
                base.SetValue(IsDragTooltipEnabledProperty, value);
            }
        }

        [ScriptableMember, DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public bool IsDropPreviewLineEnabled
        {
            get
            {
                return (bool)base.GetValue(IsDropPreviewLineEnabledProperty);
            }
            set
            {
                base.SetValue(IsDropPreviewLineEnabledProperty, value);
            }
        }

        [ScriptableMember]
        public bool IsEditable
        {
            get
            {
                return (bool)base.GetValue(IsEditableProperty);
            }
            set
            {
                base.SetValue(IsEditableProperty, value);
            }
        }

        [Browsable(false), ScriptableMember, Description("Gets if there is an open editor in the tree view."), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual bool IsEditing
        {
            get
            {
                return (this.CurrentEditedItem != null);
            }
        }

        [ScriptableMember]
        public bool IsExpandOnDblClickEnabled
        {
            get
            {
                return (bool)base.GetValue(IsExpandOnDblClickEnabledProperty);
            }
            set
            {
                base.SetValue(IsExpandOnDblClickEnabledProperty, value);
            }
        }

        [ScriptableMember]
        public bool IsExpandOnSingleClickEnabled
        {
            get
            {
                return (bool)base.GetValue(IsExpandOnSingleClickEnabledProperty);
            }
            set
            {
                base.SetValue(IsExpandOnSingleClickEnabledProperty, value);
            }
        }

        [ScriptableMember]
        public bool IsLineEnabled
        {
            get
            {
                return (bool)base.GetValue(IsLineEnabledProperty);
            }
            set
            {
                base.SetValue(IsLineEnabledProperty, value);
            }
        }

        [ScriptableMember]
        public bool IsLoadOnDemandEnabled
        {
            get
            {
                return (bool)base.GetValue(IsLoadOnDemandEnabledProperty);
            }
            set
            {
                base.SetValue(IsLoadOnDemandEnabledProperty, value);
            }
        }

        [ScriptableMember, Category("Behavior"), DefaultValue(false), Description("Gets or sets a value indicating whether checkboxes/radio buttons are displayed besides item.")]
        public bool IsOptionElementsEnabled
        {
            get
            {
                return (bool)base.GetValue(IsOptionElementsEnabledProperty);
            }
            set
            {
                base.SetValue(IsOptionElementsEnabledProperty, value);
            }
        }

        [ScriptableMember]
        public bool IsRootLinesEnabled
        {
            get
            {
                return (bool)base.GetValue(IsRootLinesEnabledProperty);
            }
            set
            {
                base.SetValue(IsRootLinesEnabledProperty, value);
            }
        }

        internal bool IsSelectionActive
        {
            get
            {
                return this.isSelectionActive;
            }
            set
            {
                this.isSelectionActive = value;
                foreach (object item in this.SelectedItems)
                {
                    RadTreeViewItem itemContainer = this.ContainerFromItemRecursive(item, this);
                    if (itemContainer != null)
                    {
                        itemContainer.IsSelectionActive = value;
                    }
                }
            }
        }

        [ScriptableMember]
        public bool IsSingleExpandPath
        {
            get
            {
                return (bool)base.GetValue(IsSingleExpandPathProperty);
            }
            set
            {
                base.SetValue(IsSingleExpandPathProperty, value);
            }
        }

        [ScriptableMember]
        public bool IsTriStateMode
        {
            get
            {
                return (bool)base.GetValue(IsTriStateModeProperty);
            }
            set
            {
                base.SetValue(IsTriStateModeProperty, value);
            }
        }

        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Virtualizing", Justification = "This is the correct spelling")]
        public bool IsVirtualizing
        {
            get
            {
                return TreeViewPanel.GetIsVirtualizing(this);
            }
            set
            {
                TreeViewPanel.SetIsVirtualizing(this, value);
            }
        }

        public DataTemplate ItemEditTemplate
        {
            get
            {
                return (DataTemplate)base.GetValue(ItemEditTemplateProperty);
            }
            set
            {
                base.SetValue(ItemEditTemplateProperty, value);
            }
        }

        public DataTemplateSelector ItemEditTemplateSelector
        {
            get
            {
                return (DataTemplateSelector)base.GetValue(ItemEditTemplateSelectorProperty);
            }
            set
            {
                base.SetValue(ItemEditTemplateSelectorProperty, value);
            }
        }

        internal TreeViewPanel ItemsHost
        {
            get
            {
                return this.itemsHost;
            }
        }

        [ScriptableMember]
        public int ItemsIndent
        {
            get
            {
                return (int)base.GetValue(ItemsIndentProperty);
            }
            set
            {
                base.SetValue(ItemsIndentProperty, value);
            }
        }

        [ScriptableMember]
        public OptionListType ItemsOptionListType
        {
            get
            {
                return (OptionListType)base.GetValue(ItemsOptionListTypeProperty);
            }
            set
            {
                base.SetValue(ItemsOptionListTypeProperty, value);
            }
        }

        internal ItemAttachedStorage ItemStorage
        {
            get
            {
                return this.itemStorage;
            }
        }

        [ScriptableMember]
        public string PathSeparator
        {
            get
            {
                return (string)base.GetValue(PathSeparatorProperty);
            }
            set
            {
                base.SetValue(PathSeparatorProperty, value);
            }
        }

        private bool? PreviewDragEndResult { get; set; }

        [ScriptableMember]
        public System.Windows.Controls.ScrollViewer ScrollViewer
        {
            get
            {
                return this.scroller;
            }
        }

        [ScriptableMember]
        public RadTreeViewItem SelectedContainer
        {
            get
            {
                return (RadTreeViewItem)base.GetValue(SelectedContainerProperty);
            }
            private set
            {
                this.SetValue(SelectedContainerPropertyKey, value);
            }
        }

        [ScriptableMember, DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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

        [Category("Behaviour"), DefaultValue((string)null), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), ScriptableMember, Browsable(false), Description("Gets a collection containing the items that are currently selected.")]
        public ObservableCollection<object> SelectedItems
        {
            get
            {
                return (ObservableCollection<object>)base.GetValue(SelectedItemsProperty);
            }
            private set
            {
                this.SetValue(SelectedItemsPropertyKey, value);
            }
        }

        [ScriptableMember]
        public object SelectedValue
        {
            get
            {
                return base.GetValue(SelectedValueProperty);
            }
            internal set
            {
                base.SetValue(SelectedValueProperty, value);
            }
        }

        [ScriptableMember]
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

        [Category("Behavior"), ScriptableMember, DefaultValue(false), Description("Gets or sets the selection mode.")]
        public Telerik.Windows.Controls.SelectionMode SelectionMode
        {
            get
            {
                return (Telerik.Windows.Controls.SelectionMode)base.GetValue(SelectionModeProperty);
            }
            set
            {
                base.SetValue(SelectionModeProperty, value);
            }
        }

        [ScriptableMember]
        public string TextDropAfter { get; set; }

        [ScriptableMember]
        public string TextDropBefore { get; set; }

        [ScriptableMember]
        public string TextDropIn { get; set; }

        [ScriptableMember]
        public string TextDropRoot { get; set; }

        private delegate bool ItemTraversal(ref RadTreeViewItem treeViewItem, ref object item, ref Telerik.Windows.Controls.ItemsControl itemOwner);
    }
}

