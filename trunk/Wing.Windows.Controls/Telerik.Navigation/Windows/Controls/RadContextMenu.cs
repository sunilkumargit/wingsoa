namespace Telerik.Windows.Controls
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Automation.Peers;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Media;
    using Telerik.Windows;
    using Telerik.Windows.Input;

    [DefaultProperty("IsOpen"), StyleTypedProperty(Property="ItemContainerStyle", StyleTargetType=typeof(RadMenuItem)), DefaultEvent("Opened")]
    public class RadContextMenu : MenuBase
    {
        public static readonly DependencyProperty AttachOnHandledEventsProperty = DependencyProperty.Register("AttachOnHandledEvents", typeof(bool), typeof(RadContextMenu), new Telerik.Windows.PropertyMetadata(false, new PropertyChangedCallback(RadContextMenu.OnAttachOnHandledEventsChanged)));
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        public static readonly Telerik.Windows.RoutedEvent ClosedEvent = EventManager.RegisterRoutedEvent("Closed", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(RadContextMenu));
        public static readonly DependencyProperty ContextMenuProperty = DependencyProperty.RegisterAttached("ContextMenu", typeof(FrameworkElement), typeof(RadContextMenu), new System.Windows.PropertyMetadata(new PropertyChangedCallback(RadContextMenu.OnContextMenuChanged)));
        private FrameworkElement elementWithContextMenu;
        private Delegate emittedHandler;
        public static readonly DependencyProperty EventNameProperty = DependencyProperty.Register("EventName", typeof(string), typeof(RadContextMenu), new System.Windows.PropertyMetadata("MouseRightButtonUp", new PropertyChangedCallback(RadContextMenu.OnEventNameChanged)));
        public static readonly DependencyProperty HorizontalOffsetProperty = DependencyProperty.Register("HorizontalOffset", typeof(double), typeof(RadContextMenu), null);
        public static readonly DependencyProperty InheritDataContextProperty = DependencyProperty.Register("InheritDataContext", typeof(bool), typeof(RadContextMenu), new Telerik.Windows.PropertyMetadata(true));
        internal static readonly DependencyProperty InsideContextMenuProperty = DependencyProperty.RegisterAttached("InsideContextMenu", typeof(bool), typeof(RadContextMenu), null);
        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register("IsOpen", typeof(bool), typeof(RadContextMenu), new System.Windows.PropertyMetadata(false, new PropertyChangedCallback(RadContextMenu.OnIsOpenChanged)));
        private static Telerik.Windows.Controls.PlacementMode menuPlacementDefault = Telerik.Windows.Controls.PlacementMode.Absolute;
        private static MethodInfo methodInfo = typeof(RadContextMenu).GetMethod("OnEventTriggered", BindingFlags.NonPublic | BindingFlags.Static);
        public static readonly DependencyProperty ModifierKeyProperty = DependencyProperty.Register("ModifierKey", typeof(ModifierKeys), typeof(RadContextMenu), new Telerik.Windows.PropertyMetadata(ModifierKeys.None));
        private Point? mousePosition;
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        public static readonly Telerik.Windows.RoutedEvent OpenedEvent = EventManager.RegisterRoutedEvent("Opened", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(RadContextMenu));
        public static readonly DependencyProperty PlacementProperty = DependencyProperty.Register("Placement", typeof(Telerik.Windows.Controls.PlacementMode), typeof(RadContextMenu), new System.Windows.PropertyMetadata(menuPlacementDefault));
        public static readonly DependencyProperty PlacementRectangleProperty = DependencyProperty.Register("PlacementRectangle", typeof(Rect), typeof(RadContextMenu), new System.Windows.PropertyMetadata(Rect.Empty));
        public static readonly DependencyProperty PlacementTargetProperty = DependencyProperty.Register("PlacementTarget", typeof(System.Windows.UIElement), typeof(RadContextMenu), null);
        private PopupWrapper popupWrapper;
        private Popup rootPopup;
        private Telerik.Windows.RoutedEvent routedEvent;
        public static readonly DependencyProperty StaysOpenProperty = DependencyProperty.Register("StaysOpen", typeof(bool), typeof(RadContextMenu), null);
        private EventInfo trigger;
        public static readonly DependencyProperty VerticalOffsetProperty = DependencyProperty.Register("VerticalOffset", typeof(double), typeof(RadContextMenu), null);

        public event RoutedEventHandler Closed
        {
            add
            {
                this.AddHandler(ClosedEvent, value);
            }
            remove
            {
                this.RemoveHandler(ClosedEvent, value);
            }
        }

        public event RoutedEventHandler Opened
        {
            add
            {
                this.AddHandler(OpenedEvent, value);
            }
            remove
            {
                this.RemoveHandler(OpenedEvent, value);
            }
        }

        public RadContextMenu()
        {
            
            base.DefaultStyleKey = typeof(RadContextMenu);
            base.TabNavigation = KeyboardNavigationMode.Cycle;
            base.ClickToOpen = true;
        }

        private void AddKeyDownHandler()
        {
            if (this.elementWithContextMenu != null)
            {
                this.elementWithContextMenu.AddHandler(System.Windows.UIElement.KeyDownEvent, new KeyEventHandler(this.OnElementWithContextMenuKeyDown), true);
            }
        }

        private void AttachHandler()
        {
            if (this.elementWithContextMenu != null)
            {
                switch (this.EventName)
                {
                    case "MouseLeftButtonDown":
                        this.elementWithContextMenu.AddHandler(System.Windows.UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(this.OnElementWithContextMenuMouseClick), this.AttachOnHandledEvents);
                        return;

                    case "MouseLeftButtonUp":
                        this.elementWithContextMenu.AddHandler(System.Windows.UIElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(this.OnElementWithContextMenuMouseClick), this.AttachOnHandledEvents);
                        return;

                    case "MouseRightButtonDown":
                        this.elementWithContextMenu.MouseRightButtonDown += new MouseButtonEventHandler(this.OnElementWithContextMenuMouseClick);
                        return;

                    case "MouseRightButtonUp":
                        this.elementWithContextMenu.MouseRightButtonDown += new MouseButtonEventHandler(this.OnElementWithContextMenuMouseDown);
                        this.elementWithContextMenu.MouseRightButtonUp += new MouseButtonEventHandler(this.OnElementWithContextMenuMouseClick);
                        return;
                }
                if (!string.IsNullOrEmpty(this.EventName))
                {
                    Type elementType = this.elementWithContextMenu.GetType();
                    this.routedEvent = this.GetRoutedEvent(elementType);
                    if (this.routedEvent != null)
                    {
                        this.elementWithContextMenu.AddHandler(this.routedEvent, new RoutedEventHandler(this.RoutedEventFired), this.AttachOnHandledEvents);
                    }
                    else
                    {
                        this.trigger = this.elementWithContextMenu.GetType().GetEvent(this.EventName);
                        if (this.trigger == null)
                        {
                            throw new ArgumentException(this.EventName + " event cannot be found.");
                        }
                        this.emittedHandler = Delegate.CreateDelegate(this.trigger.EventHandlerType, methodInfo);
                        this.trigger.AddEventHandler(this.elementWithContextMenu, this.emittedHandler);
                    }
                }
            }
        }

        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            base.ClearContainerForItemOverride(element, item);
            SetInsideContextMenu(this, false);
        }

        private void ClearReferences()
        {
            if (this.popupWrapper != null)
            {
                this.popupWrapper.HidePopup();
                this.popupWrapper.ClickedOutsidePopup -= new EventHandler(this.OnPopupWrapperClickedOutsidePopup);
                this.popupWrapper.ClearReferences();
                this.popupWrapper = null;
            }
            if (this.rootPopup != null)
            {
                this.rootPopup.Child = null;
                this.rootPopup.IsOpen = false;
                this.rootPopup = null;
            }
            this.RemoveHandler(this.EventName);
            this.routedEvent = null;
            this.emittedHandler = null;
            this.trigger = null;
            this.elementWithContextMenu.Unloaded -= new RoutedEventHandler(this.OnElementWithContextMenuUnloaded);
            this.elementWithContextMenu.MouseMove -= new MouseEventHandler(this.ElementWithContextMenuMouseMove);
            this.elementWithContextMenu = null;
            if (this.InheritDataContext)
            {
                base.ClearValue(FrameworkElement.DataContextProperty);
            }
        }

        internal override void CloseAll()
        {
            if (!this.StaysOpen && this.IsOpen)
            {
                base.CloseAll();
                this.IsOpen = false;
            }
        }

        private void ClosingMenu()
        {
            this.CloseAll();
            this.IsOpen = false;
        }

        private void CreateReferences(FrameworkElement element)
        {
            this.elementWithContextMenu = element;
            this.elementWithContextMenu.MouseMove += new MouseEventHandler(this.ElementWithContextMenuMouseMove);
            this.elementWithContextMenu.Unloaded += new RoutedEventHandler(this.OnElementWithContextMenuUnloaded);
            this.AttachHandler();
        }

        private static void CreateRootPopup(Popup popup, System.Windows.UIElement child)
        {
            if (popup == null)
            {
                throw new ArgumentNullException("popup");
            }
            if (child == null)
            {
                throw new ArgumentNullException("child");
            }
            object parent = null;
            FrameworkElement fe = child as FrameworkElement;
            if ((fe != null) && (fe.Parent != null))
            {
                throw new InvalidOperationException(Telerik.Windows.Controls.SR.GetString("CreateRootPopup_ChildHasLogicalParent", new object[] { child, parent }));
            }
            parent = VisualTreeHelper.GetParent(child);
            if (parent != null)
            {
                throw new InvalidOperationException(Telerik.Windows.Controls.SR.GetString("CreateRootPopup_ChildHasVisualParent", new object[] { child, parent }));
            }
            popup.Child = child;
        }

        private void ElementWithContextMenuMouseMove(object sender, MouseEventArgs e)
        {
            if (!this.IsOpen)
            {
                this.mousePosition = new Point?(e.GetPosition(null));
            }
        }

        public T GetClickedElement<T>() where T: FrameworkElement
        {
            return this.elementWithContextMenu.GetElementsInScreenCoordinates<T>(this.MousePosition).FirstOrDefault<T>();
        }

        public static RadContextMenu GetContextMenu(FrameworkElement element)
        {
            return (RadContextMenu) element.GetValue(ContextMenuProperty);
        }

        internal static bool GetInsideContextMenu(DependencyObject obj)
        {
            return (bool) obj.GetValue(InsideContextMenuProperty);
        }

        private Telerik.Windows.RoutedEvent GetRoutedEvent(Type elementType)
        {
            while (elementType != typeof(DependencyObject))
            {
                Telerik.Windows.RoutedEvent[] routedEvents = EventManager.GetRoutedEventsForOwner(elementType);
                if (routedEvents != null)
                {
                    foreach (Telerik.Windows.RoutedEvent routEvent in routedEvents)
                    {
                        if (routEvent.Name == this.EventName)
                        {
                            return routEvent;
                        }
                    }
                }
                elementType = elementType.BaseType;
            }
            return null;
        }

        private void HookupRootPopup()
        {
            this.rootPopup = new Popup();
            this.rootPopup.Opened += new EventHandler(this.OnPopupOpened);
            this.rootPopup.Closed += new EventHandler(this.OnPopupClosed);
            this.popupWrapper = new PopupWrapper(this.rootPopup, this);
            this.popupWrapper.CatchClickOutsidePopup = true;
            this.popupWrapper.ClickedOutsidePopup += new EventHandler(this.OnPopupWrapperClickedOutsidePopup);
            CreateRootPopup(this.rootPopup, this);
        }

        private static void OnAttachOnHandledEventsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadContextMenu menu = d as RadContextMenu;
            FrameworkElement elementWithContextMenu = menu.elementWithContextMenu;
            menu.RemoveHandler(menu.EventName);
            menu.AttachHandler();
        }

        protected virtual void OnClosed(RadRoutedEventArgs e)
        {
            this.RaiseEvent(e);
        }

        private static void OnContextMenuChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement element = d as FrameworkElement;
            RadContextMenu oldMenu = e.OldValue as RadContextMenu;
            RadContextMenu newMenu = e.NewValue as RadContextMenu;
            if (element == null)
            {
                throw new ArgumentNullException("d");
            }
            if (oldMenu != null)
            {
                Telerik.Windows.RoutedEvent.SetLogicalParent(oldMenu, null);
                oldMenu.ClearReferences();
            }
            if (newMenu != null)
            {
                newMenu.CreateReferences(element);
                Telerik.Windows.RoutedEvent.SetLogicalParent(newMenu, element);
            }
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new RadContextMenuAutomationPeer(this);
        }

        private void OnElementWithContextMenuKeyDown(object sender, KeyEventArgs e)
        {
            this.ClosingMenu();
        }

        private void OnElementWithContextMenuMouseClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            OnEventTriggered(sender, e);
        }

        private void OnElementWithContextMenuMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void OnElementWithContextMenuUnloaded(object sender, RoutedEventArgs e)
        {
            this.ClosingMenu();
        }

        private static void OnEventNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadContextMenu menu = d as RadContextMenu;
            menu.RemoveHandler(e.OldValue as string);
            menu.AttachHandler();
        }

        private static void OnEventTriggered(object sender, EventArgs e)
        {
            FrameworkElement fe = sender as FrameworkElement;
            if (fe != null)
            {
                RadContextMenu menu = GetContextMenu(fe);
                if (menu != null)
                {
                    System.Windows.Input.MouseButtonEventArgs buttonArgs = e as System.Windows.Input.MouseButtonEventArgs;
                    if (buttonArgs != null)
                    {
                        if (menu.ModifierKeyPressed)
                        {
                            buttonArgs.Handled = true;
                        }
                        menu.mousePosition = new Point?(buttonArgs.GetPosition(null));
                    }
                    menu.TriggerEventFired();
                }
            }
        }

        private static void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadContextMenu menu = (RadContextMenu) d;
            if ((bool) e.NewValue)
            {
                if (menu.rootPopup == null)
                {
                    menu.HookupRootPopup();
                }
                menu.AddKeyDownHandler();
                menu.popupWrapper.UsePlacementTargetAsClipElement = false;
                menu.popupWrapper.HorizontalOffset = menu.HorizontalOffset;
                menu.popupWrapper.VerticalOffset = menu.VerticalOffset;
                menu.popupWrapper.Placement = menu.Placement;
                if (!menu.PlacementRectangle.IsEmpty)
                {
                    menu.popupWrapper.PlacementRectangle = menu.PlacementRectangle;
                }
                else if (menu.PlacementTarget != null)
                {
                    menu.popupWrapper.PlacementRectangle = Rect.Empty;
                    menu.popupWrapper.PlacementTarget = menu.PlacementTarget;
                }
                else
                {
                    Rect rect = Rect.Empty;
                    if (menu.Placement == Telerik.Windows.Controls.PlacementMode.Absolute)
                    {
                        if (!menu.mousePosition.HasValue)
                        {
                            menu.mousePosition = new Point?(menu.elementWithContextMenu.TransformToVisual(null).Transform(new Point()));
                        }
                        rect = new Rect(menu.mousePosition.Value, new Size());
                    }
                    else
                    {
                        Point topLeft = new Point();
                        topLeft = menu.elementWithContextMenu.TransformToVisual(null).Transform(topLeft);
                        rect = new Rect(topLeft, menu.elementWithContextMenu.RenderSize);
                    }
                    menu.popupWrapper.PlacementRectangle = rect;
                }
                menu.popupWrapper.ShowPopup();
            }
            else
            {
                menu.CurrentSelection = null;
                menu.RemoveKeyDownHandler();
                menu.CloseAll();
                if (menu.popupWrapper != null)
                {
                    menu.popupWrapper.HidePopup();
                }
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (!e.Handled && this.IsOpen)
            {
                switch (e.Key)
                {
                    case Key.Up:
                        if (base.CurrentSelection != null)
                        {
                            int index = base.ItemContainerGenerator.IndexFromContainer(e.OriginalSource as DependencyObject);
                            e.Handled = base.MenuNavigate(index - 1, -1, false);
                            break;
                        }
                        e.Handled = base.MenuNavigate(0, -1, false);
                        return;

                    case Key.Right:
                        break;

                    case Key.Down:
                        if (base.CurrentSelection != null)
                        {
                            int index = base.ItemContainerGenerator.IndexFromContainer(e.OriginalSource as DependencyObject);
                            e.Handled = base.MenuNavigate(index + 1, 1, false);
                            return;
                        }
                        e.Handled = base.MenuNavigate(0, 1, false);
                        return;

                    default:
                        return;
                }
            }
        }

        private void OnMouseClick(object sender, Telerik.Windows.Input.MouseButtonEventArgs e)
        {
            if ((e.ChangedButton == MouseButton.Right) && !this.IsOpen)
            {
                this.mousePosition = new Point?(e.GetPosition(null));
                this.TriggerEventFired();
                e.Handled = true;
            }
        }

        protected override void OnMouseRightButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            e.Handled = true;
            base.OnMouseRightButtonDown(e);
        }

        protected virtual void OnOpened(RadRoutedEventArgs e)
        {
            base.Focus();
            this.RaiseEvent(e);
        }

        private void OnPopupClosed(object source, EventArgs e)
        {
            this.OnClosed(new RadRoutedEventArgs(ClosedEvent, this.elementWithContextMenu));
        }

        private void OnPopupOpened(object source, EventArgs e)
        {
            if (base.CurrentSelection != null)
            {
                base.CurrentSelection = null;
            }
            if (this.InheritDataContext)
            {
                Binding b = new Binding("DataContext") {
                    Source = this.elementWithContextMenu
                };
                base.SetBinding(FrameworkElement.DataContextProperty, b);
            }
            this.OnOpened(new RadRoutedEventArgs(OpenedEvent, this.elementWithContextMenu));
        }

        private void OnPopupWrapperClickedOutsidePopup(object sender, EventArgs e)
        {
            this.ClosingMenu();
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            SetInsideContextMenu(element, true);
        }

        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId="eventName")]
        private void RemoveHandler(string eventName)
        {
            if (this.elementWithContextMenu != null)
            {
                switch (eventName)
                {
                    case "MouseLeftButtonDown":
                        this.elementWithContextMenu.RemoveHandler(System.Windows.UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(this.OnElementWithContextMenuMouseClick));
                        return;

                    case "MouseLeftButtonUp":
                        this.elementWithContextMenu.RemoveHandler(System.Windows.UIElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(this.OnElementWithContextMenuMouseClick));
                        return;

                    case "MouseRightButtonDown":
                        this.elementWithContextMenu.MouseRightButtonDown -= new MouseButtonEventHandler(this.OnElementWithContextMenuMouseClick);
                        return;

                    case "MouseRightButtonUp":
                        this.elementWithContextMenu.MouseRightButtonDown -= new MouseButtonEventHandler(this.OnElementWithContextMenuMouseDown);
                        this.elementWithContextMenu.MouseRightButtonUp -= new MouseButtonEventHandler(this.OnElementWithContextMenuMouseClick);
                        return;
                }
                if (this.routedEvent != null)
                {
                    this.elementWithContextMenu.RemoveHandler(this.routedEvent, new RoutedEventHandler(this.RoutedEventFired));
                    this.routedEvent = null;
                }
                else if ((this.trigger != null) && (this.emittedHandler != null))
                {
                    this.trigger.RemoveEventHandler(this.elementWithContextMenu, this.emittedHandler);
                }
            }
        }

        private void RemoveKeyDownHandler()
        {
            if (this.elementWithContextMenu != null)
            {
                this.elementWithContextMenu.RemoveHandler(System.Windows.UIElement.KeyDownEvent, new KeyEventHandler(this.OnElementWithContextMenuKeyDown));
            }
        }

        private void RoutedEventFired(object sender, RoutedEventArgs e)
        {
            OnEventTriggered(sender, e);
        }

        public static void SetContextMenu(FrameworkElement element, RadContextMenu value)
        {
            element.SetValue(ContextMenuProperty, value);
        }

        internal static void SetInsideContextMenu(DependencyObject obj, bool value)
        {
            obj.SetValue(InsideContextMenuProperty, value);
        }

        private void TriggerEventFired()
        {
            this.IsOpen = false;
            if (this.ModifierKeyPressed)
            {
                this.IsOpen = true;
            }
        }

        public bool AttachOnHandledEvents
        {
            get
            {
                return (bool) base.GetValue(AttachOnHandledEventsProperty);
            }
            set
            {
                base.SetValue(AttachOnHandledEventsProperty, value);
            }
        }

        public string EventName
        {
            get
            {
                return (string) base.GetValue(EventNameProperty);
            }
            set
            {
                base.SetValue(EventNameProperty, value);
            }
        }

        [Category("Layout")]
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

        public bool InheritDataContext
        {
            get
            {
                return (bool) base.GetValue(InheritDataContextProperty);
            }
            set
            {
                base.SetValue(InheritDataContextProperty, value);
            }
        }

        [Category("Appearance"), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsOpen
        {
            get
            {
                return (bool) base.GetValue(IsOpenProperty);
            }
            set
            {
                base.SetValue(IsOpenProperty, value);
            }
        }

        public ModifierKeys ModifierKey
        {
            get
            {
                return (ModifierKeys) base.GetValue(ModifierKeyProperty);
            }
            set
            {
                base.SetValue(ModifierKeyProperty, value);
            }
        }

        private bool ModifierKeyPressed
        {
            get
            {
                return ((System.Windows.Input.Keyboard.Modifiers & this.ModifierKey) == this.ModifierKey);
            }
        }

        public Point MousePosition
        {
            get
            {
                if (!this.mousePosition.HasValue)
                {
                    return new Point();
                }
                return this.mousePosition.GetValueOrDefault();
            }
        }

        [Category("Layout")]
        public Telerik.Windows.Controls.PlacementMode Placement
        {
            get
            {
                return (Telerik.Windows.Controls.PlacementMode) base.GetValue(PlacementProperty);
            }
            set
            {
                base.SetValue(PlacementProperty, value);
            }
        }

        [Category("Layout")]
        public Rect PlacementRectangle
        {
            get
            {
                return (Rect) base.GetValue(PlacementRectangleProperty);
            }
            set
            {
                base.SetValue(PlacementRectangleProperty, value);
            }
        }

        [Category("Layout")]
        public System.Windows.UIElement PlacementTarget
        {
            get
            {
                return (System.Windows.UIElement) base.GetValue(PlacementTargetProperty);
            }
            set
            {
                base.SetValue(PlacementTargetProperty, value);
            }
        }

        [Category("Behavior")]
        public bool StaysOpen
        {
            get
            {
                return (bool) base.GetValue(StaysOpenProperty);
            }
            set
            {
                base.SetValue(StaysOpenProperty, value);
            }
        }

        public System.Windows.UIElement UIElement
        {
            get
            {
                return this.elementWithContextMenu;
            }
        }

        [Category("Layout")]
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

