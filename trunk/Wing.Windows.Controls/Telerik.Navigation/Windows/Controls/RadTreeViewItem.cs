namespace Telerik.Windows.Controls
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Windows;
    using System.Windows.Automation;
    using System.Windows.Automation.Peers;
    using Wing.Windows.Controls.Telerik.Navigation.Windows.Controls;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Shapes;
    using System.Windows.Threading;
    using Telerik.Windows;
    using Telerik.Windows.Controls.Animation;
    using Telerik.Windows.Controls.TreeView;

    [DefaultEvent("Selected"), TemplateVisualState(Name="LoadingOnDemand", GroupName="LoadingOnDemandStates"), ScriptableType, DefaultProperty("IsSelected"), StyleTypedProperty(Property="ItemContainerStyle", StyleTargetType=typeof(RadTreeViewItem)), TemplateVisualState(Name="Unselected", GroupName="SelectionStates"), TemplateVisualState(Name="MouseOver", GroupName="CommonStates"), TemplateVisualState(Name="Selected", GroupName="SelectionStates"), TemplatePart(Name="Header", Type=typeof(ContentPresenter))]
    public class RadTreeViewItem : EditableHeaderedItemsControl, TreeViewPanel.IProvideStackingSize, TreeViewPanel.ICachable
    {
        private CheckBox checkBoxElement;
        public static readonly Telerik.Windows.RoutedEvent CheckedEvent = EventManager.RegisterRoutedEvent("Checked", RoutingStrategy.Bubble, typeof(EventHandler<RadRoutedEventArgs>), typeof(RadTreeViewItem));
        private static readonly CheckElement checkElementInstance = new CheckElement();
        public static readonly DependencyProperty CheckStateProperty = DependencyPropertyExtensions.Register("CheckState", typeof(ToggleState), typeof(RadTreeViewItem), new Telerik.Windows.PropertyMetadata(ToggleState.Off, new PropertyChangedCallback(RadTreeViewItem.OnCheckStatePropertyChanged), new CoerceValueCallback(RadTreeViewItem.CheckStatePropertyChangedConstrainValue)));
        private int clickCount;
        private bool clickSkipIntervalDone = true;
        public static readonly Telerik.Windows.RoutedEvent CollapsedEvent = EventManager.RegisterRoutedEvent("Collapsed", RoutingStrategy.Bubble, typeof(EventHandler<RadRoutedEventArgs>), typeof(RadTreeViewItem));
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId="Src")]
        public static readonly DependencyProperty DefaultImageSrcProperty = DependencyProperty.Register("DefaultImageSrc", typeof(object), typeof(RadTreeViewItem), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadTreeViewItem.OnDefaultImageSrcPropertyChanged)));
        private DispatcherTimer doubleClickTimer;
        public static readonly Telerik.Windows.RoutedEvent ExpandedEvent = EventManager.RegisterRoutedEvent("Expanded", RoutingStrategy.Bubble, typeof(EventHandler<RadRoutedEventArgs>), typeof(RadTreeViewItem));
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId="Src")]
        public static readonly DependencyProperty ExpandedImageSrcProperty = DependencyProperty.Register("ExpandedImageSrc", typeof(object), typeof(RadTreeViewItem), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadTreeViewItem.OnExpandedImageSrcPropertyChanged)));
        private ToggleButton expanderElement;
        private FrameworkElement headerContainerElement;
        private FrameworkElement headerRow;
        private FrameworkElement horizontalLineElement;
        private Image imageElement;
        private Panel indentContainerElement;
        private Rectangle indentFirstVerticalLine;
        private bool isCaching;
        public static readonly DependencyProperty IsCheckBoxEnabledProperty = DependencyProperty.Register("IsCheckBoxEnabled", typeof(bool), typeof(RadTreeViewItem), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadTreeViewItem.OnIsCheckBoxEnabledChanged)));
        public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register("IsChecked", typeof(bool?), typeof(RadTreeViewItem), new Telerik.Windows.PropertyMetadata(false, new PropertyChangedCallback(RadTreeViewItem.OnIsCheckedPropertyChanged)));
        private bool isCheckStateDirty;
        public static readonly DependencyProperty IsDragOverProperty = DependencyProperty.Register("IsDragOver", typeof(bool), typeof(RadTreeViewItem), new Telerik.Windows.PropertyMetadata(false, new PropertyChangedCallback(RadTreeViewItem.OnIsDragOverChanged)));
        public static readonly DependencyProperty IsDropAllowedProperty = DependencyProperty.Register("IsDropAllowed", typeof(bool), typeof(RadTreeViewItem), new Telerik.Windows.PropertyMetadata(true));
        private bool isEditPending;
        internal static readonly DependencyProperty IsExpandAllPendingProperty = DependencyProperty.Register("IsExpandAllPending", typeof(bool), typeof(RadTreeViewItem), null);
        public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register("IsExpanded", typeof(bool), typeof(RadTreeViewItem), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadTreeViewItem.OnIsExpandedPropertyChanged)));
        private bool isFocusChangingKeyPressed;
        internal static readonly DependencyProperty IsInCheckBoxPropagateStateModePropery = DependencyProperty.Register("IsInCheckBoxPropagateStateMode", typeof(bool), typeof(RadTreeViewItem), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadTreeViewItem.OnIsInCheckBoxPropagateStateModeChanged)));
        public static readonly DependencyProperty IsLoadingOnDemandProperty = DependencyProperty.Register("IsLoadingOnDemand", typeof(bool), typeof(RadTreeViewItem), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadTreeViewItem.OnIsLoadingOnDemandPropertyChanged)));
        public static readonly DependencyProperty IsLoadOnDemandEnabledProperty = DependencyProperty.Register("IsLoadOnDemandEnabled", typeof(bool), typeof(RadTreeViewItem), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadTreeViewItem.OnIsLoadOnDemandEnabledPropertyChanged)));
        public static readonly DependencyProperty IsRadioButtonEnabledProperty = DependencyProperty.Register("IsRadioButtonEnabled", typeof(bool), typeof(RadTreeViewItem), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadTreeViewItem.OnIsRadioButtonEnabledChanged)));
        private bool isRenderPending;
        public static readonly DependencyProperty IsSelectedProperty = DependencyPropertyExtensions.Register("IsSelected", typeof(bool), typeof(RadTreeViewItem), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadTreeViewItem.OnIsSelectedPropertyChanged)));
        public static readonly DependencyPropertyKey IsSelectionActivePropertyKey = DependencyPropertyExtensions.RegisterReadOnly("IsSelectionActive", typeof(bool), typeof(RadTreeViewItem), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadTreeViewItem.OnIsSelectionActivePropertyChanged)));
        public static readonly DependencyProperty IsSelectionActiveProperty = IsSelectionActivePropertyKey.DependencyProperty;
        private TreeViewPanel itemsHost;
        public static readonly DependencyProperty ItemsOptionListTypeProperty = DependencyProperty.Register("ItemsOptionListType", typeof(OptionListType), typeof(RadTreeViewItem), new Telerik.Windows.PropertyMetadata(OptionListType.Default, new PropertyChangedCallback(RadTreeViewItem.OnItemsOptionListTypePropertyChanged)));
        private FrameworkElement itemsPresenterElement;
        private bool justChangeIsSelected;
        private bool justSetIsInEditMode;
        private Size lastAvailableSize;
        private bool lastIsExpanedValue;
        private Rect lastViewport;
        private Brush lineBrush;
        private Panel listRootContainerElement;
        public static readonly Telerik.Windows.RoutedEvent LoadOnDemandEvent = EventManager.RegisterRoutedEvent("LoadOnDemand", RoutingStrategy.Bubble, typeof(EventHandler<RadRoutedEventArgs>), typeof(RadTreeViewItem));
        private Telerik.Windows.Controls.OptionElement optionElement;
        public static readonly DependencyProperty OptionTypeProperty = DependencyProperty.Register("OptionType", typeof(OptionListType), typeof(RadTreeViewItem), new Telerik.Windows.PropertyMetadata(OptionListType.Default, new PropertyChangedCallback(RadTreeViewItem.OnOptionTypePropertyChanged)));
        private WeakReference ownerReference;
        private WeakReference parentItemReference;
        private WeakReference parentTreeViewReference;
        public static readonly Telerik.Windows.RoutedEvent PreviewCheckedEvent = EventManager.RegisterRoutedEvent("PreviewChecked", RoutingStrategy.Tunnel, typeof(EventHandler<RadRoutedEventArgs>), typeof(RadTreeViewItem));
        public static readonly Telerik.Windows.RoutedEvent PreviewCollapsedEvent = EventManager.RegisterRoutedEvent("PreviewCollapsed", RoutingStrategy.Tunnel, typeof(EventHandler<RadRoutedEventArgs>), typeof(RadTreeViewItem));
        public static readonly Telerik.Windows.RoutedEvent PreviewExpandedEvent = EventManager.RegisterRoutedEvent("PreviewExpanded", RoutingStrategy.Tunnel, typeof(EventHandler<RadRoutedEventArgs>), typeof(RadTreeViewItem));
        public static readonly Telerik.Windows.RoutedEvent PreviewSelectedEvent = EventManager.RegisterRoutedEvent("PreviewSelected", RoutingStrategy.Tunnel, typeof(EventHandler<RadRoutedEventArgs>), typeof(RadTreeViewItem));
        public static readonly Telerik.Windows.RoutedEvent PreviewUncheckedEvent = EventManager.RegisterRoutedEvent("PreviewUnchecked", RoutingStrategy.Tunnel, typeof(EventHandler<RadRoutedEventArgs>), typeof(RadTreeViewItem));
        public static readonly Telerik.Windows.RoutedEvent PreviewUnselectedEvent = EventManager.RegisterRoutedEvent("PreviewUnselected", RoutingStrategy.Tunnel, typeof(EventHandler<RadRoutedEventArgs>), typeof(RadTreeViewItem));
        private bool processSelectionOnMouseUp;
        private RadioButton radioButtonElement;
        private static readonly RadioElement radioElementInstance = new RadioElement();
        private Panel rootElement;
        public static readonly Telerik.Windows.RoutedEvent SelectedEvent = EventManager.RegisterRoutedEvent("Selected", RoutingStrategy.Bubble, typeof(EventHandler<RadRoutedEventArgs>), typeof(RadTreeViewItem));
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId="Src")]
        public static readonly DependencyProperty SelectedImageSrcProperty = DependencyProperty.Register("SelectedImageSrc", typeof(object), typeof(RadTreeViewItem), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadTreeViewItem.OnSelectedImageSrcPropertyChanged)));
        private ToggleButton toggleElement;
        public static readonly Telerik.Windows.RoutedEvent UncheckedEvent = EventManager.RegisterRoutedEvent("Unchecked", RoutingStrategy.Bubble, typeof(EventHandler<RadRoutedEventArgs>), typeof(RadTreeViewItem));
        public static readonly Telerik.Windows.RoutedEvent UnselectedEvent = EventManager.RegisterRoutedEvent("Unselected", RoutingStrategy.Bubble, typeof(EventHandler<RadRoutedEventArgs>), typeof(RadTreeViewItem));
        private UserInitiatedCheck userCheckAction = UserInitiatedCheck.None;
        private FrameworkElement verticalLineElement;
        private bool wasDragged;

        [Description("Occurs when the item is checked."), Category("Behavior"), ScriptableMember]
        public event EventHandler<RadRoutedEventArgs> Checked
        {
            add
            {
                this.AddHandler(CheckedEvent, value);
            }
            remove
            {
                this.RemoveHandler(CheckedEvent, value);
            }
        }

        [ScriptableMember, Category("Behavior"), Description("Occurs when a item is collapsed.")]
        public event EventHandler<RadRoutedEventArgs> Collapsed
        {
            add
            {
                this.AddHandler(CollapsedEvent, value);
            }
            remove
            {
                this.RemoveHandler(CollapsedEvent, value);
            }
        }

        [Category("Behavior"), ScriptableMember, Description("Occurs when a item is expanded.")]
        public event EventHandler<RadRoutedEventArgs> Expanded
        {
            add
            {
                this.AddHandler(ExpandedEvent, value);
            }
            remove
            {
                this.RemoveHandler(ExpandedEvent, value);
            }
        }

        [ScriptableMember, Category("Behavior"), Description("Occurs when the tree item should load its child items on demand.")]
        public event EventHandler<RadRoutedEventArgs> LoadOnDemand
        {
            add
            {
                this.AddHandler(LoadOnDemandEvent, value);
            }
            remove
            {
                this.RemoveHandler(LoadOnDemandEvent, value);
            }
        }

        [ScriptableMember, Category("Behavior"), Description("Occurs before an item is checked.")]
        public event EventHandler<RadRoutedEventArgs> PreviewChecked
        {
            add
            {
                this.AddHandler(PreviewCheckedEvent, value);
            }
            remove
            {
                this.RemoveHandler(PreviewCheckedEvent, value);
            }
        }

        [Description("Occurs before an item is collapsed."), ScriptableMember, Category("Behavior")]
        public event EventHandler<RadRoutedEventArgs> PreviewCollapsed
        {
            add
            {
                this.AddHandler(PreviewCollapsedEvent, value);
            }
            remove
            {
                this.RemoveHandler(PreviewCollapsedEvent, value);
            }
        }

        [Category("Behavior"), Description("Occurs before an item is expanded."), ScriptableMember]
        public event EventHandler<RadRoutedEventArgs> PreviewExpanded
        {
            add
            {
                this.AddHandler(PreviewExpandedEvent, value);
            }
            remove
            {
                this.RemoveHandler(PreviewExpandedEvent, value);
            }
        }

        [Category("Behavior"), ScriptableMember, Description("Occurs before the tree item is selected.")]
        public event EventHandler<RadRoutedEventArgs> PreviewSelected
        {
            add
            {
                this.AddHandler(PreviewSelectedEvent, value);
            }
            remove
            {
                this.RemoveHandler(PreviewSelectedEvent, value);
            }
        }

        [Category("Behavior"), ScriptableMember, Description("Occurs before an item is unchecked.")]
        public event EventHandler<RadRoutedEventArgs> PreviewUnchecked
        {
            add
            {
                this.AddHandler(PreviewUncheckedEvent, value);
            }
            remove
            {
                this.RemoveHandler(PreviewUncheckedEvent, value);
            }
        }

        [Category("Behavior"), Description("Occurs before an item is unselected."), ScriptableMember]
        public event EventHandler<RadRoutedEventArgs> PreviewUnselected
        {
            add
            {
                this.AddHandler(PreviewUnselectedEvent, value);
            }
            remove
            {
                this.RemoveHandler(PreviewUnselectedEvent, value);
            }
        }

        [Description("Occurs when a item is selected."), ScriptableMember, Category("Behavior")]
        public event EventHandler<RadRoutedEventArgs> Selected
        {
            add
            {
                this.AddHandler(SelectedEvent, value);
            }
            remove
            {
                this.RemoveHandler(SelectedEvent, value);
            }
        }

        [ScriptableMember, Description("Occurs when the item is unchecked."), Category("Behavior")]
        public event EventHandler<RadRoutedEventArgs> Unchecked
        {
            add
            {
                this.AddHandler(UncheckedEvent, value);
            }
            remove
            {
                this.RemoveHandler(UncheckedEvent, value);
            }
        }

        [ScriptableMember, Description("Occurs when a item is unselected."), Category("Behavior")]
        public event EventHandler<RadRoutedEventArgs> Unselected
        {
            add
            {
                this.AddHandler(UnselectedEvent, value);
            }
            remove
            {
                this.RemoveHandler(UnselectedEvent, value);
            }
        }

        static RadTreeViewItem()
        {
            EventManager.RegisterClassHandler(typeof(RadTreeViewItem), TreeViewPanel.RequestBringIntoViewEvent, new RequestBringIntoViewEventHandler(RadTreeViewItem.OnRequestBringIntoView));
        }

        public RadTreeViewItem()
        {
            base.DefaultStyleKey = typeof(RadTreeViewItem);
            base.IsTabStop = true;
            base.TabNavigation = KeyboardNavigationMode.Once;
            this.DropPosition = Telerik.Windows.Controls.DropPosition.Inside;
        }

        public override bool BeginEdit()
        {
            if (this.ParentTreeView == null)
            {
                this.isEditPending = true;
                return false;
            }
            if (!this.ParentTreeView.IsEditable || (this.ParentTreeView.CurrentEditedItem != null))
            {
                return false;
            }
            bool successfulBegin = base.BeginEdit();
            this.isEditPending = false;
            if (successfulBegin)
            {
                this.IsSelected = true;
                this.ParentTreeView.CurrentEditedItem = this;
            }
            return successfulBegin;
        }

        private void BindEvents()
        {
            if (this.expanderElement != null)
            {
                this.expanderElement.Click += new RoutedEventHandler(this.ExpanderElement_Click);
            }
            if (this.checkBoxElement != null)
            {
                this.checkBoxElement.Click += new RoutedEventHandler(this.CheckBoxElement_Click);
            }
            if (this.radioButtonElement != null)
            {
                this.radioButtonElement.Click += new RoutedEventHandler(this.RadioButtonElement_Click);
            }
            if (base.HeaderEditPresenterElement != null)
            {
                base.HeaderEditPresenterElement.KeyDown += new KeyEventHandler(this.OnHeaderEditElementKeyDown);
            }
            if (this.headerRow != null)
            {
                this.headerRow.MouseLeftButtonDown += new MouseButtonEventHandler(this.OnHeaderMouseLeftButtonDown);
                this.headerRow.MouseEnter += new MouseEventHandler(this.OnHeaderMouseEnter);
                this.headerRow.MouseLeave += new MouseEventHandler(this.OnHeaderMouseLeave);
                this.headerRow.MouseLeftButtonUp += new MouseButtonEventHandler(this.OnHeaderMouseLeftButtonUp);
                this.headerRow.SizeChanged += new SizeChangedEventHandler(this.HeaderRow_SizeChanged);
            }
        }

        private void BindXAMLElements()
        {
            this.rootElement = base.GetTemplateChild("RootElement") as Panel;
            this.horizontalLineElement = base.GetTemplateChild("HorizontalLine") as FrameworkElement;
            this.verticalLineElement = base.GetTemplateChild("VerticalLine") as FrameworkElement;
            this.listRootContainerElement = base.GetTemplateChild("ListRootContainer") as Panel;
            if (this.rootElement != null)
            {
                this.lineBrush = this.rootElement.Resources["LineBrush"] as Brush;
            }
            if (this.lineBrush == null)
            {
                Shape rectangleElement = this.verticalLineElement as Shape;
                if (rectangleElement != null)
                {
                    this.lineBrush = rectangleElement.Stroke;
                }
            }
            this.indentFirstVerticalLine = (Rectangle) base.GetTemplateChild("IndentFirstVerticalLine");
            this.indentContainerElement = (Panel) base.GetTemplateChild("IndentContainer");
            this.checkBoxElement = (CheckBox) base.GetTemplateChild("CheckBoxElement");
            if (this.checkBoxElement != null)
            {
                this.checkBoxElement.IsTabStop = false;
            }
            this.radioButtonElement = (RadioButton) base.GetTemplateChild("RadioButtonElement");
            if (this.radioButtonElement != null)
            {
                this.radioButtonElement.IsTabStop = false;
            }
            this.imageElement = (Image) base.GetTemplateChild("Image");
            this.headerContainerElement = (FrameworkElement) base.GetTemplateChild("Header");
            this.headerRow = (FrameworkElement) base.GetTemplateChild("HeaderRow");
            this.expanderElement = (ToggleButton) base.GetTemplateChild("Expander");
            if (this.expanderElement != null)
            {
                this.expanderElement.IsTabStop = false;
            }
            if ((this.expanderElement != null) && (this.ParentTreeView != null))
            {
                this.expanderElement.Style = this.ParentTreeView.ExpanderStyle;
            }
            this.itemsPresenterElement = base.GetTemplateChild("ItemsHost") as FrameworkElement;
        }

        public void BringIndexIntoView(int index)
        {
            if (((index > -1) && (index < base.Items.Count)) && (this.itemsHost != null))
            {
                this.itemsHost.BringIndexIntoViewInternal(index);
            }
        }

        public void BringIntoView()
        {
            this.BringIntoView();
        }

        public void BringItemIntoView(object item)
        {
            int itemIndex = base.Items.IndexOf(item);
            this.BringIndexIntoView(itemIndex);
        }

        internal static ToggleState CalculateInitialItemCheckState(RadTreeViewItem item)
        {
            foreach (object child in item.Items)
            {
                RadTreeViewItem childContainer = child as RadTreeViewItem;
                if (childContainer != null)
                {
                    CalculateInitialItemCheckState(childContainer);
                }
                else
                {
                    return item.CheckState;
                }
            }
            return CalculateItemCheckState(item);
        }

        internal static ToggleState CalculateItemCheckState(RadTreeViewItem item)
        {
            int checkedCount = 0;
            int indetermCount = 0;
            int itemCount = item.Items.Count;
            RadTreeView parentTreeView = item.ParentTreeView;
            if (parentTreeView != null)
            {
                for (int index = 0; index < itemCount; index++)
                {
                    switch (parentTreeView.GetCheckStateValue(item.Items[index]))
                    {
                        case ToggleState.On:
                            checkedCount++;
                            break;

                        case ToggleState.Indeterminate:
                            indetermCount++;
                            break;
                    }
                }
            }
            else
            {
                for (int index = 0; index < itemCount; index++)
                {
                    RadTreeViewItem container = item.Items[index] as RadTreeViewItem;
                    if (container == null)
                    {
                        break;
                    }
                    switch (container.CheckState)
                    {
                        case ToggleState.On:
                            checkedCount++;
                            break;

                        case ToggleState.Indeterminate:
                            indetermCount++;
                            break;
                    }
                }
            }
            ToggleState state = ToggleState.Off;
            if (item.Items.Count > 0)
            {
                if (checkedCount == item.Items.Count)
                {
                    return ToggleState.On;
                }
                if ((checkedCount + indetermCount) > 0)
                {
                    state = ToggleState.Indeterminate;
                }
                return state;
            }
            return item.CheckState;
        }

        public override bool CancelEdit()
        {
            bool cancelSucceeded = false;
            this.isEditPending = false;
            this.justSetIsInEditMode = true;
            try
            {
                cancelSucceeded = base.CancelEdit();
                if (!cancelSucceeded)
                {
                    return cancelSucceeded;
                }
                base.Dispatcher.BeginInvoke(delegate {
                    this.FixFocusOnCancelCommitEdit();
                });
                if (this.ParentTreeView != null)
                {
                    this.ParentTreeView.CurrentEditedItem = null;
                }
            }
            finally
            {
                this.justSetIsInEditMode = false;
            }
            return cancelSucceeded;
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic"), SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId="useTransitions")]
        protected internal override void ChangeVisualState(bool useTransitions)
        {
            if (this.ParentTreeView != null)
            {
                if (this.IsLoadingOnDemand)
                {
                    this.GoToState(useTransitions, new string[] { "LoadingOnDemand" });
                }
                else
                {
                    this.GoToState(useTransitions, new string[] { "LoadingOnDemandReverse" });
                }
                if (this.IsMouseOverState || this.IsDragOver)
                {
                    this.GoToState(useTransitions, new string[] { "MouseOver" });
                }
                else
                {
                    this.GoToState(useTransitions, new string[] { "Normal" });
                }
                if (this.IsSelected)
                {
                    if (this.ParentTreeView.IsSelectionActive)
                    {
                        this.GoToState(useTransitions, new string[] { "Selected" });
                    }
                    else
                    {
                        this.GoToState(useTransitions, new string[] { "SelectedUnfocused" });
                    }
                }
                else
                {
                    this.GoToState(useTransitions, new string[] { "Unselected" });
                }
                if (base.IsFocused)
                {
                    this.GoToState(useTransitions, new string[] { "Focused" });
                }
                else
                {
                    this.GoToState(useTransitions, new string[] { "Unfocused" });
                }
                if (this.IsExpanded)
                {
                    if (this.itemsPresenterElement != null)
                    {
                        this.itemsPresenterElement.Visibility = Visibility.Visible;
                    }
                    this.GoToState(useTransitions, new string[] { "Expanded" });
                    if (!this.lastIsExpanedValue && useTransitions)
                    {
                        EventHandler playExpand = null;
                        playExpand = delegate (object sender, EventArgs layourEventArgs) {
                            AnimationManager.Play(this, "Expand");
                            this.LayoutUpdated -= playExpand;
                        };
                        base.LayoutUpdated += playExpand;
                        base.Dispatcher.BeginInvoke(new Action(this.InvalidateMeasure));
                    }
                    this.lastIsExpanedValue = true;
                }
                else
                {
                    if (this.lastIsExpanedValue)
                    {
                        AnimationManager.Play(this, "Collapse", delegate {
                            this.OnCollapseAnimatonComplete();
                        }, new object[0]);
                    }
                    this.lastIsExpanedValue = false;
                    this.GoToState(useTransitions, new string[] { "Collapsed" });
                }
                if (!base.IsEnabled)
                {
                    this.GoToState(useTransitions, new string[] { "Disabled" });
                }
                if (base.IsInEditMode)
                {
                    this.GoToState(useTransitions, new string[] { "Edit" });
                }
                else
                {
                    this.GoToState(useTransitions, new string[] { "Display" });
                }
            }
        }

        internal void CheckBoxElement_Click(object sender, RoutedEventArgs e)
        {
            if (base.IsEnabled && this.IsCheckBoxEnabled)
            {
                ToggleState checkState = (this.CheckState == ToggleState.On) ? ToggleState.Off : ToggleState.On;
                this.userCheckAction = UserInitiatedCheck.PreviewEventPending;
                this.CheckState = checkState;
                this.optionElement.Render(this.toggleElement, this.CheckState);
            }
        }

        private static object CheckStatePropertyChangedConstrainValue(DependencyObject d, object newValue)
        {
            RadTreeViewItem item = d as RadTreeViewItem;
            if (item.HasTemplate)
            {
                if ((item.ParentTreeView != null) && !item.ParentTreeView.IsOptionElementsEnabled)
                {
                    return ToggleState.Off;
                }
                if (((ToggleState) newValue) != ToggleState.Off)
                {
                    if (item.OnPreviewChecked(new RadTreeViewCheckEventArgs(PreviewCheckedEvent, item)))
                    {
                        return ToggleState.Off;
                    }
                    return newValue;
                }
                if (item.OnPreviewUnchecked(new RadTreeViewCheckEventArgs(PreviewUncheckedEvent, item)))
                {
                    return ToggleState.On;
                }
            }
            return newValue;
        }

        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            if (this.ParentTreeView != null)
            {
                this.ParentTreeView.ClearContainerForDescendant(element, item);
            }
            base.ClearContainerForItemOverride(element, item);
        }

        [ScriptableMember, Description("Collapses the item and its child items recursively.")]
        public void CollapseAll()
        {
            this.IsExpanded = false;
            this.IsExpandAllPending = false;
            if (this.ParentTreeView != null)
            {
                this.ParentTreeView.SetExpandState(base.Items, false, true);
            }
        }

        private void CollapseWithNoAnimation()
        {
            bool originalIsAnimated = AnimationManager.GetIsAnimationEnabled(this);
            AnimationManager.SetIsAnimationEnabled(this, false);
            this.IsExpanded = false;
            AnimationManager.SetIsAnimationEnabled(this, originalIsAnimated);
        }

        public override bool CommitEdit()
        {
            bool commitSuccess;
            this.justSetIsInEditMode = true;
            try
            {
                commitSuccess = base.CommitEdit();
                if (!commitSuccess)
                {
                    return commitSuccess;
                }
                base.Dispatcher.BeginInvoke(delegate {
                    this.FixFocusOnCancelCommitEdit();
                });
                if (this.ParentTreeView != null)
                {
                    this.ParentTreeView.CurrentEditedItem = null;
                }
            }
            finally
            {
                this.justSetIsInEditMode = false;
            }
            return commitSuccess;
        }

        [Description("Checks if the item has a particular item in its collection"), ScriptableMember]
        public bool Contains(RadTreeViewItem item)
        {
            if (item != null)
            {
                for (RadTreeViewItem item1 = item.ParentItem; item1 != null; item1 = item1.ParentItem)
                {
                    if (item1 == this)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        internal void CreateOptionElement()
        {
            if (this.optionElement != null)
            {
                if (((this.optionElement is CheckElement) && this.IsCheckBoxEnabled) && (this.toggleElement is CheckBox))
                {
                    return;
                }
                if (((this.optionElement is RadioElement) && this.IsRadioButtonEnabled) && (this.toggleElement is RadioButton))
                {
                    return;
                }
            }
            if (this.IsCheckBoxEnabled)
            {
                this.optionElement = checkElementInstance;
                this.toggleElement = this.checkBoxElement;
            }
            else if (this.IsRadioButtonEnabled)
            {
                this.optionElement = radioElementInstance;
                this.toggleElement = this.radioButtonElement;
            }
        }

        private void DoubleClickTimer_Tick(object sender, EventArgs e)
        {
            this.doubleClickTimer.Stop();
            this.clickCount = 0;
            this.clickSkipIntervalDone = true;
        }

        [Obsolete("Use CancelEdit(), CommitEdit() and the IsInEditMode property instead.", false), ScriptableMember]
        public bool EndEdit(bool cancelEdit)
        {
            if (cancelEdit)
            {
                return this.CancelEdit();
            }
            return this.CommitEdit();
        }

        private void EnsureItemsHost()
        {
            if (((this.itemsHost == null) && (this.itemsPresenterElement != null)) && (VisualTreeHelper.GetChildrenCount(this.itemsPresenterElement) > 0))
            {
                this.itemsHost = VisualTreeHelper.GetChild(this.itemsPresenterElement, 0) as TreeViewPanel;
            }
        }

        [ScriptableMember]
        public void EnsureVisible()
        {
            for (RadTreeViewItem item1 = this.ParentItem; item1 != null; item1 = item1.ParentItem)
            {
                if (!item1.IsExpanded)
                {
                    item1.IsExpanded = true;
                }
            }
        }

        [ScriptableMember, Description("Expands the item and its child items recursively.")]
        public void ExpandAll()
        {
            this.IsExpanded = true;
            this.IsExpandAllPending = true;
            if (this.ParentTreeView != null)
            {
                this.ParentTreeView.SetExpandState(base.Items, true, true);
            }
        }

        private void ExpanderElement_Click(object sender, RoutedEventArgs e)
        {
            this.IsExpanded = this.expanderElement.IsChecked == true;
        }

        private static RadTreeViewItem FindLastFocusableItem(RadTreeViewItem item)
        {
            RadTreeViewItem item2 = null;
            int index = -1;
            RadTreeViewItem item3 = null;
            while (item != null)
            {
                if (item.IsEnabled)
                {
                    if (!item.IsExpanded || !item.HasItems)
                    {
                        return item;
                    }
                    item2 = item;
                    item3 = item;
                    index = item.Items.Count - 1;
                }
                else
                {
                    if (index <= 0)
                    {
                        break;
                    }
                    index--;
                }
                item = item3.ItemContainerGenerator.ContainerFromIndex(index) as RadTreeViewItem;
            }
            if (item2 != null)
            {
                return item2;
            }
            return null;
        }

        private RadTreeViewItem FindNextFocusableItem(bool walkIntoSubtree)
        {
            if ((walkIntoSubtree && this.IsExpanded) && base.HasItems)
            {
                RadTreeViewItem item = base.ItemContainerGenerator.ContainerFromIndex(0) as RadTreeViewItem;
                if (item != null)
                {
                    if (item.IsEnabled)
                    {
                        return item;
                    }
                    return item.FindNextFocusableItem(false);
                }
            }
            Telerik.Windows.Controls.ItemsControl parentItemsControl = this.Owner;
            if (parentItemsControl != null)
            {
                RadTreeViewItem item2;
                int index = parentItemsControl.ItemContainerGenerator.IndexFromContainer(this);
                int count = parentItemsControl.Items.Count;
                while (index < count)
                {
                    index++;
                    item2 = parentItemsControl.ItemContainerGenerator.ContainerFromIndex(index) as RadTreeViewItem;
                    if ((item2 != null) && item2.IsEnabled)
                    {
                        return item2;
                    }
                }
                item2 = parentItemsControl as RadTreeViewItem;
                if (item2 != null)
                {
                    return item2.FindNextFocusableItem(false);
                }
            }
            return null;
        }

        private Telerik.Windows.Controls.ItemsControl FindPreviousFocusableItem()
        {
            Telerik.Windows.Controls.ItemsControl parentItemsControl = this.Owner;
            if (parentItemsControl == null)
            {
                return null;
            }
            int index = parentItemsControl.ItemContainerGenerator.IndexFromContainer(this);
            while (index > 0)
            {
                index--;
                RadTreeViewItem item = parentItemsControl.ItemContainerGenerator.ContainerFromIndex(index) as RadTreeViewItem;
                if ((item != null) && item.IsEnabled)
                {
                    RadTreeViewItem item2 = FindLastFocusableItem(item);
                    if (item2 != null)
                    {
                        return item2;
                    }
                }
            }
            return parentItemsControl;
        }

        private void FixFocusOnCancelCommitEdit()
        {
            if (this.isFocusChangingKeyPressed)
            {
                base.Focus();
            }
            this.isFocusChangingKeyPressed = false;
        }

        private void ForceUpdateIndexTree()
        {
            if ((((this.Owner != null) && (this.ParentTreeView != null)) && this.ParentTreeView.IsVirtualizing) && (this.Owner.ItemContainerGenerator.IndexFromContainer(this) != -1))
            {
                TreeViewPanel parentItemsHost = null;
                if (this.ParentTreeView != null)
                {
                    parentItemsHost = this.ParentTreeView.ItemsHost;
                }
                if (this.ParentItem != null)
                {
                    parentItemsHost = this.ParentItem.ItemsHost;
                }
                if (parentItemsHost != null)
                {
                    parentItemsHost.OnTreeViewItemCollapsed(this.Index);
                }
            }
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            RadTreeView parentTree = this.ParentTreeView;
            if (parentTree != null)
            {
                return parentTree.GetContainerForTreeViewItem();
            }
            return new RadTreeViewItem();
        }

        public virtual Telerik.Windows.Controls.DropPosition GetDropPositionFromPoint(Point absoluteMousePosition)
        {
            if (this.HeaderRow != null)
            {
                Point headerTopPoint = this.HeaderRow.TransformToVisual(null).Transform(new Point());
                double mouseTop = absoluteMousePosition.Y - headerTopPoint.Y;
                if (mouseTop < ((this.HeaderRow.ActualHeight * 1.0) / 4.0))
                {
                    return Telerik.Windows.Controls.DropPosition.Before;
                }
                if (mouseTop > ((this.HeaderRow.ActualHeight * 3.0) / 4.0))
                {
                    return Telerik.Windows.Controls.DropPosition.After;
                }
            }
            return Telerik.Windows.Controls.DropPosition.Inside;
        }

        private Control GetFirstFocusableElement()
        {
            Control firstfocusable = base.HeaderEditPresenterElement.GetFirstDescendantOfType<Control>();
            if (firstfocusable != null)
            {
                return firstfocusable;
            }
            return null;
        }

        private static RadTreeViewItem GetFirstVisibleChild(RadTreeViewItem itemContainer)
        {
            if (itemContainer.IsExpanded && (itemContainer.Items.Count > 0))
            {
                int itemsCount = itemContainer.Items.Count;
                for (int i = 0; i < itemsCount; i++)
                {
                    RadTreeViewItem childContainer = itemContainer.ItemContainerGenerator.ContainerFromIndex(i) as RadTreeViewItem;
                    if (childContainer != null)
                    {
                        return childContainer;
                    }
                }
            }
            return itemContainer;
        }

        internal string GetImageSrc(string src)
        {
            if ((this.ParentTreeView != null) && !string.IsNullOrEmpty(this.ParentTreeView.ImagesBaseDir))
            {
                src = System.IO.Path.Combine(this.ParentTreeView.ImagesBaseDir, src);
            }
            return src;
        }

        private Queue<bool> GetIndentQueue()
        {
            Queue<bool> indentQueue = new Queue<bool>();
            if (this.Level > 0)
            {
                indentQueue = this.ParentItem.GetIndentQueue();
            }
            if (this.ParentTreeView != null)
            {
                indentQueue.Enqueue(this.ParentTreeView.ShallLineBePrinted(this) && this.HasNextSibling());
            }
            return indentQueue;
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic"), SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId="stateNames"), SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId="useTransitions")]
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

        private void HandleBringIntoView(RequestBringIntoViewEventArgs e)
        {
            for (RadTreeViewItem item = this.ParentItem; item != null; item = item.ParentItem)
            {
                if (!item.IsExpanded)
                {
                    item.IsExpanded = true;
                }
            }
            if (e.TargetRect.IsEmpty)
            {
                FrameworkElement headerElement = this.HeaderContainerElement;
                if (headerElement != null)
                {
                    e.Handled = true;
                    if (!this.IsInViewport)
                    {
                        if (this.headerContainerElement != null)
                        {
                            double bringIntoViewHeight = this.headerContainerElement.ActualHeight;
                            if ((this.ParentTreeView != null) && (this.ParentTreeView.BringIntoViewMode == BringIntoViewMode.HeaderAndItems))
                            {
                                bringIntoViewHeight = base.DesiredSize.Height;
                            }
                            this.headerContainerElement.BringIntoView(new Rect(-24.0, 0.0, this.headerContainerElement.ActualWidth + 24.0, bringIntoViewHeight));
                        }
                        else
                        {
                            headerElement.BringIntoView(e.TargetRect);
                        }
                    }
                }
            }
        }

        private void HandleKeyboardWhenInEditMode(KeyEventArgs e)
        {
            if (((e.Key == Key.Enter) || (e.Key == Key.Escape)) || ((e.Key == Key.Tab) && !base.IsInEditMode))
            {
                this.isFocusChangingKeyPressed = true;
            }
        }

        internal void HandleSingleClickFunctionality()
        {
            if (this.ParentTreeView != null)
            {
                bool isSingleClick = this.ParentTreeView.IsExpandOnSingleClickEnabled;
                bool isDoubleClick = !isSingleClick && this.ParentTreeView.IsExpandOnDblClickEnabled;
                if ((isSingleClick || isDoubleClick) && ((base.Items.Count != 0) || this.IsLoadOnDemandEnabled))
                {
                    int clickCountToCheck = 0;
                    if (isDoubleClick)
                    {
                        clickCountToCheck = 1;
                    }
                    this.clickCount++;
                    if (this.clickCount > clickCountToCheck)
                    {
                        if (isDoubleClick || (isSingleClick && this.clickSkipIntervalDone))
                        {
                            this.clickCount = 0;
                            this.clickSkipIntervalDone = false;
                            this.IsExpanded = !this.IsExpanded;
                        }
                    }
                    else if (isDoubleClick)
                    {
                        this.StartDoubleClickTime();
                    }
                    if (isSingleClick)
                    {
                        this.StartDoubleClickTime();
                    }
                }
            }
        }

        private bool HasNextSibling()
        {
            Telerik.Windows.Controls.ItemsControl parentItemsControl = this.Owner;
            return ((parentItemsControl != null) && (this.Index < (parentItemsControl.Items.Count - 1)));
        }

        private bool HasPreviousSibling()
        {
            return (this.Index > 0);
        }

        private void HeaderRow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.ChangeVisualState();
            this.RenderIndent();
            this.RenderListRoot();
        }

        internal static void InvalidateRender(RadTreeViewItem containerItem)
        {
            containerItem.isRenderPending = true;
            containerItem.InvalidateMeasure();
            containerItem.InvalidateArrange();
            foreach (object item in containerItem.Items)
            {
                RadTreeViewItem childTreeViewItem = item as RadTreeViewItem;
                if (childTreeViewItem != null)
                {
                    InvalidateRender(childTreeViewItem);
                }
            }
        }

        [ScriptableMember]
        public bool IsBefore(RadTreeViewItem item)
        {
            if (this.RootItem != item.RootItem)
            {
                return (this.RootItem.Index < item.RootItem.Index);
            }
            if (this.Level == item.Level)
            {
                if (this.ParentItem == item.ParentItem)
                {
                    return (this.Index < item.Index);
                }
                return true;
            }
            if (this.Contains(item))
            {
                return true;
            }
            if (item.Contains(this))
            {
                return false;
            }
            RadTreeViewItem commonParent = this.ParentItem;
            while (commonParent != null)
            {
                if (commonParent.Contains(item))
                {
                    break;
                }
                commonParent = commonParent.ParentItem;
            }
            if (commonParent == null)
            {
                return (this.Level < item.Level);
            }
            int index1 = 0;
            IEnumerable<RadTreeViewItem> allItems = this.ParentTreeView.GetAllItemContainers(commonParent);
            foreach (RadTreeViewItem treeItem in allItems)
            {
                if ((treeItem == this) || treeItem.Contains(this))
                {
                    index1 = treeItem.Index;
                    break;
                }
            }
            int index2 = 0;
            foreach (RadTreeViewItem treeItem in allItems)
            {
                if ((treeItem == item) || treeItem.Contains(item))
                {
                    index2 = treeItem.Index;
                    break;
                }
            }
            return (index2 > index1);
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification="It will be refactored.")]
        public virtual bool IsDropPossible(IEnumerable draggedItems)
        {
            RadTreeView treeView = this.ParentTreeView;
            Telerik.Windows.Controls.DropPosition dropPosition = this.DropPosition;
            if ((draggedItems == null) || (this.ParentTreeView == null))
            {
                return false;
            }
            if (((base.ItemsSource != null) && !(base.ItemsSource is IList)) && (dropPosition == Telerik.Windows.Controls.DropPosition.Inside))
            {
                return false;
            }
            if ((dropPosition == Telerik.Windows.Controls.DropPosition.Inside) && !this.IsDropAllowed)
            {
                return false;
            }
            if (((this.Owner != null) && (this.DropPosition != Telerik.Windows.Controls.DropPosition.Inside)) && ((this.Owner.ItemsSource != null) && !(this.Owner.ItemsSource is IList)))
            {
                return false;
            }
            RadTreeViewItem owner = this.Owner as RadTreeViewItem;
            if (((dropPosition != Telerik.Windows.Controls.DropPosition.Inside) && (owner != null)) && !owner.IsDropAllowed)
            {
                return false;
            }
            RadTreeView treeOwner = this.Owner as RadTreeView;
            if (((treeOwner != null) && (dropPosition != Telerik.Windows.Controls.DropPosition.Inside)) && ((treeOwner.ItemsSource != null) && !(treeOwner.ItemsSource is IList)))
            {
                return false;
            }
            foreach (object item in draggedItems)
            {
                RadTreeViewItem itemContainer = treeView.ContainerFromItemRecursive(item);
                if (this == itemContainer)
                {
                    return false;
                }
                if (itemContainer != null)
                {
                    if ((this.DropPosition == Telerik.Windows.Controls.DropPosition.Inside) && (this == itemContainer.ParentItem))
                    {
                        return false;
                    }
                    if (itemContainer.Contains(this))
                    {
                        return false;
                    }
                }
            }
            if (((this.ParentTreeView.ItemsSource != null) && (base.ItemsSource == null)) && (this.DropPosition == Telerik.Windows.Controls.DropPosition.Inside))
            {
                return false;
            }
            return true;
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return RadTreeView.IsItemTreeviewItemContainer(item);
        }

        private void JustChangeIsSelected(bool newValue)
        {
            this.justChangeIsSelected = true;
            try
            {
                this.IsSelected = newValue;
            }
            finally
            {
                this.justChangeIsSelected = false;
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (this.isRenderPending)
            {
                this.isRenderPending = false;
                this.Render();
                if ((this.ParentTreeView != null) && this.ParentTreeView.IsLineEnabled)
                {
                    this.ParentTreeView.UpdateChildItemsLine(this);
                }
            }
            if ((this.isCheckStateDirty && (this.ParentTreeView != null)) && this.ParentTreeView.IsOptionElementsEnabled)
            {
                this.isCheckStateDirty = false;
                this.SetCheckStateWithNoPropagation(CalculateItemCheckState(this));
            }
            MeasureData measureData = TreeViewPanel.GetMeasureData(this);
            Rect viewport = this.lastViewport;
            Size measureDataAvailableSize = this.lastAvailableSize;
            if (measureData != null)
            {
                viewport = measureData.Viewport;
                measureDataAvailableSize = measureData.AvailableSize;
            }
            this.EnsureItemsHost();
            if (((this.itemsHost != null) && this.IsExpanded) && (((this.lastAvailableSize != measureDataAvailableSize) || (viewport.Top != this.lastViewport.Top)) || ((viewport.Left != this.lastViewport.Left) || (Math.Abs((double) (viewport.Bottom - this.lastViewport.Bottom)) > 20.0))))
            {
                this.itemsHost.InvalidateMeasure();
            }
            this.lastViewport = viewport;
            this.lastAvailableSize = measureDataAvailableSize;
            return base.MeasureOverride(availableSize);
        }

        public override void OnApplyTemplate()
        {
            this.UnbindEvents();
            base.OnApplyTemplate();
            this.HasTemplate = true;
            this.BindXAMLElements();
            this.BindEvents();
            if (this.IsCheckBoxEnabled)
            {
                this.toggleElement = this.checkBoxElement;
            }
            if (this.IsRadioButtonEnabled)
            {
                this.toggleElement = this.radioButtonElement;
            }
            this.Render();
            if (this.isEditPending)
            {
                this.isEditPending = false;
                base.Dispatcher.BeginInvoke(delegate {
                    this.BeginEdit();
                });
            }
        }

        protected internal virtual void OnChecked(RadTreeViewCheckEventArgs e)
        {
            if (!this.SupressEventRaising)
            {
                if (this.userCheckAction == UserInitiatedCheck.EventPending)
                {
                    this.userCheckAction = UserInitiatedCheck.None;
                    e.IsUserInitiated = true;
                }
                this.RaiseEvent(e);
            }
        }

        private static void OnCheckStatePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadTreeViewItem treeViewItem = sender as RadTreeViewItem;
            ToggleState newCheckState = (ToggleState) args.NewValue;
            bool newStateIsChecked = newCheckState != ToggleState.Off;
            RadTreeView parentTreeView = treeViewItem.ParentTreeView;
            if ((parentTreeView == null) && !treeViewItem.HasTemplate)
            {
                parentTreeView = treeViewItem.SearchForParentTreeView();
            }
            if (parentTreeView != null)
            {
                parentTreeView.StoreCheckState(treeViewItem, treeViewItem.Item, newCheckState);
            }
            treeViewItem.SetOptionElementCheckState();
            if (newStateIsChecked)
            {
                treeViewItem.OnChecked(new RadTreeViewCheckEventArgs(CheckedEvent, treeViewItem));
            }
            else
            {
                treeViewItem.OnUnchecked(new RadTreeViewCheckEventArgs(UncheckedEvent, treeViewItem));
            }
        }

        private void OnCollapseAnimatonComplete()
        {
            if (!this.IsExpanded)
            {
                this.ForceUpdateIndexTree();
                if (this.itemsPresenterElement != null)
                {
                    this.itemsPresenterElement.Visibility = Visibility.Collapsed;
                    this.itemsPresenterElement.ClearValue(FrameworkElement.HeightProperty);
                }
                base.Dispatcher.BeginInvoke(delegate {
                    if (((this.itemsHost != null) && TreeViewPanel.GetIsVirtualizing(this)) && !this.IsExpanded)
                    {
                        this.itemsHost.ClearAllContainers(this);
                    }
                });
            }
        }

        protected internal virtual void OnCollapsed(RadRoutedEventArgs e)
        {
            if (!this.SupressEventRaising)
            {
                this.RaiseEvent(e);
            }
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new RadTreeViewItemAutomationPeer(this);
        }

        private static void OnDefaultImageSrcPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            RadTreeViewItem item = d as RadTreeViewItem;
            if (item != null)
            {
                item.SetImage();
            }
        }

        protected internal virtual void OnExpanded(RadRoutedEventArgs e)
        {
            if (!this.SupressEventRaising)
            {
                this.RaiseEvent(e);
            }
        }

        private static void OnExpandedImageSrcPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            RadTreeViewItem item = d as RadTreeViewItem;
            if (item != null)
            {
                item.SetImage();
            }
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            if (base.IsEnabled && (e.OriginalSource == this))
            {
                base.IsFocused = true;
                this.ChangeVisualState();
                this.IsKeyboardFocusWithin = true;
                base.OnGotFocus(e);
            }
        }

        [SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers", Justification="There is no need to make the emthod private.")]
        protected virtual void OnHeaderEditElementKeyDown(object sender, KeyEventArgs e)
        {
            this.HandleKeyboardWhenInEditMode(e);
            if (e.Key == Key.Tab)
            {
                if (System.Windows.Input.Keyboard.Modifiers == ModifierKeys.None)
                {
                    Control lastFocusable = (from c in base.HeaderEditPresenterElement.ChildrenOfType<Control>()
                        where c.IsEnabled && c.IsTabStop
                        select c).LastOrDefault<Control>();
                    if (((lastFocusable != null) && (lastFocusable == FocusManager.GetFocusedElement())) && !this.CommitEdit())
                    {
                        this.CancelEdit();
                    }
                }
                else if (System.Windows.Input.Keyboard.Modifiers == ModifierKeys.Shift)
                {
                    Control firstFocusable = (from c in base.HeaderEditPresenterElement.ChildrenOfType<Control>()
                        where c.IsEnabled && c.IsTabStop
                        select c).FirstOrDefault<Control>();
                    if (((firstFocusable != null) && (firstFocusable == FocusManager.GetFocusedElement())) && !this.CommitEdit())
                    {
                        this.CancelEdit();
                    }
                }
            }
            if ((e.Key == Key.Enter) && (System.Windows.Input.Keyboard.Modifiers == ModifierKeys.None))
            {
                if (this.CommitEdit())
                {
                    base.Focus();
                }
                e.Handled = true;
            }
            if ((e.Key == Key.Escape) && (System.Windows.Input.Keyboard.Modifiers == ModifierKeys.None))
            {
                base.Focus();
                this.CancelEdit();
                e.Handled = true;
            }
        }

        private void OnHeaderMouseEnter(object sender, MouseEventArgs e)
        {
            if (base.IsEnabled)
            {
                this.IsMouseOverState = true;
                this.ChangeVisualState();
            }
        }

        private void OnHeaderMouseLeave(object sender, EventArgs e)
        {
            if (base.IsEnabled)
            {
                this.IsMouseOverState = false;
                this.ChangeVisualState();
            }
        }

        internal void OnHeaderMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (base.IsEnabled)
            {
                if (base.Focus() && (e != null))
                {
                    e.Handled = true;
                }
                this.processSelectionOnMouseUp = false;
                if ((!this.IsSelected || (this.ParentTreeView == null)) || (this.ParentTreeView.SelectedItems.Count <= 1))
                {
                    if (this.ParentTreeView != null)
                    {
                        this.ParentTreeView.HandleItemSelectionFromUI(this);
                    }
                }
                else
                {
                    this.processSelectionOnMouseUp = true;
                }
            }
        }

        internal void OnHeaderMouseLeftButtonUp(object sender, MouseEventArgs e)
        {
            if (!this.wasDragged && this.processSelectionOnMouseUp)
            {
                this.ParentTreeView.HandleItemSelectionFromUI(this);
            }
            this.wasDragged = false;
            this.HandleSingleClickFunctionality();
        }

        private static void OnIsCheckBoxEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            RadTreeViewItem item = d as RadTreeViewItem;
            if (((bool) args.NewValue) && item.IsRadioButtonEnabled)
            {
                item.IsRadioButtonEnabled = false;
            }
            item.CreateOptionElement();
            item.RenderOptionElement();
        }

        private static void OnIsCheckedPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            bool? value = (bool?) e.NewValue;
            RadTreeViewItem treeViewItem = sender as RadTreeViewItem;
            if (value == true)
            {
                treeViewItem.CheckState = ToggleState.On;
            }
            else if (value == false)
            {
                treeViewItem.CheckState = ToggleState.Off;
            }
            else
            {
                treeViewItem.CheckState = ToggleState.Indeterminate;
            }
            switch (treeViewItem.CheckState)
            {
                case ToggleState.Off:
                    treeViewItem.IsChecked = false;
                    return;

                case ToggleState.On:
                    treeViewItem.IsChecked = true;
                    return;

                case ToggleState.Indeterminate:
                    treeViewItem.IsChecked = null;
                    return;
            }
        }

        private static void OnIsDragOverChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as RadTreeViewItem).ChangeVisualState();
        }

        protected internal override void OnIsEnabledChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnIsEnabledChanged(e);
            if (((this.ParentTreeView != null) && this.ParentTreeView.IsVirtualizing) && this.ParentTreeView.IsEnabled)
            {
                this.ParentTreeView.ItemStorage.StoreValue(this, Control.IsEnabledProperty, this.Item, true);
            }
        }

        protected internal virtual void OnIsExpandedChanged(bool oldValue, bool newValue)
        {
            this.lastAvailableSize = Size.Empty;
            base.InvalidateMeasure();
            if (newValue)
            {
                if (this.ParentTreeView == null)
                {
                    return;
                }
                if (this.ParentTreeView.IsSingleExpandPath && !this.SupressEventRaising)
                {
                    foreach (RadTreeViewItem temp in this.NonParentItems)
                    {
                        if (temp.IsExpanded)
                        {
                            temp.IsExpanded = false;
                        }
                    }
                    if (this.ParentTreeView.ItemStorage != null)
                    {
                        this.ParentTreeView.ItemStorage.ClearProperty(IsExpandedProperty);
                        base.UpdateLayout();
                        this.BringIntoView();
                    }
                }
                if (base.Items.Count == 0)
                {
                    this.IsExpanded = false;
                    if ((this.IsLoadOnDemandEnabled && !this.IsLoadingOnDemand) && !this.SupressEventRaising)
                    {
                        this.IsLoadingOnDemand = true;
                        this.ChangeVisualState();
                        this.OnLoadOnDemand(new RadRoutedEventArgs(LoadOnDemandEvent, this));
                    }
                    return;
                }
                if (this.OnPreviewExpanded(new RadRoutedEventArgs(PreviewExpandedEvent, this)))
                {
                    this.IsExpanded = false;
                    return;
                }
                this.SetExpandedState();
                this.OnExpanded(new RadRoutedEventArgs(ExpandedEvent, this));
            }
            else
            {
                if (base.Items.Count == 0)
                {
                    return;
                }
                if (this.OnPreviewCollapsed(new RadRoutedEventArgs(PreviewCollapsedEvent, this)))
                {
                    this.IsExpanded = true;
                    return;
                }
                this.SetExpandedState();
                RadTreeViewItemAutomationPeer peer = FrameworkElementAutomationPeer.FromElement(this) as RadTreeViewItemAutomationPeer;
                if ((peer != null) && !this.SupressEventRaising)
                {
                    peer.RaiseExpandCollapseAutomationEvent(oldValue, newValue);
                }
                if (!this.SupressEventRaising)
                {
                    this.ForceUpdateIndexTree();
                }
                this.OnCollapsed(new RadRoutedEventArgs(CollapsedEvent, this));
            }
            if ((this.ParentTreeView != null) && this.ParentTreeView.IsVirtualizing)
            {
                this.ParentTreeView.ItemStorage.StoreValue(this, IsExpandedProperty, this.Item, null);
            }
            if (!this.SupressEventRaising)
            {
                this.IsExpandAllPending = false;
                if ((this.ParentItem != null) && !newValue)
                {
                    this.ParentItem.IsExpandAllPending = false;
                }
                this.SetImage();
                this.ChangeVisualState(AnimationManager.IsGlobalAnimationEnabled);
            }
        }

        internal static void OnIsExpandedPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as RadTreeViewItem).OnIsExpandedChanged(Convert.ToBoolean(e.OldValue), Convert.ToBoolean(e.NewValue));
        }

        private static void OnIsInCheckBoxPropagateStateModeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            RadTreeViewItem treeViewItem = sender as RadTreeViewItem;
            if ((treeViewItem.ParentTreeView != null) && treeViewItem.ParentTreeView.IsVirtualizing)
            {
                treeViewItem.ParentTreeView.ItemStorage.StoreValue(treeViewItem, IsInCheckBoxPropagateStateModePropery, treeViewItem.Item, false);
            }
        }

        protected override void OnIsInEditModeChanged(bool oldValue, bool newValue)
        {
            if (!this.justSetIsInEditMode)
            {
                if (newValue)
                {
                    if (this.ParentTreeView == null)
                    {
                        this.isEditPending = true;
                    }
                    else
                    {
                        this.BeginEdit();
                    }
                }
                else if (!newValue)
                {
                    this.isEditPending = false;
                    this.CancelEdit();
                }
            }
        }

        private static void OnIsLoadingOnDemandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            RadTreeViewItem item = d as RadTreeViewItem;
            if (!((bool) args.NewValue) && (item.Items.Count > 0))
            {
                if (item.IsExpandAllPending)
                {
                    item.ExpandAll();
                }
                else
                {
                    item.IsExpanded = true;
                }
            }
            item.SetExpandedState();
            item.ChangeVisualState();
        }

        private static void OnIsLoadOnDemandEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            RadTreeViewItem item = d as RadTreeViewItem;
            if (!((bool) args.NewValue))
            {
                item.IsLoadingOnDemand = false;
            }
            item.SetExpandedState();
            item.ChangeVisualState();
        }

        private static void OnIsRadioButtonEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            RadTreeViewItem item = d as RadTreeViewItem;
            if (((bool) args.NewValue) && item.IsCheckBoxEnabled)
            {
                item.IsCheckBoxEnabled = false;
            }
            item.CreateOptionElement();
            item.RenderOptionElement();
        }

        private static void OnIsSelectedPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadTreeViewItem treeViewItem = sender as RadTreeViewItem;
            if ((treeViewItem.ParentTreeView != null) && !treeViewItem.justChangeIsSelected)
            {
                bool newValue = Convert.ToBoolean(args.NewValue, CultureInfo.InvariantCulture);
                Telerik.Windows.Controls.ItemsControl owner = treeViewItem.Owner;
                if (owner != null)
                {
                    object item = owner.ItemContainerGenerator.ItemFromContainer(treeViewItem);
                    if (!treeViewItem.SupressEventRaising)
                    {
                        Telerik.Windows.Controls.SelectionChanger<Object> selectedItems = treeViewItem.ParentTreeView.SelectedItems as Telerik.Windows.Controls.SelectionChanger<Object>;
                        if (newValue)
                        {
                            if (!selectedItems.Contains(item))
                            {
                                if (treeViewItem.ParentTreeView.SelectionMode == Telerik.Windows.Controls.SelectionMode.Single)
                                {
                                    selectedItems.AddJustThis(item);
                                }
                                else
                                {
                                    selectedItems.Add(item);
                                }
                            }
                            if (!selectedItems.Contains(item))
                            {
                                treeViewItem.JustChangeIsSelected(false);
                                return;
                            }
                            treeViewItem.BringIntoView();
                            RadTreeViewItemAutomationPeer peer = FrameworkElementAutomationPeer.FromElement(treeViewItem) as RadTreeViewItemAutomationPeer;
                            if (peer != null)
                            {
                                peer.RaiseAutomationIsSelectedChanged((bool) args.NewValue);
                            }
                            treeViewItem.OnSelected(new RadRoutedEventArgs(SelectedEvent, treeViewItem));
                        }
                        else
                        {
                            if (selectedItems.Contains(item))
                            {
                                selectedItems.Remove(item);
                            }
                            if (selectedItems.Contains(item))
                            {
                                treeViewItem.JustChangeIsSelected(true);
                                return;
                            }
                            RadTreeViewItemAutomationPeer peer = FrameworkElementAutomationPeer.FromElement(treeViewItem) as RadTreeViewItemAutomationPeer;
                            if (peer != null)
                            {
                                peer.RaiseAutomationIsSelectedChanged((bool) args.NewValue);
                            }
                            treeViewItem.OnUnselected(new RadRoutedEventArgs(UnselectedEvent, treeViewItem));
                        }
                        if ((treeViewItem.ParentTreeView != null) && treeViewItem.ParentTreeView.IsVirtualizing)
                        {
                            treeViewItem.ParentTreeView.ItemStorage.StoreValue(treeViewItem, IsSelectedProperty, treeViewItem.Item, false);
                        }
                    }
                    treeViewItem.IsSelectionActive = ((treeViewItem.ParentTreeView != null) && treeViewItem.ParentTreeView.IsSelectionActive) && newValue;
                    treeViewItem.SetImage();
                    treeViewItem.ChangeVisualState(true);
                }
            }
        }

        private static void OnIsSelectionActivePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            RadTreeViewItem itemContainer = d as RadTreeViewItem;
            if (itemContainer.IsSelected)
            {
                itemContainer.ChangeVisualState();
            }
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            if (((this.IsExpanded && this.IsLoadOnDemandEnabled) && (base.Items.Count == 0)) && ((e.Action == NotifyCollectionChangedAction.Remove) || (e.Action == NotifyCollectionChangedAction.Reset)))
            {
                this.CollapseWithNoAnimation();
            }
            RadTreeViewItem containerItem = null;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    containerItem = e.NewItems[0] as RadTreeViewItem;
                    if (containerItem != null)
                    {
                        this.UpdateCheckStateAfterItemsChange(containerItem, containerItem.CheckState);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Replace:
                    if (this.ParentTreeView != null)
                    {
                        if (this.ParentTreeView.SelectedItems.Contains(e.OldItems[0]))
                        {
                            this.ParentTreeView.SelectedItems.Remove(e.OldItems[0]);
                        }
                        this.ParentTreeView.OnDescendantRemoved(e.OldItems[0]);
                    }
                    containerItem = e.OldItems[0] as RadTreeViewItem;
                    if (containerItem != null)
                    {
                        this.UpdateCheckStateAfterItemsChange(containerItem, ToggleState.Off);
                        InvalidateRender(containerItem);
                    }
                    break;
            }
            this.SetExpandedState();
            if ((this.IsLoadingOnDemand && (base.Items.Count > 0)) && ((base.ReadLocalValue(FrameworkElement.DataContextProperty) != DependencyProperty.UnsetValue) && (base.GetBindingExpression(System.Windows.Controls.ItemsControl.ItemsSourceProperty) != null)))
            {
                this.IsLoadingOnDemand = false;
            }
            this.isRenderPending = true;
            base.InvalidateMeasure();
            if (this.Item == this)
            {
                this.SetCheckStateWithPropagation(CalculateItemCheckState(this));
            }
            else if (((this.ParentTreeView != null) && this.ParentTreeView.IsOptionElementsEnabled) && (e.Action != NotifyCollectionChangedAction.Reset))
            {
                this.isCheckStateDirty = true;
            }
        }

        private static void OnItemsOptionListTypePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadTreeViewItem treeViewItem = sender as RadTreeViewItem;
            RadTreeView parentTree = treeViewItem.ParentTreeView;
            if (parentTree != null)
            {
                if (parentTree.IsVirtualizing)
                {
                    parentTree.ItemStorage.StoreValue(treeViewItem, ItemsOptionListTypeProperty, treeViewItem.Item, OptionListType.Default);
                }
                treeViewItem.ParentTreeView.SetItemsOptionListType(treeViewItem);
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (base.IsEnabled && (this.ParentTreeView != null))
            {
                this.ParentTreeView.HandleKeyDown(e);
            }
        }

        protected internal virtual void OnLoadOnDemand(RadRoutedEventArgs e)
        {
            this.RaiseEvent(e);
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            this.IsKeyboardFocusWithin = this.IsAncestorOf(e.OriginalSource as DependencyObject);
        }

        private static void OnOptionTypePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadTreeViewItem treeViewItem = sender as RadTreeViewItem;
            OptionListType newValue = (OptionListType) args.NewValue;
            if ((treeViewItem != null) && (newValue != OptionListType.Default))
            {
                RadTreeView parentTree = treeViewItem.ParentTreeView;
                if ((parentTree != null) && parentTree.IsVirtualizing)
                {
                    parentTree.ItemStorage.StoreValue(treeViewItem, OptionTypeProperty, treeViewItem.Item, OptionListType.Default);
                }
                switch (newValue)
                {
                    case OptionListType.CheckList:
                        treeViewItem.IsCheckBoxEnabled = true;
                        break;

                    case OptionListType.OptionList:
                        treeViewItem.IsRadioButtonEnabled = true;
                        break;

                    case OptionListType.None:
                        treeViewItem.IsCheckBoxEnabled = false;
                        treeViewItem.IsRadioButtonEnabled = false;
                        break;

                    case OptionListType.Default:
                        break;

                    default:
                        treeViewItem.IsCheckBoxEnabled = false;
                        treeViewItem.IsRadioButtonEnabled = false;
                        break;
                }
                treeViewItem.CreateOptionElement();
                treeViewItem.RenderOptionElement();
            }
        }

        protected internal virtual bool OnPreviewChecked(RadTreeViewCheckEventArgs e)
        {
            if (!this.SupressEventRaising)
            {
                this.RaiseEvent(e);
            }
            if (this.userCheckAction == UserInitiatedCheck.PreviewEventPending)
            {
                this.userCheckAction = UserInitiatedCheck.EventPending;
                e.IsUserInitiated = true;
            }
            return e.Handled;
        }

        protected internal virtual bool OnPreviewCollapsed(RadRoutedEventArgs e)
        {
            if (!this.SupressEventRaising)
            {
                this.RaiseEvent(e);
            }
            return e.Handled;
        }

        protected override void OnPreviewEditorPrepare(EditorPrepareEventArgs e)
        {
            base.OnPreviewEditorPrepare(e);
            if (!e.Handled)
            {
                TextBox textBoxEditor = e.Editor as TextBox;
                if (((textBoxEditor != null) && (this.Item != this)) && ((this.headerContainerElement != null) && (textBoxEditor.GetBindingExpression(TextBox.TextProperty) == null)))
                {
                    TextBlock boundTextBlock = this.headerContainerElement.ChildrenOfType<TextBlock>().FirstOrDefault<TextBlock>();
                    if (boundTextBlock != null)
                    {
                        BindingExpression textBindingExpression = boundTextBlock.GetBindingExpression(TextBlock.TextProperty);
                        if (textBindingExpression != null)
                        {
                            Binding textBinding = textBindingExpression.ParentBinding;
                            if (!string.IsNullOrEmpty(textBinding.Path.Path))
                            {
                                Binding editBinding = new Binding {
                                    Converter = textBinding.Converter,
                                    ConverterCulture = textBinding.ConverterCulture,
                                    ConverterParameter = textBinding.ConverterParameter,
                                    Mode = BindingMode.TwoWay,
                                    NotifyOnValidationError = true,
                                    ValidatesOnExceptions = true,
                                    Path = textBinding.Path,
                                    RelativeSource = textBinding.RelativeSource
                                };
                                textBoxEditor.SetBinding(TextBox.TextProperty, editBinding);
                            }
                        }
                    }
                }
            }
        }

        protected internal virtual bool OnPreviewExpanded(RadRoutedEventArgs e)
        {
            if (!this.SupressEventRaising)
            {
                this.RaiseEvent(e);
            }
            return e.Handled;
        }

        protected internal virtual bool OnPreviewSelected(RadRoutedEventArgs e)
        {
            if (!this.SupressEventRaising)
            {
                this.RaiseEvent(e);
            }
            return e.Handled;
        }

        protected internal virtual bool OnPreviewUnchecked(RadTreeViewCheckEventArgs e)
        {
            if (!this.SupressEventRaising)
            {
                if (this.userCheckAction == UserInitiatedCheck.PreviewEventPending)
                {
                    this.userCheckAction = UserInitiatedCheck.EventPending;
                    e.IsUserInitiated = true;
                }
                this.RaiseEvent(e);
            }
            return e.Handled;
        }

        protected internal virtual bool OnPreviewUnselected(RadRoutedEventArgs e)
        {
            if (!this.SupressEventRaising)
            {
                this.RaiseEvent(e);
            }
            return e.Handled;
        }

        private static void OnRequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {
            if (e.TargetObject == sender)
            {
                ((RadTreeViewItem) sender).HandleBringIntoView(e);
            }
        }

        protected internal virtual void OnSelected(RadRoutedEventArgs e)
        {
            if (!this.SupressEventRaising)
            {
                this.RaiseEvent(e);
            }
        }

        private static void OnSelectedImageSrcPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            RadTreeViewItem item = d as RadTreeViewItem;
            if (item != null)
            {
                item.SetImage();
            }
        }

        protected internal virtual void OnUnchecked(RadTreeViewCheckEventArgs e)
        {
            if (!this.SupressEventRaising)
            {
                if (this.userCheckAction == UserInitiatedCheck.EventPending)
                {
                    this.userCheckAction = UserInitiatedCheck.None;
                    e.IsUserInitiated = true;
                }
                this.RaiseEvent(e);
            }
        }

        protected internal virtual void OnUnselected(RadRoutedEventArgs e)
        {
            if (!this.SupressEventRaising)
            {
                this.RaiseEvent(e);
            }
        }

        internal DependencyObject PredictFocusInternal(Telerik.Windows.Controls.TreeView.FocusNavigationDirection direction)
        {
            switch (direction)
            {
                case Telerik.Windows.Controls.TreeView.FocusNavigationDirection.Left:
                case Telerik.Windows.Controls.TreeView.FocusNavigationDirection.Up:
                    return this.FindPreviousFocusableItem();

                case Telerik.Windows.Controls.TreeView.FocusNavigationDirection.Right:
                case Telerik.Windows.Controls.TreeView.FocusNavigationDirection.Down:
                    return this.FindNextFocusableItem(true);
            }
            return null;
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            if (this.ParentTreeView != null)
            {
                this.ParentTreeView.PrepareContainerForDescendant(element, item, this);
            }
        }

        private void RadioButtonElement_Click(object sender, RoutedEventArgs e)
        {
            if (base.IsEnabled && (this.IsRadioButtonEnabled && (this.CheckState != ToggleState.On)))
            {
                ToggleState checkState = (this.CheckState == ToggleState.On) ? ToggleState.Off : ToggleState.On;
                this.userCheckAction = UserInitiatedCheck.PreviewEventPending;
                this.CheckState = checkState;
                this.optionElement.Render(this.toggleElement, checkState);
            }
        }

        internal void Render()
        {
            if (this.HasTemplate)
            {
                this.RenderIndent();
                this.SetImage();
                this.SetExpandedState();
                if ((this.ParentTreeView != null) && this.ParentTreeView.IsOptionElementsEnabled)
                {
                    this.RenderOptionElement();
                }
                this.RenderListRoot();
                this.ChangeVisualState(false);
            }
        }

        internal void RenderIndent()
        {
            if (this.HasTemplate && (this.indentContainerElement != null))
            {
                while (this.indentContainerElement.Children.Count > 1)
                {
                    this.indentContainerElement.Children.RemoveAt(1);
                }
                if (this.ParentItem != null)
                {
                    Queue<bool> indentQueue = this.ParentItem.GetIndentQueue();
                    for (int currentLevel = 0; indentQueue.Count > 0; currentLevel++)
                    {
                        this.RenderIndentBlock(indentQueue.Dequeue(), currentLevel);
                    }
                }
                if (this.ParentTreeView != null)
                {
                    this.indentContainerElement.Width = this.Level * this.ParentTreeView.ItemsIndent;
                }
            }
        }

        private void RenderIndentBlock(bool printLine, int level)
        {
            if (((this.lineBrush != null) && (this.indentFirstVerticalLine != null)) && (this.indentContainerElement != null))
            {
                if (level == 0)
                {
                    this.indentFirstVerticalLine.VerticalAlignment = VerticalAlignment.Stretch;
                    if (!printLine)
                    {
                        this.indentFirstVerticalLine.Stroke = null;
                    }
                    else
                    {
                        this.indentFirstVerticalLine.Stroke = this.lineBrush;
                    }
                    this.indentFirstVerticalLine.Visibility = Visibility.Visible;
                    this.indentFirstVerticalLine.Margin = new Thickness(this.listRootContainerElement.ActualWidth / 2.0, 0.0, 0.0, 0.0);
                }
                else
                {
                    Line indentLine = new Line {
                        Y2 = 10.0,
                        Stretch = Stretch.Fill,
                        VerticalAlignment = VerticalAlignment.Stretch,
                        StrokeEndLineCap = PenLineCap.Square
                    };
                    if (printLine)
                    {
                        indentLine.StrokeThickness = this.indentFirstVerticalLine.StrokeThickness;
                        DoubleCollection newDashArray = new DoubleCollection();
                        for (int i = 0; i < this.indentFirstVerticalLine.StrokeDashArray.Count; i++)
                        {
                            newDashArray.Add(this.indentFirstVerticalLine.StrokeDashArray[i]);
                        }
                        indentLine.StrokeDashArray = newDashArray;
                        indentLine.Stroke = this.lineBrush;
                    }
                    indentLine.Width = this.indentFirstVerticalLine.Width;
                    int leftMargin = this.ParentTreeView.ItemsIndent - ((int) indentLine.StrokeThickness);
                    indentLine.Margin = new Thickness((double) leftMargin, 0.0, 0.0, 0.0);
                    this.indentContainerElement.Children.Add(indentLine);
                }
            }
        }

        internal void RenderListRoot()
        {
            if ((this.ParentTreeView == null) || !this.ParentTreeView.ShallLineBePrinted(this))
            {
                if (this.horizontalLineElement != null)
                {
                    this.horizontalLineElement.Visibility = Visibility.Collapsed;
                }
                if (this.verticalLineElement != null)
                {
                    this.verticalLineElement.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                if (this.horizontalLineElement != null)
                {
                    this.horizontalLineElement.Visibility = Visibility.Visible;
                    if (this.listRootContainerElement != null)
                    {
                        this.horizontalLineElement.Width = this.listRootContainerElement.ActualWidth / 2.0;
                    }
                }
                if (this.verticalLineElement != null)
                {
                    this.verticalLineElement.Visibility = Visibility.Visible;
                    this.verticalLineElement.Margin = new Thickness(0.0);
                }
                switch (this.Position)
                {
                    case RadTreeViewItemPosition.Top:
                        if ((this.verticalLineElement != null) && (this.headerRow != null))
                        {
                            this.verticalLineElement.Height = this.headerRow.ActualHeight / 2.0;
                            this.verticalLineElement.VerticalAlignment = VerticalAlignment.Bottom;
                        }
                        break;

                    case RadTreeViewItemPosition.Middle:
                        if ((this.verticalLineElement != null) && (this.headerRow != null))
                        {
                            this.verticalLineElement.ClearValue(FrameworkElement.HeightProperty);
                            this.verticalLineElement.VerticalAlignment = VerticalAlignment.Stretch;
                        }
                        break;

                    case RadTreeViewItemPosition.Bottom:
                        if ((this.ParentItem != null) || (this.ParentTreeView.Items.Count != 1))
                        {
                            if ((this.verticalLineElement != null) && (this.headerRow != null))
                            {
                                this.verticalLineElement.Height = this.headerRow.ActualHeight / 2.0;
                                this.verticalLineElement.VerticalAlignment = VerticalAlignment.Top;
                            }
                            break;
                        }
                        if (this.verticalLineElement != null)
                        {
                            this.verticalLineElement.Visibility = Visibility.Collapsed;
                        }
                        break;
                }
                if (this.ParentItem != null)
                {
                    this.ParentItem.RenderListRoot();
                }
            }
        }

        internal virtual void RenderOptionElement()
        {
            if (this.checkBoxElement != null)
            {
                this.checkBoxElement.Visibility = Visibility.Collapsed;
            }
            if (this.radioButtonElement != null)
            {
                this.radioButtonElement.Visibility = Visibility.Collapsed;
            }
            if (this.IsCheckBoxEnabled || this.IsRadioButtonEnabled)
            {
                if (this.optionElement == null)
                {
                    this.CreateOptionElement();
                }
                this.optionElement.Render(this.toggleElement, this.CheckState);
            }
            else if (this.optionElement != null)
            {
                this.optionElement.Hide(this.toggleElement);
            }
        }

        internal RadTreeView SearchForParentTreeView()
        {
            RadTreeView parentTreeView = null;
            for (Telerik.Windows.Controls.ItemsControl parentItemsControl = Telerik.Windows.Controls.ItemsControl.ItemsControlFromItemContainer(this); parentItemsControl != null; parentItemsControl = Telerik.Windows.Controls.ItemsControl.ItemsControlFromItemContainer(parentItemsControl))
            {
                parentTreeView = parentItemsControl as RadTreeView;
            }
            return parentTreeView;
        }

        internal object SelectImage()
        {
            object selectedImageSource = this.SetSelectedImageSourceValue();
            string selectedImageSourceString = selectedImageSource as string;
            if (selectedImageSourceString != null)
            {
                return new Uri(this.GetImageSrc(selectedImageSourceString), UriKind.RelativeOrAbsolute);
            }
            return selectedImageSource;
        }

        internal void SetCheckStateWithNoPropagation(ToggleState state)
        {
            bool oldState = this.IsInCheckBoxPropagateStateMode;
            this.IsInCheckBoxPropagateStateMode = true;
            this.CheckState = state;
            this.IsInCheckBoxPropagateStateMode = oldState;
        }

        internal void SetCheckStateWithPropagation(ToggleState state)
        {
            bool oldState = this.IsInCheckBoxPropagateStateMode;
            this.IsInCheckBoxPropagateStateMode = false;
            this.CheckState = state;
            this.IsInCheckBoxPropagateStateMode = oldState;
        }

        private void SetExpandedState()
        {
            if (this.expanderElement != null)
            {
                bool shouldBeVisible = false;
                if (base.Items.Count == 0)
                {
                    if (this.IsLoadOnDemandEnabled)
                    {
                        shouldBeVisible = true;
                        this.expanderElement.IsChecked = false;
                    }
                }
                else
                {
                    shouldBeVisible = true;
                    this.expanderElement.IsChecked = new bool?(this.IsExpanded);
                }
                if (shouldBeVisible)
                {
                    this.expanderElement.Opacity = 1.0;
                    this.expanderElement.IsHitTestVisible = true;
                }
                else
                {
                    this.expanderElement.Opacity = 0.0;
                    this.expanderElement.IsHitTestVisible = false;
                }
            }
        }

        private void SetImage()
        {
            if (this.HasTemplate && (this.imageElement != null))
            {
                object image = this.SelectImage();
                Uri imageUri = image as Uri;
                if (imageUri != null)
                {
                    this.imageElement.Source = new BitmapImage(imageUri);
                }
                else
                {
                    ImageSource imageString = image as ImageSource;
                    if (imageString != null)
                    {
                        this.imageElement.Source = imageString;
                    }
                    else
                    {
                        this.imageElement.Source = null;
                    }
                }
            }
        }

        internal void SetIsLoadOnDemandBinding()
        {
            if ((this.ParentTreeView != null) && (base.ReadLocalValue(IsLoadOnDemandEnabledProperty) == DependencyProperty.UnsetValue))
            {
                Binding loadOnDemandEnabledBinding = new Binding {
                    Source = this.ParentTreeView,
                    Path = new PropertyPath("IsLoadOnDemandEnabled", new object[0])
                };
                base.SetBinding(IsLoadOnDemandEnabledProperty, loadOnDemandEnabledBinding);
            }
        }

        private void SetOptionElementCheckState()
        {
            if (this.optionElement != null)
            {
                this.OptionElement.Render(this.toggleElement, this.CheckState);
                if (!this.IsInCheckBoxPropagateStateMode)
                {
                    this.optionElement.PropagateItemState(this);
                }
            }
        }

        internal object SetSelectedImageSourceValue()
        {
            object effectiveImageSource = null;
            if (this.IsSelected)
                effectiveImageSource = this.SelectedImageSrc;
            else if (this.IsExpanded)
                effectiveImageSource = this.ExpandedImageSrc;
            else
                effectiveImageSource = this.DefaultImageSrc;

            if ((effectiveImageSource != null) && effectiveImageSource.Equals(string.Empty))
            {
                return null;
            }
            return effectiveImageSource;
        }

        private void StartDoubleClickTime()
        {
            if (this.doubleClickTimer != null)
            {
                this.doubleClickTimer.Stop();
            }
            this.doubleClickTimer = new DispatcherTimer();
            this.doubleClickTimer.Interval = TimeSpan.FromMilliseconds(300.0);
            this.doubleClickTimer.Tick += new EventHandler(this.DoubleClickTimer_Tick);
            this.doubleClickTimer.Start();
        }

        public void StopEdit(bool cancelEdit)
        {
            if (cancelEdit)
            {
                this.CancelEdit();
            }
            else
            {
                this.CommitEdit();
            }
        }

        double TreeViewPanel.IProvideStackingSize.EstimatedContainerSize()
        {
            return base.DesiredSize.Height;
        }

        double TreeViewPanel.IProvideStackingSize.HeaderSize()
        {
            if (this.headerRow != null)
            {
                return this.headerRow.DesiredSize.Height;
            }
            return 0.0;
        }

        public override string ToString()
        {
            return Convert.ToString(base.Header, CultureInfo.InvariantCulture);
        }

        private void UnbindEvents()
        {
            if (this.expanderElement != null)
            {
                this.expanderElement.Click -= new RoutedEventHandler(this.ExpanderElement_Click);
            }
            if (this.checkBoxElement != null)
            {
                this.checkBoxElement.Click -= new RoutedEventHandler(this.CheckBoxElement_Click);
            }
            if (this.radioButtonElement != null)
            {
                this.radioButtonElement.Click -= new RoutedEventHandler(this.RadioButtonElement_Click);
            }
            if (base.HeaderEditPresenterElement != null)
            {
                base.HeaderEditPresenterElement.KeyDown -= new KeyEventHandler(this.OnHeaderEditElementKeyDown);
            }
            if (this.headerRow != null)
            {
                this.headerRow.MouseLeftButtonDown -= new MouseButtonEventHandler(this.OnHeaderMouseLeftButtonDown);
                this.headerRow.MouseEnter -= new MouseEventHandler(this.OnHeaderMouseEnter);
                this.headerRow.MouseLeave -= new MouseEventHandler(this.OnHeaderMouseLeave);
                this.headerRow.MouseLeftButtonUp -= new MouseButtonEventHandler(this.OnHeaderMouseLeftButtonUp);
                this.headerRow.SizeChanged -= new SizeChangedEventHandler(this.HeaderRow_SizeChanged);
            }
        }

        private void UpdateCheckStateAfterItemsChange(RadTreeViewItem containerItem, ToggleState state)
        {
            if (containerItem != null)
            {
                RadTreeView parentTreeView = this.ParentTreeView;
                if ((parentTreeView == null) && !this.HasTemplate)
                {
                    parentTreeView = this.SearchForParentTreeView();
                }
                if ((parentTreeView != null) && parentTreeView.IsOptionElementsEnabled)
                {
                    parentTreeView.StoreCheckState(containerItem, containerItem, state);
                    if (parentTreeView.IsTriStateMode)
                    {
                        this.SetCheckStateWithNoPropagation(CalculateItemCheckState(this));
                    }
                }
            }
        }

        public ToggleState CheckState
        {
            get
            {
                return (ToggleState) base.GetValue(CheckStateProperty);
            }
            set
            {
                base.SetValue(CheckStateProperty, value);
            }
        }

        [Description("Gets or sets the image of the item."), SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId="Src"), ScriptableMember, Category("Appearance"), DefaultValue((string) null)]
        public virtual object DefaultImageSrc
        {
            get
            {
                return base.GetValue(DefaultImageSrcProperty);
            }
            set
            {
                base.SetValue(DefaultImageSrcProperty, value);
            }
        }

        internal DateTime? DropExpandStartTime { get; set; }

        public Telerik.Windows.Controls.DropPosition DropPosition { get; set; }

        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId="Src"), ScriptableMember]
        public object ExpandedImageSrc
        {
            get
            {
                return base.GetValue(ExpandedImageSrcProperty);
            }
            set
            {
                base.SetValue(ExpandedImageSrcProperty, value);
            }
        }

        internal ToggleButton ExpanderElement
        {
            get
            {
                return this.expanderElement;
            }
        }

        [ScriptableMember, Category("Behavior"), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Description("Gets the path from the root tree item to the current tree item  delimited with the PathSeparator specified by RadTreeView.")]
        public string FullPath
        {
            get
            {
                string separator = string.Empty;
                RadTreeView treeview = this.ParentTreeView;
                if (treeview == null)
                {
                    return string.Empty;
                }
                separator = treeview.PathSeparator;
                StringBuilder pathBuilder = new StringBuilder();
                RadTreeViewItem treeViewItem = this;
                object item = treeViewItem.Item ?? treeViewItem;
                pathBuilder.Append(TextSearch.GetPrimaryTextFromItem(treeViewItem.ParentTreeView, item));
                while (treeViewItem.ParentItem != null)
                {
                    treeViewItem = treeViewItem.ParentItem;
                    pathBuilder.Insert(0, separator);
                    item = treeViewItem.Item ?? treeViewItem;
                    pathBuilder.Insert(0, TextSearch.GetPrimaryTextFromItem(treeViewItem.ParentTreeView, item));
                }
                return pathBuilder.ToString();
            }
        }

        internal bool HasTemplate { get; set; }

        internal FrameworkElement HeaderContainerElement
        {
            get
            {
                return this.headerRow;
            }
        }

        internal double HeaderHeight
        {
            get
            {
                if (this.headerRow != null)
                {
                    return this.headerRow.ActualHeight;
                }
                return 0.0;
            }
        }

        internal FrameworkElement HeaderRow
        {
            get
            {
                return this.headerRow;
            }
        }

        internal Image ImageElement
        {
            get
            {
                return this.imageElement;
            }
        }

        internal double IndentWidth
        {
            get
            {
                if (this.indentContainerElement != null)
                {
                    return this.indentContainerElement.ActualWidth;
                }
                return 0.0;
            }
        }

        [Category("Behavior"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), ScriptableMember, DefaultValue(-1), Browsable(false)]
        public int Index
        {
            get
            {
                Telerik.Windows.Controls.ItemsControl owner = Telerik.Windows.Controls.ItemsControl.ItemsControlFromItemContainer(this);
                if (owner == null)
                {
                    return -1;
                }
                return owner.ItemContainerGenerator.IndexFromContainer(this);
            }
        }

        [ScriptableMember, DefaultValue(false), Description("Gets or sets whether the tree item will display a check box."), Category("Behavior"), Browsable(false)]
        public bool IsCheckBoxEnabled
        {
            get
            {
                return (bool) base.GetValue(IsCheckBoxEnabledProperty);
            }
            internal set
            {
                base.SetValue(IsCheckBoxEnabledProperty, value);
            }
        }

        [ScriptableMember]
        public bool? IsChecked
        {
            get
            {
                return (bool?) base.GetValue(IsCheckedProperty);
            }
            set
            {
                base.SetValue(IsCheckedProperty, value);
            }
        }

        [DefaultValue(false), Browsable(false), ScriptableMember]
        public bool IsDragOver
        {
            get
            {
                return (bool) base.GetValue(IsDragOverProperty);
            }
            set
            {
                base.SetValue(IsDragOverProperty, value);
            }
        }

        [ScriptableMember, Description("Gets or sets a value indicating whether the tree item can accept data that the user drags onto it."), DefaultValue(true), Category("Behavior")]
        public bool IsDropAllowed
        {
            get
            {
                return (bool) base.GetValue(IsDropAllowedProperty);
            }
            set
            {
                base.SetValue(IsDropAllowedProperty, value);
            }
        }

        internal bool IsExpandAllPending
        {
            get
            {
                return (bool) base.GetValue(IsExpandAllPendingProperty);
            }
            set
            {
                base.SetValue(IsExpandAllPendingProperty, value);
            }
        }

        [ScriptableMember]
        public bool IsExpanded
        {
            get
            {
                return (bool) base.GetValue(IsExpandedProperty);
            }
            set
            {
                base.SetValue(IsExpandedProperty, value);
            }
        }

        internal bool IsInCheckBoxPropagateStateMode
        {
            get
            {
                return (bool) base.GetValue(IsInCheckBoxPropagateStateModePropery);
            }
            set
            {
                base.SetValue(IsInCheckBoxPropagateStateModePropery, value);
            }
        }

        [ScriptableMember]
        public bool IsInSelectedPath
        {
            get
            {
                if (!this.IsSelected)
                {
                    return this.Contains(this.ParentTreeView.SelectedContainer);
                }
                return true;
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes"), ScriptableMember]
        public bool IsInViewport
        {
            get
            {
                if (this.ParentTreeView == null)
                {
                    return false;
                }
                if (this.ParentTreeView.ScrollViewer != null)
                {
                    GeneralTransform q1 = null;
                    try
                    {
                        q1 = base.TransformToVisual(this.ParentTreeView.ScrollViewer);
                    }
                    catch
                    {
                        return false;
                    }
                    Point q2 = q1.Transform(new Point(0.0, 0.0));
                    if ((0.0 > q2.Y) || ((q2.Y + this.HeaderHeight) > this.ParentTreeView.ScrollViewer.ViewportHeight))
                    {
                        return false;
                    }
                    if ((this.ParentTreeView.BringIntoViewMode == BringIntoViewMode.HeaderAndItems) && ((q2.Y + base.ActualHeight) > this.ParentTreeView.ScrollViewer.ViewportHeight))
                    {
                        return false;
                    }
                    if (0.0 > (q2.X + this.IndentWidth))
                    {
                        return false;
                    }
                    if (this.headerContainerElement != null)
                    {
                        GeneralTransform headerTransform = null;
                        try
                        {
                            headerTransform = this.headerContainerElement.TransformToVisual(this.ParentTreeView.ScrollViewer);
                        }
                        catch
                        {
                            return false;
                        }
                        double headerRight = headerTransform.Transform(new Point(0.0, 0.0)).X + this.headerContainerElement.ActualWidth;
                        if (headerRight > this.ParentTreeView.ScrollViewer.ViewportWidth)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
        }

        internal bool IsKeyboardFocusWithin { get; set; }

        [ScriptableMember]
        public bool IsLoadingOnDemand
        {
            get
            {
                return (bool) base.GetValue(IsLoadingOnDemandProperty);
            }
            set
            {
                base.SetValue(IsLoadingOnDemandProperty, value);
            }
        }

        [ScriptableMember]
        public bool IsLoadOnDemandEnabled
        {
            get
            {
                return (bool) base.GetValue(IsLoadOnDemandEnabledProperty);
            }
            set
            {
                base.SetValue(IsLoadOnDemandEnabledProperty, value);
            }
        }

        internal bool IsMouseOverState { get; set; }

        [Description("Gets or sets whether the tree item will display a radio button."), Category("Behavior"), Browsable(false), DefaultValue(false), ScriptableMember]
        public bool IsRadioButtonEnabled
        {
            get
            {
                return (bool) base.GetValue(IsRadioButtonEnabledProperty);
            }
            internal set
            {
                base.SetValue(IsRadioButtonEnabledProperty, value);
            }
        }

        internal bool IsRenderPending
        {
            get
            {
                return this.isRenderPending;
            }
            set
            {
                this.isRenderPending = value;
            }
        }

        [ScriptableMember, Description("Gets a value if the item is root item"), Category("Behavior"), Browsable(false)]
        public bool IsRootItem
        {
            get
            {
                return (this.ParentItem == null);
            }
        }

        [ScriptableMember]
        public bool IsSelected
        {
            get
            {
                return (bool) base.GetValue(IsSelectedProperty);
            }
            set
            {
                base.SetValue(IsSelectedProperty, value);
            }
        }

        [Category("Behavior"), DefaultValue(false), ScriptableMember, Description("Gets whether the tree item selection is active - e.g. the item is selected and the treeview is focused."), Browsable(false)]
        public bool IsSelectionActive
        {
            get
            {
                return (bool) base.GetValue(IsSelectionActiveProperty);
            }
            internal set
            {
                this.SetValue(IsSelectionActivePropertyKey, value);
            }
        }

        public object Item { get; internal set; }

        internal TreeViewPanel ItemsHost
        {
            get
            {
                return this.itemsHost;
            }
        }

        [ScriptableMember]
        public OptionListType ItemsOptionListType
        {
            get
            {
                return (OptionListType) base.GetValue(ItemsOptionListTypeProperty);
            }
            set
            {
                base.SetValue(ItemsOptionListTypeProperty, value);
            }
        }

        internal FrameworkElement ItemsPresenterElement
        {
            get
            {
                return this.itemsPresenterElement;
            }
        }

        [Browsable(false), DefaultValue(-1), ScriptableMember, DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int Level
        {
            get
            {
                if (this.ParentItem != null)
                {
                    return (this.ParentItem.Level + 1);
                }
                return 0;
            }
            internal set
            {
                if ((this.Level != value) && this.HasTemplate)
                {
                    this.ForEachContainerItem<RadTreeViewItem>(delegate (RadTreeViewItem childContainerItem) {
                        childContainerItem.Level = value;
                    });
                    this.RenderIndent();
                }
            }
        }

        [Browsable(false), Description("Gets the next available item. Used for traversal of the tree view."), Category("Behavior"), ScriptableMember, DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadTreeViewItem NextItem
        {
            get
            {
                RadTreeViewItem firstVisibleChildContainer = GetFirstVisibleChild(this);
                if (firstVisibleChildContainer != this)
                {
                    return firstVisibleChildContainer;
                }
                RadTreeViewItem parentContainer = this.ParentItem;
                RadTreeViewItem currentContainer = this;
                do
                {
                    Telerik.Windows.Controls.ItemsControl owner = Telerik.Windows.Controls.ItemsControl.ItemsControlFromItemContainer(currentContainer);
                    int nextSiblingIndex = owner.ItemContainerGenerator.IndexFromContainer(currentContainer) + 1;
                    int siblingsCount = owner.Items.Count;
                    if (nextSiblingIndex < siblingsCount)
                    {
                        return (owner.ItemContainerGenerator.ContainerFromIndex(nextSiblingIndex) as RadTreeViewItem);
                    }
                    currentContainer = parentContainer;
                    if (parentContainer != null)
                    {
                        parentContainer = parentContainer.ParentItem;
                    }
                }
                while (currentContainer != null);
                return null;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Description("Gets the next sibling tree item."), Category("Behavior"), ScriptableMember, Browsable(false)]
        public RadTreeViewItem NextSiblingItem
        {
            get
            {
                Telerik.Windows.Controls.ItemsControl owner = this.Owner;
                int nextIndex = owner.ItemContainerGenerator.IndexFromContainer(this) + 1;
                if (nextIndex < owner.Items.Count)
                {
                    return (owner.ItemContainerGenerator.ContainerFromIndex(nextIndex) as RadTreeViewItem);
                }
                return null;
            }
        }

        internal Collection<RadTreeViewItem> NonParentItems
        {
            get
            {
                IEnumerable<RadTreeViewItem> allItems = this.ParentTreeView.GetAllItemContainers(this.ParentTreeView);
                Collection<RadTreeViewItem> nonParentItems = new Collection<RadTreeViewItem>();
                foreach (RadTreeViewItem tempContainer in allItems)
                {
                    if ((!tempContainer.Contains(this) && (tempContainer != this)) && !this.Contains(tempContainer))
                    {
                        nonParentItems.Add(tempContainer);
                    }
                }
                return nonParentItems;
            }
        }

        internal Telerik.Windows.Controls.OptionElement OptionElement
        {
            get
            {
                return this.optionElement;
            }
        }

        [ScriptableMember]
        public OptionListType OptionType
        {
            get
            {
                return (OptionListType) base.GetValue(OptionTypeProperty);
            }
            set
            {
                base.SetValue(OptionTypeProperty, value);
            }
        }

        internal Telerik.Windows.Controls.ItemsControl Owner
        {
            get
            {
                if (this.ownerReference == null)
                {
                    return null;
                }
                return (Telerik.Windows.Controls.ItemsControl) this.ownerReference.Target;
            }
            set
            {
                this.ownerReference = new WeakReference(value);
            }
        }

        [Description("Gets the parent tree item of the current tree item."), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Category("Behavior"), ScriptableMember, DefaultValue((string) null), Browsable(false)]
        public RadTreeViewItem ParentItem
        {
            get
            {
                if (this.parentItemReference == null)
                {
                    return null;
                }
                return (RadTreeViewItem) this.parentItemReference.Target;
            }
            internal set
            {
                this.parentItemReference = new WeakReference(value);
                if (value != null)
                {
                    if (((this.optionElement != null) && this.ParentTreeView.IsTriStateMode) && ((this.ParentItem.ItemsOptionListType == OptionListType.CheckList) && (this.optionElement is CheckElement)))
                    {
                        (this.optionElement as CheckElement).PropagateStateOnParentItem(this);
                    }
                    this.RenderListRoot();
                    this.ChangeVisualState(false);
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), DefaultValue((string) null), ScriptableMember, Description("Gets the parent tree view that the tree item is assigned to."), Browsable(false)]
        public RadTreeView ParentTreeView
        {
            get
            {
                if (this.parentTreeViewReference == null)
                {
                    return null;
                }
                return (RadTreeView) this.parentTreeViewReference.Target;
            }
            internal set
            {
                this.parentTreeViewReference = new WeakReference(value);
            }
        }

        internal RadTreeViewItemPosition Position
        {
            get
            {
                if (!this.HasNextSibling())
                {
                    return RadTreeViewItemPosition.Bottom;
                }
                if (this.HasPreviousSibling())
                {
                    return RadTreeViewItemPosition.Middle;
                }
                if (this.ParentItem != null)
                {
                    return RadTreeViewItemPosition.Middle;
                }
                return RadTreeViewItemPosition.Top;
            }
        }

        [Description("Gets the previous available item. Used for traversal of the tree view."), Category("Behavior"), ScriptableMember, DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public RadTreeViewItem PreviousItem
        {
            get
            {
                Telerik.Windows.Controls.ItemsControl owner = this.Owner;
                RadTreeViewItem prevSibling = null;
                int prevIndex = owner.ItemContainerGenerator.IndexFromContainer(this) - 1;
                if (prevIndex > -1)
                {
                    prevSibling = owner.ItemContainerGenerator.ContainerFromIndex(prevIndex) as RadTreeViewItem;
                }
                if (prevSibling == null)
                {
                    return this.ParentItem;
                }
                while (((prevSibling != null) && prevSibling.IsExpanded) && (prevSibling.Items.Count > 0))
                {
                    prevSibling = prevSibling.ItemContainerGenerator.ContainerFromIndex(prevSibling.Items.Count - 1) as RadTreeViewItem;
                }
                return prevSibling;
            }
        }

        [Category("Behavior"), ScriptableMember, DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), Description("Gets the previous sibling tree item.")]
        public RadTreeViewItem PreviousSiblingItem
        {
            get
            {
                Telerik.Windows.Controls.ItemsControl owner = Telerik.Windows.Controls.ItemsControl.ItemsControlFromItemContainer(this);
                int prevIndex = owner.ItemContainerGenerator.IndexFromContainer(this) - 1;
                if (prevIndex > -1)
                {
                    return (owner.ItemContainerGenerator.ContainerFromIndex(prevIndex) as RadTreeViewItem);
                }
                return null;
            }
        }

        [ScriptableMember]
        public RadTreeViewItem RootItem
        {
            get
            {
                RadTreeViewItem item = this;
                while (item.ParentItem != null)
                {
                    item = item.ParentItem;
                }
                return item;
            }
        }

        [DefaultValue((string) null), Category("Appearance"), ScriptableMember, SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId="Src")]
        public object SelectedImageSrc
        {
            get
            {
                return base.GetValue(SelectedImageSrcProperty);
            }
            set
            {
                base.SetValue(SelectedImageSrcProperty, value);
            }
        }

        internal bool SupressEventRaising { get; set; }

        bool TreeViewPanel.ICachable.IsCaching
        {
            get
            {
                return this.isCaching;
            }
            set
            {
                this.isCaching = value;
                base.Visibility = value ? Visibility.Collapsed : Visibility.Visible;
                base.IsHitTestVisible = !value;
            }
        }

        internal ToggleButton ToggleElement
        {
            get
            {
                return this.toggleElement;
            }
        }

        private enum UserInitiatedCheck
        {
            PreviewEventPending,
            EventPending,
            None
        }
    }
}

