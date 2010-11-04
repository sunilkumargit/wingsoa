namespace Telerik.Windows.Controls.Docking
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Media;
    using Telerik.Windows;
    using Telerik.Windows.Controls;
    using Telerik.Windows.Controls.Animation;
    using Telerik.Windows.Controls.DragDrop;

    public class ToolWindow : HeaderedContentControl, IThemable, IWindow, INotifyLayoutChange
    {
        private bool bringToFrontWhenLoaded;
        internal static readonly Telerik.Windows.RoutedEvent CloseEvent = EventManager.RegisterRoutedEvent("Close", RoutingStrategy.Bubble, typeof(EventHandler<RadRoutedEventArgs>), typeof(ToolWindow));
        internal static readonly Telerik.Windows.RoutedEvent DragCompletedEvent = RadPane.DragCompletedEvent.AddOwner(typeof(ToolWindow));
        internal static readonly Telerik.Windows.RoutedEvent DragDeltaEvent = RadPane.DragDeltaEvent.AddOwner(typeof(ToolWindow));
        private Point draggingStartPoint;
        internal static readonly Telerik.Windows.RoutedEvent DragStartedEvent = RadPane.DragStartedEvent.AddOwner(typeof(ToolWindow));
        private TranslateTransform dragTransform;
        private UIElement headerElement;
        public static readonly DependencyProperty HorizontalOffsetProperty = DependencyProperty.Register("HorizontalOffset", typeof(double), typeof(ToolWindow), new Telerik.Windows.PropertyMetadata(0.0, new PropertyChangedCallback(ToolWindow.OnHorizontalOffsetChange)));
        private Rect initialRect;
        public static readonly DependencyProperty IsDraggingProperty;
        private static readonly DependencyPropertyKey IsDraggingPropertyKey = DependencyPropertyExtensions.RegisterReadOnly("IsDragging", typeof(bool), typeof(ToolWindow), new Telerik.Windows.PropertyMetadata(false, new PropertyChangedCallback(ToolWindow.OnIsDraggingPropertyChanged)));
        private bool isLoaded;
        private bool isMouseCaptured;
        private bool isOpen;
        public static readonly DependencyProperty IsResizingProperty;
        private static readonly DependencyPropertyKey IsResizingPropertyKey = DependencyPropertyExtensions.RegisterReadOnly("IsResizing", typeof(bool), typeof(ToolWindow), new Telerik.Windows.PropertyMetadata(false, new PropertyChangedCallback(ToolWindow.OnIsResizingPropertyChanged)));
        private static int openedWindows;
        internal static readonly Telerik.Windows.RoutedEvent PreviewCloseEvent = EventManager.RegisterRoutedEvent("PreviewClose", RoutingStrategy.Tunnel, typeof(EventHandler<RadRoutedEventArgs>), typeof(ToolWindow));
        private ResizeDirection resizeDirection;
        private bool templateApllied;
        public static readonly DependencyProperty VerticalOffsetProperty = DependencyProperty.Register("VerticalOffset", typeof(double), typeof(ToolWindow), new Telerik.Windows.PropertyMetadata(0.0, new PropertyChangedCallback(ToolWindow.OnVerticalOffsetChange)));
        private UIElement visualRoot;

        public event EventHandler LayoutChangeEnded;

        public event EventHandler LayoutChangeStarted;

        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static ToolWindow()
        {
            RadDockingCommands.EnsureCommandsClassLoaded();
            IsDraggingProperty = IsDraggingPropertyKey.DependencyProperty;
            IsResizingProperty = IsResizingPropertyKey.DependencyProperty;
            EventManager.RegisterClassHandler(typeof(ToolWindow), RadPane.ShowEvent, new EventHandler<StateChangeCommandEventArgs>(ToolWindow.OnPaneShown), true);
            CommandManager.RegisterClassCommandBinding(typeof(ToolWindow), new CommandBinding(WindowCommands.Close, new ExecutedRoutedEventHandler(ToolWindow.OnCloseButtonClick), new CanExecuteRoutedEventHandler(ToolWindow.OnCanCloseButtonClick)));
            CommandManager.RegisterClassCommandBinding(typeof(ToolWindow), new CommandBinding(RadDockingCommands.PaneHeaderMenuOpen, new ExecutedRoutedEventHandler(ToolWindow.OnPaneHeaderMenuOpen), new CanExecuteRoutedEventHandler(ToolWindow.OnCanPaneHeaderMenuOpen)));
        }

        public ToolWindow()
        {
            base.DefaultStyleKey = typeof(ToolWindow);
            this.bringToFrontWhenLoaded = true;
            this.isOpen = true;
            base.LostMouseCapture += new MouseEventHandler(this.OnLostMouseCapture);
            base.Loaded += new RoutedEventHandler(this.OnLoaded);
            base.MouseMove += new MouseEventHandler(this.OnToolWindowMouseMove);
            this.dragTransform = this.GetTranslateTransform();
        }

        private static void BringToFront(ToolWindow window)
        {
            Panel windowParent = window.Parent as Panel;
            if (windowParent != null)
            {
                IEnumerable<UIElement> toolWindows = from w in windowParent.Children.Cast<UIElement>()
                    where w is ToolWindow
                    select w;
                int maxZIndex = ((IEnumerable<int>) (from w in toolWindows select Canvas.GetZIndex(w))).Max();
                if ((Canvas.GetZIndex(window) < maxZIndex) || (toolWindows.Count<UIElement>(w => (Canvas.GetZIndex(w) == maxZIndex)) > 1))
                {
                    Canvas.SetZIndex(window, maxZIndex + 1);
                }
            }
        }

        private void CancelDrag()
        {
            if (this.IsDragging)
            {
                if (this.isMouseCaptured)
                {
                    base.ReleaseMouseCapture();
                    this.isMouseCaptured = false;
                }
                this.ClearValue(IsDraggingPropertyKey);
                this.StopDrag();
            }
        }

        private bool CanClose()
        {
            RadSplitContainer container = this.SplitContainer;
            return ((container == null) || !container.EnumeratePanes().Any<RadPane>(p => !p.CanUserClose));
        }

        internal void Close()
        {
            if ((this.isOpen && this.OnPreviewClose(new RadRoutedEventArgs(PreviewCloseEvent, this))) && this.CloseAllPanes())
            {
                this.CloseWindow();
            }
        }

        private bool CloseAllPanes()
        {
            if (this.SplitContainer == null)
            {
                return true;
            }
            List<RadPane> panes = (from p in this.SplitContainer.EnumeratePanes()
                where !p.IsHidden
                select p).ToList<RadPane>();
            foreach (RadPane pane in panes)
            {
                if (pane.CanUserClose)
                {
                    pane.IsHidden = true;
                }
            }
            return panes.All<RadPane>(p => p.IsHidden);
        }

        internal void CloseWindow()
        {
            if (this.isOpen && this.CanClose())
            {
                this.isOpen = false;
                Action hiddenCallback = delegate {
                    openedWindows--;
                    base.Visibility = Visibility.Collapsed;
                    this.OnClose(new RadRoutedEventArgs(CloseEvent, this));
                };
                AnimationManager.Play(this, "Hide", hiddenCallback, new object[0]);
            }
        }

        private bool Drag(MouseEventArgs e)
        {
            if (base.Parent != null)
            {
                this.draggingStartPoint = e.GetPosition(this.UIParent);
                if (!this.IsDragging)
                {
                    MouseButtonEventArgs buttonEventArgs = e as MouseButtonEventArgs;
                    if (buttonEventArgs != null)
                    {
                        buttonEventArgs.Handled = true;
                    }
                    base.Focus();
                    this.isMouseCaptured = base.CaptureMouse();
                    this.IsDragging = this.isMouseCaptured;
                    if (this.isMouseCaptured)
                    {
                        this.draggingStartPoint = new Point(this.draggingStartPoint.X - this.HorizontalOffset, this.draggingStartPoint.Y - this.VerticalOffset);
                        bool flag = false;
                        try
                        {
                            this.StartDrag(e);
                            flag = true;
                        }
                        finally
                        {
                            if (!flag)
                            {
                                this.CancelDrag();
                            }
                        }
                    }
                }
            }
            return this.IsDragging;
        }

        private static string GetPaneTitle(RadPane radPane)
        {
            if (string.IsNullOrEmpty(radPane.Title))
            {
                return (radPane.Header as string);
            }
            return radPane.Title;
        }

        private ResizeDirection GetResizeDirection(Point pointerPosition)
        {
            Point offset = base.TransformToVisual(this.UIParent).Transform(new Point(0.0, 0.0));
            double resizerSize = RadDragAndDropManager.DragStartThreshold + 2.0;
            pointerPosition = new Point(pointerPosition.X - offset.X, pointerPosition.Y - offset.Y);
            if ((pointerPosition.X >= 0.0) && (pointerPosition.X < resizerSize))
            {
                if ((pointerPosition.Y >= 0.0) && (pointerPosition.Y < resizerSize))
                {
                    return ResizeDirection.TopLeft;
                }
                if ((pointerPosition.Y > (base.ActualHeight - resizerSize)) && (pointerPosition.Y <= base.ActualHeight))
                {
                    return ResizeDirection.BottomLeft;
                }
                return ResizeDirection.Left;
            }
            if ((pointerPosition.X > (base.ActualWidth - resizerSize)) && (pointerPosition.X <= base.ActualWidth))
            {
                if ((pointerPosition.Y >= 0.0) && (pointerPosition.Y < resizerSize))
                {
                    return ResizeDirection.TopRight;
                }
                if ((pointerPosition.Y > (base.ActualHeight - resizerSize)) && (pointerPosition.Y <= base.ActualHeight))
                {
                    return ResizeDirection.BottomRight;
                }
                return ResizeDirection.Right;
            }
            if ((pointerPosition.Y >= 0.0) && (pointerPosition.Y < resizerSize))
            {
                return ResizeDirection.Top;
            }
            if ((pointerPosition.Y > (base.ActualHeight - resizerSize)) && (pointerPosition.Y <= base.ActualHeight))
            {
                return ResizeDirection.Bottom;
            }
            return ResizeDirection.None;
        }

        public override void OnApplyTemplate()
        {
            if (this.headerElement != null)
            {
                this.headerElement.MouseLeftButtonDown -= new MouseButtonEventHandler(this.OnHeaderMouseLeftButtonDown);
            }
            if (this.visualRoot != null)
            {
                this.visualRoot.MouseMove -= new MouseEventHandler(this.OnVisualRootMouseMove);
                this.visualRoot.MouseLeftButtonDown -= new MouseButtonEventHandler(this.OnVisualRootMouseDown);
                this.visualRoot.MouseLeftButtonUp -= new MouseButtonEventHandler(this.OnVisualRootMouseUp);
                this.visualRoot.MouseLeave -= new MouseEventHandler(this.OnVisualRootMouseLeave);
            }
            base.OnApplyTemplate();
            this.headerElement = base.GetTemplateChild("HeaderElement") as UIElement;
            this.visualRoot = base.GetTemplateChild("VisualRoot") as UIElement;
            if (this.headerElement != null)
            {
                this.headerElement.MouseLeftButtonDown += new MouseButtonEventHandler(this.OnHeaderMouseLeftButtonDown);
            }
            if (this.visualRoot != null)
            {
                this.visualRoot.MouseMove += new MouseEventHandler(this.OnVisualRootMouseMove);
                this.visualRoot.MouseLeftButtonDown += new MouseButtonEventHandler(this.OnVisualRootMouseDown);
                this.visualRoot.MouseLeftButtonUp += new MouseButtonEventHandler(this.OnVisualRootMouseUp);
                this.visualRoot.MouseLeave += new MouseEventHandler(this.OnVisualRootMouseLeave);
            }
            this.UpdateTransform();
            this.templateApllied = true;
            if (this.isOpen)
            {
                AnimationManager.Play(this, "Show");
            }
        }

        private static void OnCanCloseButtonClick(object sender, CanExecuteRoutedEventArgs args)
        {
            ToolWindow toolWindow = sender as ToolWindow;
            if (toolWindow != null)
            {
                args.CanExecute = toolWindow.CanClose();
            }
        }

        private static void OnCanPaneHeaderMenuOpen(object sender, CanExecuteRoutedEventArgs args)
        {
            ToolWindow toolWindow = sender as ToolWindow;
            if (toolWindow != null)
            {
                IEnumerable<RadPaneGroup> groups = (from pane in toolWindow.SplitContainer.EnumeratePanes()
                    select pane.PaneGroup into g
                    where g.Items.Count > 0
                    select g).Distinct<RadPaneGroup>();
                args.CanExecute = groups.Count<RadPaneGroup>() == 1;
            }
        }

        protected virtual void OnClose(RadRoutedEventArgs args)
        {
            this.RaiseEvent(args);
        }

        private static void OnCloseButtonClick(object sender, ExecutedRoutedEventArgs args)
        {
            ToolWindow toolWindow = sender as ToolWindow;
            if (toolWindow != null)
            {
                toolWindow.Close();
            }
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
            IToolWindowAware oldToolWindowAware = oldContent as IToolWindowAware;
            IToolWindowAware newToolWindowAware = newContent as IToolWindowAware;
            if (oldToolWindowAware != null)
            {
                oldToolWindowAware.IsInToolWindow = false;
            }
            if (newToolWindowAware != null)
            {
                newToolWindowAware.IsInToolWindow = true;
            }
            if (newContent == null)
            {
                this.Close();
            }
            DependencyObject contentAsDO = newContent as DependencyObject;
            if (contentAsDO != null)
            {
                contentAsDO.CopyValue(this, StyleManager.ThemeProperty);
            }
            this.UpdateToolWindowHeader();
        }

        protected virtual void OnDraggingChanged(DependencyPropertyChangedEventArgs eventArgs)
        {
            if ((bool) eventArgs.NewValue)
            {
                this.OnLayoutChangeStarted();
            }
            else
            {
                this.OnLayoutChangeEnded();
            }
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            BringToFront(this);
        }

        private void OnHeaderMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point mousePosition = e.GetPosition(this.UIParent);
            if (this.GetResizeDirection(mousePosition) == ResizeDirection.None)
            {
                this.Drag(e);
            }
        }

        private static void OnHorizontalOffsetChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ToolWindow window = d as ToolWindow;
            window.GetTranslateTransform().X = window.HorizontalOffset;
        }

        private static void OnIsDraggingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ToolWindow) d).OnDraggingChanged(e);
        }

        private static void OnIsResizingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ToolWindow) d).OnResizingChanged(e);
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

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.isLoaded = true;
            openedWindows++;
            if (this.bringToFrontWhenLoaded)
            {
                BringToFront(this);
            }
        }

        private void OnLostMouseCapture(object sender, MouseEventArgs e)
        {
            this.CancelDrag();
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (this.isMouseCaptured && this.IsDragging)
            {
                e.Handled = true;
                base.ReleaseMouseCapture();
                this.isMouseCaptured = false;
                this.ClearValue(IsDraggingPropertyKey);
                this.RaiseEvent(new DragInfoEventArgs(DragCompletedEvent, e.GetPosition(null)));
            }
            base.OnMouseLeftButtonUp(e);
        }

        private static void OnPaneHeaderMenuOpen(object sender, ExecutedRoutedEventArgs e)
        {
            ToolWindow toolWindow = sender as ToolWindow;
            if (toolWindow != null)
            {
                IEnumerable<RadPaneGroup> groups = (from pane in toolWindow.SplitContainer.EnumeratePanes()
                    select pane.PaneGroup into g
                    where g.Items.Count > 0
                    select g).Distinct<RadPaneGroup>();
                if (groups.Count<RadPaneGroup>() == 1)
                {
                    PaneGroupBase.ShowContextMenu(groups.First<RadPaneGroup>(), e);
                }
            }
        }

        private static void OnPaneShown(object sender, StateChangeCommandEventArgs e)
        {
            ToolWindow window = sender as ToolWindow;
            if (((window != null) && !e.Canceled) && !window.isOpen)
            {
                window.isOpen = true;
                window.Visibility = Visibility.Visible;
            }
        }

        protected virtual bool OnPreviewClose(RadRoutedEventArgs args)
        {
            this.RaiseEvent(args);
            return !args.Handled;
        }

        protected virtual void OnResizingChanged(DependencyPropertyChangedEventArgs eventArgs)
        {
            if ((bool) eventArgs.NewValue)
            {
                this.OnLayoutChangeStarted();
            }
            else
            {
                this.OnLayoutChangeEnded();
            }
        }

        private void OnToolWindowMouseMove(object sender, MouseEventArgs e)
        {
            if (this.IsDragging)
            {
                Point position = new Point();
                e.GetPosition(null);
                if (base.Parent != null)
                {
                    position = e.GetPosition(this.UIParent);
                }
                else
                {
                    this.CancelDrag();
                }
                if (position != this.draggingStartPoint)
                {
                    this.DragDelta(e);
                    this.HorizontalOffset = position.X - this.draggingStartPoint.X;
                    this.VerticalOffset = position.Y - this.draggingStartPoint.Y;
                }
            }
        }

        private static void OnVerticalOffsetChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ToolWindow window = d as ToolWindow;
            window.GetTranslateTransform().Y = window.VerticalOffset;
        }

        private void OnVisualRootMouseDown(object sender, MouseButtonEventArgs e)
        {
            Point mousePosition = e.GetPosition(this.UIParent);
            if ((this.GetResizeDirection(mousePosition) != ResizeDirection.None) && this.visualRoot.CaptureMouse())
            {
                Point offset = base.TransformToVisual(this.UIParent).Transform(new Point(0.0, 0.0));
                this.IsResizing = true;
                this.draggingStartPoint = mousePosition;
                this.resizeDirection = this.GetResizeDirection(mousePosition);
                this.initialRect = new Rect(offset.X, offset.Y, base.ActualWidth, base.ActualHeight);
                this.UpdateMouseCursor(this.resizeDirection);
            }
        }

        private void OnVisualRootMouseLeave(object sender, MouseEventArgs e)
        {
            if (!this.IsResizing)
            {
                this.UpdateMouseCursor(ResizeDirection.None);
            }
        }

        private void OnVisualRootMouseMove(object sender, MouseEventArgs e)
        {
            if (this.isOpen)
            {
                if (!this.IsResizing)
                {
                    this.UpdateMouseCursor(this.GetResizeDirection(e.GetPosition(this.UIParent)));
                }
                else
                {
                    double newWidth;
                    double newHeight;
                    double minWidth = double.IsNaN(base.MinWidth) ? 0.0 : base.MinWidth;
                    double minHeight = double.IsNaN(base.MinHeight) ? 0.0 : base.MinHeight;
                    double maxWidth = double.IsNaN(base.MaxWidth) ? double.PositiveInfinity : base.MaxWidth;
                    double maxHeight = double.IsNaN(base.MaxHeight) ? double.PositiveInfinity : base.MaxHeight;
                    Point thisStartPoint = new Point(0.0, 0.0);
                    switch (this.resizeDirection)
                    {
                        case ResizeDirection.Left:
                            newWidth = Math.Min(maxWidth, Math.Max(minWidth, (this.initialRect.Width + this.draggingStartPoint.X) - e.GetPosition(this.UIParent).X));
                            this.HorizontalOffset = (this.initialRect.Right - newWidth) - thisStartPoint.X;
                            base.Width = newWidth;
                            return;

                        case ResizeDirection.TopLeft:
                            newWidth = Math.Min(maxWidth, Math.Max(minWidth, (this.initialRect.Width + this.draggingStartPoint.X) - e.GetPosition(this.UIParent).X));
                            this.HorizontalOffset = (this.initialRect.Right - newWidth) - thisStartPoint.X;
                            base.Width = newWidth;
                            newHeight = Math.Min(maxHeight, Math.Max(minHeight, (this.initialRect.Height + this.draggingStartPoint.Y) - e.GetPosition(this.UIParent).Y));
                            this.VerticalOffset = (this.initialRect.Bottom - newHeight) - thisStartPoint.Y;
                            base.Height = newHeight;
                            return;

                        case ResizeDirection.Top:
                            newHeight = Math.Min(maxHeight, Math.Max(minHeight, (this.initialRect.Height + this.draggingStartPoint.Y) - e.GetPosition(this.UIParent).Y));
                            this.VerticalOffset = (this.initialRect.Bottom - newHeight) - thisStartPoint.Y;
                            base.Height = newHeight;
                            return;

                        case ResizeDirection.TopRight:
                            newHeight = Math.Min(maxHeight, Math.Max(minHeight, (this.initialRect.Height + this.draggingStartPoint.Y) - e.GetPosition(this.UIParent).Y));
                            this.VerticalOffset = (this.initialRect.Bottom - newHeight) - thisStartPoint.Y;
                            base.Height = newHeight;
                            base.Width = Math.Min(maxWidth, Math.Max(minWidth, (this.initialRect.Width + e.GetPosition(this.UIParent).X) - this.draggingStartPoint.X));
                            return;

                        case ResizeDirection.Right:
                            base.Width = Math.Min(maxWidth, Math.Max(minWidth, (this.initialRect.Width + e.GetPosition(this.UIParent).X) - this.draggingStartPoint.X));
                            return;

                        case ResizeDirection.BottomRight:
                            base.Height = Math.Min(maxHeight, Math.Max(minHeight, (this.initialRect.Height + e.GetPosition(this.UIParent).Y) - this.draggingStartPoint.Y));
                            base.Width = Math.Min(maxWidth, Math.Max(minWidth, (this.initialRect.Width + e.GetPosition(this.UIParent).X) - this.draggingStartPoint.X));
                            return;

                        case ResizeDirection.Bottom:
                            base.Height = Math.Min(maxHeight, Math.Max(minHeight, (this.initialRect.Height + e.GetPosition(this.UIParent).Y) - this.draggingStartPoint.Y));
                            return;

                        case ResizeDirection.BottomLeft:
                            newWidth = Math.Min(maxWidth, Math.Max(minWidth, (this.initialRect.Width + this.draggingStartPoint.X) - e.GetPosition(this.UIParent).X));
                            this.HorizontalOffset = (this.initialRect.Right - newWidth) - thisStartPoint.X;
                            base.Width = newWidth;
                            base.Height = Math.Min(maxHeight, Math.Max(minHeight, (this.initialRect.Height + e.GetPosition(this.UIParent).Y) - this.draggingStartPoint.Y));
                            return;

                        case ResizeDirection.None:
                            return;
                    }
                }
            }
        }

        private void OnVisualRootMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (this.IsResizing)
            {
                this.IsResizing = false;
                this.visualRoot.ReleaseMouseCapture();
                this.UpdateMouseCursor(ResizeDirection.None);
            }
        }

        internal void Open()
        {
            this.isOpen = true;
            base.Visibility = Visibility.Visible;
            if (this.templateApllied)
            {
                AnimationManager.Play(this, "Show");
            }
        }

        public void ResetTheme()
        {
            DependencyObject contentAsDO = base.Content as DependencyObject;
            if (contentAsDO != null)
            {
                contentAsDO.CopyValue(this, StyleManager.ThemeProperty);
            }
        }

        internal void SetBringToFrontWhenLoaded(bool value)
        {
            this.bringToFrontWhenLoaded = value;
        }

        internal bool StartDrag(MouseEventArgs e)
        {
            return this.Drag(e);
        }

        private void UpdateMouseCursor(ResizeDirection direction)
        {
            switch (direction)
            {
                case ResizeDirection.Left:
                case ResizeDirection.Right:
                    base.Cursor = Cursors.SizeWE;
                    return;

                case ResizeDirection.TopLeft:
                case ResizeDirection.BottomRight:
                    base.Cursor = Cursors.SizeNWSE;
                    return;

                case ResizeDirection.Top:
                case ResizeDirection.Bottom:
                    base.Cursor = Cursors.SizeNS;
                    return;

                case ResizeDirection.TopRight:
                case ResizeDirection.BottomLeft:
                    base.Cursor = Cursors.SizeNESW;
                    return;
            }
            base.Cursor = Cursors.Arrow;
        }

        internal void UpdateToolWindowHeader()
        {
            if (this.SplitContainer != null)
            {
                IEnumerable<RadPaneGroup> groups = (from pane in this.SplitContainer.EnumeratePanes()
                    select pane.PaneGroup into pg
                    where pg.Visibility == Visibility.Visible
                    select pg).Distinct<RadPaneGroup>();
                if (groups.Count<RadPaneGroup>() == 1)
                {
                    RadPaneGroup group = groups.First<RadPaneGroup>();
                    group.IsPaneHeaderVisible = false;
                    base.SetBinding(HeaderedContentControl.HeaderProperty, new Binding("SelectedPane.Title") { Source = group });
                    base.SetBinding(HeaderedContentControl.HeaderTemplateProperty, new Binding("SelectedPane.TitleTemplate") { Source = group });
                }
                else
                {
                    foreach (RadPaneGroup paneGroup in groups)
                    {
                        paneGroup.IsPaneHeaderVisible = true;
                    }
                    base.ClearValue(HeaderedContentControl.HeaderProperty);
                    base.ClearValue(HeaderedContentControl.HeaderTemplateProperty);
                }
            }
        }

        private void UpdateTransform()
        {
            if (this.dragTransform != null)
            {
                this.dragTransform.X = this.HorizontalOffset;
                this.dragTransform.Y = this.VerticalOffset;
            }
        }

        internal bool CanDockInDockumentHost
        {
            get
            {
                if (this.SplitContainer == null)
                {
                    return false;
                }
                return this.SplitContainer.EnumeratePanes().All<RadPane>(pane => pane.CanDockInDocumentHost);
            }
        }

        public double HorizontalOffset
        {
            get
            {
                return (double) base.GetValue(HorizontalOffsetProperty);
            }
            set
            {
                base.SetValue(HorizontalOffsetProperty, value);
            }
        }

        [Browsable(false), Category("Appearance")]
        public bool IsDragging
        {
            get
            {
                return (bool) base.GetValue(IsDraggingProperty);
            }
            protected set
            {
                this.SetValue(IsDraggingPropertyKey, value);
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

        [Browsable(false), Category("Appearance")]
        public bool IsResizing
        {
            get
            {
                return (bool) base.GetValue(IsResizingProperty);
            }
            protected set
            {
                this.SetValue(IsResizingPropertyKey, value);
            }
        }

        internal bool IsToolWindowLoaded
        {
            get
            {
                return this.isLoaded;
            }
        }

        internal RadSplitContainer SplitContainer
        {
            get
            {
                return (base.Content as RadSplitContainer);
            }
        }

        bool IWindow.IsOpen
        {
            get
            {
                return this.isOpen;
            }
        }

        int IWindow.Z
        {
            get
            {
                return Canvas.GetZIndex(this);
            }
        }

        internal bool TemplateApplied
        {
            get
            {
                return this.templateApllied;
            }
        }

        private UIElement UIParent
        {
            get
            {
                UIElement uiParent = base.Parent as UIElement;
                if (uiParent != null)
                {
                    return uiParent;
                }
                return (VisualTreeHelper.GetParent(this) as UIElement);
            }
        }

        public double VerticalOffset
        {
            get
            {
                return (double) base.GetValue(VerticalOffsetProperty);
            }
            set
            {
                base.SetValue(VerticalOffsetProperty, value);
            }
        }
    }
}

