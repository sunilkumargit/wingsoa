namespace Telerik.Windows.Controls
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Input;
    using Telerik.Windows;
    using Telerik.Windows.Controls.OutlookBar;
    using Telerik.Windows.Controls.TabControl;

    [TemplateVisualState(GroupName="DropDownDisplayStates", Name="DropDownButtonCollapsed"), TemplateVisualState(GroupName="DropDownDisplayStates", Name="DropDownButtonVisible"), StyleTypedProperty(Property="HorizontalSplitterStyle", StyleTargetType=typeof(Thumb)), StyleTypedProperty(Property="DropDownStyle", StyleTargetType=typeof(RadContextMenu)), TemplatePart(Name="TitleElement", Type=typeof(ContentControl)), TemplatePart(Name="HorizontalSplitter", Type=typeof(Thumb)), StyleTypedProperty(Property="DropDownButtonStyle", StyleTargetType=typeof(ToggleButton)), StyleTypedProperty(Property="ItemContainerStyle", StyleTargetType=typeof(RadOutlookBarItem)), TemplatePart(Name="OverflowButton", Type=typeof(ToggleButton))]
    public class RadOutlookBar : RadTabControl
    {
        private int activeItemsCount;
        public static readonly DependencyProperty ActiveItemsMaxCountProperty = DependencyProperty.Register("ActiveItemsMaxCount", typeof(int), typeof(RadOutlookBar), new System.Windows.PropertyMetadata(0x7fffffff, new PropertyChangedCallback(RadOutlookBar.OnActiveItemsMaxCountChanged)));
        private ContentPresenter contentElement;
        private bool dragStarted;
        private bool hasContentElement;
        private bool hasHorizontalSplitter;
        private bool hasItemsPresenter;
        private bool hasMinimizedArea;
        private bool hasOverflowButton;
        private bool hasTitleElement;
        public static readonly DependencyProperty HeaderVisibilityProperty = DependencyProperty.Register("HeaderVisibility", typeof(Visibility), typeof(RadOutlookBar), new System.Windows.PropertyMetadata(Visibility.Visible));
        private Thumb horizontalSplitter;
        public static readonly DependencyProperty HorizontalSplitterStyleProperty = DependencyProperty.Register("HorizontalSplitterStyle", typeof(Style), typeof(RadOutlookBar), null);
        public static readonly DependencyProperty ItemMinimizedTemplateProperty = DependencyProperty.Register("ItemMinimizedTemplate", typeof(DataTemplate), typeof(RadOutlookBar), null);
        public static readonly DependencyProperty ItemMinimizedTemplateSelectorProperty = DependencyProperty.Register("ItemMinimizedTemplateSelector", typeof(DataTemplateSelector), typeof(RadOutlookBar), null);
        private FrameworkElement itemsPresenterElement;
        public static readonly DependencyProperty MinContentHeightProperty = DependencyProperty.Register("MinContentHeight", typeof(double), typeof(RadOutlookBar), new System.Windows.PropertyMetadata(50.0, new PropertyChangedCallback(RadOutlookBar.OnMinContentHeightChanged)));
        private Telerik.Windows.Controls.OutlookBar.MinimizedOutlookBarArea minimizedArea;
        public static readonly DependencyProperty MinimizedAreaMinHeightProperty = DependencyProperty.Register("MinimizedAreaMinHeight", typeof(double), typeof(RadOutlookBar), null);
        public static readonly DependencyProperty MinimizedAreaVisibilityProperty = DependencyProperty.Register("MinimizedAreaVisibility", typeof(Visibility), typeof(RadOutlookBar), new System.Windows.PropertyMetadata(Visibility.Visible));
        private ObservableCollection<MinimizedOutlookBarItem> minimizedItemsCollection = new ObservableCollection<MinimizedOutlookBarItem>();
        private Dictionary<object, RadOutlookBarItem> minimizedItemsOutlookBarItems = new Dictionary<object, RadOutlookBarItem>(0x10);
        private ToggleButton overflowButton;
        private ObservableCollection<object> overflowItemsCollection = new ObservableCollection<object>();
        public static readonly DependencyProperty SelectedItemHeaderTemplateProperty = DependencyProperty.Register("SelectedItemHeaderTemplate", typeof(DataTemplate), typeof(RadOutlookBar), null);
        public static readonly DependencyProperty SelectedItemHeaderTemplateSelectorProperty = DependencyProperty.Register("SelectedItemHeaderTemplateSelector", typeof(DataTemplateSelector), typeof(RadOutlookBar), null);
        private ContentControl titleElement;
        public static readonly DependencyProperty TitleTemplateProperty = DependencyProperty.Register("TitleTemplate", typeof(DataTemplate), typeof(RadOutlookBar), null);
        public static readonly DependencyProperty TitleTemplateSelectorProperty = DependencyProperty.Register("TitleTemplateSelector", typeof(DataTemplateSelector), typeof(RadOutlookBar), null);

        public event PositionChangedEventHandler ItemPositionChanged
        {
            add
            {
                this.AddHandler(RadOutlookBarItem.PositionChangedEvent, value);
            }
            remove
            {
                this.RemoveHandler(RadOutlookBarItem.PositionChangedEvent, value);
            }
        }

        public RadOutlookBar()
        {
            base.DefaultStyleKey = typeof(RadOutlookBar);
            
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Size result = base.ArrangeOverride(finalSize);
            if ((this.hasContentElement && this.hasItemsPresenter) && (!double.IsPositiveInfinity(this.MinContentHeight) && (this.contentElement.ActualHeight <= this.MinContentHeight)))
            {
                double newMaxHeight = this.itemsPresenterElement.ActualHeight - (this.MinContentHeight - this.contentElement.ActualHeight);
                this.itemsPresenterElement.MaxHeight = Math.Max(0.0, newMaxHeight);
            }
            return result;
        }

        private void AttachResizeListener()
        {
            WeakEventListener<RadOutlookBar, object, EventArgs> weakListener = new WeakEventListener<RadOutlookBar, object, EventArgs>(this) {
                OnEventAction = delegate (RadOutlookBar outlookBar, object sender, EventArgs args) {
                    Deployment.Current.Dispatcher.BeginInvoke(delegate {
                        outlookBar.IsDropDownOpen = false;
                    });
                },
                OnDetachAction = delegate (WeakEventListener<RadOutlookBar, object, EventArgs> listener) {
                    ApplicationHelper.RootVisual.SizeChanged -= new SizeChangedEventHandler(listener.OnEvent);
                }
            };
            ApplicationHelper.RootVisual.SizeChanged += new SizeChangedEventHandler(weakListener.OnEvent);
        }

        private void ChangeOverfloButtonEnabledProperty()
        {
            if (this.hasOverflowButton)
            {
                this.overflowButton.IsEnabled = this.overflowItemsCollection.Count > 0;
            }
        }

        protected internal override void ChangeVisualState(bool useTransitions)
        {
            base.ChangeVisualState(useTransitions);
            if (this.overflowItemsCollection.Count == 0)
            {
                VisualStateManager.GoToState(this, "DropDownButtonCollapsed", useTransitions);
            }
            else
            {
                VisualStateManager.GoToState(this, "DropDownButtonVisible", useTransitions);
            }
        }

        private void ClearContent()
        {
            if (this.contentElement != null)
            {
                this.contentElement.ContentTemplate = null;
                this.contentElement.Content = null;
            }
            if (this.titleElement != null)
            {
                this.titleElement.ContentTemplate = null;
                this.titleElement.Content = null;
            }
        }

        private void ClearMinimizedAreaItemsSource()
        {
            if (this.hasMinimizedArea)
            {
                this.minimizedArea.ItemsSource = null;
                this.minimizedArea.ItemsSource = this.minimizedItemsCollection;
            }
        }

        private MinimizedOutlookBarItem CreateMinimizedItem(object item, RadOutlookBarItem container)
        {
            MinimizedOutlookBarItem minimizedItem = this.GetContainerForMinimizedItem();
            minimizedItem.Content = this.GetMinimizedContent(item);
            minimizedItem.SmallIcon = container.SmallIcon;
            if ((ToolTipService.GetToolTip(minimizedItem) == null) && (container.Header is string))
            {
                ToolTipService.SetToolTip(minimizedItem, container.Header.ToString());
            }
            minimizedItem.ParentOutlookBar = this;
            minimizedItem.ContentTemplate = this.GetMinimizedContentTemplate(item, container);
            SetBinding(container, minimizedItem, Control.IsEnabledProperty, "IsEnabled");
            SetBinding(container, minimizedItem, UIElement.VisibilityProperty, "Visibility");
            return minimizedItem;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new RadOutlookBarItem();
        }

        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        protected virtual MinimizedOutlookBarItem GetContainerForMinimizedItem()
        {
            return new MinimizedOutlookBarItem();
        }

        internal object GetMinimizedContent(object item)
        {
            if (!(item is UIElement))
            {
                return item;
            }
            RadOutlookBarItem outlookItem = item as RadOutlookBarItem;
            if (outlookItem == null)
            {
                return TextSearch.GetPrimaryTextFromItem(this, item);
            }
            if (outlookItem.MinimizedContent != null)
            {
                return outlookItem.MinimizedContent;
            }
            if (outlookItem.SmallIcon != null)
            {
                return null;
            }
            if (outlookItem.Header == null)
            {
                return item.ToString();
            }
            if (outlookItem.Header is DependencyObject)
            {
                return TextSearch.GetPrimaryText(outlookItem.Header, string.Empty);
            }
            return outlookItem.Header;
        }

        private DataTemplate GetMinimizedContentTemplate(object item, RadOutlookBarItem container)
        {
            DataTemplate template = container.MinimizedContentTemplate;
            if ((template == null) && (container.MinimizedContentTemplateSelector != null))
            {
                template = container.MinimizedContentTemplateSelector.SelectTemplate(item, container);
            }
            if (template == null)
            {
                template = this.ItemMinimizedTemplate;
            }
            if ((template == null) && (this.ItemMinimizedTemplateSelector != null))
            {
                template = this.ItemMinimizedTemplateSelector.SelectTemplate(item, container);
            }
            return template;
        }

        private void HorizontalSplitter_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            this.dragStarted = false;
        }

        private void HorizontalSplitter_DragStarted(object sender, DragStartedEventArgs e)
        {
            if (this.hasItemsPresenter && (base.Items.Count > 0))
            {
                this.dragStarted = true;
            }
        }

        private void HorizontalSplitter_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.dragStarted)
            {
                double relativePosition = e.GetPosition(this.itemsPresenterElement).Y;
                double newMaxHeight = Math.Max((double) 0.0, (double) ((this.itemsPresenterElement.ActualHeight - relativePosition) + 10.0));
                if ((!this.hasContentElement || double.IsInfinity(this.MinContentHeight)) || ((this.contentElement.ActualHeight > this.MinContentHeight) || (newMaxHeight <= this.itemsPresenterElement.MaxHeight)))
                {
                    this.itemsPresenterElement.MaxHeight = newMaxHeight;
                }
            }
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return (item is RadOutlookBarItem);
        }

        private void MinimizedArea_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            object selectedItem = this.minimizedArea.SelectedItem;
            if (selectedItem != null)
            {
                this.MinimizedItemsOutlookBarItems[selectedItem].IsSelected = true;
            }
        }

        private void NullifyMinimizedItemsContent()
        {
            foreach (MinimizedOutlookBarItem item in this.minimizedItemsCollection)
            {
                item.Content = null;
            }
        }

        private static void OnActiveItemsMaxCountChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            RadOutlookBar outlookBar = sender as RadOutlookBar;
            if ((outlookBar != null) && (outlookBar.OutlookBarPanel != null))
            {
                outlookBar.OutlookBarPanel.ItemsMaxCount = outlookBar.ActiveItemsMaxCount;
            }
        }

        public override void OnApplyTemplate()
        {
            this.ClearContent();
            this.ForEachContainerItem<RadOutlookBarItem>(delegate (RadOutlookBarItem item) {
                item.ClearContent();
            });
            base.OnApplyTemplate();
            this.titleElement = base.GetTemplateChild("TitleElement") as ContentControl;
            this.hasTitleElement = this.titleElement != null;
            this.contentElement = base.GetTemplateChild("ContentElement") as ContentPresenter;
            this.hasContentElement = this.contentElement != null;
            if (this.contentElement != null)
            {
                this.contentElement.Content = base.SelectedContent;
            }
            this.itemsPresenterElement = base.GetTemplateChild("ItemsPresenterElement") as FrameworkElement;
            this.hasItemsPresenter = this.itemsPresenterElement != null;
            if (this.hasHorizontalSplitter)
            {
                this.horizontalSplitter.DragStarted -= new DragStartedEventHandler(this.HorizontalSplitter_DragStarted);
                this.horizontalSplitter.DragCompleted -= new DragCompletedEventHandler(this.HorizontalSplitter_DragCompleted);
                this.horizontalSplitter.MouseMove -= new MouseEventHandler(this.HorizontalSplitter_MouseMove);
            }
            this.horizontalSplitter = base.GetTemplateChild("HorizontalSplitter") as Thumb;
            this.hasHorizontalSplitter = this.horizontalSplitter != null;
            if (this.hasHorizontalSplitter)
            {
                this.horizontalSplitter.DragStarted += new DragStartedEventHandler(this.HorizontalSplitter_DragStarted);
                this.horizontalSplitter.DragCompleted += new DragCompletedEventHandler(this.HorizontalSplitter_DragCompleted);
                this.horizontalSplitter.MouseMove += new MouseEventHandler(this.HorizontalSplitter_MouseMove);
            }
            this.overflowButton = base.GetTemplateChild("DropDownButtonElement") as ToggleButton;
            this.hasOverflowButton = this.overflowButton != null;
            if (this.hasMinimizedArea)
            {
                this.minimizedArea.ItemsSource = null;
                this.minimizedArea.SelectionChanged -= new System.Windows.Controls.SelectionChangedEventHandler(this.MinimizedArea_SelectionChanged);
                this.minimizedArea.ParentOutlookBar = null;
            }
            this.minimizedArea = base.GetTemplateChild("MinimizedAreaControl") as Telerik.Windows.Controls.OutlookBar.MinimizedOutlookBarArea;
            this.hasMinimizedArea = this.minimizedArea != null;
            if (this.hasMinimizedArea)
            {
                this.minimizedArea.ItemsSource = this.minimizedItemsCollection;
                this.minimizedArea.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.MinimizedArea_SelectionChanged);
                this.minimizedArea.ParentOutlookBar = this;
            }
            this.UpdateTitle();
            this.ChangeOverfloButtonEnabledProperty();
            this.AttachResizeListener();
        }

        protected override void OnDropDownOpened(DropDownEventArgs e)
        {
            base.OnDropDownOpened(e);
            e.DropDownItemsSource = this.overflowItemsCollection;
            this.ChangeVisualState();
        }

        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId="value"), SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId="radOutlookBarItem")]
        internal void OnItemContainerPositionChanged(IOutlookBarItem radOutlookBarItem, OutlookBarItemPosition value)
        {
            this.SafeDispatch(delegate {
                this.UpdateMinimizedItems();
                this.UpdateOverflowItems();
                this.ChangeOverfloButtonEnabledProperty();
            });
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            this.SafeDispatch(delegate {
                this.UpdateMinimizedItems();
                this.UpdateOverflowItems();
                this.ChangeOverfloButtonEnabledProperty();
            });
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
        }

        private static void OnMinContentHeightChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            RadOutlookBar outlookBar = sender as RadOutlookBar;
            if (outlookBar != null)
            {
                outlookBar.InvalidateArrange();
            }
        }

        protected override void OnSelectionChanged(RadSelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);
            if (e.AddedItems.Count != 0)
            {
                object selectedItem = e.AddedItems[0];
                if (selectedItem != null)
                {
                    IOutlookBarItem container = (base.ItemContainerGenerator.ContainerFromItem(selectedItem) as IOutlookBarItem) ?? (selectedItem as IOutlookBarItem);
                    if (container != null)
                    {
                        if ((container.Location != OutlookBarItemPosition.MinimizedArea) && this.hasMinimizedArea)
                        {
                            this.minimizedArea.SelectedItem = null;
                        }
                        if (((container.Location == OutlookBarItemPosition.MinimizedArea) && this.hasMinimizedArea) && (container is DependencyObject))
                        {
                            int selectedItemIndex = base.ItemContainerGenerator.IndexFromContainer(container as DependencyObject);
                            if ((selectedItemIndex != -1) && ((selectedItemIndex - this.activeItemsCount) < this.minimizedArea.Items.Count))
                            {
                                this.minimizedArea.SelectedIndex = selectedItemIndex - this.activeItemsCount;
                            }
                        }
                    }
                }
            }
        }

        protected override void OnSelectionChanged(IList removedItems, IList addedItems)
        {
            base.OnSelectionChanged(removedItems, addedItems);
            this.UpdateTitle();
            if (this.contentElement != null)
            {
                this.contentElement.Content = base.SelectedContent;
            }
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            if (this.OutlookBarPanel != null)
            {
                this.OutlookBarPanel.ItemsMaxCount = this.ActiveItemsMaxCount;
            }
        }

        private void SafeDispatch(Action action)
        {
            if (RadControl.IsInDesignMode)
            {
                action();
            }
            else
            {
                base.Dispatcher.BeginInvoke(action);
            }
        }

        private static void SetBinding(RadOutlookBarItem outlookBarItem, MinimizedOutlookBarItem minimizedItem, DependencyProperty dp, string propertyName)
        {
            Binding binding = new Binding(propertyName) {
                Source = outlookBarItem
            };
            minimizedItem.SetBinding(dp, binding);
        }

        private void UpdateMinimizedItems()
        {
            object selectedItem = this.hasMinimizedArea ? this.minimizedArea.SelectedItem : null;
            this.ClearMinimizedAreaItemsSource();
            this.NullifyMinimizedItemsContent();
            this.minimizedItemsCollection.Clear();
            this.MinimizedItemsOutlookBarItems.Clear();
            this.activeItemsCount = 0;
            foreach (object item in base.Items)
            {
                IOutlookBarItem outlookBarItemContainer = base.ItemContainerGenerator.ContainerFromItem(item) as IOutlookBarItem;
                if (outlookBarItemContainer != null)
                {
                    if (outlookBarItemContainer.Location != OutlookBarItemPosition.ActiveArea)
                    {
                        RadOutlookBarItem outlookBarItem = outlookBarItemContainer as RadOutlookBarItem;
                        MinimizedOutlookBarItem minimizedItem = this.CreateMinimizedItem(item, outlookBarItem);
                        outlookBarItem.MinimizedItem = minimizedItem;
                        this.MinimizedItemsOutlookBarItems.Add(minimizedItem, outlookBarItem);
                        ((IOutlookBarItem) minimizedItem).Location = outlookBarItemContainer.Location;
                        this.minimizedItemsCollection.Add(minimizedItem);
                        if (((minimizedItem == selectedItem) || outlookBarItem.IsSelected) && outlookBarItem.IsEnabled)
                        {
                            selectedItem = minimizedItem;
                        }
                        continue;
                    }
                    this.activeItemsCount++;
                }
            }
            if (this.hasMinimizedArea)
            {
                this.minimizedArea.SelectedItem = selectedItem;
            }
        }

        private void UpdateOverflowItems()
        {
            this.overflowItemsCollection.Clear();
            foreach (object item in base.Items)
            {
                IOutlookBarItem outlookBarItemContainer = base.ItemContainerGenerator.ContainerFromItem(item) as IOutlookBarItem;
                if ((outlookBarItemContainer != null) && (outlookBarItemContainer.Location == OutlookBarItemPosition.OverflowArea))
                {
                    object representation = base.GetDropDownItem(item);
                    this.overflowItemsCollection.Add(representation);
                }
            }
            if (this.overflowItemsCollection.Count == 0)
            {
                base.IsDropDownOpen = false;
            }
        }

        internal void UpdateTitle()
        {
            if (this.hasTitleElement)
            {
                object selectedItem = base.SelectedItem;
                RadOutlookBarItem selectedContainer = (base.ItemContainerGenerator.ContainerFromItem(base.SelectedItem) as RadOutlookBarItem) ?? (base.SelectedItem as RadOutlookBarItem);
                if ((selectedContainer != null) && (selectedContainer.Title != null))
                {
                    this.titleElement.Content = selectedContainer.Title;
                    this.titleElement.ContentTemplate = selectedContainer.TitleTemplate;
                    if ((selectedContainer.TitleTemplate == null) && (selectedContainer.TitleTemplateSelector != null))
                    {
                        this.titleElement.ContentTemplate = selectedContainer.TitleTemplateSelector.SelectTemplate(selectedItem, this.titleElement);
                    }
                    if ((this.titleElement.ContentTemplate == null) && !(this.titleElement.Content is FrameworkElement))
                    {
                        this.titleElement.ContentTemplate = this.TitleTemplate;
                        if ((this.titleElement.ContentTemplate == null) && (this.TitleTemplateSelector != null))
                        {
                            this.titleElement.ContentTemplate = this.TitleTemplateSelector.SelectTemplate(selectedItem, this.titleElement);
                        }
                    }
                }
                else
                {
                    RadOutlookBarItem outlookBarItem = selectedItem as RadOutlookBarItem;
                    if (outlookBarItem != null)
                    {
                        selectedItem = outlookBarItem.Header;
                    }
                    FrameworkElement headerVisual = selectedItem as FrameworkElement;
                    if (headerVisual != null)
                    {
                        selectedItem = TextSearch.GetPrimaryText(headerVisual, string.Empty);
                    }
                    DataTemplate titleTemplate = this.TitleTemplate;
                    if ((titleTemplate == null) && (this.TitleTemplateSelector != null))
                    {
                        titleTemplate = this.TitleTemplateSelector.SelectTemplate(selectedItem, this.titleElement);
                    }
                    this.titleElement.ContentTemplate = titleTemplate;
                    this.titleElement.Content = selectedItem;
                }
            }
        }

        public int ActiveItemsMaxCount
        {
            get
            {
                return (int) base.GetValue(ActiveItemsMaxCountProperty);
            }
            set
            {
                base.SetValue(ActiveItemsMaxCountProperty, value);
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override TabStripAlign Align
        {
            get
            {
                return base.Align;
            }
            set
            {
                throw new NotSupportedException("Align is not supported by RadOutlookBar");
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override bool AllTabsEqualHeight
        {
            get
            {
                return base.AllTabsEqualHeight;
            }
            set
            {
                throw new NotSupportedException("AllTabsEqualHeight is not supported by RadOutlookBar");
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override Visibility BackgroundVisibility
        {
            get
            {
                return base.BackgroundVisibility;
            }
            set
            {
                throw new NotSupportedException("BackgroundVisibility is not supported by RadOutlookBar");
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public override ControlTemplate BottomTemplate
        {
            get
            {
                return base.BottomTemplate;
            }
            set
            {
                throw new NotSupportedException("BottomTemplate is not supported by RadOutlookBar");
            }
        }

        public Visibility HeaderVisibility
        {
            get
            {
                return (Visibility) base.GetValue(HeaderVisibilityProperty);
            }
            set
            {
                base.SetValue(HeaderVisibilityProperty, value);
            }
        }

        public Style HorizontalSplitterStyle
        {
            get
            {
                return (Style) base.GetValue(HorizontalSplitterStyleProperty);
            }
            set
            {
                base.SetValue(HorizontalSplitterStyleProperty, value);
            }
        }

        public RadOutlookBarItem this[int index]
        {
            get
            {
                return (base.ContainerFromIndex(index) as RadOutlookBarItem);
            }
        }

        public DataTemplate ItemMinimizedTemplate
        {
            get
            {
                return (DataTemplate) base.GetValue(ItemMinimizedTemplateProperty);
            }
            set
            {
                base.SetValue(ItemMinimizedTemplateProperty, value);
            }
        }

        public DataTemplateSelector ItemMinimizedTemplateSelector
        {
            get
            {
                return (DataTemplateSelector) base.GetValue(ItemMinimizedTemplateSelectorProperty);
            }
            set
            {
                base.SetValue(ItemMinimizedTemplateSelectorProperty, value);
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override ControlTemplate LeftTemplate
        {
            get
            {
                return base.LeftTemplate;
            }
            set
            {
                throw new NotSupportedException("LeftTemplate is not supported by RadOutlookBar");
            }
        }

        public double MinContentHeight
        {
            get
            {
                return (double) base.GetValue(MinContentHeightProperty);
            }
            set
            {
                base.SetValue(MinContentHeightProperty, value);
            }
        }

        public double MinimizedAreaMinHeight
        {
            get
            {
                return (double) base.GetValue(MinimizedAreaMinHeightProperty);
            }
            set
            {
                base.SetValue(MinimizedAreaMinHeightProperty, value);
            }
        }

        public Visibility MinimizedAreaVisibility
        {
            get
            {
                return (Visibility) base.GetValue(MinimizedAreaVisibilityProperty);
            }
            set
            {
                base.SetValue(MinimizedAreaVisibilityProperty, value);
            }
        }

        internal Dictionary<object, RadOutlookBarItem> MinimizedItemsOutlookBarItems
        {
            get
            {
                return this.minimizedItemsOutlookBarItems;
            }
        }

        internal Telerik.Windows.Controls.OutlookBar.MinimizedOutlookBarArea MinimizedOutlookBarArea
        {
            get
            {
                return this.minimizedArea;
            }
        }

        internal Telerik.Windows.Controls.OutlookBar.OutlookBarPanel OutlookBarPanel
        {
            get
            {
                return (base.TabStrip as Telerik.Windows.Controls.OutlookBar.OutlookBarPanel);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public override bool ReorderTabRows
        {
            get
            {
                return base.ReorderTabRows;
            }
            set
            {
                throw new NotSupportedException("ReorderTabRows is not supported by RadOutlookBar");
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override ControlTemplate RightTemplate
        {
            get
            {
                return base.RightTemplate;
            }
            set
            {
                throw new NotSupportedException("RightTemplate is not supported by RadOutlookBar");
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override TabControlScrollMode ScrollMode
        {
            get
            {
                return base.ScrollMode;
            }
            set
            {
                throw new NotSupportedException("ScrollMode is not supported by RadOutlookBar");
            }
        }

        [Obsolete("This property is obsolete.", false)]
        public DataTemplate SelectedItemHeaderTemplate
        {
            get
            {
                return (DataTemplate) base.GetValue(SelectedItemHeaderTemplateProperty);
            }
            set
            {
                base.SetValue(SelectedItemHeaderTemplateProperty, value);
            }
        }

        [Obsolete("This property is obsolete.", false)]
        public DataTemplateSelector SelectedItemHeaderTemplateSelector
        {
            get
            {
                return (DataTemplateSelector) base.GetValue(SelectedItemHeaderTemplateSelectorProperty);
            }
            set
            {
                base.SetValue(SelectedItemHeaderTemplateSelectorProperty, value);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public override Orientation TabOrientation
        {
            get
            {
                return base.TabOrientation;
            }
            set
            {
                throw new NotSupportedException("TabOrientation is not supported by RadOutlookBar");
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override Dock TabStripPlacement
        {
            get
            {
                return base.TabStripPlacement;
            }
            set
            {
                throw new NotSupportedException("TabStripPlacement is not supported by RadOutlookBar");
            }
        }

        public DataTemplate TitleTemplate
        {
            get
            {
                return (DataTemplate) base.GetValue(TitleTemplateProperty);
            }
            set
            {
                base.SetValue(TitleTemplateProperty, value);
            }
        }

        public DataTemplateSelector TitleTemplateSelector
        {
            get
            {
                return (DataTemplateSelector) base.GetValue(TitleTemplateSelectorProperty);
            }
            set
            {
                base.SetValue(TitleTemplateSelectorProperty, value);
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override ControlTemplate TopTemplate
        {
            get
            {
                return base.TopTemplate;
            }
            set
            {
                throw new NotSupportedException("TopTemplate is not supported by RadOutlookBar");
            }
        }
    }
}

