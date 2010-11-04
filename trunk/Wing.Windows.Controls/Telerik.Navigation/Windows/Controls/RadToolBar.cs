namespace Telerik.Windows.Controls
{
    using System;
    using System.Collections;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using Telerik.Windows;

    [TemplatePart(Name="PART_Overflow", Type=typeof(Popup)), TemplatePart(Name="PART_StripPanel", Type=typeof(StackPanel)), TemplatePart(Name="PART_DropdownButton", Type=typeof(ToggleButton)), DefaultProperty("Items"), TemplatePart(Name="PART_OverflowPanel", Type=typeof(StackPanel))]
    public class RadToolBar : Telerik.Windows.Controls.ItemsControl
    {
        public static readonly DependencyProperty BandIndexProperty = DependencyProperty.Register("BandIndex", typeof(int), typeof(RadToolBar), new Telerik.Windows.PropertyMetadata(-1, new PropertyChangedCallback(RadToolBar.OnBandIndexChanged)));
        public static readonly DependencyProperty BandProperty = DependencyProperty.Register("Band", typeof(int), typeof(RadToolBar), new Telerik.Windows.PropertyMetadata(0, new PropertyChangedCallback(RadToolBar.OnBandChanged)));
        private static readonly DependencyProperty ContainerStyleSelectedByPanelbarProperty = DependencyProperty.RegisterAttached("ContainerStyleSelectedByPanelbar", typeof(Style), typeof(RadToolBar), null);
        public static readonly DependencyProperty GripVisibilityProperty = DependencyProperty.Register("GripVisibility", typeof(Visibility), typeof(RadToolBar), new Telerik.Windows.PropertyMetadata(Visibility.Visible));
        private static readonly DependencyPropertyKey HasOverflowItemsPropertyKey = DependencyPropertyExtensions.RegisterReadOnly("HasOverflowItems", typeof(bool), typeof(RadToolBar), new Telerik.Windows.PropertyMetadata(false));
        public static readonly DependencyProperty HasOverflowItemsProperty = HasOverflowItemsPropertyKey.DependencyProperty;
        public static readonly DependencyProperty IsOverflowOpenProperty = DependencyProperty.Register("IsOverflowOpen", typeof(bool), typeof(RadToolBar), new Telerik.Windows.PropertyMetadata(false, new PropertyChangedCallback(RadToolBar.OnIsOverflowOpenChanged)));
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(System.Windows.Controls.Orientation), typeof(RadToolBar), new Telerik.Windows.PropertyMetadata(System.Windows.Controls.Orientation.Horizontal, new PropertyChangedCallback(RadToolBar.OnOrientationChanged)));
        public static readonly Telerik.Windows.RoutedEvent OverflowAreaClosedEvent = EventManager.RegisterRoutedEvent("OverflowAreaClosed", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(RadToolBar));
        public static readonly Telerik.Windows.RoutedEvent OverflowAreaOpenedEvent = EventManager.RegisterRoutedEvent("OverflowAreaOpened", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(RadToolBar));
        public static readonly DependencyProperty OverflowButtonVisibilityProperty = DependencyProperty.Register("OverflowButtonVisibility", typeof(Visibility), typeof(RadToolBar), new Telerik.Windows.PropertyMetadata(Visibility.Visible));
        private static readonly DependencyProperty OverflowItemsProperty = DependencyProperty.Register("OverflowItems", typeof(IList), typeof(RadToolBar), null);
        public static readonly DependencyProperty OverflowModeProperty = DependencyProperty.RegisterAttached("OverflowMode", typeof(OverflowMode), typeof(RadToolBar), new Telerik.Windows.PropertyMetadata(OverflowMode.AsNeeded, new PropertyChangedCallback(RadToolBar.OnOverflowModeChanged)));
        private Popup overflowPopup;
        private PopupWrapper overflowPopupWrapper;
        private static readonly DependencyProperty StripItemsProperty = DependencyProperty.Register("StripItems", typeof(IList), typeof(RadToolBar), null);

        public event RoutedEventHandler OverflowAreaClosed
        {
            add
            {
                this.AddHandler(OverflowAreaClosedEvent, value);
            }
            remove
            {
                this.RemoveHandler(OverflowAreaClosedEvent, value);
            }
        }

        public event RoutedEventHandler OverflowAreaOpened
        {
            add
            {
                this.AddHandler(OverflowAreaOpenedEvent, value);
            }
            remove
            {
                this.RemoveHandler(OverflowAreaOpenedEvent, value);
            }
        }

        public RadToolBar()
        {
            base.DefaultStyleKey = typeof(RadToolBar);
            ObservableCollection<object> observableCollection = new ObservableCollection<object>();
            observableCollection.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnStripItemsCollectionChanged);
            this.StripItems = observableCollection;
            observableCollection = new ObservableCollection<object>();
            observableCollection.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnOverflowItemsCollectionChanged);
            this.OverflowItems = observableCollection;
            base.KeyDown += new KeyEventHandler(this.OnKeyDown);
        }

        private double CalcMinLength()
        {
            bool isHorizontal = this.Orientation == System.Windows.Controls.Orientation.Horizontal;
            double minLength = isHorizontal ? base.MinWidth : base.MinHeight;
            double calcLength = 0.0;
            if (this.StripPanel != null)
            {
                double thisLength = GetLength(isHorizontal, base.DesiredSize);
                double panelLength = GetLength(isHorizontal, this.StripPanel.DesiredSize);
                if (thisLength.HasValue() && panelLength.HasValue())
                {
                    double stripMarginNear = GetLength(isHorizontal, this.StripPanel.Margin.Left, this.StripPanel.Margin.Top);
                    double stripMarginFar = GetLength(isHorizontal, this.StripPanel.Margin.Right, this.StripPanel.Margin.Bottom);
                    calcLength += ((thisLength - panelLength) + stripMarginNear) + stripMarginFar;
                }
                foreach (object item in base.Items)
                {
                    UIElement container = base.ItemContainerGenerator.ContainerFromItem(item) as UIElement;
                    if (((container != null) && (container.Visibility == Visibility.Visible)) && (GetOverflowMode(container) == OverflowMode.Never))
                    {
                        double containerLength = GetLength(isHorizontal, container.DesiredSize);
                        if (containerLength.HasValue())
                        {
                            calcLength += containerLength;
                        }
                    }
                }
            }
            if (calcLength <= minLength)
            {
                return minLength;
            }
            return calcLength;
        }

        internal bool CanUnstripItem(Size availableSize, Size returnedSize)
        {
            if (!this.HasItemForStrip())
            {
                return false;
            }
            double availableLen = this.GetLength(availableSize);
            double returnedLen = this.GetLength(returnedSize);
            return IsSpaceLarger(availableLen, returnedLen);
        }

        protected internal override void ChangeVisualState(bool useTransitions)
        {
            base.ChangeVisualState(useTransitions);
            if (this.Orientation == System.Windows.Controls.Orientation.Vertical)
            {
                this.GoToState(useTransitions, new string[] { "Vertical" });
                if ((this.overflowPopupWrapper != null) && (this.overflowPopupWrapper.Placement != Telerik.Windows.Controls.PlacementMode.Right))
                {
                    this.overflowPopupWrapper.Placement = Telerik.Windows.Controls.PlacementMode.Right;
                }
            }
            else
            {
                this.GoToState(useTransitions, new string[] { "Horizontal" });
                if ((this.overflowPopupWrapper != null) && (this.overflowPopupWrapper.Placement != Telerik.Windows.Controls.PlacementMode.Bottom))
                {
                    this.overflowPopupWrapper.Placement = Telerik.Windows.Controls.PlacementMode.Bottom;
                }
            }
        }

        private void ClearPanels()
        {
            if (this.StripPanel != null)
            {
                this.StripPanel.Children.Clear();
            }
            if (this.OverflowPanel != null)
            {
                this.OverflowPanel.Children.Clear();
            }
        }

        internal void ConformToOverflowMode()
        {
            if (this.HasPanels)
            {
                bool invalidateMeasure = false;
                int i = this.StripItems.Count;
                while (i-- > 0)
                {
                    object item = this.StripItems[i];
                    if (DetermineOverflowMode(item) == OverflowMode.Always)
                    {
                        this.TryMoveStripItemToOverflow(i);
                        invalidateMeasure = true;
                    }
                }
                i = this.OverflowItems.Count;
                while (i-- > 0)
                {
                    object item = this.OverflowItems[i];
                    if (DetermineOverflowMode(item) == OverflowMode.Never)
                    {
                        this.TryMoveOverflowItemToStrip(i);
                        invalidateMeasure = true;
                    }
                }
                if (invalidateMeasure)
                {
                    this.OverflowPanel.InvalidateMeasure();
                    this.StripPanel.InvalidateMeasure();
                    base.InvalidateMeasure();
                    this.InvalidateHostingTray();
                }
            }
        }

        internal static void CorrectLength(bool isHorizontal, double length, ref Rect rect)
        {
            if (isHorizontal)
            {
                rect.Width += length;
            }
            else
            {
                rect.Height += length;
            }
        }

        internal static void CorrectLength(bool isHorizontal, double length, ref Size size)
        {
            if (isHorizontal)
            {
                size.Width += length;
            }
            else
            {
                size.Height += length;
            }
        }

        internal static void CorrectLength(bool isHorizontal, double length, ref double width, ref double height)
        {
            if (isHorizontal)
            {
                width += length;
            }
            else
            {
                height += length;
            }
        }

        internal static void CorrectOffset(bool isHorizontal, double offset, ref Rect rect)
        {
            CorrectOffset(isHorizontal, offset, offset, ref rect);
        }

        internal static void CorrectOffset(bool isHorizontal, double offsetX, double offsetY, ref Rect rect)
        {
            if (isHorizontal)
            {
                rect.X += offsetX;
            }
            else
            {
                rect.Y += offsetY;
            }
        }

        internal static void CorrectThickness(bool isHorizontal, double thickness, ref Rect rect)
        {
            if (isHorizontal)
            {
                rect.Height += thickness;
            }
            else
            {
                rect.Width += thickness;
            }
        }

        internal static void CorrectThickness(bool isHorizontal, double thickness, ref Size size)
        {
            if (isHorizontal)
            {
                size.Height += thickness;
            }
            else
            {
                size.Width += thickness;
            }
        }

        internal static void CorrectThickness(bool isHorizontal, double thickness, ref double width, ref double height)
        {
            if (isHorizontal)
            {
                height += thickness;
            }
            else
            {
                width += thickness;
            }
        }

        internal static OverflowMode DetermineOverflowMode(object o)
        {
            DependencyObject dependencyObject = o as DependencyObject;
            if (dependencyObject == null)
            {
                return OverflowMode.AsNeeded;
            }
            return GetOverflowMode(dependencyObject);
        }

        internal int FindOverflowIndexForStrip()
        {
            for (int position = 0; position < this.OverflowItems.Count; position++)
            {
                if (DetermineOverflowMode(this.OverflowItems[position]) != OverflowMode.Always)
                {
                    return position;
                }
            }
            return -1;
        }

        internal int FindOverflowInsertIndex(object item)
        {
            int index = base.Items.IndexOf(item);
            for (int position = 0; position < this.OverflowItems.Count; position++)
            {
                if (index <= base.Items.IndexOf(this.OverflowItems[position]))
                {
                    return position;
                }
            }
            return this.OverflowItems.Count;
        }

        internal int FindStripIndexForOverflow()
        {
            int position = this.StripItems.Count;
            while (position-- > 0)
            {
                if (DetermineOverflowMode(this.StripItems[position]) != OverflowMode.Never)
                {
                    return position;
                }
            }
            return -1;
        }

        internal int FindStripInsertIndex(object item)
        {
            int index = base.Items.IndexOf(item);
            for (int position = 0; position < this.StripItems.Count; position++)
            {
                if (index <= base.Items.IndexOf(this.StripItems[position]))
                {
                    return position;
                }
            }
            return this.StripItems.Count;
        }

        internal Style GetContainerStyle(object item, DependencyObject element)
        {
            Style style = null;
            if (base.ItemContainerStyleSelector != null)
            {
                style = base.ItemContainerStyleSelector.SelectStyle(item, element);
            }
            return style;
        }

        internal double GetLength(Size size)
        {
            return GetLength(this.Orientation == System.Windows.Controls.Orientation.Horizontal, size);
        }

        internal static double GetLength(bool isHorizontal, Rect rect)
        {
            return GetLength(isHorizontal, rect.Width, rect.Height);
        }

        internal static double GetLength(bool isHorizontal, Size size)
        {
            return GetLength(isHorizontal, size.Width, size.Height);
        }

        internal static double GetLength(bool isHorizontal, double width, double height)
        {
            if (!isHorizontal)
            {
                return height;
            }
            return width;
        }

        internal Size GetMeasureTestSize(Size availableSize)
        {
            if (this.Orientation != System.Windows.Controls.Orientation.Horizontal)
            {
                availableSize.Height = double.PositiveInfinity;
                return availableSize;
            }
            availableSize.Width = double.PositiveInfinity;
            return availableSize;
        }

        public static OverflowMode GetOverflowMode(DependencyObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return (OverflowMode) element.GetValue(OverflowModeProperty);
        }

        private void GetTemplateChildren()
        {
            this.StripPanel = base.GetTemplateChild("PART_StripPanel") as Panel;
            this.OverflowPanel = base.GetTemplateChild("PART_OverflowPanel") as Panel;
            this.DropdownButton = base.GetTemplateChild("PART_DropdownButton") as ToggleButton;
            if (this.DropdownButton != null)
            {
                if ((this.DropdownButton.IsChecked.GetValueOrDefault() != this.IsOverflowOpen) || !this.DropdownButton.IsChecked.HasValue)
                {
                    this.DropdownButton.IsChecked = new bool?(this.IsOverflowOpen);
                }
            }
        }

        internal static double GetThickness(bool isHorizontal, Rect rect)
        {
            return GetThickness(isHorizontal, rect.Width, rect.Height);
        }

        internal static double GetThickness(bool isHorizontal, Size size)
        {
            return GetThickness(isHorizontal, size.Width, size.Height);
        }

        internal static double GetThickness(bool isHorizontal, double width, double height)
        {
            if (!isHorizontal)
            {
                return width;
            }
            return height;
        }

        private void GoToState(bool useTransitions, params string[] stateNames)
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

        internal bool HasItemForOverflow()
        {
            return (this.FindStripIndexForOverflow() >= 0);
        }

        internal bool HasItemForStrip()
        {
            return (this.FindOverflowIndexForStrip() >= 0);
        }

        private void InitializeOverflowPopup()
        {
            if (this.overflowPopup != null)
            {
                this.overflowPopupWrapper.ClickedOutsidePopup -= new EventHandler(this.OverflowPopupWrapperClickedOutsidePopup);
                this.overflowPopupWrapper.CatchClickOutsidePopup = false;
                this.overflowPopupWrapper.PopupOwner = null;
                this.overflowPopupWrapper = null;
            }
            this.overflowPopup = base.GetTemplateChild("PART_Overflow") as Popup;
            if (this.overflowPopup != null)
            {
                this.overflowPopupWrapper = new PopupWrapper(this.overflowPopup, this.DropdownButton);
                this.overflowPopupWrapper.CatchClickOutsidePopup = true;
                this.overflowPopupWrapper.ClickedOutsidePopup += new EventHandler(this.OverflowPopupWrapperClickedOutsidePopup);
                this.overflowPopupWrapper.Placement = Telerik.Windows.Controls.PlacementMode.Bottom;
            }
        }

        private static void InsertInPanel(Panel panel, UIElement container, int position)
        {
            try
            {
                if (panel != null)
                {
                    if ((0 <= position) && (position < panel.Children.Count))
                    {
                        panel.Children.Insert(position, container);
                    }
                    else
                    {
                        panel.Children.Add(container);
                    }
                }
            }
            catch (ArgumentException)
            {
            }
        }

        internal void InsertOverflowItem(object item, int position)
        {
            if ((0 <= position) && (position < this.OverflowItems.Count))
            {
                this.OverflowItems.Insert(position, item);
            }
            else
            {
                this.OverflowItems.Add(item);
            }
        }

        internal void InsertStripItem(object item, int position)
        {
            if ((0 <= position) && (position < this.StripItems.Count))
            {
                this.StripItems.Insert(position, item);
            }
            else
            {
                this.StripItems.Add(item);
            }
        }

        internal void InvalidateHostingTray()
        {
            if (base.Parent != null)
            {
                RadToolBarTray tray = base.Parent as RadToolBarTray;
                if (tray != null)
                {
                    tray.InvalidateMeasure();
                }
            }
        }

        internal static bool IsSpaceLarger(double newSpace, double oldSpace)
        {
            if (double.IsPositiveInfinity(newSpace))
            {
                return !double.IsPositiveInfinity(oldSpace);
            }
            if (double.IsNegativeInfinity(newSpace))
            {
                return false;
            }
            if (double.IsPositiveInfinity(oldSpace))
            {
                return false;
            }
            if (double.IsNegativeInfinity(oldSpace))
            {
                return true;
            }
            if (double.IsNaN(newSpace))
            {
                newSpace = 0.0;
            }
            if (double.IsNaN(oldSpace))
            {
                oldSpace = 0.0;
            }
            return (newSpace > oldSpace);
        }

        internal static bool IsValidOrientation(object value)
        {
            return Enum.IsDefined(typeof(System.Windows.Controls.Orientation), value);
        }

        private static bool IsValidOverflowMode(object value)
        {
            return Enum.IsDefined(typeof(OverflowMode), value);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (this.HasPanels)
            {
                this.ConformToOverflowMode();
                Size testSize = this.GetMeasureTestSize(availableSize);
                Size returnSize = base.MeasureOverride(testSize);
                while (this.CanUnstripItem(availableSize, returnSize))
                {
                    this.TryMoveOverflowItemToStrip();
                    returnSize = base.MeasureOverride(testSize);
                }
                while (this.ShouldStripItem(availableSize, returnSize))
                {
                    this.TryMoveStripItemToOverflow();
                    returnSize = base.MeasureOverride(testSize);
                }
                return returnSize;
            }
            return new Size(0.0, 0.0);
        }

        public override void OnApplyTemplate()
        {
            this.TemplateInitialized = false;
            base.OnApplyTemplate();
            this.ClearPanels();
            this.UnregisterEvents();
            this.GetTemplateChildren();
            this.RepositionItems();
            this.RegisterEvents();
            this.InitializeOverflowPopup();
            this.TemplateInitialized = true;
            this.ChangeVisualState(false);
        }

        private static void OnBandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadToolBar toolBar = d as RadToolBar;
            if (toolBar != null)
            {
                RadToolBarTray tray = toolBar.HostTray;
                if (tray != null)
                {
                    tray.InvalidateMeasure();
                }
            }
        }

        private static void OnBandIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadToolBar toolBar = d as RadToolBar;
            if (toolBar != null)
            {
                RadToolBarTray tray = toolBar.HostTray;
                if (tray != null)
                {
                    tray.InvalidateMeasure();
                }
            }
        }

        private void OnDropdownClick(object sender, RoutedEventArgs e)
        {
            if (this.DropdownButton != null)
            {
                this.IsOverflowOpen = this.DropdownButton.IsChecked == true;
            }
            else
            {
                this.IsOverflowOpen = !this.IsOverflowOpen;
            }
        }

        private static void OnIsOverflowOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadToolBar toolBar = d as RadToolBar;
            if (toolBar != null)
            {
                if ((bool) e.NewValue)
                {
                    if (toolBar.overflowPopupWrapper != null)
                    {
                        toolBar.overflowPopupWrapper.ShowPopup();
                        RadRoutedEventArgs eventArgs = new RadRoutedEventArgs(OverflowAreaOpenedEvent, toolBar);
                        toolBar.RaiseEvent(eventArgs);
                    }
                }
                else if (toolBar.overflowPopupWrapper != null)
                {
                    RadRoutedEventArgs eventArgs = new RadRoutedEventArgs(OverflowAreaClosedEvent, toolBar);
                    toolBar.RaiseEvent(eventArgs);
                    toolBar.overflowPopupWrapper.HidePopup();
                }
                if (toolBar.DropdownButton != null)
                {
                    toolBar.DropdownButton.IsChecked = new bool?(toolBar.IsOverflowOpen);
                }
            }
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            this.RepositionItems();
        }

        private void OnItemsCollectionChanged(Panel panel, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                {
                    int position = e.NewStartingIndex;
                    foreach (object item in e.NewItems)
                    {
                        UIElement container = null;
                        if (this.IsItemItsOwnContainerOverride(item))
                        {
                            container = item as UIElement;
                        }
                        else
                        {
                            container = this.GetContainerForItemOverride() as UIElement;
                        }
                        if (container != null)
                        {
                            this.PrepareContainerForItemOverride(container, item);
                            InsertInPanel(panel, container, position);
                        }
                        position++;
                    }
                    break;
                }
                case NotifyCollectionChangedAction.Remove:
                    foreach (object item in e.OldItems)
                    {
                        UIElement container = base.ItemContainerGenerator.ContainerFromItem(item) as UIElement;
                        if (container == null)
                        {
                            container = item as UIElement;
                        }
                        if ((container != null) && (panel != null))
                        {
                            panel.Children.Remove(container);
                        }
                        this.ClearContainerForItemOverride(container, item);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    throw new NotImplementedException();

                case (NotifyCollectionChangedAction.Replace | NotifyCollectionChangedAction.Remove):
                    break;

                case NotifyCollectionChangedAction.Reset:
                    if (panel != null)
                    {
                        panel.Children.Clear();
                    }
                    break;

                default:
                    return;
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.Escape) && this.IsOverflowOpen)
            {
                this.IsOverflowOpen = false;
                e.Handled = true;
            }
        }

        internal static void OnOrientationChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!IsValidOrientation(e.NewValue))
            {
                throw new ArgumentException("Invalid Orientation Value", "e");
            }
            RadToolBar toolbar = sender as RadToolBar;
            if (toolbar != null)
            {
                toolbar.ChangeVisualState(true);
            }
        }

        internal virtual void OnOverflowItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.OnItemsCollectionChanged(this.OverflowPanel, e);
            this.HasOverflowItems = this.OverflowItems.Count > 0;
        }

        internal static void OnOverflowModeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!IsValidOverflowMode(e.NewValue))
            {
                throw new ArgumentException("Invalid OverflowMode Value", "e");
            }
            FrameworkElement element = sender as FrameworkElement;
            if (element != null)
            {
                RadToolBar toolbar = element.Parent as RadToolBar;
                if (toolbar != null)
                {
                    toolbar.InvalidateMeasure();
                }
            }
        }

        internal virtual void OnStripItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.OnItemsCollectionChanged(this.StripPanel, e);
        }

        private void OverflowPopupWrapperClickedOutsidePopup(object sender, EventArgs e)
        {
            this.IsOverflowOpen = false;
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            object currentStyle = element.GetValue(FrameworkElement.StyleProperty);
            if ((currentStyle == null) || (element.GetValue(ContainerStyleSelectedByPanelbarProperty) == currentStyle))
            {
                Style elementStyle = this.GetContainerStyle(item, element);
                if (elementStyle != null)
                {
                    element.SetValue(ContainerStyleSelectedByPanelbarProperty, elementStyle);
                    element.SetValue(FrameworkElement.StyleProperty, elementStyle);
                }
            }
        }

        private void RegisterEvents()
        {
            if (this.DropdownButton != null)
            {
                this.DropdownButton.Click += new RoutedEventHandler(this.OnDropdownClick);
            }
        }

        internal void RepositionItems()
        {
            if (this.HasPanels)
            {
                this.StripItems.Clear();
                this.OverflowItems.Clear();
                for (int i = 0; i < base.Items.Count; i++)
                {
                    object item = base.Items[i];
                    if (DetermineOverflowMode(item) != OverflowMode.Always)
                    {
                        this.StripItems.Add(item);
                    }
                    else
                    {
                        this.OverflowItems.Add(item);
                    }
                }
                this.OverflowPanel.InvalidateMeasure();
                this.StripPanel.InvalidateMeasure();
                base.InvalidateMeasure();
                this.InvalidateHostingTray();
            }
        }

        internal static void SetLength(bool isHorizontal, double length, ref Rect rect)
        {
            if (isHorizontal)
            {
                rect.Width = length;
            }
            else
            {
                rect.Height = length;
            }
        }

        internal static void SetLength(bool isHorizontal, double length, ref Size size)
        {
            if (isHorizontal)
            {
                size.Width = length;
            }
            else
            {
                size.Height = length;
            }
        }

        internal static void SetLength(bool isHorizontal, double length, ref double width, ref double height)
        {
            if (isHorizontal)
            {
                width = length;
            }
            else
            {
                height = length;
            }
        }

        internal static void SetOffset(bool isHorizontal, double offset, ref Rect rect)
        {
            SetOffset(isHorizontal, offset, offset, ref rect);
        }

        internal static void SetOffset(bool isHorizontal, double offsetX, double offsetY, ref Rect rect)
        {
            if (isHorizontal)
            {
                rect.X = offsetX;
            }
            else
            {
                rect.Y = offsetY;
            }
        }

        public static void SetOverflowMode(DependencyObject element, OverflowMode mode)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            element.SetValue(OverflowModeProperty, mode);
        }

        internal static void SetThickness(bool isHorizontal, double thickness, ref Rect rect)
        {
            if (isHorizontal)
            {
                rect.Height = thickness;
            }
            else
            {
                rect.Width = thickness;
            }
        }

        internal static void SetThickness(bool isHorizontal, double thickness, ref Size size)
        {
            if (isHorizontal)
            {
                size.Width = thickness;
            }
            else
            {
                size.Height = thickness;
            }
        }

        internal static void SetThickness(bool isHorizontal, double thickness, ref double width, ref double height)
        {
            if (isHorizontal)
            {
                height = thickness;
            }
            else
            {
                width = thickness;
            }
        }

        internal bool ShouldStripItem(Size availableSize, Size returnedSize)
        {
            if (!this.HasItemForOverflow())
            {
                return false;
            }
            double availableLen = this.GetLength(availableSize);
            double returnedLen = this.GetLength(returnedSize);
            return !IsSpaceLarger(availableLen, returnedLen);
        }

        internal void TryMoveOverflowItemToStrip()
        {
            this.TryMoveOverflowItemToStrip(this.FindOverflowIndexForStrip());
        }

        internal void TryMoveOverflowItemToStrip(int index)
        {
            if ((0 <= index) && (index < this.OverflowItems.Count))
            {
                object item = this.OverflowItems[index];
                this.OverflowItems.Remove(item);
                this.InsertStripItem(item, this.FindStripInsertIndex(item));
            }
        }

        internal void TryMoveStripItemToOverflow()
        {
            this.TryMoveStripItemToOverflow(this.FindStripIndexForOverflow());
        }

        internal void TryMoveStripItemToOverflow(int index)
        {
            if ((0 <= index) && (index < this.StripItems.Count))
            {
                object item = this.StripItems[index];
                this.StripItems.Remove(item);
                this.InsertOverflowItem(item, this.FindOverflowInsertIndex(item));
            }
        }

        private void UnregisterEvents()
        {
            if (this.DropdownButton != null)
            {
                this.DropdownButton.Click -= new RoutedEventHandler(this.OnDropdownClick);
            }
        }

        public int Band
        {
            get
            {
                return (int) base.GetValue(BandProperty);
            }
            set
            {
                base.SetValue(BandProperty, value);
            }
        }

        public int BandIndex
        {
            get
            {
                return (int) base.GetValue(BandIndexProperty);
            }
            set
            {
                base.SetValue(BandIndexProperty, value);
            }
        }

        internal ToggleButton DropdownButton { get; private set; }

        public Visibility GripVisibility
        {
            get
            {
                return (Visibility) base.GetValue(GripVisibilityProperty);
            }
            set
            {
                base.SetValue(GripVisibilityProperty, value);
            }
        }

        public bool HasOverflowItems
        {
            get
            {
                return (bool) base.GetValue(HasOverflowItemsProperty);
            }
            private set
            {
                this.SetValue(HasOverflowItemsPropertyKey, value);
            }
        }

        internal bool HasOverflowPanel
        {
            get
            {
                return (this.OverflowPanel != null);
            }
        }

        internal bool HasPanels
        {
            get
            {
                return (this.HasStripPanel && this.HasOverflowPanel);
            }
        }

        internal bool HasStripPanel
        {
            get
            {
                return (this.StripPanel != null);
            }
        }

        internal RadToolBarTray HostTray { get; set; }

        [DefaultValue(false)]
        public bool IsOverflowOpen
        {
            get
            {
                return (bool) base.GetValue(IsOverflowOpenProperty);
            }
            set
            {
                base.SetValue(IsOverflowOpenProperty, value);
            }
        }

        internal double MinLength
        {
            get
            {
                return this.CalcMinLength();
            }
        }

        internal double MinThickness
        {
            get
            {
                if (this.Orientation == System.Windows.Controls.Orientation.Horizontal)
                {
                    return base.MinHeight;
                }
                return base.MinWidth;
            }
        }

        [DefaultValue(1)]
        public System.Windows.Controls.Orientation Orientation
        {
            get
            {
                if (this.HostTray == null)
                {
                    return (System.Windows.Controls.Orientation) base.GetValue(OrientationProperty);
                }
                return this.HostTray.Orientation;
            }
            set
            {
                base.SetValue(OrientationProperty, value);
            }
        }

        public Visibility OverflowButtonVisibility
        {
            get
            {
                return (Visibility) base.GetValue(OverflowButtonVisibilityProperty);
            }
            set
            {
                base.SetValue(OverflowButtonVisibilityProperty, value);
            }
        }

        internal IList OverflowItems
        {
            get
            {
                return (IList) base.GetValue(OverflowItemsProperty);
            }
            private set
            {
                base.SetValue(OverflowItemsProperty, value);
            }
        }

        internal Panel OverflowPanel { get; private set; }

        internal IList StripItems
        {
            get
            {
                return (IList) base.GetValue(StripItemsProperty);
            }
            private set
            {
                base.SetValue(StripItemsProperty, value);
            }
        }

        internal Panel StripPanel { get; private set; }

        internal bool TemplateInitialized { get; private set; }
    }
}

