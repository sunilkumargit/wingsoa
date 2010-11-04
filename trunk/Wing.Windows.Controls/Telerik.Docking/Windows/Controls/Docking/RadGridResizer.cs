namespace Telerik.Windows.Controls.Docking
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using System.Windows.Media;
    using Telerik.Windows;
    using Telerik.Windows.Controls;

    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId="Resizer", Justification="The name is correct."), TemplateVisualState(Name="Focused", GroupName="FocusStates"), TemplateVisualState(Name="Disabled", GroupName="CommonStates"), TemplatePart(Name="HorizontalTemplate", Type=typeof(FrameworkElement)), TemplatePart(Name="VerticalTemplate", Type=typeof(FrameworkElement)), TemplateVisualState(Name="Normal", GroupName="CommonStates"), TemplateVisualState(Name="MouseOver", GroupName="CommonStates"), TemplateVisualState(Name="Unfocused", GroupName="FocusStates")]
    public class RadGridResizer : Control
    {
        private DragValidator dragValidator;
        private FrameworkElement elementHorizontalTemplateFrameworkElement;
        internal const string ElementHorizontalTemplateName = "HorizontalTemplate";
        private FrameworkElement elementVerticalTemplateFrameworkElement;
        internal const string ElementVerticalTemplateName = "VerticalTemplate";
        private const double KeyboardIncrement = 10.0;
        internal static readonly Telerik.Windows.RoutedEvent LayoutChangeEndedEvent = EventManager.RegisterRoutedEvent("LayoutChangeEnded", RoutingStrategy.Bubble, typeof(EventHandler<RadRoutedEventArgs>), typeof(RadGridResizer));
        internal static readonly Telerik.Windows.RoutedEvent LayoutChangeStartedEvent = EventManager.RegisterRoutedEvent("LayoutChangeStarted", RoutingStrategy.Bubble, typeof(EventHandler<RadRoutedEventArgs>), typeof(RadGridResizer));
        public static readonly DependencyProperty PlacementProperty = DependencyProperty.Register("Placement", typeof(Dock), typeof(RadGridResizer), new Telerik.Windows.PropertyMetadata(Dock.Left, new PropertyChangedCallback(RadGridResizer.OnPlacementChanged)));
        private Telerik.Windows.Controls.Docking.PreviewControl previewControl;
        private Panel previewLayer;
        public static readonly Telerik.Windows.RoutedEvent PreviewResizeStartEvent = EventManager.RegisterRoutedEvent("PreviewResizeStart", RoutingStrategy.Tunnel, typeof(EventHandler<ResizeEventArgs>), typeof(RadGridResizer));
        public static readonly DependencyProperty PreviewStyleProperty = DependencyProperty.Register("PreviewStyle", typeof(Style), typeof(RadGridResizer), null);
        private ResizeData resizeData;
        public static readonly DependencyProperty ShowsPreviewProperty = DependencyProperty.Register("ShowsPreview", typeof(bool), typeof(RadGridResizer), null);

        public event EventHandler<ResizeEventArgs> PreviewResizeStart
        {
            add
            {
                this.AddHandler(PreviewResizeStartEvent, value);
            }
            remove
            {
                this.RemoveHandler(PreviewResizeStartEvent, value);
            }
        }

        public RadGridResizer()
        {
            base.DefaultStyleKey = typeof(RadGridResizer);
            base.IsEnabledChanged += new DependencyPropertyChangedEventHandler(this.OnIsEnabledChanged);
            this.dragValidator = new DragValidator(this);
            this.dragValidator.DragStartedEvent += new EventHandler<DragStartedEventArgs>(this.DragValidator_DragStartedEvent);
            this.dragValidator.DragDeltaEvent += new EventHandler<DragDeltaEventArgs>(this.DragValidator_DragDeltaEvent);
            this.dragValidator.DragCompletedEvent += new EventHandler<DragCompletedEventArgs>(this.DragValidator_DragCompletedEvent);
        }

        private void CancelResize()
        {
            if (this.resizeData.ShowsPreview)
            {
                this.RemovePreviewControl();
            }
            this.resizeData.CancelResize();
            this.resizeData = null;
        }

        private void ChangeVisualState()
        {
            this.ChangeVisualState(true);
        }

        private void ChangeVisualState(bool useTransitions)
        {
            if (!base.IsEnabled)
            {
                this.GoToState(useTransitions, new string[] { "Disabled", "Normal" });
            }
            else if (this.IsMouseOver)
            {
                this.GoToState(useTransitions, new string[] { "MouseOver", "Normal" });
            }
            else
            {
                this.GoToState(useTransitions, new string[] { "Normal" });
            }
            if (this.HasKeyboardFocus && base.IsEnabled)
            {
                this.GoToState(useTransitions, new string[] { "Focused", "Unfocused" });
            }
            else
            {
                this.GoToState(useTransitions, new string[] { "Unfocused" });
            }
            if ((this.Placement == Dock.Left) || (this.Placement == Dock.Right))
            {
                base.Cursor = Cursors.SizeWE;
            }
            else
            {
                base.Cursor = Cursors.SizeNS;
            }
        }

        private void CreatePreviewLayer()
        {
            this.previewLayer = new Canvas();
            Panel parent = base.Parent as Panel;
            Grid parentGrid = parent as Grid;
            if (parentGrid != null)
            {
                if (parentGrid.RowDefinitions.Count > 0)
                {
                    this.previewLayer.SetValue(Grid.RowSpanProperty, parentGrid.RowDefinitions.Count);
                }
                if (parentGrid.ColumnDefinitions.Count > 0)
                {
                    this.previewLayer.SetValue(Grid.ColumnSpanProperty, parentGrid.ColumnDefinitions.Count);
                }
            }
            if (parent != null)
            {
                parent.Children.Add(this.previewLayer);
            }
        }

        internal void DragValidator_DragCompletedEvent(object sender, DragCompletedEventArgs e)
        {
            if (this.resizeData != null)
            {
                if (e.Canceled)
                {
                    this.CancelResize();
                }
                else if (this.resizeData.ShowsPreview)
                {
                    Canvas.SetZIndex(this.resizeData.ResizedElement, 0);
                    this.MoveSplitter(this.resizeData.PreviewControl.OffsetX, this.resizeData.PreviewControl.OffsetY, false);
                    this.RemovePreviewControl();
                    this.resizeData.ClearReferences();
                    this.resizeData = null;
                }
            }
            this.ChangeVisualState();
            this.OnLayoutChangeEnded();
        }

        internal void DragValidator_DragDeltaEvent(object sender, DragDeltaEventArgs e)
        {
            if (this.resizeData != null)
            {
                this.MoveSplitter(e.HorizontalChange, e.VerticalChange, this.resizeData.ShowsPreview);
            }
        }

        internal void DragValidator_DragStartedEvent(object sender, DragStartedEventArgs e)
        {
            this.OnLayoutChangeStarted();
            if (base.IsEnabled)
            {
                base.Focus();
                this.InitializeData();
                this.SetupPreview();
            }
        }

        private double GetChange(double horizontalChange, double verticalChange)
        {
            switch (this.Placement)
            {
                case Dock.Left:
                case Dock.Right:
                    return horizontalChange;

                case Dock.Top:
                case Dock.Bottom:
                    return verticalChange;
            }
            return 0.0;
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

        internal bool InitializeAndMoveSplitter(double horizontalChange, double verticalChange)
        {
            if (this.resizeData != null)
            {
                return false;
            }
            this.InitializeData();
            if (this.resizeData == null)
            {
                return false;
            }
            this.MoveSplitter(horizontalChange, verticalChange, false);
            this.resizeData = null;
            return true;
        }

        private void InitializeData()
        {
            ResizeEventArgs args = new ResizeEventArgs(PreviewResizeStartEvent, this);
            this.RaiseEvent(args);
            Panel parent = base.Parent as Panel;
            if (parent != null)
            {
                this.resizeData = new ResizeData();
                this.resizeData.ResizedElement = args.ResizedElement ?? parent;
                this.resizeData.ShowsPreview = args.ShowsPreview;
                this.resizeData.ResizePlacement = this.Placement;
                this.resizeData.MaxSize = args.AvailableSize;
                this.resizeData.MinSize = args.MinSize;
                if (args.AffectedElement != null)
                {
                    this.resizeData.AffectedElement = args.AffectedElement;
                    this.resizeData.ResizeBehavior = ResizeBehavior.Split;
                }
                this.resizeData.InitializeDefaults();
            }
        }

        private bool KeyboardMoveSplitter(double horizontalChange, double verticalChange)
        {
            return ((this.HasKeyboardFocus && base.IsEnabled) && this.InitializeAndMoveSplitter(horizontalChange, verticalChange));
        }

        private void MoveSplitter(double horizontalChange, double verticalChange, bool showPreview)
        {
            double change = this.GetChange(horizontalChange, verticalChange);
            if ((this.resizeData != null) && (change != 0.0))
            {
                this.resizeData.ResizeControl(change, showPreview);
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.elementHorizontalTemplateFrameworkElement = base.GetTemplateChild("HorizontalTemplate") as FrameworkElement;
            this.elementVerticalTemplateFrameworkElement = base.GetTemplateChild("VerticalTemplate") as FrameworkElement;
            this.UpdateTemplateOrientation();
            this.ChangeVisualState(false);
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            this.ChangeVisualState();
        }

        [SuppressMessage("Microsoft.Design", "CA1061:DoNotHideBaseClassMethods")]
        private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!((bool) e.NewValue))
            {
                this.IsMouseOver = false;
            }
            this.ChangeVisualState();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            switch (e.Key)
            {
                case Key.Left:
                    e.Handled = this.KeyboardMoveSplitter(-10.0, 0.0);
                    return;

                case Key.Up:
                    e.Handled = this.KeyboardMoveSplitter(0.0, -10.0);
                    return;

                case Key.Right:
                    e.Handled = this.KeyboardMoveSplitter(10.0, 0.0);
                    return;

                case Key.Down:
                    e.Handled = this.KeyboardMoveSplitter(0.0, 10.0);
                    return;

                case Key.Escape:
                    if (this.resizeData != null)
                    {
                        this.CancelResize();
                        e.Handled = true;
                    }
                    return;
            }
        }

        private void OnLayoutChangeEnded()
        {
            this.RaiseEvent(new RadRoutedEventArgs(LayoutChangeEndedEvent, this));
        }

        private void OnLayoutChangeStarted()
        {
            this.RaiseEvent(new RadRoutedEventArgs(LayoutChangeStartedEvent, this));
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            this.ChangeVisualState();
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            this.IsMouseOver = true;
            this.ChangeVisualState();
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            this.IsMouseOver = false;
            if (this.resizeData == null)
            {
                this.ChangeVisualState();
            }
        }

        private static void OnPlacementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadGridResizer resizer = d as RadGridResizer;
            if (resizer != null)
            {
                resizer.UpdateTemplateOrientation();
            }
        }

        private void RemovePreviewControl()
        {
            if (this.resizeData.PreviewControl != null)
            {
                this.resizeData.PreviewControl.Visibility = Visibility.Collapsed;
            }
        }

        private void SetupPreview()
        {
            if (this.resizeData.ShowsPreview)
            {
                this.resizeData.PreviewControl = this.PreviewControl;
                this.resizeData.PreviewControl.Bind(this);
                this.resizeData.PreviewControl.Visibility = Visibility.Visible;
            }
        }

        private void UpdateTemplateOrientation()
        {
            if ((this.Placement == Dock.Left) || (this.Placement == Dock.Right))
            {
                if (this.elementHorizontalTemplateFrameworkElement != null)
                {
                    this.elementHorizontalTemplateFrameworkElement.Visibility = Visibility.Collapsed;
                }
                if (this.elementVerticalTemplateFrameworkElement != null)
                {
                    this.elementVerticalTemplateFrameworkElement.Visibility = Visibility.Visible;
                }
            }
            else
            {
                if (this.elementHorizontalTemplateFrameworkElement != null)
                {
                    this.elementHorizontalTemplateFrameworkElement.Visibility = Visibility.Visible;
                }
                if (this.elementVerticalTemplateFrameworkElement != null)
                {
                    this.elementVerticalTemplateFrameworkElement.Visibility = Visibility.Collapsed;
                }
            }
        }

        internal ResizeData Data
        {
            get
            {
                return this.resizeData;
            }
        }

        private bool HasKeyboardFocus
        {
            get
            {
                return (FocusManager.GetFocusedElement() == this);
            }
        }

        private bool IsMouseOver { get; set; }

        public Dock Placement
        {
            get
            {
                return (Dock) base.GetValue(PlacementProperty);
            }
            set
            {
                base.SetValue(PlacementProperty, value);
            }
        }

        private Telerik.Windows.Controls.Docking.PreviewControl PreviewControl
        {
            get
            {
                if (this.previewControl == null)
                {
                    this.previewControl = new Telerik.Windows.Controls.Docking.PreviewControl();
                    if (this.previewLayer == null)
                    {
                        this.CreatePreviewLayer();
                    }
                    this.previewLayer.Children.Add(this.previewControl);
                }
                return this.previewControl;
            }
        }

        public Style PreviewStyle
        {
            get
            {
                return (Style) base.GetValue(PreviewStyleProperty);
            }
            set
            {
                base.SetValue(PreviewStyleProperty, value);
            }
        }

        public bool ShowsPreview
        {
            get
            {
                return (bool) base.GetValue(ShowsPreviewProperty);
            }
            set
            {
                base.SetValue(ShowsPreviewProperty, value);
            }
        }

        internal enum ResizeBehavior
        {
            Resize,
            Split
        }

        internal class ResizeData
        {
            internal void CancelResize()
            {
                if (this.ResizeBehavior == Telerik.Windows.Controls.Docking.RadGridResizer.ResizeBehavior.Resize)
                {
                    if (this.ResizerPlacementHorizontal)
                    {
                        this.ResizedElement.Width = this.ResizedElementOriginalLength;
                    }
                    else
                    {
                        this.ResizedElement.Height = this.ResizedElementOriginalLength;
                    }
                }
                else
                {
                    ProportionalStackPanel.SetSplitterChange(this.ResizedElement, this.ResizedElementOriginalChange);
                    ProportionalStackPanel.SetSplitterChange(this.AffectedElement, this.AffectedElementOriginalChange);
                }
                this.InvalidatePanelMeasure();
                this.ClearReferences();
            }

            internal void ClearReferences()
            {
                Canvas.SetZIndex(this.ResizedElement, this.ResizedElementOriginalZIndex);
                this.PreviewControl = null;
                this.ResizedElement = null;
                this.AffectedElement = null;
            }

            private double[] GetDeltaConstraints()
            {
                double minSize = 0.0;
                double maxSize = 0.0;
                if (this.ResizeBehavior == Telerik.Windows.Controls.Docking.RadGridResizer.ResizeBehavior.Split)
                {
                    switch (this.ResizePlacement)
                    {
                        case Dock.Left:
                            minSize = this.AffectedElement.MinWidth - this.AffectedElementOriginalLength;
                            maxSize = this.ResizedElementOriginalLength - this.ResizedElement.MinWidth;
                            goto Label_0227;

                        case Dock.Top:
                            minSize = this.AffectedElement.MinHeight - this.AffectedElementOriginalLength;
                            maxSize = this.ResizedElementOriginalLength - this.ResizedElement.MinHeight;
                            goto Label_0227;

                        case Dock.Right:
                            minSize = this.ResizedElementOriginalLength - this.ResizedElement.MinWidth;
                            maxSize = this.AffectedElementOriginalLength - this.AffectedElement.MinWidth;
                            goto Label_0227;

                        case Dock.Bottom:
                            minSize = this.ResizedElementOriginalLength - this.ResizedElement.MinHeight;
                            maxSize = this.AffectedElementOriginalLength - this.AffectedElement.MinHeight;
                            goto Label_0227;
                    }
                }
                else
                {
                    Size maximum = this.MaxSize.IsEmpty ? ApplicationHelper.ApplicationSize : this.MaxSize;
                    switch (this.ResizePlacement)
                    {
                        case Dock.Left:
                            minSize = this.MinSize.Width - this.MaxSize.Width;
                            maxSize = this.ResizedElementOriginalLength - this.ResizedElement.MinWidth;
                            goto Label_0227;

                        case Dock.Top:
                            minSize = this.MinSize.Height - this.MaxSize.Height;
                            maxSize = this.ResizedElementOriginalLength - this.ResizedElement.MinHeight;
                            goto Label_0227;

                        case Dock.Right:
                            minSize = this.ResizedElement.MinWidth - this.ResizedElementOriginalLength;
                            maxSize = Math.Min((double) (maximum.Width - this.MinSize.Width), (double) (this.ResizedElement.MaxWidth - this.ResizedElementOriginalLength));
                            goto Label_0227;

                        case Dock.Bottom:
                            minSize = this.ResizedElement.MinHeight - this.ResizedElementOriginalLength;
                            maxSize = Math.Min((double) (maximum.Height - this.MinSize.Height), (double) (this.ResizedElement.MaxHeight - this.ResizedElementOriginalLength));
                            goto Label_0227;
                    }
                }
            Label_0227:;
                return new double[] { minSize, maxSize };
            }

            internal void InitializeDefaults()
            {
                this.ResizedElementOriginalZIndex = Canvas.GetZIndex(this.ResizedElement);
                Canvas.SetZIndex(this.ResizedElement, 0xf423f);
                if (this.ResizeBehavior == Telerik.Windows.Controls.Docking.RadGridResizer.ResizeBehavior.Resize)
                {
                    this.ResizedElementOriginalSize = new Size(this.ResizedElement.ActualWidth, this.ResizedElement.ActualHeight);
                }
                else
                {
                    this.ResizedElementOriginalChange = ProportionalStackPanel.GetSplitterChange(this.ResizedElement);
                    this.AffectedElementOriginalChange = ProportionalStackPanel.GetSplitterChange(this.AffectedElement);
                    this.ResizedElementOriginalSize = this.ResizedElement.RenderSize;
                    this.AffectedElementOriginalSize = this.AffectedElement.RenderSize;
                }
            }

            private void InvalidatePanelMeasure()
            {
                if (this.Panel != null)
                {
                    this.Panel.InvalidateMeasure();
                }
            }

            public void ResizeControl(double change, bool showsPreview)
            {
                double[] deltaConstraints = this.GetDeltaConstraints();
                double minSize = deltaConstraints[0];
                double maxSize = deltaConstraints[1];
                change = Math.Min(Math.Max(change, minSize), maxSize);
                if (showsPreview)
                {
                    if (this.ResizerPlacementHorizontal)
                    {
                        this.PreviewControl.OffsetX = change;
                    }
                    else
                    {
                        this.PreviewControl.OffsetY = change;
                    }
                }
                else
                {
                    switch (this.ResizePlacement)
                    {
                        case Dock.Left:
                        case Dock.Top:
                            change = -change;
                            break;
                    }
                    if (this.ResizeBehavior == Telerik.Windows.Controls.Docking.RadGridResizer.ResizeBehavior.Split)
                    {
                        this.SplitItems(change);
                    }
                    else if (this.ResizerPlacementHorizontal)
                    {
                        this.ResizedElement.Width = this.ResizedElementOriginalLength + change;
                    }
                    else
                    {
                        this.ResizedElement.Height = this.ResizedElementOriginalLength + change;
                    }
                    this.InvalidatePanelMeasure();
                }
            }

            private void SplitItems(double change)
            {
                ProportionalStackPanel.SetSplitterChange(this.ResizedElement, this.ResizedElementOriginalLength + change);
                ProportionalStackPanel.SetSplitterChange(this.AffectedElement, this.AffectedElementOriginalLength - change);
            }

            public FrameworkElement AffectedElement { get; set; }

            private double AffectedElementOriginalChange { get; set; }

            private double AffectedElementOriginalLength
            {
                get
                {
                    if (this.ResizerPlacementHorizontal)
                    {
                        return this.AffectedElementOriginalSize.Width;
                    }
                    return this.AffectedElementOriginalSize.Height;
                }
            }

            private Size AffectedElementOriginalSize { get; set; }

            public Size MaxSize { get; set; }

            public Size MinSize { get; set; }

            private System.Windows.Controls.Panel Panel
            {
                get
                {
                    if (this.ResizedElement == null)
                    {
                        return null;
                    }
                    return (VisualTreeHelper.GetParent(this.ResizedElement) as System.Windows.Controls.Panel);
                }
            }

            public Telerik.Windows.Controls.Docking.PreviewControl PreviewControl { get; set; }

            internal Telerik.Windows.Controls.Docking.RadGridResizer.ResizeBehavior ResizeBehavior { get; set; }

            public FrameworkElement ResizedElement { get; set; }

            private double ResizedElementOriginalChange { get; set; }

            private double ResizedElementOriginalLength
            {
                get
                {
                    if (this.ResizerPlacementHorizontal)
                    {
                        return this.ResizedElementOriginalSize.Width;
                    }
                    return this.ResizedElementOriginalSize.Height;
                }
            }

            private Size ResizedElementOriginalSize { get; set; }

            private int ResizedElementOriginalZIndex { get; set; }

            public Dock ResizePlacement { get; set; }

            private bool ResizerPlacementHorizontal
            {
                get
                {
                    if (this.ResizePlacement != Dock.Left)
                    {
                        return (this.ResizePlacement == Dock.Right);
                    }
                    return true;
                }
            }

            public bool ShowsPreview { get; set; }
        }
    }
}

