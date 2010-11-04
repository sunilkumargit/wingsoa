namespace Telerik.Windows.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Windows;
    using System.Windows.Automation.Peers;
    using System.Windows.Automation.Provider;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Media;
    using Telerik.Windows;
    using Telerik.Windows.Controls.Animation;
    using Telerik.Windows.Controls.DragDrop;
    using Telerik.Windows.Controls.RadWindowPopup;
    using Telerik.Windows.Input;

    [TemplatePart(Name = "PART_RestoreButton", Type = typeof(Button)), TemplateVisualState(Name = "Dragging", GroupName = "DragStates"), TemplatePart(Name = "PART_MaximizeButton", Type = typeof(Button)), TemplatePart(Name = "W", Type = typeof(Thumb)), TemplatePart(Name = "NW", Type = typeof(Thumb)), TemplatePart(Name = "N", Type = typeof(Thumb)), TemplatePart(Name = "NE", Type = typeof(Thumb)), TemplatePart(Name = "E", Type = typeof(Thumb)), TemplatePart(Name = "SE", Type = typeof(Thumb)), TemplatePart(Name = "S", Type = typeof(Thumb)), TemplatePart(Name = "SW", Type = typeof(Thumb)), TemplatePart(Name = "titleThumb", Type = typeof(Thumb)), TemplatePart(Name = "PART_MinimizeButton", Type = typeof(Button)), TemplateVisualState(Name = "Maximized", GroupName = "WindowStates"), TemplatePart(Name = "PART_CloseButton", Type = typeof(Button)), TemplatePart(Name = "PART_HeaderButtonsBorder", Type = typeof(Border)), TemplateVisualState(Name = "NormalWindow", GroupName = "WindowStates"), TemplateVisualState(Name = "Minimized", GroupName = "WindowStates"), TemplateVisualState(Name = "Resizing", GroupName = "DragStates"), TemplateVisualState(Name = "NotDragging", GroupName = "DragStates")]
    public class RadWindow : HeaderedContentControl, INotifyLayoutChange, IWindow
    {
        private Panel backCanvas;
        public static readonly DependencyProperty BorderBackgroundProperty = DependencyProperty.Register("BorderBackground", typeof(Brush), typeof(RadWindow), null);
        public static readonly DependencyProperty CanCloseProperty = DependencyProperty.Register("CanClose", typeof(bool), typeof(RadWindow), new Telerik.Windows.PropertyMetadata(true, new PropertyChangedCallback(RadWindow.OnCanCloseChanged)));
        public static readonly DependencyProperty CanMoveProperty = DependencyProperty.Register("CanMove", typeof(bool), typeof(RadWindow), new Telerik.Windows.PropertyMetadata(true, new PropertyChangedCallback(RadWindow.OnCanMoveChanged)));
        private FrameworkElement closeButton;
        private FrameworkElement contentElement;
        private const double defaultSizeRatio = 0.75;
        public static readonly DependencyProperty DialogResultProperty = DependencyProperty.Register("DialogResult", typeof(bool?), typeof(RadWindow), null);
        private static readonly TimeSpan doubleClickInterval = TimeSpan.FromMilliseconds(300.0);
        private DragInfo dragInfo;
        private FrameworkElement headerButtonsBorder;
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(object), typeof(RadWindow), null);
        public static readonly DependencyProperty IconTemplateProperty = DependencyProperty.Register("IconTemplate", typeof(DataTemplate), typeof(RadWindow), null);
        public static readonly DependencyProperty IsActiveWindowProperty;
        private static readonly DependencyPropertyKey IsActiveWindowPropertyKey = DependencyPropertyExtensions.RegisterReadOnly("IsActiveWindow", typeof(bool), typeof(RadWindow), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadWindow.OnIsActiveWindowChanged)));
        public static readonly DependencyProperty IsDraggingProperty;
        private static readonly DependencyPropertyKey IsDraggingPropertyKey = DependencyPropertyExtensions.RegisterReadOnly("IsDragging", typeof(bool), typeof(RadWindow), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadWindow.OnIsDraggingChanged)));
        public static readonly DependencyProperty IsHeaderHitTestVisibleProperty = DependencyProperty.Register("IsHeaderHitTestVisible", typeof(bool), typeof(RadWindow), null);
        public static readonly DependencyProperty IsModalProperty;
        private static readonly DependencyPropertyKey IsModalPropertyKey = DependencyPropertyExtensions.RegisterReadOnly("IsModal", typeof(bool), typeof(RadWindow), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadWindow.OnIsModalChanged)));
        private bool isOpen;
        private bool isOpening;
        public static readonly DependencyProperty IsResizingProperty;
        private static readonly DependencyPropertyKey IsResizingPropertyKey = DependencyPropertyExtensions.RegisterReadOnly("IsResizing", typeof(bool), typeof(RadWindow), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadWindow.OnIsResizingChanged)));
        public static readonly DependencyProperty IsRestrictedProperty = DependencyProperty.Register("IsRestricted", typeof(bool), typeof(RadWindow), null);
        private bool isTemplateApplied;
        public static readonly DependencyProperty LeftOffsetProperty = DependencyProperty.Register("LeftOffset", typeof(double), typeof(RadWindow), new Telerik.Windows.PropertyMetadata(0.0));
        public static readonly DependencyProperty LeftProperty = DependencyProperty.Register("Left", typeof(double), typeof(RadWindow), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadWindow.OnLeftChanged)));
        private RadWindowStateManager manager;
        private FrameworkElement maximizeButton;
        private FrameworkElement minimizeButton;
        public static readonly DependencyProperty ModalBackgroundProperty = DependencyProperty.Register("ModalBackground", typeof(Brush), typeof(RadWindow), null);
        private WindowPopup popup;
        private static WindowPopupFactory popupFactory = GetFactory();
        public static readonly DependencyProperty PromptResultProperty = DependencyProperty.Register("PromptResult", typeof(string), typeof(RadWindow), null);
        public static readonly DependencyProperty ResizeModeProperty = DependencyProperty.Register("ResizeMode", typeof(Telerik.Windows.Controls.ResizeMode), typeof(RadWindow), new Telerik.Windows.PropertyMetadata(Telerik.Windows.Controls.ResizeMode.CanResize, new PropertyChangedCallback(RadWindow.OnResizeModeChanged)));
        public static readonly DependencyProperty ResponseButtonProperty = DependencyProperty.RegisterAttached("ResponseButton", typeof(ResponseButton), typeof(RadWindow), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadWindow.OnResponseButtonPropertyChanged)));
        private FrameworkElement restoreButton;
        public static readonly DependencyProperty RestoreMinimizedLocationProperty = DependencyProperty.Register("RestoreMinimizedLocation", typeof(bool), typeof(RadWindow), null);
        public static readonly DependencyProperty RestrictedAreaMarginProperty = DependencyProperty.Register("RestrictedAreaMargin", typeof(Thickness), typeof(RadWindow), new Telerik.Windows.PropertyMetadata(new Thickness(0.0)));
        private bool sizeChangedEventsAttached;
        private static readonly string[] thumbNames = new string[] { "W", "NW", "N", "NE", "E", "SE", "S", "SW", "titleThumb" };
        private List<Thumb> thumbs;
        private DateTime titleLastClick;
        public static readonly DependencyProperty TopOffsetProperty = DependencyProperty.Register("TopOffset", typeof(double), typeof(RadWindow), new Telerik.Windows.PropertyMetadata(0.0));
        public static readonly DependencyProperty TopProperty = DependencyProperty.Register("Top", typeof(double), typeof(RadWindow), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadWindow.OnTopChanged)));
        public static readonly DependencyProperty WindowStartupLocationProperty = DependencyProperty.Register("WindowStartupLocation", typeof(Telerik.Windows.Controls.WindowStartupLocation), typeof(RadWindow), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadWindow.OnWindowStartupLocationChanged)));
        public static readonly DependencyProperty WindowStateProperty = DependencyProperty.Register("WindowState", typeof(System.Windows.WindowState), typeof(RadWindow), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadWindow.OnWindowStateChanged)));

        [Telerik.Windows.Controls.SRCategory("Behavior")]
        public event EventHandler Activated;

        [Telerik.Windows.Controls.SRCategory("Behavior")]
        public event EventHandler<WindowClosedEventArgs> Closed;

        public event EventHandler LayoutChangeEnded;

        public event EventHandler LayoutChangeStarted;

        [Telerik.Windows.Controls.SRCategory("Behavior")]
        public event RoutedEventHandler LocationChanged;

        [Telerik.Windows.Controls.SRCategory("Behavior")]
        public event RoutedEventHandler Opened;

        [Telerik.Windows.Controls.SRCategory("Behavior")]
        public event EventHandler<WindowPreviewClosedEventArgs> PreviewClosed;

        [Telerik.Windows.Controls.SRCategory("Behavior")]
        public event RoutedEventHandler WindowStateChanged;

        static RadWindow()
        {
            CommandManager.RegisterClassCommandBinding(typeof(RadWindow), new CommandBinding(WindowCommands.Close, new ExecutedRoutedEventHandler(RadWindow.OnCloseClick)));
            CommandManager.RegisterClassCommandBinding(typeof(RadWindow), new CommandBinding(WindowCommands.Minimize, new ExecutedRoutedEventHandler(RadWindow.OnMinimizeClick)));
            CommandManager.RegisterClassCommandBinding(typeof(RadWindow), new CommandBinding(WindowCommands.Maximize, new ExecutedRoutedEventHandler(RadWindow.OnMaximizeClick)));
            CommandManager.RegisterClassCommandBinding(typeof(RadWindow), new CommandBinding(WindowCommands.Restore, new ExecutedRoutedEventHandler(RadWindow.OnRestoreClick)));
            IsResizingProperty = IsResizingPropertyKey.DependencyProperty;
            IsDraggingProperty = IsDraggingPropertyKey.DependencyProperty;
            IsActiveWindowProperty = IsActiveWindowPropertyKey.DependencyProperty;
            IsModalProperty = IsModalPropertyKey.DependencyProperty;
            DefaultSizeRatio = 0.75;
        }

        public RadWindow()
        {
            base.DefaultStyleKey = typeof(RadWindow);
            this.manager = new RadWindowStateManager(this);
            this.backCanvas = new Canvas();
            this.backCanvas.SetBinding(Panel.BackgroundProperty, new Binding("ModalBackground") { Source = this, Mode = BindingMode.OneWay });
            if (!this.IsInDesignMode)
            {
                Mouse.AddMouseDownHandler(this.backCanvas, new EventHandler<Telerik.Windows.Input.MouseButtonEventArgs>(this.OnModalBackgroundClick));
                Mouse.AddMouseUpHandler(this.backCanvas, new EventHandler<Telerik.Windows.Input.MouseButtonEventArgs>(this.OnModalBackgroundClick));
                base.Visibility = Visibility.Collapsed;
                base.AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(this.OnMouseButtonDown), true);
            }
            this.thumbs = new List<Thumb>(9);
            
        }

        public static void Alert(object content)
        {
            DialogParameters pars = new DialogParameters
            {
                Content = content
            };
            ConfigureModal(new RadAlert(), pars);
        }

        public static void Alert(DialogParameters dialogParameters)
        {
            ConfigureModal(new RadAlert(), dialogParameters);
        }

        public static void Alert(object content, EventHandler<WindowClosedEventArgs> closed)
        {
            DialogParameters pars = new DialogParameters
            {
                Content = content,
                Closed = closed
            };
            ConfigureModal(new RadAlert(), pars);
        }

        private void AttachSizeChangedEvents()
        {
            if (!this.IsInDesignMode && !this.sizeChangedEventsAttached)
            {
                WeakEventListener<RadWindow, object, EventArgs> weakListener = new WeakEventListener<RadWindow, object, EventArgs>(this)
                {
                    OnEventAction = delegate(RadWindow window, object sender, EventArgs args)
                    {
                        window.OnApplicationSizeChanged(sender, args);
                    },
                    OnDetachAction = delegate(WeakEventListener<RadWindow, object, EventArgs> listener)
                    {
                        ApplicationHelper.RootVisual.SizeChanged -= new SizeChangedEventHandler(listener.OnEvent);
                    }
                };
                ApplicationHelper.RootVisual.SizeChanged += new SizeChangedEventHandler(weakListener.OnEvent);
                this.sizeChangedEventsAttached = true;
            }
        }

        public void BringToFront()
        {
            base.Dispatcher.BeginInvoke(delegate
            {
                Control fe = FocusManager.GetFocusedElement() as Control;
                bool hasFocus = (fe != null) && this.IsAncestorOf(fe);
                RadWindowManager.Current.BringToFront(this);
                this.OnActivated();
                if (hasFocus)
                {
                    fe.Focus();
                }
                else if (IsInBrowser)
                {
                    base.Focus();
                }
            });
        }

        protected internal override void ChangeVisualState(bool useTransitions)
        {
            base.ChangeVisualState(useTransitions);
            if (this.IsResizing)
            {
                VisualStateManager.GoToState(this, "Resizing", useTransitions);
            }
            else if (this.IsDragging)
            {
                VisualStateManager.GoToState(this, "Dragging", useTransitions);
            }
            else
            {
                VisualStateManager.GoToState(this, "NotDragging", useTransitions);
            }
            switch (this.WindowState)
            {
                case System.Windows.WindowState.Minimized:
                    VisualStateManager.GoToState(this, "Minimized", useTransitions);
                    return;

                case System.Windows.WindowState.Maximized:
                    VisualStateManager.GoToState(this, "Maximized", useTransitions);
                    return;
            }
            VisualStateManager.GoToState(this, "NormalWindow", useTransitions);
        }

        private void CheckRestrictedArea()
        {
            if (this.IsRestricted && !this.IsInDesignMode)
            {
                Size appSize = this.Popup.GetRootSize();
                double maxWidth = Math.Max(0.0, Math.Max(base.MinWidth, (appSize.Width - this.RestrictedAreaMargin.Right) - this.RestrictedAreaMargin.Left));
                double maxHeight = Math.Max(0.0, Math.Max(base.MinHeight, (appSize.Height - this.RestrictedAreaMargin.Bottom) - this.RestrictedAreaMargin.Top));
                double actualWidth = 0.0;
                double actualHeight = 0.0;
                if (IsInBrowser && (double.IsNaN(base.Width) || double.IsNaN(base.Height)))
                {
                    base.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                }
                if (!double.IsNaN(base.Width))
                {
                    maxWidth = base.Width;
                    actualWidth = base.Width;
                }
                else
                {
                    actualWidth = base.ActualWidth;
                }
                if (!double.IsNaN(base.Height))
                {
                    maxHeight = base.Height;
                    actualHeight = base.Height;
                }
                else
                {
                    actualHeight = base.ActualHeight;
                }
                Rect restrictionRect = new Rect(this.RestrictedAreaMargin.Left, this.RestrictedAreaMargin.Top, maxWidth, maxHeight);
                Rect rect = this.CoerceLocation(this.Left, this.Top, actualWidth, actualHeight, restrictionRect);
                this.Left = rect.Left;
                this.Top = rect.Top;
                if (!double.IsNaN(base.Width))
                {
                    base.Width = rect.Width;
                }
                if (!double.IsNaN(base.Height))
                {
                    base.Height = rect.Height;
                }
            }
        }

        public void Close()
        {
            if (this.IsOpen && this.OnPreviewClosed())
            {
                foreach (RadWindow win in (from w in RadWindowManager.Current.Windows
                                           where w.Owner == this
                                           select w).ToList<RadWindow>())
                {
                    win.Close();
                }
                this.IsOpen = false;
                Action closeCallback = delegate
                {
                    this.popup.Close();
                    RadWindowManager.Current.RemoveWindow(this);
                    this.OnClosed();
                };
                this.StopAllAnimations();
                AnimationManager.Play(this, "Hide", closeCallback, new object[0]);
            }
            else if (this.isOpening)
            {
                this.isOpening = false;
                this.IsOpen = false;
                this.popup.Close();
            }
        }

        internal Rect CoerceLocation(double x, double y, double width, double height, Rect initial)
        {
            double left = x;
            double right = x + width;
            double top = y;
            double bottom = y + height;
            if ((width < base.MinWidth) || (width > base.MaxWidth))
            {
                double v = (width < base.MinWidth) ? base.MinWidth : base.MaxWidth;
                if (left == initial.Left)
                {
                    right = left + v;
                }
                else
                {
                    left = right - v;
                }
            }
            if ((height < base.MinHeight) || (height > base.MaxHeight))
            {
                double v = (height < base.MinHeight) ? base.MinHeight : base.MaxHeight;
                if (top == initial.Top)
                {
                    bottom = top + v;
                }
                else
                {
                    top = bottom - v;
                }
            }
            if (this.IsRestricted)
            {
                Size appSize = this.Popup.GetRootSize();
                double restrictLeft = this.RestrictedAreaMargin.Left;
                double restrictTop = this.RestrictedAreaMargin.Top;
                double restrictRight = appSize.Width - this.RestrictedAreaMargin.Right;
                double restrictBottom = appSize.Height - this.RestrictedAreaMargin.Bottom;
                if (right > restrictRight)
                {
                    if (left != initial.Left)
                    {
                        left -= right - restrictRight;
                    }
                    right = restrictRight;
                }
                if (left < restrictLeft)
                {
                    if (right != initial.Right)
                    {
                        right += restrictLeft - left;
                    }
                    left = restrictLeft;
                }
                if (bottom > restrictBottom)
                {
                    if (top != initial.Top)
                    {
                        top -= bottom - restrictBottom;
                    }
                    bottom = restrictBottom;
                }
                if (top < restrictTop)
                {
                    if (bottom != initial.Bottom)
                    {
                        bottom += restrictTop - top;
                    }
                    top = restrictTop;
                }
            }
            return new Rect(left, top, Math.Max((double)0.0, (double)(right - left)), Math.Max((double)0.0, (double)(bottom - top)));
        }

        internal static void ConfigureModal(RadAlert content, DialogParameters dialogParams)
        {
            RadWindow window = new RadWindow();
            if (dialogParams.Header != null)
            {
                window.Header = dialogParams.Header;
            }
            else
            {
                window.Header = string.Empty;
            }
            content.Content = dialogParams.Content;
            window.Content = content;
            window.Icon = dialogParams.IconContent;
            window.ResizeMode = Telerik.Windows.Controls.ResizeMode.NoResize;
            window.CanMove = true;
            window.WindowStartupLocation = Telerik.Windows.Controls.WindowStartupLocation.CenterScreen;
            window.IsActiveWindow = true;
            if (dialogParams.ModalBackground != null)
            {
                window.ModalBackground = dialogParams.ModalBackground;
            }
            if (dialogParams.Theme != null)
            {
                StyleManager.SetTheme(content, dialogParams.Theme);
                StyleManager.SetTheme(window, dialogParams.Theme);
            }
            if (dialogParams.WindowStyle != null)
            {
                window.Style = dialogParams.WindowStyle;
            }
            if (dialogParams.ContentStyle != null)
            {
                content.Style = dialogParams.ContentStyle;
            }
            if (dialogParams.Owner != null)
            {
                window.Owner = dialogParams.Owner;
            }
            content.Configure(window, dialogParams);
            RoutedEventHandler openedHandler = null;
            if (dialogParams.Opened != null)
            {
                openedHandler = null;
                openedHandler = delegate(object s, RoutedEventArgs e)
                {
                    dialogParams.Opened(s, e);
                    window.Opened -= openedHandler;
                };
                window.Opened += openedHandler;
            }
            EventHandler<WindowClosedEventArgs> helpHandler = null;
            if (dialogParams.Closed != null)
            {
                helpHandler = null;
                helpHandler = delegate(object s, WindowClosedEventArgs args)
                {
                    dialogParams.Closed(s, args);
                    window.Closed -= helpHandler;
                };
                window.Closed += helpHandler;
            }
            window.ShowDialog();
        }

        public static void Confirm(DialogParameters dialogParameters)
        {
            ConfigureModal(new RadConfirm(), dialogParameters);
        }

        public static void Confirm(object content, EventHandler<WindowClosedEventArgs> closed)
        {
            DialogParameters pars = new DialogParameters
            {
                Content = content,
                Closed = closed
            };
            ConfigureModal(new RadConfirm(), pars);
        }

        private void DoDrag(double horizontalChange, double verticalChange)
        {
            if (this.dragInfo != null)
            {
                this.dragInfo.DoDrag(horizontalChange, verticalChange, this);
            }
        }

        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "canceled")]
        private void EndDrag(bool canceled)
        {
            if (this.dragInfo != null)
            {
                DragInfo info = this.dragInfo;
                this.dragInfo = null;
                info.DraggedElement.ReleaseMouseCapture();
            }
            this.OnLayoutChangeEnded();
            this.IsDragging = false;
            this.IsResizing = false;
        }

        private static WindowPopupFactory GetFactory()
        {
            return new WindowPopupSilverlightFactory();
        }

        public static RadWindow GetParentRadWindow(DependencyObject child)
        {
            return child.GetParent<RadWindow>();
        }

        public static ResponseButton GetResponseButton(DependencyObject button)
        {
            return (ResponseButton)button.GetValue(ResponseButtonProperty);
        }

        internal Thumb GetThumbByType(ThumbType type)
        {
            return (from t in this.thumbs
                    where GetThumbType(t) == type
                    select t).FirstOrDefault<Thumb>();
        }

        internal static ThumbType GetThumbType(Thumb draggedElement)
        {
            if (draggedElement != null)
            {
                return (ThumbType)thumbNames.ToList<string>().IndexOf(draggedElement.Name);
            }
            return ThumbType.NONE;
        }

        private static bool HasFocus(UIElement target)
        {
            if (target != null)
            {
                FrameworkElement focusedSubtree = FocusManager.GetFocusedElement() as FrameworkElement;
                while (focusedSubtree != null)
                {
                    if (object.ReferenceEquals(focusedSubtree, target))
                    {
                        return true;
                    }
                    if (focusedSubtree.ReadLocalValue(Telerik.Windows.RoutedEvent.LogicalParentProperty) != DependencyProperty.UnsetValue)
                    {
                        focusedSubtree = Telerik.Windows.RoutedEvent.GetLogicalParent(focusedSubtree) as FrameworkElement;
                    }
                    else
                    {
                        focusedSubtree = (VisualTreeHelper.GetParent(focusedSubtree) ?? focusedSubtree.Parent) as FrameworkElement;
                    }
                }
            }
            return false;
        }

        internal static void InvokeButton(Button button)
        {
            if (button != null)
            {
                button.Focus();
                ButtonAutomationPeer peer = new ButtonAutomationPeer(button);
                IInvokeProvider invkProv = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
                if (invkProv != null)
                {
                    invkProv.Invoke();
                }
            }
        }

        private static bool IsCompassThumb(Thumb t)
        {
            switch (GetThumbType(t))
            {
                case ThumbType.ElementWestThumb:
                case ThumbType.ElementNorthWestThumb:
                case ThumbType.ElementNorthThumb:
                case ThumbType.ElementNorthEastThumb:
                case ThumbType.ElementEastThumb:
                case ThumbType.ElementSouthEastThumb:
                case ThumbType.ElementSouthThumb:
                case ThumbType.ElementSouthWestThumb:
                    return true;
            }
            return false;
        }

        private static bool IsRadButtonWithCommandSet(DependencyObject element)
        {
            RadButton button = element as RadButton;
            return ((button != null) && (button.Command != null));
        }

        private bool IsTitleDoubleClickClick()
        {
            DateTime localTitleLastClick = this.titleLastClick;
            this.titleLastClick = DateTime.Now;
            return ((DateTime.Now - localTitleLastClick) < doubleClickInterval);
        }

        private static bool IsWestOrNorthThumb(Thumb t)
        {
            switch (GetThumbType(t))
            {
                case ThumbType.ElementWestThumb:
                case ThumbType.ElementNorthWestThumb:
                case ThumbType.ElementNorthThumb:
                case ThumbType.ElementNorthEastThumb:
                case ThumbType.ElementSouthWestThumb:
                    return true;
            }
            return false;
        }

        private void LoadButtons()
        {
            this.closeButton = base.GetTemplateChild("PART_CloseButton") as FrameworkElement;
            this.minimizeButton = base.GetTemplateChild("PART_MinimizeButton") as FrameworkElement;
            this.maximizeButton = base.GetTemplateChild("PART_MaximizeButton") as FrameworkElement;
            this.restoreButton = base.GetTemplateChild("PART_RestoreButton") as FrameworkElement;
            this.headerButtonsBorder = base.GetTemplateChild("PART_HeaderButtonsBorder") as FrameworkElement;
            this.PutCommandsToButtons();
        }

        private void LoadLayoutControls()
        {
            this.contentElement = base.GetTemplateChild("ContentElement") as FrameworkElement;
        }

        private void LoadThumbControls()
        {
            foreach (Thumb thumb in this.thumbs)
            {
                thumb.DragStarted -= new DragStartedEventHandler(this.OnThumbDragStarted);
                thumb.DragDelta -= new DragDeltaEventHandler(this.OnThumbDragDelta);
                thumb.DragCompleted -= new DragCompletedEventHandler(this.OnThumbDragCompleted);
            }
            this.thumbs.Clear();
            foreach (string thumbName in thumbNames)
            {
                Thumb thumb = base.GetTemplateChild(thumbName) as Thumb;
                if (thumb != null)
                {
                    thumb.DragStarted += new DragStartedEventHandler(this.OnThumbDragStarted);
                    thumb.DragDelta += new DragDeltaEventHandler(this.OnThumbDragDelta);
                    thumb.DragCompleted += new DragCompletedEventHandler(this.OnThumbDragCompleted);
                    this.thumbs.Add(thumb);
                }
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (this.Popup == null)
            {
                return base.MeasureOverride(availableSize);
            }
            Size appSize = this.Popup.GetRootSize();
            double width = availableSize.Width;
            double height = availableSize.Height;
            if (this.IsRestricted)
            {
                if (double.IsInfinity(availableSize.Width))
                {
                    width = (appSize.Width - this.RestrictedAreaMargin.Right) - this.Left;
                }
                if (double.IsInfinity(availableSize.Height))
                {
                    height = (appSize.Height - this.RestrictedAreaMargin.Bottom) - this.Top;
                }
                return base.MeasureOverride(new Size(width, height));
            }
            if (double.IsInfinity(availableSize.Width))
            {
                width = appSize.Width * DefaultSizeRatio;
            }
            if (double.IsInfinity(availableSize.Height))
            {
                height = appSize.Height * DefaultSizeRatio;
            }
            return base.MeasureOverride(new Size(width, height));
        }

        private void Move(double x, double y)
        {
            if (this.IsInDesignMode)
            {
                TranslateTransform translate = this.GetTranslateTransform();
                translate.X = x;
                translate.Y = y;
            }
            else if (this.popup != null)
            {
                this.popup.Move(Math.Round(x, 0), Math.Round(y, 0));
            }
            this.OnLocationChanged();
        }

        private void OnActivated()
        {
            this.OnActivated(EventArgs.Empty);
        }

        protected virtual void OnActivated(EventArgs args)
        {
            if (this.Activated != null)
            {
                this.Activated(this, args);
            }
        }

        private void OnApplicationSizeChanged(object sender, EventArgs args)
        {
            if (this.IsOpen && IsInBrowser)
            {
                Size appSize = ApplicationHelper.ApplicationSize;
                this.backCanvas.Width = appSize.Width;
                this.backCanvas.Height = appSize.Height;
                if (this.WindowState == System.Windows.WindowState.Maximized)
                {
                    Rect sizeAndPosition = this.manager.SizeAndPosition;
                    base.Width = sizeAndPosition.Width;
                    base.Height = sizeAndPosition.Height;
                    this.Left = sizeAndPosition.Left;
                    this.Top = sizeAndPosition.Top;
                }
                this.CheckRestrictedArea();
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.isTemplateApplied = true;
            this.LoadButtons();
            this.LoadLayoutControls();
            this.LoadThumbControls();
            this.CheckRestrictedArea();
            if (this.WindowState == System.Windows.WindowState.Normal)
            {
                this.manager.UpdateLayout();
            }
            this.UpdateWindowState();
        }

        private static void OnCanCloseChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadWindow window = d as RadWindow;
            if (window != null)
            {
                window.UpdateWindowState();
            }
        }

        private static void OnCanMoveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadWindow window = d as RadWindow;
            if (window != null)
            {
                window.UpdateWindowState();
            }
        }

        private static void OnCloseClick(object sender, ExecutedRoutedEventArgs e)
        {
            RadWindow window = sender as RadWindow;
            if (window != null)
            {
                window.Close();
            }
        }

        private void OnClosed()
        {
            this.OnClosed(new WindowClosedEventArgs { DialogResult = this.DialogResult, PromptResult = this.PromptResult });
        }

        protected virtual void OnClosed(WindowClosedEventArgs args)
        {
            if (this.Closed != null)
            {
                this.Closed(this, args);
            }
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new RadWindowAutomationPeer(this);
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            if (e.OriginalSource == this)
            {
                AutomationPeer peer = FrameworkElementAutomationPeer.FromElement(this);
                if (peer != null)
                {
                    peer.RaiseAutomationEvent(AutomationEvents.AutomationFocusChanged);
                }
            }
        }

        private static void OnIsActiveWindowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadWindow window = d as RadWindow;
            if (window != null)
            {
                window.ChangeVisualState();
            }
        }

        private static void OnIsDraggingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadWindow window = d as RadWindow;
            if (window != null)
            {
                window.ChangeVisualState();
            }
        }

        protected override void OnIsFocusedChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnIsFocusedChanged(e);
            RadWindowAutomationPeer peer = FrameworkElementAutomationPeer.FromElement(this) as RadWindowAutomationPeer;
            if (peer != null)
            {
                peer.RaiseAutomationHasKeyboardFocusChanged((bool)e.NewValue);
            }
        }

        private static void OnIsModalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadWindow window = d as RadWindow;
            if (window != null)
            {
                window.ChangeVisualState();
            }
        }

        private static void OnIsResizingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadWindow window = d as RadWindow;
            if (window != null)
            {
                window.ChangeVisualState();
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if ((e.Key == Key.Enter) || (e.Key == Key.Escape))
            {
                ResponseButton searchedResponseButton = (e.Key == Key.Enter) ? ResponseButton.Accept : ResponseButton.Cancel;
                InvokeButton(this.ChildrenOfType<Button>().FirstOrDefault<Button>(b => (GetResponseButton(b) == searchedResponseButton) && b.IsEnabled));
            }
        }

        private void OnLayoutChangeEnded()
        {
            this.OnLayoutChangeEnded(EventArgs.Empty);
        }

        protected virtual void OnLayoutChangeEnded(EventArgs args)
        {
            if (this.LayoutChangeEnded != null)
            {
                this.LayoutChangeEnded(this, args);
            }
        }

        private void OnLayoutChangeStarted()
        {
            this.OnLayoutChangeStarted(EventArgs.Empty);
        }

        protected virtual void OnLayoutChangeStarted(EventArgs args)
        {
            if (this.LayoutChangeStarted != null)
            {
                this.LayoutChangeStarted(this, args);
            }
        }

        private static void OnLeftChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadWindow window = d as RadWindow;
            double left = (double)e.NewValue;
            window.Move(left, window.Top);
            window.manager.UpdateLeft(left);
            RadWindowAutomationPeer peer = FrameworkElementAutomationPeer.FromElement(window) as RadWindowAutomationPeer;
            if (peer != null)
            {
                Rect oldRect = new Rect((double)e.OldValue, window.Top, window.RenderSize.Width, window.RenderSize.Height);
                Rect newRect = new Rect(window.Left, window.Top, window.RenderSize.Width, window.RenderSize.Height);
                peer.RaiseAutomationBoundingRectangleChanged(oldRect, newRect);
            }
        }

        private void OnLocationChanged()
        {
            this.OnLocationChanged(new RoutedEventArgs());
        }

        protected virtual void OnLocationChanged(RoutedEventArgs args)
        {
            if (this.LocationChanged != null)
            {
                this.LocationChanged(this, args);
            }
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            if ((e.OriginalSource == this) || (GetParentRadWindow(e.OriginalSource as DependencyObject) != this))
            {
                base.OnLostFocus(e);
                AutomationPeer peer = FrameworkElementAutomationPeer.FromElement(this);
                if (peer != null)
                {
                    peer.RaiseAutomationEvent(AutomationEvents.AutomationFocusChanged);
                }
            }
        }

        private static void OnMaximizeClick(object sender, ExecutedRoutedEventArgs e)
        {
            RadWindow window = sender as RadWindow;
            if (window != null)
            {
                window.WindowState = System.Windows.WindowState.Maximized;
            }
        }

        private static void OnMinimizeClick(object sender, ExecutedRoutedEventArgs e)
        {
            RadWindow window = sender as RadWindow;
            if (window != null)
            {
                window.WindowState = System.Windows.WindowState.Minimized;
            }
        }

        private void OnModalBackgroundClick(object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "PressedOutsideModal", true);
            Telerik.Windows.Input.MouseButtonEventArgs args = e as Telerik.Windows.Input.MouseButtonEventArgs;
            UIElement fe = FocusManager.GetFocusedElement() as UIElement;
            if ((fe == null) || !this.IsAncestorOf(fe))
            {
                base.Focus();
            }
            args.Handled = true;
        }

        private void OnMouseButtonDown(object sender, RoutedEventArgs e)
        {
            if (!this.IsActiveWindow)
            {
                this.BringToFront();
            }
        }

        private void OnOpened()
        {
            this.UpdateWindowState();
            if (this.IsModal)
            {
                Size appSize = ApplicationHelper.ApplicationSize;
                this.backCanvas.Width = appSize.Width;
                this.backCanvas.Height = appSize.Height;
            }
            Action callback = delegate
            {
                this.OnLayoutChangeStarted();
                this.OnLayoutChangeEnded();
                this.OnActivated();
            };
            RadWindowManager.Current.AddWindow(this);
            this.OnOpened(new RoutedEventArgs());
            this.CheckRestrictedArea();
            this.IsOpen = true;
            this.isOpening = false;
            this.StopAllAnimations();
            AnimationManager.Play(this, "Show", callback, new object[0]);
        }

        protected virtual void OnOpened(RoutedEventArgs args)
        {
            if (this.Opened != null)
            {
                this.Opened(this, args);
            }
        }

        private bool OnPreviewClosed()
        {
            WindowPreviewClosedEventArgs closingArgs = new WindowPreviewClosedEventArgs();
            this.OnPreviewClosed(closingArgs);
            if (closingArgs.Cancel.GetValueOrDefault())
            {
                return !closingArgs.Cancel.HasValue;
            }
            return true;
        }

        protected virtual void OnPreviewClosed(WindowPreviewClosedEventArgs args)
        {
            if (this.PreviewClosed != null)
            {
                this.PreviewClosed(this, args);
            }
        }

        private static void OnResizeModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!Enum.IsDefined(typeof(Telerik.Windows.Controls.ResizeMode), (Telerik.Windows.Controls.ResizeMode)e.NewValue))
            {
                throw new InvalidCastException("Invalid value for ResizeMode");
            }
            RadWindow window = d as RadWindow;
            if (window != null)
            {
                window.UpdateWindowState();
            }
        }

        private static void OnResponseButtonPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!Enum.IsDefined(typeof(ResponseButton), e.NewValue))
            {
                throw new ArgumentException("Unknown value for property ResponseButton");
            }
            if (!(sender is Button))
            {
                throw new ArgumentException("Attached property ResponseButton can only be part of Button.");
            }
        }

        private static void OnRestoreClick(object sender, ExecutedRoutedEventArgs e)
        {
            RadWindow window = sender as RadWindow;
            if (window != null)
            {
                window.WindowState = System.Windows.WindowState.Normal;
            }
        }

        private void OnThumbDragCompleted(object sender, DragCompletedEventArgs e)
        {
            this.EndDrag(e.Canceled);
        }

        private void OnThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            this.DoDrag(e.HorizontalChange, e.VerticalChange);
        }

        private void OnThumbDragStarted(object sender, DragStartedEventArgs e)
        {
            Thumb thumb = sender as Thumb;
            if ((GetThumbType(thumb) == ThumbType.TitleThumb) && this.IsTitleDoubleClickClick())
            {
                this.OnTitleDoubleClick();
            }
            else
            {
                UIElement thumbParent;
                if (((this.WindowState != System.Windows.WindowState.Maximized) && (thumb != null)) && ((thumbParent = thumb.Parent as UIElement) != null))
                {
                    GeneralTransform transform = thumbParent.TransformToVisual(this.popup.GetVisual());
                    Point mouseLocation = new Point(e.HorizontalOffset, e.VerticalOffset);
                    DragInfo info = new DragInfo(transform.Transform(mouseLocation), thumb, this);
                    this.StartDragging(info);
                }
            }
        }

        private void OnTitleDoubleClick()
        {
            if (this.ResizeMode != Telerik.Windows.Controls.ResizeMode.NoResize)
            {
                if (this.WindowState == System.Windows.WindowState.Normal)
                {
                    this.WindowState = (this.ResizeMode == Telerik.Windows.Controls.ResizeMode.CanMinimize) ? System.Windows.WindowState.Minimized : System.Windows.WindowState.Maximized;
                }
                else
                {
                    this.WindowState = System.Windows.WindowState.Normal;
                }
            }
        }

        private static void OnTopChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadWindow window = d as RadWindow;
            double top = (double)e.NewValue;
            window.Move(window.Left, top);
            window.manager.UpdateTop(top);
            RadWindowAutomationPeer peer = FrameworkElementAutomationPeer.FromElement(window) as RadWindowAutomationPeer;
            if (peer != null)
            {
                Rect oldRect = new Rect(window.Left, (double)e.OldValue, window.RenderSize.Width, window.RenderSize.Height);
                Rect newRect = new Rect(window.Left, window.Top, window.RenderSize.Width, window.RenderSize.Height);
                peer.RaiseAutomationBoundingRectangleChanged(oldRect, newRect);
            }
        }

        private static void OnWindowStartupLocationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!Enum.IsDefined(typeof(Telerik.Windows.Controls.WindowStartupLocation), (Telerik.Windows.Controls.WindowStartupLocation)e.NewValue))
            {
                throw new InvalidCastException("Invalid value for WindowStartupLocation");
            }
        }

        private void OnWindowStateChanged()
        {
            this.UpdateWindowState();
            this.OnWindowStateChanged(new RoutedEventArgs());
        }

        protected virtual void OnWindowStateChanged(RoutedEventArgs args)
        {
            if (this.WindowStateChanged != null)
            {
                this.WindowStateChanged(this, args);
            }
        }

        private static void OnWindowStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!Enum.IsDefined(typeof(System.Windows.WindowState), (System.Windows.WindowState)e.NewValue))
            {
                throw new InvalidCastException("Invalid value for WindowState");
            }
            RadWindow window = d as RadWindow;
            if (((System.Windows.WindowState)e.OldValue) == System.Windows.WindowState.Normal)
            {
                window.manager.UpdateLayout();
            }
            window.OnWindowStateChanged();
            RadWindowAutomationPeer peer = FrameworkElementAutomationPeer.FromElement(window) as RadWindowAutomationPeer;
            if (peer != null)
            {
                peer.RaiseAutomationWindowStateChanged(e.OldValue, e.NewValue);
            }
        }

        public static void Prompt(DialogParameters dialogParameters)
        {
            ConfigureModal(new RadPrompt(), dialogParameters);
        }

        public static void Prompt(object content, EventHandler<WindowClosedEventArgs> closed)
        {
            DialogParameters pars = new DialogParameters
            {
                Content = content,
                Closed = closed
            };
            ConfigureModal(new RadPrompt(), pars);
        }

        public static void Prompt(object content, EventHandler<WindowClosedEventArgs> closed, string defaultPromptResult)
        {
            DialogParameters pars = new DialogParameters
            {
                Content = content,
                Closed = closed,
                DefaultPromptResultValue = defaultPromptResult
            };
            ConfigureModal(new RadPrompt(), pars);
        }

        private void PutCommandsToButtons()
        {
            PutCommandToElement(this.closeButton, WindowCommands.Close);
            PutCommandToElement(this.minimizeButton, WindowCommands.Minimize);
            PutCommandToElement(this.maximizeButton, WindowCommands.Maximize);
            PutCommandToElement(this.restoreButton, WindowCommands.Restore);
        }

        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "command"), SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "element")]
        private static void PutCommandToElement(DependencyObject element, ICommand command)
        {
            if (((element != null) && (CommandManager.GetInputBindings(element) == null)) && !IsRadButtonWithCommandSet(element))
            {
                InputBindingCollection bindings = new InputBindingCollection {
                    new InputBinding(command, new MouseGesture(MouseAction.LeftClick))
                };
                CommandManager.SetInputBindings(element, bindings);
            }
        }

        private void RemoveParent()
        {
            if (!this.IsInDesignMode && (this.popup == null))
            {
                HACKS.RemoveParent(this);
            }
        }

        private static void SetElementVisibility(FrameworkElement element, Visibility newValue)
        {
            if (element != null)
            {
                Control c = element as Control;
                if (c != null)
                {
                    c.IsEnabled = newValue == Visibility.Visible;
                }
                element.Visibility = newValue;
            }
        }

        public static void SetResponseButton(DependencyObject button, ResponseButton value)
        {
            button.SetValue(ResponseButtonProperty, value);
        }

        public void Show()
        {
            this.ShowWindow(false);
        }

        public void ShowDialog()
        {
            this.ShowWindow(true);
        }

        private void ShowWindow(bool modal)
        {
            base.ClearValue(DialogResultProperty);
            base.ClearValue(PromptResultProperty);
            this.isOpening = true;
            this.IsModal = modal;
            if (this.popup == null)
            {
                this.RemoveParent();
                this.popup = popupFactory.GetPopup(this, this.backCanvas, this.IsTopMost);
                this.popup.Opened += delegate(object s, EventArgs e)
                {
                    this.AttachSizeChangedEvents();
                    base.Dispatcher.BeginInvoke(delegate
                    {
                        Control fe = FocusManager.GetFocusedElement() as Control;
                        if ((fe == null) || !this.IsAncestorOf(fe))
                        {
                            base.Focus();
                        }
                    });
                    if (!this.isTemplateApplied)
                    {
                        RoutedEventHandler loadedHandler = null;
                        loadedHandler = delegate(object sender, RoutedEventArgs args)
                        {
                            this.Loaded -= loadedHandler;
                            this.OnOpened();
                        };
                        base.Loaded += loadedHandler;
                    }
                    else
                    {
                        this.OnOpened();
                    }
                };
            }
            this.popup.WindowStartupLocation = this.WindowStartupLocation;
            this.popup.Owner = this.Owner;
            base.Visibility = Visibility.Visible;
            this.popup.Open(modal);
        }

        private void StartDragging(DragInfo info)
        {
            if ((((this.dragInfo != null) || (info.DraggedThumbType == ThumbType.NONE)) || ((info.DraggedThumbType == ThumbType.TitleThumb) && !this.CanMove)) || ((info.DraggedThumbType != ThumbType.TitleThumb) && (this.ResizeMode != Telerik.Windows.Controls.ResizeMode.CanResize)))
            {
                this.EndDrag(true);
            }
            else
            {
                this.OnLayoutChangeStarted();
                if (info.DraggedThumbType == ThumbType.TitleThumb)
                {
                    this.IsDragging = true;
                }
                else
                {
                    this.IsResizing = true;
                }
                this.dragInfo = info;
            }
        }

        private void StopAllAnimations()
        {
            AnimationManager.StopIfRunning(this, "Show");
            AnimationManager.StopIfRunning(this, "Hide");
        }

        private void UpdateButtonsVisibility()
        {
            Visibility visibility = this.CanClose ? Visibility.Visible : Visibility.Collapsed;
            SetElementVisibility(this.closeButton, visibility);
            visibility = ((this.WindowState != System.Windows.WindowState.Normal) && (this.ResizeMode != Telerik.Windows.Controls.ResizeMode.NoResize)) ? Visibility.Visible : Visibility.Collapsed;
            SetElementVisibility(this.restoreButton, visibility);
            visibility = ((this.WindowState != System.Windows.WindowState.Maximized) && (this.ResizeMode == Telerik.Windows.Controls.ResizeMode.CanResize)) ? Visibility.Visible : Visibility.Collapsed;
            SetElementVisibility(this.maximizeButton, visibility);
            visibility = ((this.WindowState != System.Windows.WindowState.Minimized) && (this.ResizeMode != Telerik.Windows.Controls.ResizeMode.NoResize)) ? Visibility.Visible : Visibility.Collapsed;
            SetElementVisibility(this.minimizeButton, visibility);
            visibility = ((this.CanClose || (this.WindowState != System.Windows.WindowState.Normal)) || ((this.ResizeMode == Telerik.Windows.Controls.ResizeMode.CanResize) || (this.ResizeMode != Telerik.Windows.Controls.ResizeMode.NoResize))) ? Visibility.Visible : Visibility.Collapsed;
            SetElementVisibility(this.headerButtonsBorder, visibility);
        }

        private void UpdateContentVisibility()
        {
            Visibility visibility = (this.WindowState != System.Windows.WindowState.Minimized) ? Visibility.Visible : Visibility.Collapsed;
            SetElementVisibility(this.contentElement, visibility);
        }

        private void UpdateThumbsVisibility()
        {
            foreach (Thumb thumb in this.thumbs.Where<Thumb>(new Func<Thumb, bool>(RadWindow.IsCompassThumb)))
            {
                thumb.Visibility = (((this.WindowState != System.Windows.WindowState.Normal) || (this.ResizeMode != Telerik.Windows.Controls.ResizeMode.CanResize)) || (!this.CanMove && IsWestOrNorthThumb(thumb))) ? Visibility.Collapsed : Visibility.Visible;
            }
            Thumb titleThumb = this.GetThumbByType(ThumbType.TitleThumb);
            if (titleThumb != null)
            {
                titleThumb.Visibility = this.CanMove ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        internal void UpdateWindowState()
        {
            if (this.isTemplateApplied)
            {
                Rect sizeAndPosition = this.manager.SizeAndPosition;
                base.Width = sizeAndPosition.Width;
                base.Height = sizeAndPosition.Height;
                this.Left = sizeAndPosition.Left;
                this.Top = sizeAndPosition.Top;
                this.CheckRestrictedArea();
                this.UpdateContentVisibility();
                this.UpdateButtonsVisibility();
                this.UpdateThumbsVisibility();
                this.ChangeVisualState(true);
            }
        }

        public Brush BorderBackground
        {
            get
            {
                return (Brush)base.GetValue(BorderBackgroundProperty);
            }
            set
            {
                base.SetValue(BorderBackgroundProperty, value);
            }
        }

        public bool CanClose
        {
            get
            {
                return (bool)base.GetValue(CanCloseProperty);
            }
            set
            {
                base.SetValue(CanCloseProperty, value);
            }
        }

        public bool CanMove
        {
            get
            {
                return (bool)base.GetValue(CanMoveProperty);
            }
            set
            {
                base.SetValue(CanMoveProperty, value);
            }
        }

        public static double DefaultSizeRatio { get; set; }

        public bool? DialogResult
        {
            get
            {
                return (bool?)base.GetValue(DialogResultProperty);
            }
            set
            {
                base.SetValue(DialogResultProperty, value);
            }
        }

        public object Icon
        {
            get
            {
                return base.GetValue(IconProperty);
            }
            set
            {
                base.SetValue(IconProperty, value);
            }
        }

        public DataTemplate IconTemplate
        {
            get
            {
                return (DataTemplate)base.GetValue(IconTemplateProperty);
            }
            set
            {
                base.SetValue(IconTemplateProperty, value);
            }
        }

        public bool IsActiveWindow
        {
            get
            {
                return (bool)base.GetValue(IsActiveWindowProperty);
            }
            internal set
            {
                this.SetValue(IsActiveWindowPropertyKey, value);
            }
        }

        public bool IsDragging
        {
            get
            {
                return (bool)base.GetValue(IsDraggingProperty);
            }
            private set
            {
                this.SetValue(IsDraggingPropertyKey, value);
            }
        }

        public bool IsHeaderHitTestVisible
        {
            get
            {
                return (bool)base.GetValue(IsHeaderHitTestVisibleProperty);
            }
            set
            {
                base.SetValue(IsHeaderHitTestVisibleProperty, value);
            }
        }

        internal static bool IsInBrowser
        {
            get
            {
                return true;
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        internal bool IsInDesignMode
        {
            get
            {
                return RadControl.IsInDesignMode;
            }
        }

        public bool IsLayoutChanging
        {
            get
            {
                if (!this.IsDragging)
                {
                    return this.IsResizing;
                }
                return true;
            }
        }

        public bool IsModal
        {
            get
            {
                return (bool)base.GetValue(IsModalProperty);
            }
            private set
            {
                this.SetValue(IsModalPropertyKey, value);
            }
        }

        public bool IsOpen
        {
            get
            {
                return this.isOpen;
            }
            private set
            {
                this.isOpen = value;
            }
        }

        public bool IsResizing
        {
            get
            {
                return (bool)base.GetValue(IsResizingProperty);
            }
            private set
            {
                this.SetValue(IsResizingPropertyKey, value);
            }
        }

        public bool IsRestricted
        {
            get
            {
                return (bool)base.GetValue(IsRestrictedProperty);
            }
            set
            {
                base.SetValue(IsRestrictedProperty, value);
            }
        }

        internal bool IsTopMost { get; set; }

        public double Left
        {
            get
            {
                return (double)base.GetValue(LeftProperty);
            }
            set
            {
                base.SetValue(LeftProperty, value);
            }
        }

        public double LeftOffset
        {
            get
            {
                return (double)base.GetValue(LeftOffsetProperty);
            }
            set
            {
                base.SetValue(LeftOffsetProperty, value);
            }
        }

        public Brush ModalBackground
        {
            get
            {
                return (Brush)base.GetValue(ModalBackgroundProperty);
            }
            set
            {
                base.SetValue(ModalBackgroundProperty, value);
            }
        }

        public ContentControl Owner { get; set; }

        internal WindowPopup Popup
        {
            get
            {
                return this.popup;
            }
        }

        public string PromptResult
        {
            get
            {
                return (string)base.GetValue(PromptResultProperty);
            }
            set
            {
                base.SetValue(PromptResultProperty, value);
            }
        }

        public Telerik.Windows.Controls.ResizeMode ResizeMode
        {
            get
            {
                return (Telerik.Windows.Controls.ResizeMode)base.GetValue(ResizeModeProperty);
            }
            set
            {
                base.SetValue(ResizeModeProperty, value);
            }
        }

        public bool RestoreMinimizedLocation
        {
            get
            {
                return (bool)base.GetValue(RestoreMinimizedLocationProperty);
            }
            set
            {
                base.SetValue(RestoreMinimizedLocationProperty, value);
            }
        }

        public Thickness RestrictedAreaMargin
        {
            get
            {
                return (Thickness)base.GetValue(RestrictedAreaMarginProperty);
            }
            set
            {
                base.SetValue(RestrictedAreaMarginProperty, value);
            }
        }

        bool IWindow.IsOpen
        {
            get
            {
                return this.IsOpen;
            }
        }

        int IWindow.Z
        {
            get
            {
                return this.Z;
            }
        }

        public double Top
        {
            get
            {
                return (double)base.GetValue(TopProperty);
            }
            set
            {
                base.SetValue(TopProperty, value);
            }
        }

        public double TopOffset
        {
            get
            {
                return (double)base.GetValue(TopOffsetProperty);
            }
            set
            {
                base.SetValue(TopOffsetProperty, value);
            }
        }

        public Telerik.Windows.Controls.WindowStartupLocation WindowStartupLocation
        {
            get
            {
                return (Telerik.Windows.Controls.WindowStartupLocation)base.GetValue(WindowStartupLocationProperty);
            }
            set
            {
                base.SetValue(WindowStartupLocationProperty, value);
            }
        }

        public System.Windows.WindowState WindowState
        {
            get
            {
                return (System.Windows.WindowState)base.GetValue(WindowStateProperty);
            }
            set
            {
                base.SetValue(WindowStateProperty, value);
            }
        }

        internal int Z
        {
            get
            {
                if (this.popup != null)
                {
                    return this.popup.GetZIndex();
                }
                return 0;
            }
        }
    }
}

