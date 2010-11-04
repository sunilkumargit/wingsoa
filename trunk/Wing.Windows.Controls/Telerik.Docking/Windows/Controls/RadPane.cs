namespace Telerik.Windows.Controls
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Input;
    using Telerik.Windows;
    using Telerik.Windows.Controls.Docking;

    [TemplateVisualState(Name="Unfocused", GroupName="FocusStates"), TemplateVisualState(Name="Unpinned", GroupName="PinnedStates"), DefaultProperty("IsSelected"), TemplateVisualState(Name="Normal", GroupName="CommonStateGroup"), TemplateVisualState(Name="Selected", GroupName="CommonStateGroup"), TemplateVisualState(Name="MouseOver", GroupName="CommonStateGroup"), TemplateVisualState(Name="Disabled", GroupName="CommonStateGroup"), TemplateVisualState(Name="Focused", GroupName="FocusStates"), TemplateVisualState(Name="LastInGroup", GroupName="LastInGroupStates"), TemplateVisualState(Name="NotLastInGroup", GroupName="LastInGroupStates"), TemplateVisualState(Name="Pinned", GroupName="PinnedStates")]
    public class RadPane : RadTabItem, IPane, IDocumentHostAware, IToolWindowAware
    {
        public static readonly DependencyProperty AutoHideHeightProperty = DependencyProperty.Register("AutoHideHeight", typeof(double), typeof(RadPane), null);
        public static readonly DependencyProperty AutoHideWidthProperty = DependencyProperty.Register("AutoHideWidth", typeof(double), typeof(RadPane), null);
        public static readonly DependencyProperty CanDockInDocumentHostProperty = DependencyProperty.Register("CanDockInDocumentHost", typeof(bool), typeof(RadPane), new Telerik.Windows.PropertyMetadata(true, new PropertyChangedCallback(RadPane.OnCanDockInDocumentHostChanged)));
        public static readonly DependencyProperty CanFloatProperty = DependencyProperty.Register("CanFloat", typeof(bool), typeof(RadPane), new Telerik.Windows.PropertyMetadata(true, new PropertyChangedCallback(RadPane.OnCanFloatChanged)));
        public static readonly DependencyProperty CanUserCloseProperty = DependencyProperty.RegisterAttached("CanUserClose", typeof(bool), typeof(RadPane), new Telerik.Windows.PropertyMetadata(true, new PropertyChangedCallback(RadPane.OnCanUserCloseChanged)));
        public static readonly DependencyProperty CanUserPinProperty = DependencyProperty.RegisterAttached("CanUserPin", typeof(bool), typeof(RadPane), new Telerik.Windows.PropertyMetadata(true, new PropertyChangedCallback(RadPane.OnCanUserPinChanged)));
        internal static readonly Telerik.Windows.RoutedEvent CloseEvent = EventManager.RegisterRoutedEvent("Close", RoutingStrategy.Tunnel, typeof(EventHandler<StateChangeCommandEventArgs>), typeof(RadPane));
        public static readonly DependencyProperty ContextMenuTemplateProperty = DependencyProperty.Register("ContextMenuTemplate", typeof(DataTemplate), typeof(RadPane), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadPane.OnContextMenuTemplateChanged)));
        public static readonly DependencyProperty DocumentHostTemplateProperty = DependencyProperty.Register("DocumentHostTemplate", typeof(ControlTemplate), typeof(RadPane), new Telerik.Windows.PropertyMetadata(null, new PropertyChangedCallback(RadPane.OnDocumentHostTemplateChanged)));
        internal static readonly Telerik.Windows.RoutedEvent DragCompletedEvent = EventManager.RegisterRoutedEvent("DragCompleted", RoutingStrategy.Bubble, typeof(DragInfoEventHandler), typeof(RadPane));
        internal static readonly Telerik.Windows.RoutedEvent DragDeltaEvent = EventManager.RegisterRoutedEvent("DragDelta", RoutingStrategy.Bubble, typeof(DragInfoEventHandler), typeof(RadPane));
        internal static readonly Telerik.Windows.RoutedEvent DragStartedEvent = EventManager.RegisterRoutedEvent("DragStarted", RoutingStrategy.Bubble, typeof(DragInfoEventHandler), typeof(RadPane));
        public static readonly DependencyProperty IsDockableOptionCheckedProperty;
        private static readonly DependencyPropertyKey IsDockableOptionCheckedPropertyKey = DependencyPropertyExtensions.RegisterReadOnly("IsDockableOptionChecked", typeof(bool), typeof(RadPane), null);
        public static readonly DependencyProperty IsDockableProperty;
        private static readonly DependencyPropertyKey IsDockablePropertyKey = DependencyPropertyExtensions.RegisterReadOnly("IsDockable", typeof(bool), typeof(RadPane), new Telerik.Windows.PropertyMetadata(true, new PropertyChangedCallback(RadPane.OnIsDockableChange)));
        public static readonly DependencyProperty IsDraggingProperty;
        private static readonly DependencyPropertyKey IsDraggingPropertyKey = DependencyPropertyExtensions.RegisterReadOnly("IsDragging", typeof(bool), typeof(RadPane), new Telerik.Windows.PropertyMetadata(false, new PropertyChangedCallback(RadPane.OnIsDraggingPropertyChanged)));
        public static readonly DependencyProperty IsFloatingOnlyProperty;
        private static readonly DependencyPropertyKey IsFloatingOnlyPropertyKey = DependencyPropertyExtensions.RegisterReadOnly("IsFloatingOnly", typeof(bool), typeof(RadPane), null);
        public static readonly DependencyProperty IsFloatingProperty;
        private static readonly DependencyPropertyKey IsFloatingPropertyKey = DependencyPropertyExtensions.RegisterReadOnly("IsFloating", typeof(bool), typeof(RadPane), new Telerik.Windows.PropertyMetadata(false, new PropertyChangedCallback(RadPane.OnIsFloatingChanged)));
        public static readonly DependencyProperty IsHiddenProperty = DependencyProperty.RegisterAttached("IsHidden", typeof(bool), typeof(RadPane), new Telerik.Windows.PropertyMetadata(false, new PropertyChangedCallback(RadPane.OnIsHiddenChanged)));
        public static readonly DependencyProperty IsInDocumentHostProperty;
        private static readonly DependencyPropertyKey IsInDocumentHostPropertyKey = DependencyPropertyExtensions.RegisterReadOnly("IsInDocumentHost", typeof(bool), typeof(RadPane), new Telerik.Windows.PropertyMetadata(false, new PropertyChangedCallback(RadPane.OnIsInDocumentHostChanged)));
        public static readonly DependencyProperty IsLastInGroupProperty;
        private static readonly DependencyPropertyKey IsLastInGroupPropertyKey = DependencyPropertyExtensions.RegisterReadOnly("IsLastInGroup", typeof(bool), typeof(RadPane), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadPane.OnIsLastInGroupChange)));
        private bool isMouseCaptured;
        public static readonly DependencyProperty IsPinnedProperty = DependencyProperty.RegisterAttached("IsPinned", typeof(bool), typeof(RadPane), new Telerik.Windows.PropertyMetadata(true, new PropertyChangedCallback(RadPane.OnIsPinnedChanged)));
        public static readonly DependencyProperty MenuCommandsProperty;
        public static readonly DependencyProperty MenuItemTemplateProperty = DependencyProperty.Register("MenuItemTemplate", typeof(DataTemplate), typeof(RadPane), null);
        public static readonly DependencyProperty MenuItemTemplateSelectorProperty = DependencyProperty.Register("MenuItemTemplateSelector", typeof(DataTemplateSelector), typeof(RadPane), null);
        internal static readonly Telerik.Windows.RoutedEvent MoveToDocumentHostEvent = EventManager.RegisterRoutedEvent("MoveToDocumentHost", RoutingStrategy.Tunnel, typeof(EventHandler<RadRoutedEventArgs>), typeof(RadPane));
        internal static readonly Telerik.Windows.RoutedEvent MoveToToolWindowEvent = EventManager.RegisterRoutedEvent("MoveToToolWindow", RoutingStrategy.Tunnel, typeof(EventHandler<RadRoutedEventArgs>), typeof(RadPane));
        private WeakReference paneGroup;
        private ContentControl paneHeader;
        public static readonly DependencyProperty PaneHeaderVisibilityProperty = DependencyProperty.Register("PaneHeaderVisibility", typeof(Visibility), typeof(RadPane), new Telerik.Windows.PropertyMetadata(Visibility.Visible));
        public static readonly DependencyProperty PinButtonVisibilityProperty;
        private static readonly DependencyPropertyKey PinButtonVisibilityPropertyKey = DependencyPropertyExtensions.RegisterReadOnly("PinButtonVisibility", typeof(Visibility), typeof(RadPane), null);
        internal static readonly Telerik.Windows.RoutedEvent PinChangeEvent = EventManager.RegisterRoutedEvent("PinChange", RoutingStrategy.Tunnel, typeof(EventHandler<PinChangeEventArgs>), typeof(RadPane));
        internal static readonly Telerik.Windows.RoutedEvent ShowEvent = EventManager.RegisterRoutedEvent("Show", RoutingStrategy.Tunnel, typeof(EventHandler<StateChangeCommandEventArgs>), typeof(RadPane));
        public static readonly Telerik.Windows.RoutedEvent StateChangeEvent = EventManager.RegisterRoutedEvent("StateChange", RoutingStrategy.Bubble, typeof(EventHandler<RadRoutedEventArgs>), typeof(RadPane));
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(RadPane), null);
        public static readonly DependencyProperty TitleTemplateProperty = DependencyProperty.Register("TitleTemplate", typeof(DataTemplate), typeof(RadPane), null);

        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static RadPane()
        {
            RadDockingCommands.EnsureCommandsClassLoaded();
            IsDraggingProperty = IsDraggingPropertyKey.DependencyProperty;
            IsLastInGroupProperty = IsLastInGroupPropertyKey.DependencyProperty;
            IsInDocumentHostProperty = IsInDocumentHostPropertyKey.DependencyProperty;
            IsFloatingProperty = IsFloatingPropertyKey.DependencyProperty;
            IsDockableOptionCheckedProperty = IsDockableOptionCheckedPropertyKey.DependencyProperty;
            IsDockableProperty = IsDockablePropertyKey.DependencyProperty;
            IsFloatingOnlyProperty = IsFloatingOnlyPropertyKey.DependencyProperty;
            PinButtonVisibilityProperty = PinButtonVisibilityPropertyKey.DependencyProperty;
        }

        public RadPane()
        {
            base.DefaultStyleKey = typeof(RadPane);
            this.UpdateMenuCommands();
            base.Loaded += new RoutedEventHandler(this.OnPaneLoaded);
            base.LostMouseCapture += new MouseEventHandler(this.OnLostMouseCapture);
            this.IsDockable = true;
        }

        private void Activate()
        {
            this.Activate(false);
        }

        private void Activate(bool isMouseEnter)
        {
            AutoHideArea autoHideArea = base.Parent as AutoHideArea;
            if (!this.IsPinned && (autoHideArea != null))
            {
                if (isMouseEnter)
                {
                    autoHideArea.IsMouseInsidePane = true;
                }
                autoHideArea.NotifyMouseEnter(this);
            }
        }

        internal void AddAsUnpinned()
        {
            this.OnPinChange(new PinChangeEventArgs(PinChangeEvent, this, false, false, true));
        }

        internal void CancelDrag()
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

        private bool ChangePin(bool isPinned)
        {
            bool res = this.OnPinChange(new PinChangeEventArgs(PinChangeEvent, this, isPinned, false, false));
            if (res)
            {
                this.OnStateChange();
            }
            return res;
        }

        protected internal override void ChangeVisualState(bool useTransitions)
        {
            base.ChangeVisualState(useTransitions);
            if (this.IsPinned)
            {
                this.GoToState(useTransitions, new string[] { "Pinned", "Unpinned" });
            }
            else
            {
                this.GoToState(useTransitions, new string[] { "Unpinned" });
            }
            if (this.IsLastInGroup)
            {
                this.GoToState(useTransitions, new string[] { "LastInGroup" });
            }
            else
            {
                this.GoToState(useTransitions, new string[] { "NotLastInGroup" });
            }
        }

        private bool Close()
        {
            bool res = this.OnClose(new StateChangeCommandEventArgs(CloseEvent, this, false));
            if (res)
            {
                this.OnStateChange();
            }
            return res;
        }

        private void Deactivate()
        {
            this.Deactivate(false);
        }

        private void Deactivate(bool isMouseLeave)
        {
            AutoHideArea autoHideArea = base.Parent as AutoHideArea;
            if (!this.IsPinned && (autoHideArea != null))
            {
                if (isMouseLeave)
                {
                    autoHideArea.IsMouseInsidePane = false;
                }
                autoHideArea.NotifyMouseLeave();
            }
        }

        protected override ControlTemplate FindTemplateFromPosition(Dock position)
        {
            if (this.IsInDocumentHost)
            {
                return this.DocumentHostTemplate;
            }
            return base.FindTemplateFromPosition(position);
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

        public void MakeDockable()
        {
            if (this.IsInDocumentHost)
            {
                this.OnMoveToToolWindow(new RadRoutedEventArgs(MoveToToolWindowEvent, this));
            }
            this.IsDockable = true;
            this.UpdateMenuCommands();
        }

        public void MakeFloatingDockable()
        {
            if (!this.IsPinned)
            {
                throw new InvalidOperationException("Cannot make floating pane that is not pinned!");
            }
            if (this.IsFloating)
            {
                this.IsDockable = true;
                this.UpdateMenuCommands();
            }
            else
            {
                this.OnMoveToToolWindow(new RadRoutedEventArgs(MoveToToolWindowEvent, this));
                this.IsDockable = true;
                this.UpdateMenuCommands();
            }
        }

        public void MakeFloatingOnly()
        {
            if (!this.IsPinned)
            {
                throw new InvalidOperationException("Cannot make floating pane that is not pinned!");
            }
            if (this.IsDockable || !this.IsFloating)
            {
                this.OnMoveToToolWindow(new RadRoutedEventArgs(MoveToToolWindowEvent, this));
                this.IsDockable = false;
                this.IsFloating = true;
                this.UpdateMenuCommands();
            }
        }

        public void MoveToDocumentHost()
        {
            if (!this.IsInDocumentHost)
            {
                if (!this.IsPinned)
                {
                    throw new InvalidOperationException("Cannot move unpinned pane to dockument host!");
                }
                this.IsFloating = false;
                this.OnMoveToDocumentHost(new RadRoutedEventArgs(MoveToDocumentHostEvent, this));
                this.UpdateMenuCommands();
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (this.paneHeader != null)
            {
                this.paneHeader.ClearValue(ContentControl.ContentProperty);
                this.paneHeader.ClearValue(ContentControl.ContentTemplateProperty);
            }
            this.paneHeader = base.GetTemplateChild("HeaderElement") as ContentControl;
        }

        private static void OnCanDockInDocumentHostChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadPane pane = d as RadPane;
            if (pane != null)
            {
                pane.UpdateMenuCommands();
            }
        }

        private static void OnCanFloatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadPane pane = d as RadPane;
            if (pane != null)
            {
                pane.UpdateMenuCommands();
            }
        }

        private static void OnCanUserCloseChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadPane pane = d as RadPane;
            if (pane != null)
            {
                pane.UpdateMenuCommands();
            }
        }

        private static void OnCanUserPinChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadPane pane = d as RadPane;
            if (pane != null)
            {
                pane.UpdateMenuCommands();
            }
        }

        private bool OnClose(StateChangeCommandEventArgs args)
        {
            this.RaiseEvent(args);
            return !args.Canceled;
        }

        protected virtual void OnContextMenuTemplateChanged()
        {
            if (this.ContextMenuTemplate != null)
            {
                RadContextMenu contextMenu = this.ContextMenuTemplate.LoadContent() as RadContextMenu;
                if (contextMenu == null)
                {
                    throw new InvalidOperationException("ContextMenuTemplate should contain a RadContextMenu as root element.");
                }
                contextMenu.DataContext = this;
                RadContextMenu.SetContextMenu(this, contextMenu);
            }
            else
            {
                base.ClearValue(RadContextMenu.ContextMenuProperty);
            }
        }

        private static void OnContextMenuTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            new Canvas { DataContext = args.NewValue };
            ((RadPane) d).OnContextMenuTemplateChanged();
        }

        private static void OnDocumentHostTemplateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            RadPane pane = sender as RadPane;
            if (pane.IsInDocumentHost)
            {
                pane.UpdateTemplate();
                pane.UpdateLayout();
            }
        }

        protected virtual void OnDraggingChanged(DependencyPropertyChangedEventArgs eventArgs)
        {
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            this.Activate();
        }

        protected override void OnHeaderMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.OnHeaderMouseLeftButtonDown(sender, e);
            if ((!this.IsDragging && this.IsPinned) && this.CanFloat)
            {
                e.Handled = true;
                base.Focus();
                this.isMouseCaptured = base.CaptureMouse();
                this.SetValue(IsDraggingPropertyKey, this.isMouseCaptured);
                if (this.isMouseCaptured)
                {
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

        private static void OnIsDockableChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RadPane) d).UpdateMenuCommands();
        }

        private static void OnIsDraggingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RadPane) d).OnDraggingChanged(e);
        }

        private static void OnIsFloatingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadPane pane = (RadPane) d;
            pane.UpdateMenuCommands();
            pane.OnStateChange();
        }

        private static void OnIsHiddenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadPane pane = d as RadPane;
            if (pane != null)
            {
                if (!pane.SupressCloseEvents)
                {
                    pane.SupressCloseEvents = true;
                    if (pane.IsHidden)
                    {
                        if (!pane.Close())
                        {
                            pane.IsHidden = false;
                        }
                    }
                    else if (!pane.Show())
                    {
                        pane.IsHidden = true;
                    }
                    pane.SupressCloseEvents = false;
                }
                pane.UpdateMenuCommands();
            }
        }

        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId="oldValue"), SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId="newValue")]
        private void OnIsInDocumentHostChanged(bool oldValue, bool newValue)
        {
            base.UpdateTemplate();
            this.UpdateMenuCommands();
            this.OnStateChange();
        }

        private static void OnIsInDocumentHostChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            RadPane pane = sender as RadPane;
            bool oldValue = (bool) e.OldValue;
            bool newValue = (bool) e.NewValue;
            if (pane != null)
            {
                pane.OnIsInDocumentHostChanged(oldValue, newValue);
                pane.UpdateMenuCommands();
            }
        }

        private static void OnIsLastInGroupChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadPane pane = d as RadPane;
            if (pane != null)
            {
                pane.ChangeVisualState();
            }
        }

        private static void OnIsPinnedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadPane pane = d as RadPane;
            if (pane != null)
            {
                pane.IsDockable = (bool) e.NewValue;
                if (!((bool) e.NewValue) && (pane.IsFloating || pane.IsInDocumentHost))
                {
                    pane.SupressPinEvents = true;
                    pane.IsPinned = (bool) e.OldValue;
                    pane.SupressPinEvents = false;
                    throw new InvalidOperationException("Cannot unpin pane that is floating or in document host!");
                }
                bool isCanceled = false;
                if (!pane.SupressPinEvents)
                {
                    pane.SupressPinEvents = true;
                    if (!pane.ChangePin(pane.IsPinned))
                    {
                        pane.IsPinned = (bool) e.OldValue;
                        isCanceled = true;
                    }
                    pane.SupressPinEvents = false;
                }
                if (!isCanceled && pane.IsPinned)
                {
                    pane.IsLastInGroup = false;
                }
                pane.UpdateMenuCommands();
            }
            pane.ChangeVisualState();
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            this.Deactivate();
        }

        private void OnLostMouseCapture(object sender, MouseEventArgs e)
        {
            this.CancelDrag();
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            this.Activate(true);
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            this.Deactivate(true);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (this.isMouseCaptured && this.IsDragging)
            {
                e.Handled = true;
                base.ReleaseMouseCapture();
                this.isMouseCaptured = false;
                this.ClearValue(IsDraggingPropertyKey);
                this.EndDrag(e);
            }
            base.OnMouseLeftButtonUp(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (this.IsDragging)
            {
                this.DragDelta(e);
            }
        }

        private void OnMoveToDocumentHost(RadRoutedEventArgs args)
        {
            this.RaiseEvent(args);
        }

        private void OnMoveToToolWindow(RadRoutedEventArgs args)
        {
            this.RaiseEvent(args);
        }

        private void OnPaneLoaded(object sender, RoutedEventArgs e)
        {
            base.Loaded -= new RoutedEventHandler(this.OnPaneLoaded);
            if (string.IsNullOrEmpty(this.Title) && (base.ReadLocalValue(TitleProperty) == DependencyProperty.UnsetValue))
            {
                Binding titleBinding = new Binding("Header") {
                    Source = this
                };
                base.SetBinding(TitleProperty, titleBinding);
            }
            else if ((base.Header == null) && (base.ReadLocalValue(HeaderedContentControl.HeaderProperty) == DependencyProperty.UnsetValue))
            {
                Binding headerBinding = new Binding("Title") {
                    Source = this
                };
                base.SetBinding(HeaderedContentControl.HeaderProperty, headerBinding);
            }
        }

        private bool OnPinChange(PinChangeEventArgs args)
        {
            this.RaiseEvent(args);
            return !args.Canceled;
        }

        private bool OnShow(StateChangeCommandEventArgs args)
        {
            if (this.PaneGroup != null)
            {
                this.PaneGroup.RaiseEvent(args);
            }
            else
            {
                this.RaiseEvent(args);
            }
            return !args.Canceled;
        }

        private void OnStateChange()
        {
            this.OnStateChange(new RadRoutedEventArgs(StateChangeEvent, this));
        }

        protected virtual void OnStateChange(RadRoutedEventArgs routedEventArgs)
        {
            this.RaiseEvent(routedEventArgs);
        }

        public void RemoveFromParent()
        {
            if (this.PaneGroup == null)
            {
                throw new ArgumentException("This pane cannot be removed from parent, because it doesn't have one!");
            }
            base.ClearValue(ProportionalStackPanel.SplitterChangeProperty);
            this.PaneGroup.RemovePane(this);
        }

        private bool Show()
        {
            bool res = this.OnShow(new StateChangeCommandEventArgs(ShowEvent, this, false));
            if (res)
            {
                this.OnStateChange();
            }
            return res;
        }

        private void UpdateMenuCommands()
        {
            this.IsDockableOptionChecked = this.IsDockable && !this.IsInDocumentHost;
            this.IsFloatingOnly = this.IsFloating && !this.IsDockable;
            this.UpdatePinButtonVisibility();
        }

        private void UpdatePinButtonVisibility()
        {
            this.PinButtonVisibility = this.CanPin ? Visibility.Visible : Visibility.Collapsed;
        }

        public double AutoHideHeight
        {
            get
            {
                return (double) base.GetValue(AutoHideHeightProperty);
            }
            set
            {
                base.SetValue(AutoHideHeightProperty, value);
            }
        }

        public double AutoHideWidth
        {
            get
            {
                return (double) base.GetValue(AutoHideWidthProperty);
            }
            set
            {
                base.SetValue(AutoHideWidthProperty, value);
            }
        }

        public bool CanDockInDocumentHost
        {
            get
            {
                return (bool) base.GetValue(CanDockInDocumentHostProperty);
            }
            set
            {
                base.SetValue(CanDockInDocumentHostProperty, value);
            }
        }

        public bool CanFloat
        {
            get
            {
                return (bool) base.GetValue(CanFloatProperty);
            }
            set
            {
                base.SetValue(CanFloatProperty, value);
            }
        }

        internal bool CanPin
        {
            get
            {
                return ((this.CanUserPin && !this.IsInDocumentHost) && !this.IsFloating);
            }
        }

        public bool CanUserClose
        {
            get
            {
                return (bool) base.GetValue(CanUserCloseProperty);
            }
            set
            {
                base.SetValue(CanUserCloseProperty, value);
            }
        }

        public bool CanUserPin
        {
            get
            {
                return (bool) base.GetValue(CanUserPinProperty);
            }
            set
            {
                base.SetValue(CanUserPinProperty, value);
            }
        }

        public DataTemplate ContextMenuTemplate
        {
            get
            {
                return (DataTemplate) base.GetValue(ContextMenuTemplateProperty);
            }
            set
            {
                base.SetValue(ContextMenuTemplateProperty, value);
            }
        }

        public ControlTemplate DocumentHostTemplate
        {
            get
            {
                return (ControlTemplate) base.GetValue(DocumentHostTemplateProperty);
            }
            set
            {
                base.SetValue(DocumentHostTemplateProperty, value);
            }
        }

        public bool IsDockable
        {
            get
            {
                return (bool) base.GetValue(IsDockableProperty);
            }
            internal set
            {
                this.SetValue(IsDockablePropertyKey, value);
            }
        }

        public bool IsDockableOptionChecked
        {
            get
            {
                return (bool) base.GetValue(IsDockableOptionCheckedProperty);
            }
            internal set
            {
                this.SetValue(IsDockableOptionCheckedPropertyKey, value);
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

        public bool IsFloating
        {
            get
            {
                return (bool) base.GetValue(IsFloatingProperty);
            }
            internal set
            {
                this.SetValue(IsFloatingPropertyKey, value);
            }
        }

        public bool IsFloatingOnly
        {
            get
            {
                return (bool) base.GetValue(IsFloatingOnlyProperty);
            }
            internal set
            {
                this.SetValue(IsFloatingOnlyPropertyKey, value);
            }
        }

        public bool IsHidden
        {
            get
            {
                return (bool) base.GetValue(IsHiddenProperty);
            }
            set
            {
                base.SetValue(IsHiddenProperty, value);
            }
        }

        public bool IsInDocumentHost
        {
            get
            {
                return ((IDocumentHostAware) this).IsInDocumentHost;
            }
        }

        public bool IsLastInGroup
        {
            get
            {
                return (bool) base.GetValue(IsLastInGroupProperty);
            }
            internal set
            {
                this.SetValue(IsLastInGroupPropertyKey, value);
            }
        }

        public bool IsPinned
        {
            get
            {
                return (bool) base.GetValue(IsPinnedProperty);
            }
            set
            {
                base.SetValue(IsPinnedProperty, value);
            }
        }

        public RadPaneGroup PaneGroup
        {
            get
            {
                if (this.paneGroup != null)
                {
                    return (RadPaneGroup) this.paneGroup.Target;
                }
                return null;
            }
            internal set
            {
                if (this.paneGroup != null)
                {
                    this.paneGroup.Target = null;
                }
                this.paneGroup = new WeakReference(value);
            }
        }

        public Visibility PaneHeaderVisibility
        {
            get
            {
                return (Visibility) base.GetValue(PaneHeaderVisibilityProperty);
            }
            set
            {
                base.SetValue(PaneHeaderVisibilityProperty, value);
            }
        }

        public Visibility PinButtonVisibility
        {
            get
            {
                return (Visibility) base.GetValue(PinButtonVisibilityProperty);
            }
            private set
            {
                this.SetValue(PinButtonVisibilityPropertyKey, value);
            }
        }

        private bool SupressCloseEvents
        {
            get
            {
                if (this.PaneGroup == null)
                {
                    return false;
                }
                return this.PaneGroup.Flags.SupressCloseEvents;
            }
            set
            {
                if (this.PaneGroup != null)
                {
                    this.PaneGroup.Flags.SupressCloseEvents = value;
                }
            }
        }

        private bool SupressPinEvents
        {
            get
            {
                if (this.PaneGroup == null)
                {
                    return false;
                }
                return this.PaneGroup.Flags.SupressPinEvents;
            }
            set
            {
                if (this.PaneGroup != null)
                {
                    this.PaneGroup.Flags.SupressPinEvents = value;
                }
            }
        }

        bool IDocumentHostAware.IsInDocumentHost
        {
            get
            {
                return (bool) base.GetValue(IsInDocumentHostProperty);
            }
            set
            {
                this.SetValue(IsInDocumentHostPropertyKey, value);
            }
        }

        bool IToolWindowAware.IsInToolWindow
        {
            get
            {
                return this.IsFloating;
            }
            set
            {
                if (this.IsFloating != value)
                {
                    this.IsFloating = value;
                    this.UpdateMenuCommands();
                }
            }
        }

        public string Title
        {
            get
            {
                return (string) base.GetValue(TitleProperty);
            }
            set
            {
                base.SetValue(TitleProperty, value);
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

        internal class InternalTestHook
        {
            internal InternalTestHook(RadPane pane)
            {
                this.Pane = pane;
            }

            internal void SetIsFloating(bool newValue)
            {
                this.Pane.IsFloating = newValue;
            }

            internal void SetIsInTabDocument(bool newValue)
            {
                ((IDocumentHostAware) this.Pane).IsInDocumentHost = newValue;
            }

            internal RadPane Pane { get; private set; }
        }
    }
}

