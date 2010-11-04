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
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Automation.Peers;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using System.Windows.Media;
    using Telerik.Windows;
    using Telerik.Windows.Controls.Animation;
    using Telerik.Windows.Controls.DragDrop;
    using Telerik.Windows.Controls.Primitives;
    using Telerik.Windows.Controls.TabControl;

    [TemplateVisualState(GroupName = "PlacementStates", Name = "Right"), TemplateVisualState(GroupName = "ScrollDisplayStates", Name = "ScrollCollapsed"), TemplateVisualState(GroupName = "PlacementStates", Name = "Top"), TemplatePart(Name = "ContentElement", Type = typeof(ContentControl)), TemplatePart(Name = "ItemsPresenterElement", Type = typeof(ItemsPresenter)), TemplatePart(Name = "ArrangeGridElement", Type = typeof(Grid)), TemplatePart(Name = "LeftScrollButtonElement", Type = typeof(ButtonBase)), TemplateVisualState(GroupName = "PlacementStates", Name = "Left"), TemplatePart(Name = "ScrollViewerElement", Type = typeof(ScrollViewer)), TemplatePart(Name = "DropDownButtonElement", Type = typeof(ToggleButton)), StyleTypedProperty(Property = "DropDownButtonStyle", StyleTargetType = typeof(ToggleButton)), TemplateVisualState(GroupName = "ScrollDisplayStates", Name = "ScrollNormal"), StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(RadTabItem)), StyleTypedProperty(Property = "DropDownStyle", StyleTargetType = typeof(Telerik.Windows.Controls.TabControl.DropDownMenu)), TemplatePart(Name = "RightScrollButtonElement", Type = typeof(ButtonBase)), TemplateVisualState(GroupName = "PlacementStates", Name = "Bottom"), TemplateVisualState(GroupName = "DropDownDisplayStates", Name = "DropDownButtonVisible"), TemplateVisualState(GroupName = "DropDownDisplayStates", Name = "DropDownButtonCollapsed")]
    public class RadTabControl : Telerik.Windows.Controls.ItemsControl
    {
        public static readonly DependencyProperty AlignProperty = DependencyProperty.Register("Align", typeof(TabStripAlign), typeof(RadTabControl), new Telerik.Windows.PropertyMetadata(TabStripAlign.Left, new PropertyChangedCallback(RadTabControl.OnAlignChanged), null));
        public static readonly DependencyProperty AllowDragOverTabProperty = DependencyProperty.Register("AllowDragOverTab", typeof(bool), typeof(RadTabControl), new Telerik.Windows.PropertyMetadata(false));
        public static readonly DependencyProperty AllowDragReorderProperty = DependencyProperty.Register("AllowDragReorder", typeof(bool), typeof(RadTabControl), new Telerik.Windows.PropertyMetadata(false));
        public static readonly DependencyProperty AllTabsEqualHeightProperty = DependencyProperty.Register("AllTabsEqualHeight", typeof(bool), typeof(RadTabControl), new Telerik.Windows.PropertyMetadata(true, new PropertyChangedCallback(RadTabControl.OnAllTabsEqualheightChanged), null));
        public static readonly DependencyProperty BackgroundVisibilityProperty = DependencyProperty.Register("BackgroundVisibility", typeof(Visibility), typeof(RadTabControl), null);
        public static readonly DependencyProperty BottomTemplateProperty = DependencyProperty.Register("BottomTemplate", typeof(ControlTemplate), typeof(RadTabControl), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadTabControl.OnBottomTemplateChanged)));
        private ContentPresenter contentElement;
        public static readonly DependencyProperty ContentTemplateProperty = DependencyProperty.Register("ContentTemplate", typeof(DataTemplate), typeof(RadTabControl), null);
        public static readonly DependencyProperty ContentTemplateSelectorProperty = DependencyProperty.Register("ContentTemplateSelector", typeof(DataTemplateSelector), typeof(RadTabControl), null);
        private int desiredSelectedIndex;
        private WeakReference desiredSelectedItem = new WeakReference(null);
        private WeakReference draggedTabItem;
        private Visibility dropDownButtonState;
        public static readonly DependencyProperty DropDownButtonStyleProperty = DependencyProperty.Register("DropDownButtonStyle", typeof(Style), typeof(RadTabControl), null);
        public static readonly Telerik.Windows.RoutedEvent DropDownClosedEvent = EventManager.RegisterRoutedEvent("DropDownClosed", RoutingStrategy.Bubble, typeof(DropDownEventHandler), typeof(RadTabControl));
        public static readonly DependencyProperty DropDownDisplayMemberPathProperty = DependencyProperty.Register("DropDownDisplayMemberPath", typeof(string), typeof(RadTabControl), null);
        public static readonly DependencyProperty DropDownDisplayModeProperty = DependencyProperty.Register("DropDownDisplayMode", typeof(TabControlDropDownDisplayMode), typeof(RadTabControl), new Telerik.Windows.PropertyMetadata(TabControlDropDownDisplayMode.Collapsed, new PropertyChangedCallback(RadTabControl.OnDropDownDisplayModeChanged)));
        private ObservableCollection<object> dropDownItems = new ObservableCollection<object>();
        public static readonly Telerik.Windows.RoutedEvent DropDownOpenedEvent = EventManager.RegisterRoutedEvent("DropDownOpened", RoutingStrategy.Bubble, typeof(DropDownEventHandler), typeof(RadTabControl));
        public static readonly DependencyProperty DropDownStyleProperty = DependencyProperty.Register("DropDownStyle", typeof(Style), typeof(RadTabControl), null);
        private StateFlags flags = new StateFlags();
        private bool hasDropDownButton;
        private bool hasDropDownMenu;
        private bool hasLeftScrollButton;
        private bool hasRightScrollButton;
        private bool hasScrollViewer;
        private FrameworkElement headerDockedElement;
        private IList<object> indexedDropDownItems;
        private bool isDropDownOpen;
        public static readonly DependencyProperty IsDropDownOpenProperty = DependencyProperty.Register("IsDropDownOpen", typeof(bool), typeof(RadTabControl), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadTabControl.OnIsDropDownOpenChanged)));
        private bool isSelectedContainerUpdated;
        public static readonly DependencyProperty ItemDropDownContentTemplateProperty = DependencyProperty.Register("ItemDropDownContentTemplate", typeof(DataTemplate), typeof(RadTabControl), null);
        public static readonly DependencyProperty ItemDropDownContentTemplateSelectorProperty = DependencyProperty.Register("ItemDropDownContentTemplateSelector", typeof(DataTemplateSelector), typeof(RadTabControl), null);
        private TabSwap lastSwap;
        private ButtonBase leftScrollButton;
        public static readonly DependencyProperty LeftTemplateProperty = DependencyProperty.Register("LeftTemplate", typeof(ControlTemplate), typeof(RadTabControl), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadTabControl.OnLeftTemplateChanged)));
        public static readonly DependencyProperty OverflowModeProperty = DependencyProperty.Register("OverflowMode", typeof(TabOverflowMode), typeof(RadTabControl), new Telerik.Windows.PropertyMetadata(TabOverflowMode.Scroll, new PropertyChangedCallback(RadTabControl.OnOverflowModeChanged)));
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The Routed event args cannot be changed.")]
        public static readonly Telerik.Windows.RoutedEvent PreviewSelectionChangedEvent = EventManager.RegisterRoutedEvent("PreviewSelectionChanged", RoutingStrategy.Tunnel, typeof(Telerik.Windows.Controls.SelectionChangedEventHandler), typeof(RadTabControl));
        public static readonly DependencyProperty PropagateItemDataContextToContentProperty = DependencyProperty.Register("PropagateItemDataContextToContent", typeof(bool), typeof(RadTabControl), new Telerik.Windows.PropertyMetadata(true));
        public static readonly DependencyProperty ReorderTabRowsProperty = DependencyProperty.Register("ReorderTabRows", typeof(bool), typeof(RadTabControl), new Telerik.Windows.PropertyMetadata(true, new PropertyChangedCallback(RadTabControl.OnReorderTabRowsChanged), null));
        private ButtonBase rightScrollButton;
        public static readonly DependencyProperty RightTemplateProperty = DependencyProperty.Register("RightTemplate", typeof(ControlTemplate), typeof(RadTabControl), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadTabControl.OnRightTemplateChanged)));
        public static readonly DependencyProperty ScrollModeProperty = DependencyProperty.Register("ScrollMode", typeof(TabControlScrollMode), typeof(RadTabControl), new Telerik.Windows.PropertyMetadata(TabControlScrollMode.Pixel));
        private ScrollViewer scrollViewer;
        public static readonly DependencyProperty SelectedContentProperty = DependencyProperty.Register("SelectedContent", typeof(object), typeof(RadTabControl), null);
        public static readonly DependencyProperty SelectedContentTemplateProperty = DependencyProperty.Register("SelectedContentTemplate", typeof(DataTemplate), typeof(RadTabControl), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadTabControl.OnSelectedContentTemplateChanged)));
        public static readonly DependencyProperty SelectedContentTemplateSelectorProperty = DependencyProperty.Register("SelectedContentTemplateSelector", typeof(DataTemplateSelector), typeof(RadTabControl), null);
        public static readonly DependencyProperty SelectedIndexProperty = DependencyProperty.Register("SelectedIndex", typeof(int), typeof(RadTabControl), new Telerik.Windows.PropertyMetadata(-1, new PropertyChangedCallback(RadTabControl.OnSelectedIndexChanged), null));
        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(object), typeof(RadTabControl), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadTabControl.OnSelectedItemChanged)));
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The Routed event args cannot be changed.")]
        public static readonly Telerik.Windows.RoutedEvent SelectionChangedEvent = EventManager.RegisterRoutedEvent("SelectionChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(RadTabControl));
        private Telerik.Windows.Controls.SelectionChanger<Object> selectionChanger = new Telerik.Windows.Controls.SelectionChanger<Object>();
        private bool shouldSetIsTabStop = true;
        private bool suspendSelectionNotification;
        public static readonly DependencyProperty TabOrientationProperty = DependencyProperty.Register("TabOrientation", typeof(Orientation), typeof(RadTabControl), new Telerik.Windows.PropertyMetadata(Orientation.Horizontal, new PropertyChangedCallback(RadTabControl.OnTabOrientationChanged), null));
        private Panel tabStrip;
        public static readonly DependencyProperty TabStripPlacementProperty = DependencyProperty.Register("TabStripPlacement", typeof(Dock), typeof(RadTabControl), new System.Windows.PropertyMetadata(Dock.Top, new PropertyChangedCallback(RadTabControl.OnTabStripPlacementChanged)));
        public static readonly DependencyProperty TopTemplateProperty = DependencyProperty.Register("TopTemplate", typeof(ControlTemplate), typeof(RadTabControl), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadTabControl.OnTopTemplateChanged)));

        public event DropDownEventHandler DropDownClosed
        {
            add
            {
                this.AddHandler(DropDownClosedEvent, value);
            }
            remove
            {
                this.RemoveHandler(DropDownClosedEvent, value);
            }
        }

        public event DropDownEventHandler DropDownOpened
        {
            add
            {
                this.AddHandler(DropDownOpenedEvent, value);
            }
            remove
            {
                this.RemoveHandler(DropDownOpenedEvent, value);
            }
        }

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

        [Telerik.Windows.Controls.SRDescription("TabControlSelectionChangedDescription")]
        public event RoutedEventHandler SelectionChanged
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

        static RadTabControl()
        {
            EventManager.RegisterClassHandler(typeof(RadTabControl), RadDragAndDropManager.DropQueryEvent, new EventHandler<DragDropQueryEventArgs>(RadTabControl.OnTabItemDropQuery));
            EventManager.RegisterClassHandler(typeof(RadTabControl), RadDragAndDropManager.DragQueryEvent, new EventHandler<DragDropQueryEventArgs>(RadTabControl.OnTabItemDragQuery));
        }

        public RadTabControl()
        {
            base.DefaultStyleKey = typeof(RadTabControl);
            base.Loaded += new RoutedEventHandler(this.OnLoaded);
            this.selectionChanger.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.OnSelectionChanged);
            this.SelectedIndex = -1;
            
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "The AddHandler method in WPF is on UIElement")]
        public static void AddDropDownClosedHandler(UIElement target, DropDownEventHandler handler)
        {
            target.AddHandler(DropDownClosedEvent, handler);
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "The AddHandler method in WPF is on UIElement")]
        public static void AddDropDownOpenedHandler(UIElement target, DropDownEventHandler handler)
        {
            target.AddHandler(DropDownOpenedEvent, handler);
        }

        private void AdjustScrollBarOffset(double adjustment)
        {
            if (adjustment > 0.0)
            {
                adjustment = this.CoerceScrollForward(adjustment);
            }
            if (adjustment < 0.0)
            {
                adjustment = -this.CoerceScrollBack(-adjustment);
            }
            if (this.IsPanelHorizontal)
            {
                this.scrollViewer.ScrollToHorizontalOffset(adjustment + this.scrollViewer.HorizontalOffset);
                double currentDisplacement = 0.0;
                if (this.TabStrip.RenderTransform != null)
                {
                    currentDisplacement = this.TabStrip.RenderTransform.Transform(new Point()).X;
                }
                adjustment += currentDisplacement;
                if (!RadDragAndDropManager.IsDragging)
                {
                    double[] xCoord = new double[4];
                    xCoord[1] = adjustment;
                    xCoord[2] = 0.5;
                    AnimationExtensions.Create().Animate(new FrameworkElement[] { this.TabStrip })
                        .EnsureDefaultTransforms()
                        .MoveX(xCoord)
                            .Easings(Easings.SlideUp1).AdjustSpeed()
                            .OnComplete(new Action(this.UpdateScrollButtonsState))
                            .PlayIfPossible(this);
                }
            }
            else
            {
                this.scrollViewer.ScrollToVerticalOffset(adjustment + this.scrollViewer.VerticalOffset);
                double currentDisplacement = 0.0;
                if (this.TabStrip.RenderTransform != null)
                {
                    currentDisplacement = this.TabStrip.RenderTransform.Transform(new Point()).Y;
                }
                adjustment += currentDisplacement;
                if (!RadDragAndDropManager.IsDragging)
                {
                    double[] yCoord = new double[4];
                    yCoord[1] = adjustment;
                    yCoord[2] = 0.5;
                    AnimationExtensions.Create().Animate(new FrameworkElement[] { this.TabStrip })
                        .EnsureDefaultTransforms().MoveY(yCoord).Easings(Easings.SlideUp1).AdjustSpeed().OnComplete(new Action(this.UpdateScrollButtonsState)).PlayIfPossible(this);
                }
            }
            base.Dispatcher.BeginInvoke(delegate
            {
                this.UpdateScrollButtonsState();
            });
        }

        private void BringContainerIntoView(FrameworkElement container, ViewportRelation currentRelativePosition)
        {
            if (this.TabStrip != null)
            {
                double adjustment = 0.0;
                if (this.IsPanelHorizontal)
                {
                    double leftEdge = 0.0;
                    try
                    {
                        leftEdge = container.TransformToVisual(this.TabStrip).Transform(new Point()).X;
                    }
                    catch (ArgumentException)
                    {
                    }
                    double rightEdge = leftEdge + container.ActualWidth;
                    if (currentRelativePosition == ViewportRelation.BeforeViewport)
                    {
                        adjustment = ((leftEdge - this.scrollViewer.HorizontalOffset) + this.TabStrip.Margin.Left) + this.TabStrip.Margin.Right;
                    }
                    else
                    {
                        adjustment = (((rightEdge - this.scrollViewer.HorizontalOffset) - this.scrollViewer.ViewportWidth) + this.TabStrip.Margin.Left) + this.TabStrip.Margin.Right;
                    }
                }
                else
                {
                    double topEdge = 0.0;
                    try
                    {
                        topEdge = container.TransformToVisual(this.TabStrip).Transform(new Point()).Y;
                    }
                    catch (ArgumentException)
                    {
                    }
                    double bottomEdge = topEdge + container.ActualHeight;
                    if (currentRelativePosition == ViewportRelation.BeforeViewport)
                    {
                        adjustment = ((topEdge - this.scrollViewer.VerticalOffset) + this.TabStrip.Margin.Top) + this.TabStrip.Margin.Bottom;
                    }
                    else
                    {
                        adjustment = (((bottomEdge - this.scrollViewer.VerticalOffset) - this.scrollViewer.ViewportHeight) + this.TabStrip.Margin.Top) + this.TabStrip.Margin.Bottom;
                    }
                }
                this.AdjustScrollBarOffset(adjustment);
            }
        }

        private void BringSelectedItemIntoView()
        {
            base.Dispatcher.BeginInvoke(delegate
            {
                if (this.SelectedIndex != -1)
                {
                    this.ScrollIntoView(this.SelectedItem);
                }
            });
        }

        private ViewportRelation CalculateContainerDirectionalVisibility(FrameworkElement container)
        {
            if (this.IsPanelHorizontal)
            {
                return this.CalculateContainerHorizontalVisibility(container);
            }
            return this.CalculateContainerVerticalVisibility(container);
        }

        private ViewportRelation CalculateContainerHorizontalVisibility(FrameworkElement container)
        {
            if ((this.scrollViewer.ComputedHorizontalScrollBarVisibility != Visibility.Collapsed) && (this.TabStrip != null))
            {
                double leftEdge = 0.0;
                try
                {
                    leftEdge = container.TransformToVisual(this.TabStrip).Transform(new Point()).X;
                }
                catch (ArgumentException)
                {
                }
                double rightEdge = leftEdge + container.ActualWidth;
                if ((leftEdge - this.scrollViewer.HorizontalOffset) < 0.0)
                {
                    return ViewportRelation.BeforeViewport;
                }
                if ((rightEdge - this.scrollViewer.HorizontalOffset) > this.scrollViewer.ViewportWidth)
                {
                    return ViewportRelation.AfterViewport;
                }
            }
            return ViewportRelation.InView;
        }

        private ViewportRelation CalculateContainerVerticalVisibility(FrameworkElement container)
        {
            if ((this.scrollViewer.ComputedVerticalScrollBarVisibility != Visibility.Collapsed) && (this.TabStrip != null))
            {
                double topEdge = 0.0;
                try
                {
                    topEdge = container.TransformToVisual(this.TabStrip).Transform(new Point()).Y;
                }
                catch (ArgumentException)
                {
                }
                double bottomEdge = topEdge + container.ActualHeight;
                if ((topEdge - this.scrollViewer.VerticalOffset) < 0.0)
                {
                    return ViewportRelation.BeforeViewport;
                }
                if ((bottomEdge - this.scrollViewer.VerticalOffset) > this.scrollViewer.ViewportHeight)
                {
                    return ViewportRelation.AfterViewport;
                }
            }
            return ViewportRelation.InView;
        }

        private bool CanScrollBack()
        {
            if (!this.hasScrollViewer)
            {
                return false;
            }
            if (this.IsPanelHorizontal)
            {
                return (this.scrollViewer.HorizontalOffset > 0.0);
            }
            return (this.scrollViewer.VerticalOffset > 0.0);
        }

        private bool CanScrollForward()
        {
            if (!this.hasScrollViewer || (this.TabStrip == null))
            {
                return false;
            }
            if (this.IsPanelHorizontal)
            {
                return (((((this.scrollViewer.HorizontalOffset + this.scrollViewer.ViewportWidth) - this.tabStrip.ActualWidth) - this.tabStrip.Margin.Left) - this.tabStrip.Margin.Right) != 0.0);
            }
            return (((((this.scrollViewer.VerticalOffset + this.scrollViewer.ViewportHeight) - this.tabStrip.ActualHeight) - this.tabStrip.Margin.Top) - this.tabStrip.Margin.Bottom) != 0.0);
        }

        protected internal override void ChangeVisualState(bool useTransitions)
        {
            base.ChangeVisualState(useTransitions);
            Visibility computedVisibility = this.GetComputedVisibility();
            if (this.IsDropDownButtonCollapsed)
            {
                this.dropDownButtonState = Visibility.Collapsed;
                this.GoToState(useTransitions, new string[] { "DropDownButtonCollapsed" });
            }
            else
            {
                this.dropDownButtonState = Visibility.Visible;
                this.GoToState(useTransitions, new string[] { "DropDownButtonVisible" });
            }
            if (this.hasScrollViewer)
            {
                if (computedVisibility == Visibility.Visible)
                {
                    this.GoToState(useTransitions, new string[] { "ScrollVisible" });
                }
                else
                {
                    this.GoToState(useTransitions, new string[] { "ScrollCollapsed" });
                }
            }
            this.GoToState(useTransitions, new string[] { this.TabStripPlacement.ToString() });
            if (this.headerDockedElement != null)
            {
                RadDockPanel.SetDock(this.headerDockedElement, this.TabStripPlacement);
            }
        }

        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            base.ClearContainerForItemOverride(element, item);
            RadDragAndDropManager.SetAllowDrop(element, false);
            RadDragAndDropManager.SetAllowDrag(element, false);
        }

        private double CoerceScrollBack(double desiredScroll)
        {
            if (!this.hasScrollViewer)
            {
                return desiredScroll;
            }
            if (this.IsPanelHorizontal)
            {
                return Math.Max(0.0, Math.Min(this.scrollViewer.HorizontalOffset, desiredScroll));
            }
            return Math.Max(0.0, Math.Min(this.scrollViewer.VerticalOffset, desiredScroll));
        }

        private double CoerceScrollForward(double desiredScroll)
        {
            if (!this.hasScrollViewer || (this.TabStrip == null))
            {
                return desiredScroll;
            }
            if (this.IsPanelHorizontal)
            {
                return Math.Max(Math.Min((((this.tabStrip.ActualWidth + this.tabStrip.Margin.Left) + this.tabStrip.Margin.Right) - this.scrollViewer.HorizontalOffset) - this.scrollViewer.ViewportWidth, desiredScroll), 0.0);
            }
            return Math.Max(Math.Min((((this.tabStrip.ActualHeight + this.tabStrip.Margin.Top) + this.tabStrip.Margin.Bottom) - this.scrollViewer.VerticalOffset) - this.scrollViewer.ViewportHeight, desiredScroll), 0.0);
        }

        internal RadTabItem ContainerFromIndex(int index)
        {
            return (base.ItemContainerGenerator.ContainerFromIndex(index) as RadTabItem);
        }

        internal RadTabItem ContainerFromItem(object item)
        {
            return (base.ItemContainerGenerator.ContainerFromItem(item) as RadTabItem);
        }

        private void DeselectAll()
        {
            this.ForEachContainerItem<RadTabItem>(delegate(RadTabItem o)
            {
                if (o != null)
                {
                    o.IsSelected = false;
                }
            });
            this.SelectedContentTemplate = null;
            this.SelectedContentTemplateSelector = null;
            this.SelectedItem = null;
            this.SelectedContent = null;
        }

        private FrameworkElement FindReoderDestinationItem(Point absolutePoint)
        {
            if (this.TabStrip != null)
            {
                try
                {
                    Point relative = Application.Current.RootVisual.TransformToVisual(this.TabStrip).Transform(absolutePoint);
                    for (int index = 0; index < base.Items.Count; index++)
                    {
                        FrameworkElement container = base.ItemContainerGenerator.ContainerFromIndex(index) as FrameworkElement;
                        if ((container != null) && LayoutInformation.GetLayoutSlot(container).Contains(relative))
                        {
                            return container;
                        }
                    }
                }
                catch
                {
                    return null;
                }
            }
            return null;
        }

        protected virtual ControlTemplate FindTemplateFromPosition(Dock position)
        {
            switch (position)
            {
                case Dock.Left:
                    return this.LeftTemplate;

                case Dock.Top:
                    return this.TopTemplate;

                case Dock.Right:
                    return this.RightTemplate;

                case Dock.Bottom:
                    return this.BottomTemplate;
            }
            return null;
        }

        private Visibility GetComputedVisibility()
        {
            Visibility computedVisibility = Visibility.Collapsed;
            if (this.hasScrollViewer)
            {
                computedVisibility = this.IsPanelHorizontal ? this.scrollViewer.ComputedHorizontalScrollBarVisibility : this.scrollViewer.ComputedVerticalScrollBarVisibility;
            }
            return computedVisibility;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new RadTabItem();
        }

        internal object GetDropDownItem(object item)
        {
            if (!(item is DependencyObject))
            {
                return item;
            }
            RadTabItem tabItem = base.ItemContainerGenerator.ContainerFromItem(item) as RadTabItem;
            if (tabItem == null)
            {
                return null;
            }
            if (tabItem.DropDownContent != null)
            {
                return tabItem.DropDownContent;
            }
            if (!(tabItem.Header is DependencyObject) && (tabItem.Header != null))
            {
                return tabItem.Header;
            }
            string text = TextSearch.GetPrimaryTextFromItem(this, tabItem);
            if (!string.IsNullOrEmpty(text))
            {
                return text;
            }
            return tabItem.ToString();
        }

        protected void GoToState(bool useTransitions, params string[] stateNames)
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

        private bool HandleKeyDown(FrameworkElement focusedItem)
        {
            if ((this.TabStripPlacement != Dock.Left) && (this.TabStripPlacement != Dock.Right))
            {
                return false;
            }
            return this.SelectNextItem(focusedItem);
        }

        private bool HandleKeyEnd()
        {
            int startIndex = base.Items.Count - 1;
            for (int i = startIndex; i >= 0; i--)
            {
                Control item = this.ContainerFromIndex(i);
                if (item.IsEnabled)
                {
                    if (this.SelectedIndex != i)
                    {
                        this.SelectedIndex = i;
                        item.Focus();
                        return true;
                    }
                    return false;
                }
            }
            return false;
        }

        private bool HandleKeyHome()
        {
            int startIndex = 0;
            for (int i = startIndex; i < base.Items.Count; i++)
            {
                Control item = this.ContainerFromIndex(i);
                if (item.IsEnabled)
                {
                    if (this.SelectedIndex == i)
                    {
                        return false;
                    }
                    this.SelectedIndex = i;
                    if (this.SelectedIndex == i)
                    {
                        item.Focus();
                    }
                    return true;
                }
            }
            return false;
        }

        private bool HandleKeyLeft(FrameworkElement focusedItem)
        {
            if ((this.TabStripPlacement != Dock.Top) && (this.TabStripPlacement != Dock.Bottom))
            {
                return false;
            }
            return this.SelectPreviousItem(focusedItem);
        }

        private bool HandleKeyRight(FrameworkElement focusedItem)
        {
            if ((this.TabStripPlacement != Dock.Top) && (this.TabStripPlacement != Dock.Bottom))
            {
                return false;
            }
            return this.SelectNextItem(focusedItem);
        }

        private bool HandleKeyUp(FrameworkElement focusedItem)
        {
            if ((this.TabStripPlacement != Dock.Left) && (this.TabStripPlacement != Dock.Right))
            {
                return false;
            }
            return this.SelectPreviousItem(focusedItem);
        }

        private static bool IsAlignValid(TabStripAlign value)
        {
            return Enum.IsDefined(typeof(TabStripAlign), value);
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return (item is RadTabItem);
        }

        private static bool IsOrientationValid(object value)
        {
            return Enum.IsDefined(typeof(Orientation), value);
        }

        private static bool IsTabStripPlacementValid(Dock placement)
        {
            return Enum.IsDefined(typeof(Dock), placement);
        }

        private void JustChangeSelectedIndex(int newSelectedIndex)
        {
            this.Flags.NonSelectionIndexUpdate = true;
            try
            {
                this.SelectedIndex = newSelectedIndex;
            }
            finally
            {
                this.Flags.NonSelectionIndexUpdate = false;
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if ((this.hasScrollViewer && this.hasDropDownMenu) && ((this.DropDownDisplayMode == TabControlDropDownDisplayMode.WhenNeeded) && (this.dropDownButtonState != this.scrollViewer.ComputedHorizontalScrollBarVisibility)))
            {
                this.ChangeVisualState();
            }
            Size result = base.MeasureOverride(availableSize);
            this.UpdateScrollButtonsState();
            return result;
        }

        internal void NotifyChildContentChanged(RadTabItem tabItem)
        {
            if (this.SelectedIndex == base.ItemContainerGenerator.IndexFromContainer(tabItem))
            {
                this.SelectedContent = tabItem.Content;
                this.UpdateContentElementSafely();
            }
        }

        internal void NotifyChildIsBreakChanged()
        {
            this.RearrangeTabs();
        }

        internal void NotifyChildIsSelectedChanged(RadTabItem child)
        {
            if (!this.Flags.SelectionInProgress)
            {
                int childIndex = base.ItemContainerGenerator.IndexFromContainer(child);
                if (child.IsSelected)
                {
                    this.SelectedIndex = childIndex;
                }
                else if (this.SelectedIndex == childIndex)
                {
                    this.SelectedIndex = -1;
                }
            }
        }

        private static void OnAlignChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            RadTabControl tabControl = sender as RadTabControl;
            if (!IsAlignValid((TabStripAlign)e.NewValue))
            {
                throw new ArgumentException(Telerik.Windows.Controls.SR.GetString("TabControlAlignInvalidValue"), "e");
            }
            if (tabControl.Flags.IsLoaded)
            {
                TabStripPanel tabStrip = tabControl.tabStrip as TabStripPanel;
                if (tabStrip != null)
                {
                    tabStrip.Align = (TabStripAlign)e.NewValue;
                    tabStrip.InvalidateArrange();
                }
            }
        }

        private static void OnAllTabsEqualheightChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            RadTabControl tabControl = sender as RadTabControl;
            TabStripPanel tabStrip = tabControl.tabStrip as TabStripPanel;
            if (tabStrip != null)
            {
                tabStrip.AllTabsEqualHeight = (bool)e.NewValue;
                tabStrip.InvalidateMeasure();
            }
        }

        public override void OnApplyTemplate()
        {
            if (this.contentElement != null)
            {
                this.contentElement.Content = null;
            }
            base.OnApplyTemplate();
            this.contentElement = base.GetTemplateChild("ContentElement") as ContentPresenter;
            this.Flags.HasContentElement = this.contentElement != null;
            this.tabStrip = null;
            if (this.TabStrip != null)
            {
                this.UpdateTabStrip();
            }
            else
            {
                this.Flags.IsTabStripUpdated = false;
            }
            if (this.hasDropDownMenu)
            {
                this.DropDownMenu.ItemsSource = null;
                this.DropDownMenu.ParentTabControl = null;
                this.DropDownMenu.RemoveHandler(RadMenuItem.ClickEvent, new RoutedEventHandler(this.OnDropDownMenuItemClick));
                this.DropDownMenu.Closed -= new RoutedEventHandler(this.OnDropDownMenuClosed);
                StyleManager.SetTheme(this.DropDownMenu, StyleManager.GetTheme(this));
            }
            this.DropDownMenu = base.GetTemplateChild("DropDownMenuElement") as Telerik.Windows.Controls.TabControl.DropDownMenu;
            this.hasDropDownMenu = this.DropDownMenu != null;
            if (this.hasDropDownMenu)
            {
                this.DropDownMenu.ItemsSource = this.DropDownItems;
                this.DropDownMenu.ParentTabControl = this;
                this.DropDownMenu.AddHandler(RadMenuItem.ClickEvent, new RoutedEventHandler(this.OnDropDownMenuItemClick));
                this.DropDownMenu.Closed += new RoutedEventHandler(this.OnDropDownMenuClosed);
            }
            this.headerDockedElement = base.GetTemplateChild("HeaderDockedElement") as FrameworkElement;
            if (this.hasLeftScrollButton)
            {
                this.leftScrollButton.Click -= new RoutedEventHandler(this.OnLeftScrollButtonClick);
            }
            this.leftScrollButton = base.GetTemplateChild("LeftScrollButtonElement") as ButtonBase;
            this.hasLeftScrollButton = this.leftScrollButton != null;
            if (this.hasLeftScrollButton)
            {
                this.leftScrollButton.Click += new RoutedEventHandler(this.OnLeftScrollButtonClick);
            }
            if (this.hasRightScrollButton)
            {
                this.rightScrollButton.Click -= new RoutedEventHandler(this.OnRightScrollButtonClick);
            }
            this.rightScrollButton = base.GetTemplateChild("RightScrollButtonElement") as ButtonBase;
            this.hasRightScrollButton = this.rightScrollButton != null;
            if (this.hasRightScrollButton)
            {
                this.rightScrollButton.Click += new RoutedEventHandler(this.OnRightScrollButtonClick);
            }
            this.scrollViewer = base.GetTemplateChild("ScrollViewerElement") as ScrollViewer;
            this.hasScrollViewer = this.scrollViewer != null;
            this.DropDownButton = base.GetTemplateChild("DropDownButtonElement") as ToggleButton;
            this.hasDropDownButton = this.DropDownButton != null;
            this.UpdateContentElementSafely();
            this.Flags.IsLoaded = true;
            this.IsDropDownOpen = this.isDropDownOpen;
            this.UpdateScrollButtonsState();
            this.ChangeVisualState();
        }

        private static void OnBottomTemplateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            RadTabControl tabControl = sender as RadTabControl;
            if (tabControl.TabStripPlacement == Dock.Bottom)
            {
                tabControl.SetTemplate(e.NewValue as ControlTemplate);
            }
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new RadTabControlAutomationPeer(this);
        }

        protected virtual void OnDropDownClosed(DropDownEventArgs e)
        {
            if (this.hasDropDownButton)
            {
                this.DropDownButton.IsChecked = false;
            }
            this.RaiseEvent(e);
        }

        private static void OnDropDownDisplayModeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as RadTabControl).UpdateDropDownButtonVisibility();
        }

        private void OnDropDownMenuClosed(object sender, RoutedEventArgs e)
        {
            RadRoutedEventArgs radArgs = e as RadRoutedEventArgs;
            radArgs.Handled = true;
            List<object> closingItems = this.dropDownItems.ToList<object>();
            DropDownEventArgs closedArgs = new DropDownEventArgs(DropDownClosedEvent, this, closingItems);
            this.OnDropDownClosed(closedArgs);
        }

        [SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers", Justification = "There is no need to make the method private.")]
        protected virtual void OnDropDownMenuItemClick(object sender, RoutedEventArgs e)
        {
            RadRoutedEventArgs radArgs = e as RadRoutedEventArgs;
            object menuItem = (radArgs.OriginalSource as FrameworkElement).DataContext;
            if (this.indexedDropDownItems.Contains(menuItem))
            {
                this.SelectItemFromDropDownClick(menuItem);
                radArgs.Handled = true;
            }
        }

        protected virtual void OnDropDownOpened(DropDownEventArgs e)
        {
            this.RaiseEvent(e);
        }

        protected virtual void OnIsDropDownOpenChanged(bool oldValue, bool newValue)
        {
            if (!this.Flags.IsLoaded)
            {
                if (this.IsDropDownOpen)
                {
                    this.isDropDownOpen = true;
                    this.IsDropDownOpen = false;
                }
            }
            else if (this.hasDropDownMenu)
            {
                if (newValue)
                {
                    if (this.IsDropDownButtonCollapsed)
                    {
                        this.IsDropDownOpen = false;
                    }
                    this.indexedDropDownItems = base.Items.Cast<object>().Select<object, object>(new Func<object, object>(this.GetDropDownItem)).ToList<object>();
                    DropDownEventArgs openedEventArgs = new DropDownEventArgs(DropDownOpenedEvent, this, this.indexedDropDownItems.ToList<object>());
                    this.OnDropDownOpened(openedEventArgs);
                    if (openedEventArgs.DropDownItemsSource == null)
                    {
                        return;
                    }
                    foreach (object dropDownItem in openedEventArgs.DropDownItemsSource)
                    {
                        this.dropDownItems.Add(dropDownItem);
                    }
                }
                else
                {
                    this.dropDownItems.Clear();
                    DropDownEventArgs closedEventArgs = new DropDownEventArgs(DropDownClosedEvent, this, null);
                    this.OnDropDownClosed(closedEventArgs);
                }
                this.DropDownMenu.IsOpen = newValue;
                this.ChangeVisualState();
            }
        }

        private static void OnIsDropDownOpenChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as RadTabControl).OnIsDropDownOpenChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "The separate switch cases are of normal complexity.")]
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            this.UpdateSelectionOnItemsChanged(e);
            this.UpdateDropDownMenuOnItemsChange(e);
            base.Dispatcher.BeginInvoke(new Action(this.UpdateScrollButtonsState));
        }

        internal bool OnKeyDownInternal(Key key)
        {
            RadTabItem item = FocusManager.GetFocusedElement() as RadTabItem;
            if (item != null)
            {
                switch (key)
                {
                    case Key.End:
                        return this.HandleKeyEnd();

                    case Key.Home:
                        return this.HandleKeyHome();

                    case Key.Left:
                        return this.HandleKeyLeft(item);

                    case Key.Up:
                        return this.HandleKeyUp(item);

                    case Key.Right:
                        return this.HandleKeyRight(item);

                    case Key.Down:
                        return this.HandleKeyDown(item);
                }
            }
            return false;
        }

        [SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers", Justification = "There is no need to make the method private.")]
        protected virtual void OnLeftScrollButtonClick(object sender, RoutedEventArgs e)
        {
            if (this.hasScrollViewer)
            {
                if (this.ScrollMode == TabControlScrollMode.Pixel)
                {
                    this.AdjustScrollBarOffset(-16.0);
                }
                else if (this.ScrollMode == TabControlScrollMode.Item)
                {
                    this.ScrollOneItemBack();
                }
                else
                {
                    this.ScrollOneViewportBack();
                }
            }
        }

        private static void OnLeftTemplateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            RadTabControl tabControl = sender as RadTabControl;
            if (tabControl.TabStripPlacement == Dock.Left)
            {
                tabControl.SetTemplate(e.NewValue as ControlTemplate);
            }
        }

        [SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers", Justification = "There is no need to make the method private.")]
        protected virtual void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (((this.SelectedIndex == -1) && (base.Items.Count > 0)) && ((base.GetBindingExpression(SelectedIndexProperty) == null) && (base.GetBindingExpression(SelectedItemProperty) == null)))
            {
                this.SelectedIndex = 0;
            }
            this.BringSelectedItemIntoView();
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            FrameworkElement focusedElement = FocusManager.GetFocusedElement() as FrameworkElement;
            if (base.IsFocused || (((this.tabStrip != null) && (focusedElement != null)) && this.tabStrip.IsAncestorOf(focusedElement)))
            {
                if (e.Delta < 0)
                {
                    e.Handled = this.CanScrollBack();
                    this.ScrollOneItemBack();
                }
                if (e.Delta > 0)
                {
                    e.Handled = this.CanScrollForward();
                    this.ScrollOneItemForward();
                }
            }
        }

        private static void OnOverflowModeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            RadTabControl tabControl = sender as RadTabControl;
            ScrollBarVisibility scrollbarVisibility = (((TabOverflowMode)e.NewValue) == TabOverflowMode.Scroll) ? ScrollBarVisibility.Auto : ScrollBarVisibility.Disabled;
            ScrollViewer.SetHorizontalScrollBarVisibility(tabControl, scrollbarVisibility);
        }

        protected virtual void OnPreviewSelectionChanged(Telerik.Windows.Controls.SelectionChangedEventArgs e)
        {
            this.RaiseEvent(e);
        }

        private static void OnReorderTabRowsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            RadTabControl tabControl = sender as RadTabControl;
            TabStripPanel tabStrip = tabControl.TabStrip as TabStripPanel;
            if (tabStrip != null)
            {
                tabStrip.RearrangeTabs = (bool)e.NewValue;
                tabStrip.InvalidateMeasure();
            }
        }

        [SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers", Justification = "There is no need to make the method private.")]
        protected virtual void OnRightScrollButtonClick(object sender, RoutedEventArgs e)
        {
            if (this.hasScrollViewer)
            {
                if (this.ScrollMode == TabControlScrollMode.Pixel)
                {
                    this.AdjustScrollBarOffset(16.0);
                }
                else if (this.ScrollMode == TabControlScrollMode.Item)
                {
                    this.ScrollOneItemForward();
                }
                else
                {
                    this.ScrollOneViewportForward();
                }
            }
        }

        private static void OnRightTemplateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            RadTabControl tabControl = sender as RadTabControl;
            if (tabControl.TabStripPlacement == Dock.Right)
            {
                tabControl.SetTemplate(e.NewValue as ControlTemplate);
            }
        }

        private static void OnSelectedContentTemplateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
        }

        protected internal virtual void OnSelectedIndexChanged(int oldIndex, int newIndex)
        {
            if (!this.Flags.SelectionInProgress && !this.Flags.NonSelectionIndexUpdate)
            {
                try
                {
                    this.Flags.SelectionInProgress = true;
                    if (newIndex < -1)
                    {
                        try
                        {
                            throw new ArgumentOutOfRangeException("newIndex", string.Format(CultureInfo.InvariantCulture, "{0} is not a valid value for property 'SelectedIndex'", new object[] { newIndex }));
                        }
                        finally
                        {
                            this.SelectedIndex = oldIndex;
                        }
                    }
                    if (newIndex >= base.Items.Count)
                    {
                        this.Flags.IsDesiredIndexActive = true;
                        this.desiredSelectedIndex = newIndex;
                        this.SelectedIndex = oldIndex;
                    }
                    else
                    {
                        object newItem = (newIndex == -1) ? null : base.Items[newIndex];
                        if (this.RaisePreviewSelectionChanged(newItem))
                        {
                            this.SelectedIndex = oldIndex;
                            newIndex = oldIndex;
                        }
                        this.UpdateSelectedContent(newIndex);
                        this.Flags.SelectionInProgress = false;
                        if (newIndex == -1)
                        {
                            this.selectionChanger.Clear();
                        }
                        else
                        {
                            this.selectionChanger.AddJustThis(base.Items[newIndex]);
                        }
                    }
                }
                finally
                {
                    this.Flags.SelectionInProgress = false;
                    RadTabControlAutomationPeer peer = FrameworkElementAutomationPeer.FromElement(this) as RadTabControlAutomationPeer;
                    if (peer != null)
                    {
                        peer.RaiseSelectionPropertyChanged(oldIndex, newIndex);
                    }
                }
            }
        }

        private static void OnSelectedIndexChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as RadTabControl).OnSelectedIndexChanged((int)e.OldValue, (int)e.NewValue);
        }

        private static void OnSelectedItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            RadTabControl tabControl = sender as RadTabControl;
            if (!tabControl.Flags.SelectionInProgress)
            {
                if (tabControl.Items.Contains(e.NewValue))
                {
                    int newSelectedIndex = tabControl.Items.IndexOf(e.NewValue);
                    if (tabControl.SelectedIndex == newSelectedIndex)
                    {
                        tabControl.OnSelectedIndexChanged(newSelectedIndex, newSelectedIndex);
                    }
                    else
                    {
                        tabControl.SelectedIndex = newSelectedIndex;
                    }
                    tabControl.desiredSelectedItem.Target = null;
                }
                else
                {
                    tabControl.desiredSelectedItem.Target = e.NewValue;
                    tabControl.SelectedIndex = -1;
                }
            }
        }

        protected virtual void OnSelectionChanged(RadSelectionChangedEventArgs e)
        {
            this.RaiseEvent(e);
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Storyboard.Begin can lead to numerous exceptions, it's best to catch them all.")]
        protected virtual void OnSelectionChanged(IList removedItems, IList addedItems)
        {
            if (this.SelectedItem != null)
            {
                this.ScrollIntoView(this.SelectedItem);
            }
            this.UpdateZIndexes();
            if (!this.suspendSelectionNotification)
            {
                RadSelectionChangedEventArgs args = new RadSelectionChangedEventArgs(SelectionChangedEvent, this, removedItems, addedItems);
                this.OnSelectionChanged(args);
            }
        }

        private void OnSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            this.UpdateContentElementSafely();
            this.OnSelectionChanged(e.RemovedItems, e.AddedItems);
        }

        protected internal virtual void OnTabItemDragQuery(DragDropQueryEventArgs e)
        {
            RadTabItem tabItem = e.Options.Source as RadTabItem;
            if (((e.Options.Status == DragStatus.DragQuery) && (tabItem != null)) && this.AllowDragReorder)
            {
                this.lastSwap = null;
                this.draggedTabItem = new WeakReference(tabItem);
                e.Options.Payload = base.ItemContainerGenerator.ItemFromContainer(tabItem);
                e.QueryResult = true;
            }
        }

        private static void OnTabItemDragQuery(object sender, DragDropQueryEventArgs e)
        {
            (sender as RadTabControl).OnTabItemDragQuery(e);
        }

        protected internal virtual void OnTabItemDropQuery(DragDropQueryEventArgs e)
        {
            RadTabItem destinationItem = e.Options.Destination as RadTabItem;
            RadTabItem sourceItem = e.Options.Source as RadTabItem;
            if (destinationItem != null)
            {
                int destinationIndex = base.ItemContainerGenerator.IndexFromContainer(destinationItem);
                if (this.SelectedIndex != destinationIndex)
                {
                    if (sourceItem == null)
                    {
                        if (this.AllowDragOverTab)
                        {
                            this.SelectedIndex = destinationIndex;
                        }
                    }
                    else if (sourceItem != destinationItem)
                    {
                        Telerik.Windows.Controls.ItemsControl sourceParent = Telerik.Windows.Controls.ItemsControl.ItemsControlFromItemContainer(sourceItem);
                        if (((sourceParent == null) && (this.draggedTabItem != null)) && (this.draggedTabItem.Target == sourceItem))
                        {
                            sourceParent = this;
                        }
                        if ((((Telerik.Windows.Controls.ItemsControl.ItemsControlFromItemContainer(destinationItem) == sourceParent) && (this == sourceParent)) && this.AllowDragReorder) && ((e.Options.Payload != null) && base.Items.Contains(e.Options.Payload)))
                        {
                            FrameworkElement currentSourceContainer = base.ItemContainerGenerator.ContainerFromItem(e.Options.Payload) as FrameworkElement;
                            FrameworkElement layoutDestinationContainer = this.FindReoderDestinationItem(e.Options.CurrentDragPoint);
                            if (((currentSourceContainer != null) && (layoutDestinationContainer != null)) && (layoutDestinationContainer != currentSourceContainer))
                            {
                                this.SwitchItems(layoutDestinationContainer, currentSourceContainer, e.Options.CurrentDragPoint);
                            }
                        }
                    }
                }
            }
        }

        private static void OnTabItemDropQuery(object sender, DragDropQueryEventArgs e)
        {
            (sender as RadTabControl).OnTabItemDropQuery(e);
        }

        private static void OnTabOrientationChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            RadTabControl tabControl = sender as RadTabControl;
            if (!IsOrientationValid(e.NewValue))
            {
                throw new ArgumentException(Telerik.Windows.Controls.SR.GetString("TabControlOrientationInvalidValue"), "e");
            }
            if (tabControl.Flags.IsLoaded && (tabControl.TabStrip != null))
            {
                tabControl.ForEachContainerItem<RadTabItem>(delegate(RadTabItem o)
                {
                    o.TabOrientation = (Orientation)e.NewValue;
                });
                tabControl.TabStrip.InvalidateMeasure();
            }
        }

        private static void OnTabStripPlacementChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            RadTabControl tabControl = sender as RadTabControl;
            Dock newPlacement = (Dock)e.NewValue;
            if (!IsTabStripPlacementValid(newPlacement))
            {
                throw new ArgumentException(Telerik.Windows.Controls.SR.GetString("TabControlTabStripPlacementInvalidValue"), "e");
            }
            tabControl.OnTabStripPlacementChangedInternal(newPlacement);
        }

        private void OnTabStripPlacementChangedInternal(Dock newValue)
        {
            if (this.TabStrip != null)
            {
                this.Flags.IsTabStripUpdated = false;
            }
            this.UpdateTabStrip();
            this.SetTemplate(this.FindTemplateFromPosition(newValue));
            this.ForEachContainerItem<RadTabItem>(delegate(RadTabItem o)
            {
                o.TabStripPlacement = this.TabStripPlacement;
            });
            this.ChangeVisualState();
        }

        private static void OnTopTemplateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            RadTabControl tabControl = sender as RadTabControl;
            if (tabControl.TabStripPlacement == Dock.Top)
            {
                tabControl.SetTemplate(e.NewValue as ControlTemplate);
            }
        }

        internal void PrepareContainerForDropDownItem(RadMenuItem menuItem, object dropDownItem)
        {
            if (this.dropDownItems.Contains(dropDownItem) && string.IsNullOrEmpty(this.DropDownMenu.DisplayMemberPath))
            {
                int index = this.dropDownItems.IndexOf(dropDownItem);
                object item = base.Items[index];
                RadTabItem tabItem = base.ItemContainerGenerator.ContainerFromIndex(index) as RadTabItem;
                DataTemplate template = tabItem.DropDownContentTemplate;
                menuItem.IsEnabled = tabItem.IsEnabled;
                menuItem.Visibility = tabItem.Visibility;
                if ((template == null) && (tabItem.DropDownContentTemplateSelector != null))
                {
                    template = tabItem.DropDownContentTemplateSelector.SelectTemplate(item, menuItem);
                }
                if ((template == null) && (dropDownItem == tabItem.Header))
                {
                    template = tabItem.HeaderTemplate;
                }
                if ((tabItem == item) || (template != null))
                {
                    menuItem.HeaderTemplate = template;
                }
            }
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            if (this.TabStrip == null)
            {
                this.tabStrip = VisualTreeHelper.GetParent(element) as Panel;
                base.Dispatcher.BeginInvoke(delegate
                {
                    this.UpdateScrollButtonsState();
                });
            }
            if ((this.TabStrip != null) && !this.Flags.IsTabStripUpdated)
            {
                this.UpdateTabStrip();
            }
            RadTabItem tabItem = element as RadTabItem;
            tabItem.TabStripPlacement = this.TabStripPlacement;
            tabItem.TabOrientation = this.TabOrientation;
            if (tabItem.IsSelected && tabItem.IsEnabled)
            {
                this.SelectedItem = item;
                this.UpdateSelectedContent(base.Items.IndexOf(item));
                base.Dispatcher.BeginInvoke(delegate
                {
                    if (this.SelectedItem == item)
                    {
                        this.ScrollIntoView(item);
                    }
                });
            }
            if (!this.isSelectedContainerUpdated && (item == this.SelectedItem))
            {
                this.isSelectedContainerUpdated = true;
                this.UpdateSelectedContainer(false, tabItem);
                this.UpdateContentElementSafely();
            }
            if (this.AllowDragOverTab || this.AllowDragReorder)
            {
                RadDragAndDropManager.SetAllowDrop(element, true);
                RadDragAndDropManager.SetAllowDrag(element, true);
            }
            StyleManager.SetThemeFromParent(tabItem, this);
            tabItem.UpdateHeaderPresenterContent();
        }

        internal bool RaisePreviewSelectionChanged(object newlySelectedItem)
        {
            if (this.suspendSelectionNotification)
            {
                return false;
            }
            object[] addedItems = new object[] { newlySelectedItem };
            object[] removedItems = new object[] { this.SelectedItem };
            Telerik.Windows.Controls.SelectionChangedEventArgs args = new Telerik.Windows.Controls.SelectionChangedEventArgs(PreviewSelectionChangedEvent, removedItems, addedItems);
            this.OnPreviewSelectionChanged(args);
            return args.Handled;
        }

        private void RearrangeTabs()
        {
            if (this.TabStrip != null)
            {
                this.TabStrip.InvalidateMeasure();
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "The AddHandler method in WPF is on UIElement")]
        public static void RemoveDropDownClosedHandler(UIElement target, DropDownEventHandler handler)
        {
            target.RemoveHandler(DropDownClosedEvent, handler);
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "The AddHandler method in WPF is on UIElement")]
        public static void RemoveDropDownOpenedHandler(UIElement target, DropDownEventHandler handler)
        {
            target.RemoveHandler(DropDownOpenedEvent, handler);
        }

        public void ScrollIntoView(object item)
        {
            int index = base.Items.IndexOf(item);
            if ((index != -1) && this.hasScrollViewer)
            {
                FrameworkElement container = base.ItemContainerGenerator.ContainerFromIndex(index) as FrameworkElement;
                if (container != null)
                {
                    ViewportRelation viewportRelation = this.CalculateContainerDirectionalVisibility(container);
                    if (viewportRelation != ViewportRelation.InView)
                    {
                        this.BringContainerIntoView(container, viewportRelation);
                    }
                }
            }
        }

        private void ScrollOneItemBack()
        {
            if ((base.HasItems && this.hasScrollViewer) && (this.tabStrip != null))
            {
                RadTabItem partiallyVisibleContainer = null;
                if (this.IsPanelHorizontal)
                {
                    partiallyVisibleContainer = this.GetContainers<RadTabItem>().Reverse<RadTabItem>().SkipWhile<RadTabItem>(delegate(RadTabItem item)
                    {
                        double x = 0.0;
                        try
                        {
                            x = item.TransformToVisual(this.tabStrip).Transform(new Point()).X;
                        }
                        catch (ArgumentException)
                        {
                        }
                        return (x >= this.scrollViewer.HorizontalOffset);
                    }).FirstOrDefault<RadTabItem>();
                }
                else
                {
                    partiallyVisibleContainer = this.GetContainers<RadTabItem>().Reverse<RadTabItem>().SkipWhile<RadTabItem>(delegate(RadTabItem item)
                    {
                        double y = 0.0;
                        try
                        {
                            y = item.TransformToVisual(this.tabStrip).Transform(new Point()).Y;
                        }
                        catch (ArgumentException)
                        {
                        }
                        return (y >= this.scrollViewer.VerticalOffset);
                    }).FirstOrDefault<RadTabItem>();
                }
                if (partiallyVisibleContainer != null)
                {
                    this.BringContainerIntoView(partiallyVisibleContainer, ViewportRelation.BeforeViewport);
                }
            }
        }

        private void ScrollOneItemForward()
        {
            if ((base.HasItems && this.hasScrollViewer) && (this.tabStrip != null))
            {
                RadTabItem partiallyVisibleContainer = null;
                if (this.IsPanelHorizontal)
                {
                    partiallyVisibleContainer = this.GetContainers<RadTabItem>().SkipWhile<RadTabItem>(delegate(RadTabItem item)
                    {
                        double x = 0.0;
                        try
                        {
                            x = item.TransformToVisual(this.tabStrip).Transform(new Point()).X;
                        }
                        catch (ArgumentException)
                        {
                        }
                        return ((((((x + item.ActualWidth) + this.tabStrip.Margin.Left) + this.tabStrip.Margin.Right) - this.scrollViewer.HorizontalOffset) - this.scrollViewer.ViewportWidth) < 6.0);
                    }).FirstOrDefault<RadTabItem>();
                }
                else
                {
                    partiallyVisibleContainer = this.GetContainers<RadTabItem>().SkipWhile<RadTabItem>(delegate(RadTabItem item)
                    {
                        double y = 0.0;
                        try
                        {
                            y = item.TransformToVisual(this.tabStrip).Transform(new Point()).Y;
                        }
                        catch (ArgumentException)
                        {
                        }
                        return ((((((y + item.ActualHeight) + this.tabStrip.Margin.Top) + this.tabStrip.Margin.Bottom) - this.scrollViewer.VerticalOffset) - this.scrollViewer.ViewportHeight) < 6.0);
                    }).FirstOrDefault<RadTabItem>();
                }
                if (partiallyVisibleContainer != null)
                {
                    this.BringContainerIntoView(partiallyVisibleContainer, ViewportRelation.AfterViewport);
                }
            }
        }

        private void ScrollOneViewportBack()
        {
            if (base.HasItems && this.hasScrollViewer)
            {
                if (this.IsPanelHorizontal)
                {
                    this.AdjustScrollBarOffset(-this.scrollViewer.ViewportWidth);
                }
                else
                {
                    this.AdjustScrollBarOffset(-this.scrollViewer.ViewportHeight);
                }
            }
        }

        private void ScrollOneViewportForward()
        {
            if (base.HasItems && this.hasScrollViewer)
            {
                if (this.IsPanelHorizontal)
                {
                    this.AdjustScrollBarOffset(this.scrollViewer.ViewportWidth);
                }
                else
                {
                    this.AdjustScrollBarOffset(this.scrollViewer.ViewportHeight);
                }
            }
        }

        private void SelectDesiredSelectedItem()
        {
            int selectedItemIndex = base.Items.IndexOf(this.SelectedItem);
            if (selectedItemIndex != -1)
            {
                this.desiredSelectedItem.Target = null;
                this.SelectedIndex = selectedItemIndex;
            }
        }

        private void SelectItemFromDropDownClick(object menuItem)
        {
            int index = this.indexedDropDownItems.IndexOf(menuItem);
            this.SelectedIndex = index;
        }

        private bool SelectNextItem(FrameworkElement focusedItem)
        {
            int startIndex = base.ItemContainerGenerator.IndexFromContainer(focusedItem) + 1;
            for (int i = startIndex; i < base.Items.Count; i++)
            {
                Control item = this.ContainerFromIndex(i);
                if (item.IsEnabled)
                {
                    this.SelectedIndex = i;
                    if (this.SelectedIndex == i)
                    {
                        item.Focus();
                    }
                    return true;
                }
            }
            return false;
        }

        private bool SelectPreviousItem(FrameworkElement focusedItem)
        {
            int startIndex = base.ItemContainerGenerator.IndexFromContainer(focusedItem) - 1;
            for (int i = startIndex; i >= 0; i--)
            {
                Control item = this.ContainerFromIndex(i);
                if (item.IsEnabled)
                {
                    this.SelectedIndex = i;
                    if (this.SelectedIndex == i)
                    {
                        item.Focus();
                    }
                    return true;
                }
            }
            return false;
        }

        protected void SetTemplate(ControlTemplate newTemplate)
        {
            if (newTemplate != null)
            {
                if (this.contentElement != null)
                {
                    this.contentElement.ClearValue(ContentPresenter.ContentProperty);
                }
                base.Template = newTemplate;
            }
        }

        private void SwitchItems(FrameworkElement destinationContainer, FrameworkElement sourceContainer, Point dragPoint)
        {
            if (base.ItemsSource != null)
            {
                this.SwitchItems(destinationContainer, sourceContainer, dragPoint, base.ItemsSource);
            }
            else
            {
                this.SwitchItems(destinationContainer, sourceContainer, dragPoint, base.Items);
            }
        }

        private void SwitchItems(FrameworkElement destinationContainer, FrameworkElement sourceContainer, Point dragPoint, IEnumerable enumerable)
        {
            IList list = enumerable as IList;
            if ((list != null) && !list.IsReadOnly)
            {
                object destinationItem = base.ItemContainerGenerator.ItemFromContainer(destinationContainer);
                object sourceItem = base.ItemContainerGenerator.ItemFromContainer(sourceContainer);
                if ((sourceItem != null) && (destinationItem != null))
                {
                    int sourceIndex = list.IndexOf(sourceItem);
                    int destinationIndex = list.IndexOf(destinationItem);
                    int smallerIndex = Math.Min(sourceIndex, destinationIndex);
                    int biggerIndex = Math.Max(sourceIndex, destinationIndex);
                    if (((this.lastSwap != null) && (this.lastSwap.BiggerIndex == biggerIndex)) && (this.lastSwap.SmallerIndex == smallerIndex))
                    {
                        Point destinationOrigin = new Point();
                        try
                        {
                            Rect destinationSlot = LayoutInformation.GetLayoutSlot(destinationContainer);
                            Point panelTopLeft = this.TabStrip.TransformToVisual(null).Transform(new Point(0.0, 0.0));
                            destinationOrigin = new Point(panelTopLeft.X + destinationSlot.X, destinationSlot.Y + panelTopLeft.Y);
                        }
                        catch (ArgumentException)
                        {
                        }
                        int margin = 5;
                        if (((dragPoint.X < (destinationOrigin.X + margin)) || (dragPoint.Y < (destinationOrigin.Y + margin))) || (((dragPoint.X + margin) > (destinationOrigin.X + sourceContainer.ActualWidth)) || ((dragPoint.Y + margin) > (destinationOrigin.Y + sourceContainer.ActualHeight))))
                        {
                            return;
                        }
                    }
                    object smallerItem = list[smallerIndex];
                    object biggerItem = list[biggerIndex];
                    this.suspendSelectionNotification = true;
                    try
                    {
                        list.Remove(biggerItem);
                        list.RemoveAt(smallerIndex);
                        list.Insert(smallerIndex, biggerItem);
                        list.Insert(biggerIndex, smallerItem);
                        this.UpdateSelectedContent(-1);
                        this.SelectedIndex = destinationIndex;
                    }
                    finally
                    {
                        this.suspendSelectionNotification = false;
                    }
                    this.lastSwap = new TabSwap { SmallerIndex = smallerIndex, BiggerIndex = biggerIndex };
                    if (this.TabStrip != null)
                    {
                        this.TabStrip.InvalidateMeasure();
                    }
                }
            }
        }

        internal void TryFocusContent()
        {
            if (this.Flags.HasContentElement && (this.SelectedContent != null))
            {
                base.Dispatcher.BeginInvoke(delegate
                {
                    Control firstControlInContent = this.contentElement.GetFirstDescendantOfType<Control>();
                    FrameworkElement visibleContent = this.SelectedContent as FrameworkElement;
                    if ((firstControlInContent == null) && (visibleContent != null))
                    {
                        firstControlInContent = visibleContent.GetFirstDescendantOfType<Control>();
                    }
                    if (firstControlInContent != null)
                    {
                        firstControlInContent.Focus();
                    }
                });
            }
        }

        private void UpdateContentElementSafely()
        {
            if (this.contentElement != null)
            {
                UIElement visualContent = this.SelectedContent as UIElement;
                if ((visualContent == null) || (VisualTreeHelper.GetParent(visualContent) == null))
                {
                    this.contentElement.Content = this.SelectedContent;
                }
            }
        }

        private void UpdateDropDownButtonVisibility()
        {
            if (this.DropDownDisplayMode == TabControlDropDownDisplayMode.WhenNeeded)
            {
                base.InvalidateArrange();
            }
            else
            {
                this.ChangeVisualState();
            }
        }

        private void UpdateDropDownMenuOnItemsChange(NotifyCollectionChangedEventArgs e)
        {
            if (this.IsDropDownOpen)
            {
                if ((e.Action == NotifyCollectionChangedAction.Remove) && (this.dropDownItems.Count > e.OldStartingIndex))
                {
                    this.dropDownItems.RemoveAt(e.OldStartingIndex);
                }
                else if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    object addedDropDownItem = this.GetDropDownItem(e.NewItems[0]);
                    this.dropDownItems.Insert(e.NewStartingIndex, addedDropDownItem);
                }
                else if (e.Action == NotifyCollectionChangedAction.Replace)
                {
                    object addedDropDownItem = this.GetDropDownItem(e.NewItems[0]);
                    this.dropDownItems[e.NewStartingIndex] = addedDropDownItem;
                }
                else
                {
                    this.dropDownItems.Clear();
                }
            }
        }

        private void UpdateScrollButtonsState()
        {
            if (this.hasScrollViewer)
            {
                if (this.hasLeftScrollButton)
                {
                    this.leftScrollButton.IsEnabled = this.CanScrollBack();
                }
                if (this.hasRightScrollButton)
                {
                    this.rightScrollButton.IsEnabled = this.CanScrollForward();
                }
            }
        }

        private void UpdateSelectedContainer(bool shouldFocus, RadTabItem selectedItemContainer)
        {
            this.UpdateTabItemIsSelectedAfterSelection(selectedItemContainer);
            DependencyObject focusedElement = FocusManager.GetFocusedElement() as DependencyObject;
            if (((this.contentElement != null) && (focusedElement != null)) && this.contentElement.IsAncestorOf(focusedElement))
            {
                if (this.ShouldSetIsTabStop)
                {
                    selectedItemContainer.IsTabStop = true;
                }
                selectedItemContainer.Focus();
            }
            this.UpdateSelectedContentTemplate(selectedItemContainer);
            this.SelectedContent = selectedItemContainer.Content;
            this.UpdateSelectedElementDataContext();
            if (shouldFocus)
            {
                this.TryFocusContent();
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "FocusManager errors are unpredictable.")]
        private void UpdateSelectedContent(int newIndex)
        {
            if (newIndex == -1)
            {
                this.DeselectAll();
            }
            else
            {
                bool shouldFocus = false;
                FrameworkElement focusedElement = FocusManager.GetFocusedElement() as FrameworkElement;
                shouldFocus = (focusedElement == null) || ((this.contentElement != null) && this.contentElement.IsAncestorOf(focusedElement));
                this.SelectedItem = base.Items[newIndex];
                RadTabItem selectedItemContainer = this.ContainerFromIndex(newIndex) ?? (this.SelectedItem as RadTabItem);
                this.isSelectedContainerUpdated = selectedItemContainer != null;
                if (this.isSelectedContainerUpdated)
                {
                    this.UpdateSelectedContainer(shouldFocus, selectedItemContainer);
                }
            }
            if (this.ReorderTabRows && (this.TabStrip != null))
            {
                this.TabStrip.InvalidateArrange();
            }
        }

        private void UpdateSelectedContentTemplate(RadTabItem selectedItemContainer)
        {
            this.SelectedContentTemplate = selectedItemContainer.ContentTemplate;
            if (this.SelectedContentTemplate == null)
            {
                if (this.ContentTemplate != null)
                {
                    this.SelectedContentTemplate = this.ContentTemplate;
                }
                else if (this.ContentTemplateSelector != null)
                {
                    this.SelectedContentTemplate = this.ContentTemplateSelector.SelectTemplate(this.SelectedItem, selectedItemContainer);
                    this.SelectedContentTemplateSelector = this.ContentTemplateSelector;
                }
            }
        }

        private void UpdateSelectedElementDataContext()
        {
            if (this.Flags.HasContentElement && this.PropagateItemDataContextToContent)
            {
                if (this.SelectedItem == null)
                {
                    this.contentElement.DataContext = null;
                }
                if (this.SelectedItem is UIElement)
                {
                    FrameworkElement item = this.SelectedItem as FrameworkElement;
                    if (item != null)
                    {
                        this.contentElement.DataContext = item.DataContext;
                    }
                    else
                    {
                        this.contentElement.DataContext = null;
                    }
                }
                else
                {
                    this.contentElement.DataContext = this.SelectedItem;
                }
            }
        }

        private void UpdateSelectionOnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                if ((this.Flags.IsDesiredIndexActive && (this.desiredSelectedIndex >= 0)) && (this.desiredSelectedIndex < base.Items.Count))
                {
                    this.SelectedIndex = this.desiredSelectedIndex;
                    this.Flags.IsDesiredIndexActive = false;
                }
                else if (this.desiredSelectedItem.IsAlive && base.Items.Contains(this.desiredSelectedItem.Target))
                {
                    this.SelectDesiredSelectedItem();
                }
            }
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                object addedItem = e.NewItems[0];
                if (base.Items.IndexOf(addedItem) <= this.SelectedIndex)
                {
                    this.JustChangeSelectedIndex(this.SelectedIndex + 1);
                }
                RadTabItem tabItem = addedItem as RadTabItem;
                if (((tabItem != null) && tabItem.IsSelected) && tabItem.IsEnabled)
                {
                    this.SelectedIndex = base.Items.IndexOf(tabItem);
                    this.Flags.IsDesiredIndexActive = false;
                }
                if ((this.Flags.IsDesiredIndexActive && (this.desiredSelectedIndex >= 0)) && (this.desiredSelectedIndex < base.Items.Count))
                {
                    this.SelectedIndex = this.desiredSelectedIndex;
                    this.Flags.IsDesiredIndexActive = false;
                }
                else if (this.desiredSelectedItem.IsAlive && (e.NewItems[0] == this.desiredSelectedItem.Target))
                {
                    this.SelectedItem = e.NewItems[0];
                }
            }
            if (((e.Action == NotifyCollectionChangedAction.Replace) && (this.SelectedItem != null)) && this.SelectedItem.Equals(e.OldItems[0]))
            {
                this.SelectedItem = e.NewItems[0];
            }
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (object removedItem in e.OldItems)
                {
                    if (removedItem == this.SelectedItem)
                    {
                        if (base.Items.Count > 0)
                        {
                            this.SelectedItem = base.Items[Math.Min(this.SelectedIndex, base.Items.Count - 1)];
                        }
                        else
                        {
                            this.SelectedIndex = -1;
                        }
                        continue;
                    }
                    if (e.OldStartingIndex < this.SelectedIndex)
                    {
                        this.JustChangeSelectedIndex(this.SelectedIndex - 1);
                    }
                }
            }
            if (base.Items.Count == 0)
            {
                this.SelectedIndex = -1;
            }
            else if ((this.SelectedIndex == -1) && this.Flags.IsLoaded)
            {
                this.SelectedIndex = 0;
            }
        }

        private void UpdateTabItemIsSelectedAfterSelection(RadTabItem selectedItemContainer)
        {
            this.ForEachContainerItem<RadTabItem>(delegate(RadTabItem o)
            {
                if (selectedItemContainer != o)
                {
                    o.IsSelected = false;
                }
            });
            selectedItemContainer.IsSelected = true;
        }

        private void UpdateTabStrip()
        {
            TabStripPanel stripPanel = this.TabStrip as TabStripPanel;
            if (stripPanel != null)
            {
                stripPanel.TabStripPlacement = this.TabStripPlacement;
                stripPanel.Align = this.Align;
                stripPanel.AllTabsEqualHeight = this.AllTabsEqualHeight;
                stripPanel.RearrangeTabs = this.ReorderTabRows;
                stripPanel.InvalidateArrange();
                this.Flags.IsTabStripUpdated = true;
            }
        }

        private void UpdateZIndexes()
        {
            if (this.Flags.IsLoaded && (this.SelectedItem != null))
            {
                for (int itemIndex = 0; itemIndex < base.Items.Count; itemIndex++)
                {
                    FrameworkElement itemContainer = base.ItemContainerGenerator.ContainerFromIndex(itemIndex) as FrameworkElement;
                    if (itemContainer != null)
                    {
                        Canvas.SetZIndex(itemContainer, 0);
                    }
                }
                FrameworkElement selectedItemContainer = base.ItemContainerGenerator.ContainerFromItem(this.SelectedItem) as FrameworkElement;
                if (selectedItemContainer != null)
                {
                    Canvas.SetZIndex(selectedItemContainer, 1);
                }
            }
        }

        [Telerik.Windows.Controls.SRCategory("AppearanceCategory"), DefaultValue(2), Telerik.Windows.Controls.SRDescription("TabControlAlignDescription")]
        public virtual TabStripAlign Align
        {
            get
            {
                return (TabStripAlign)base.GetValue(AlignProperty);
            }
            set
            {
                base.SetValue(AlignProperty, value);
            }
        }

        [Telerik.Windows.Controls.SRCategory("Behavior"), DefaultValue(false)]
        public bool AllowDragOverTab
        {
            get
            {
                return (bool)base.GetValue(AllowDragOverTabProperty);
            }
            set
            {
                base.SetValue(AllowDragOverTabProperty, value);
            }
        }

        [DefaultValue(false), Telerik.Windows.Controls.SRCategory("Behavior")]
        public bool AllowDragReorder
        {
            get
            {
                return (bool)base.GetValue(AllowDragReorderProperty);
            }
            set
            {
                base.SetValue(AllowDragReorderProperty, value);
            }
        }

        [Telerik.Windows.Controls.SRCategory("AppearanceCategory"), Telerik.Windows.Controls.SRDescription("TabControlAllTabsEqualHeightDescription"), DefaultValue(true)]
        public virtual bool AllTabsEqualHeight
        {
            get
            {
                return (bool)base.GetValue(AllTabsEqualHeightProperty);
            }
            set
            {
                base.SetValue(AllTabsEqualHeightProperty, value);
            }
        }

        [Telerik.Windows.Controls.SRCategory("AppearanceCategory"), Telerik.Windows.Controls.SRDescription("TabControlBackgroundOpacityDescription"), DefaultValue(0)]
        public virtual Visibility BackgroundVisibility
        {
            get
            {
                return (Visibility)base.GetValue(BackgroundVisibilityProperty);
            }
            set
            {
                base.SetValue(BackgroundVisibilityProperty, value);
            }
        }

        [Telerik.Windows.Controls.SRCategory("Appearance"), DefaultValue((string)null), Telerik.Windows.Controls.SRDescription("TabControlBottomTemplateDescription")]
        public virtual ControlTemplate BottomTemplate
        {
            get
            {
                return (ControlTemplate)base.GetValue(BottomTemplateProperty);
            }
            set
            {
                base.SetValue(BottomTemplateProperty, value);
            }
        }

        internal ContentPresenter ContentElement
        {
            get
            {
                return this.contentElement;
            }
        }

        [Telerik.Windows.Controls.SRCategory("AppearanceCategory")]
        public DataTemplate ContentTemplate
        {
            get
            {
                return (DataTemplate)base.GetValue(ContentTemplateProperty);
            }
            set
            {
                base.SetValue(ContentTemplateProperty, value);
            }
        }

        [Telerik.Windows.Controls.SRCategory("AppearanceCategory")]
        public DataTemplateSelector ContentTemplateSelector
        {
            get
            {
                return (DataTemplateSelector)base.GetValue(ContentTemplateSelectorProperty);
            }
            set
            {
                base.SetValue(ContentTemplateSelectorProperty, value);
            }
        }

        internal ToggleButton DropDownButton { get; private set; }

        [Telerik.Windows.Controls.SRCategory("AppearanceCategory")]
        public Style DropDownButtonStyle
        {
            get
            {
                return (Style)base.GetValue(DropDownButtonStyleProperty);
            }
            set
            {
                base.SetValue(DropDownButtonStyleProperty, value);
            }
        }

        public string DropDownDisplayMemberPath
        {
            get
            {
                return (string)base.GetValue(DropDownDisplayMemberPathProperty);
            }
            set
            {
                base.SetValue(DropDownDisplayMemberPathProperty, value);
            }
        }

        public TabControlDropDownDisplayMode DropDownDisplayMode
        {
            get
            {
                return (TabControlDropDownDisplayMode)base.GetValue(DropDownDisplayModeProperty);
            }
            set
            {
                base.SetValue(DropDownDisplayModeProperty, value);
            }
        }

        internal IList DropDownItems
        {
            get
            {
                return this.dropDownItems;
            }
        }

        internal Telerik.Windows.Controls.TabControl.DropDownMenu DropDownMenu { get; private set; }

        [Telerik.Windows.Controls.SRCategory("AppearanceCategory")]
        public Style DropDownStyle
        {
            get
            {
                return (Style)base.GetValue(DropDownStyleProperty);
            }
            set
            {
                base.SetValue(DropDownStyleProperty, value);
            }
        }

        private StateFlags Flags
        {
            get
            {
                return this.flags;
            }
        }

        private bool IsDropDownButtonCollapsed
        {
            get
            {
                Visibility computedVisibility = this.GetComputedVisibility();
                if ((this.DropDownDisplayMode != TabControlDropDownDisplayMode.Collapsed) && (((this.DropDownDisplayMode != TabControlDropDownDisplayMode.WhenNeeded) || !this.hasScrollViewer) || (computedVisibility != Visibility.Collapsed)))
                {
                    return false;
                }
                return true;
            }
        }

        [Telerik.Windows.Controls.SRCategory("AppearanceCategory")]
        public bool IsDropDownOpen
        {
            get
            {
                return (bool)base.GetValue(IsDropDownOpenProperty);
            }
            set
            {
                base.SetValue(IsDropDownOpenProperty, value);
            }
        }

        private bool IsPanelHorizontal
        {
            get
            {
                if (!(this.TabStrip is TabWrapPanel) && (this.TabStripPlacement != Dock.Top))
                {
                    return (this.TabStripPlacement == Dock.Bottom);
                }
                return true;
            }
        }

        public virtual RadTabItem this[int index]
        {
            get
            {
                return this.ContainerFromIndex(index);
            }
        }

        [Telerik.Windows.Controls.SRCategory("AppearanceCategory")]
        public DataTemplate ItemDropDownContentTemplate
        {
            get
            {
                return (DataTemplate)base.GetValue(ItemDropDownContentTemplateProperty);
            }
            set
            {
                base.SetValue(ItemDropDownContentTemplateProperty, value);
            }
        }

        [Telerik.Windows.Controls.SRCategory("AppearanceCategory")]
        public DataTemplateSelector ItemDropDownContentTemplateSelector
        {
            get
            {
                return (DataTemplateSelector)base.GetValue(ItemDropDownContentTemplateSelectorProperty);
            }
            set
            {
                base.SetValue(ItemDropDownContentTemplateSelectorProperty, value);
            }
        }

        internal ButtonBase LeftScrollButton
        {
            get
            {
                return this.leftScrollButton;
            }
        }

        [Telerik.Windows.Controls.SRDescription("TabControlLeftTemplateDescription"), Telerik.Windows.Controls.SRCategory("Appearance"), DefaultValue((string)null)]
        public virtual ControlTemplate LeftTemplate
        {
            get
            {
                return (ControlTemplate)base.GetValue(LeftTemplateProperty);
            }
            set
            {
                base.SetValue(LeftTemplateProperty, value);
            }
        }

        public TabOverflowMode OverflowMode
        {
            get
            {
                return (TabOverflowMode)base.GetValue(OverflowModeProperty);
            }
            set
            {
                base.SetValue(OverflowModeProperty, value);
            }
        }

        public bool PropagateItemDataContextToContent
        {
            get
            {
                return (bool)base.GetValue(PropagateItemDataContextToContentProperty);
            }
            set
            {
                base.SetValue(PropagateItemDataContextToContentProperty, value);
            }
        }

        [DefaultValue(true), Telerik.Windows.Controls.SRCategory("BehaviourCategory"), Telerik.Windows.Controls.SRDescription("TabControlReorderTabRowsDescription")]
        public virtual bool ReorderTabRows
        {
            get
            {
                return (bool)base.GetValue(ReorderTabRowsProperty);
            }
            set
            {
                base.SetValue(ReorderTabRowsProperty, value);
            }
        }

        internal ButtonBase RightScrollButton
        {
            get
            {
                return this.rightScrollButton;
            }
        }

        [Telerik.Windows.Controls.SRDescription("TabControlRightTemplateDescription"), DefaultValue((string)null), Telerik.Windows.Controls.SRCategory("Appearance")]
        public virtual ControlTemplate RightTemplate
        {
            get
            {
                return (ControlTemplate)base.GetValue(RightTemplateProperty);
            }
            set
            {
                base.SetValue(RightTemplateProperty, value);
            }
        }

        public virtual TabControlScrollMode ScrollMode
        {
            get
            {
                return (TabControlScrollMode)base.GetValue(ScrollModeProperty);
            }
            set
            {
                base.SetValue(ScrollModeProperty, value);
            }
        }

        [Browsable(false), DefaultValue((string)null)]
        public object SelectedContent
        {
            get
            {
                return base.GetValue(SelectedContentProperty);
            }
            set
            {
                base.SetValue(SelectedContentProperty, value);
            }
        }

        [Browsable(false), DefaultValue((string)null)]
        public DataTemplate SelectedContentTemplate
        {
            get
            {
                return (DataTemplate)base.GetValue(SelectedContentTemplateProperty);
            }
            set
            {
                base.SetValue(SelectedContentTemplateProperty, value);
            }
        }

        [DefaultValue((string)null), Telerik.Windows.Controls.SRCategory("AppearanceCategory"), Browsable(false)]
        public DataTemplateSelector SelectedContentTemplateSelector
        {
            get
            {
                return (DataTemplateSelector)base.GetValue(SelectedContentTemplateSelectorProperty);
            }
            set
            {
                base.SetValue(SelectedContentTemplateSelectorProperty, value);
            }
        }

        [DefaultValue(-1), Telerik.Windows.Controls.SRCategory("AppearanceCategory"), Telerik.Windows.Controls.SRDescription("TabControlSelectedIndexDescription")]
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

        [DefaultValue((string)null)]
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

        internal bool ShouldSetIsTabStop
        {
            get
            {
                return this.shouldSetIsTabStop;
            }
            set
            {
                this.shouldSetIsTabStop = value;
            }
        }

        [Telerik.Windows.Controls.SRCategory("AppearanceCategory"), Telerik.Windows.Controls.SRDescription("TabControlTabOrientationDescription"), DefaultValue(1)]
        public virtual Orientation TabOrientation
        {
            get
            {
                return (Orientation)base.GetValue(TabOrientationProperty);
            }
            set
            {
                base.SetValue(TabOrientationProperty, value);
            }
        }

        internal Panel TabStrip
        {
            get
            {
                return this.tabStrip;
            }
        }

        [Telerik.Windows.Controls.SRCategory("AppearanceCategory"), Telerik.Windows.Controls.SRDescription("TabControlTabStripPlacementDescription"), DefaultValue(1)]
        public virtual Dock TabStripPlacement
        {
            get
            {
                return (Dock)base.GetValue(TabStripPlacementProperty);
            }
            set
            {
                base.SetValue(TabStripPlacementProperty, value);
            }
        }

        [Telerik.Windows.Controls.SRDescription("TabControlTopTemplateDescription"), DefaultValue((string)null), Telerik.Windows.Controls.SRCategory("Appearance")]
        public virtual ControlTemplate TopTemplate
        {
            get
            {
                return (ControlTemplate)base.GetValue(TopTemplateProperty);
            }
            set
            {
                base.SetValue(TopTemplateProperty, value);
            }
        }

        private class StateFlags
        {
            internal bool HasContentElement { get; set; }

            internal bool IsDesiredIndexActive { get; set; }

            internal bool IsLoaded { get; set; }

            internal bool IsTabStripUpdated { get; set; }

            internal bool NonSelectionIndexUpdate { get; set; }

            internal bool SelectionInProgress { get; set; }
        }

        private class TabSwap
        {
            internal int BiggerIndex { get; set; }

            internal int SmallerIndex { get; set; }
        }

        private enum ViewportRelation
        {
            InView,
            BeforeViewport,
            AfterViewport
        }
    }
}

