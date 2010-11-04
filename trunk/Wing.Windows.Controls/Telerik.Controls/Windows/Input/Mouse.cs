namespace Telerik.Windows.Input
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using Wing.Windows.Controls.Telerik.Navigation.Windows.Controls;
    using System.Windows.Input;
    using Telerik.Windows;
    using Telerik.Windows.Controls;
    using System.Windows.Browser;

    public static class Mouse
    {
        private static readonly MouseClickCounter ClickCounter = new MouseClickCounter();
        public static readonly Telerik.Windows.RoutedEvent MouseDownEvent = EventManager.RegisterRoutedEvent("MouseDown", RoutingStrategy.Bubble, typeof(EventHandler<Telerik.Windows.Input.MouseButtonEventArgs>), typeof(FrameworkElement));
        private static readonly DependencyProperty MouseDownHandlerProperty = DependencyProperty.RegisterAttached("MouseDownHandler", typeof(object), typeof(Mouse), new Telerik.Windows.PropertyMetadata(null));
        public static readonly Telerik.Windows.RoutedEvent MouseUpEvent = EventManager.RegisterRoutedEvent("MouseUp", RoutingStrategy.Bubble, typeof(EventHandler<Telerik.Windows.Input.MouseButtonEventArgs>), typeof(FrameworkElement));
        private static readonly DependencyProperty MouseUpHandlerProperty = DependencyProperty.RegisterAttached("MouseUpHandler", typeof(object), typeof(Mouse), new Telerik.Windows.PropertyMetadata(null));
        public static readonly Telerik.Windows.RoutedEvent MouseWheelEvent = EventManager.RegisterRoutedEvent("MouseWheel", RoutingStrategy.Bubble, typeof(EventHandler<Telerik.Windows.Input.MouseWheelEventArgs>), typeof(FrameworkElement));

        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification="We need to initialize the HTML events in the static constructor.")]
        static Mouse()
        {
            DoubleClickDuration = new TimeSpan(0, 0, 0, 0, 500);
            if (!RadControl.IsInDesignMode && HtmlPage.IsEnabled)
            {
                InitializeMouseWheelHandlers();
            }
        }

        public static void AddMouseDownHandler(UIElement element, EventHandler<Telerik.Windows.Input.MouseButtonEventArgs> handler)
        {
            AddMouseDownHandler(element, handler, false);
        }

        public static void AddMouseDownHandler(UIElement element, EventHandler<Telerik.Windows.Input.MouseButtonEventArgs> handler, bool handledEventsToo)
        {
            RemoveMouseDownHandler(element, handler);
            element.MouseRightButtonDown += new MouseButtonEventHandler(Mouse.OnElementMouseRightButtonDown);
            SetMouseDownHandler(element, handler);
            element.AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(Mouse.OnElementMouseLeftButtonDown), handledEventsToo);
        }

        public static void AddMouseUpHandler(UIElement element, EventHandler<Telerik.Windows.Input.MouseButtonEventArgs> handler)
        {
            AddMouseUpHandler(element, handler, false);
        }

        public static void AddMouseUpHandler(UIElement element, EventHandler<Telerik.Windows.Input.MouseButtonEventArgs> handler, bool handledEventsToo)
        {
            RemoveMouseUpHandler(element, handler);
            element.MouseRightButtonUp += new MouseButtonEventHandler(Mouse.OnElementMouseRightButtonUp);
            SetMouseUpHandler(element, handler);
            element.AddHandler(UIElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Mouse.OnElementMouseLeftButtonUp), handledEventsToo);
        }

        public static void AddMouseWheelHandler(DependencyObject element, EventHandler<Telerik.Windows.Input.MouseWheelEventArgs> handler)
        {
            element.AddHandler(MouseWheelEvent, handler);
        }

        [SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
        private static MouseButton GetMouseButton(MouseButtons buttons)
        {
            if ((buttons & MouseButtons.Left) == MouseButtons.Left)
            {
                return MouseButton.Left;
            }
            if ((buttons & MouseButtons.Right) == MouseButtons.Right)
            {
                return MouseButton.Right;
            }
            if ((buttons & MouseButtons.Middle) != MouseButtons.Middle)
            {
                throw new ArgumentException("buttons");
            }
            return MouseButton.Middle;
        }

        private static object GetMouseDownHandler(DependencyObject obj)
        {
            return obj.GetValue(MouseDownHandlerProperty);
        }

        private static object GetMouseUpHandler(DependencyObject obj)
        {
            return obj.GetValue(MouseUpHandlerProperty);
        }

        private static int GetWheelDelta(ScriptObject e)
        {
            int delta = 0;
            if (e.GetProperty("wheelDelta") != null)
            {
                delta = (int) ((double) e.GetProperty("wheelDelta"));
                if (HtmlPage.Window.GetProperty("opera") != null)
                {
                    delta = -delta;
                }
                return delta;
            }
            if (e.GetProperty("detail") != null)
            {
                delta = -((int) (((double) e.GetProperty("detail")) * 30.0));
            }
            return delta;
        }

        private static void InitializeMouseWheelHandlers()
        {
            HtmlPage.Window.AttachEvent("DOMMouseScroll", new EventHandler<HtmlEventArgs>(Mouse.OnMouseWheel));
            HtmlPage.Window.AttachEvent("onmousewheel", new EventHandler<HtmlEventArgs>(Mouse.OnMouseWheel));
            HtmlPage.Document.AttachEvent("onmousewheel", new EventHandler<HtmlEventArgs>(Mouse.OnMouseWheel));
        }

        private static void OnElementMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            RaiseMouseButtonEventImpl(sender as DependencyObject, MouseDownEvent, MouseButton.Left, MouseButtonState.Pressed, e);
        }

        private static void OnElementMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            RaiseMouseButtonEventImpl(sender as DependencyObject, MouseUpEvent, MouseButton.Left, MouseButtonState.Released, e);
        }

        private static void OnElementMouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            RaiseMouseButtonEventImpl(sender as DependencyObject, MouseDownEvent, MouseButton.Right, MouseButtonState.Pressed, e);
        }

        private static void OnElementMouseRightButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            RaiseMouseButtonEventImpl(sender as DependencyObject, MouseUpEvent, MouseButton.Right, MouseButtonState.Released, e);
        }

        private static void OnMouseWheel(object sender, HtmlEventArgs args)
        {
            int delta = GetWheelDelta(args.EventObject);
            if (delta != 0)
            {
                Point mousePosition = new Point((double) args.OffsetX, (double) args.OffsetY);
                UIElement element = VisualTreeHelperExtensions.GetElementsInScreenCoordinates<UIElement>(mousePosition).FirstOrDefault<UIElement>();
                if (element != null)
                {
                    Telerik.Windows.Input.MouseWheelEventArgs e = new Telerik.Windows.Input.MouseWheelEventArgs(MouseWheelEvent, mousePosition, delta);
                    element.RaiseEvent(e);
                    if (e.Handled)
                    {
                        args.PreventDefault();
                        args.EventObject.SetProperty("returnValue", false);
                    }
                }
            }
        }

        private static void RaiseMouseButtonEventImpl(DependencyObject element, Telerik.Windows.RoutedEvent routedEvent, MouseButton button, MouseButtonState state, System.Windows.Input.MouseButtonEventArgs e)
        {
            EventHandler<Telerik.Windows.Input.MouseButtonEventArgs> handler;
            Point position = e.GetPosition(null);
            ClickCounter.UpdateCounter(position, state);
            Telerik.Windows.Input.MouseButtonEventArgs args = new Telerik.Windows.Input.MouseButtonEventArgs(routedEvent, position, button, state, ClickCounter.GetCount(state), e);
            if (state == MouseButtonState.Pressed)
            {
                handler = GetMouseDownHandler(element) as EventHandler<Telerik.Windows.Input.MouseButtonEventArgs>;
            }
            else
            {
                handler = GetMouseUpHandler(element) as EventHandler<Telerik.Windows.Input.MouseButtonEventArgs>;
            }
            if (handler != null)
            {
                handler(element, args);
            }
        }

        public static void RemoveMouseDownHandler(UIElement element, EventHandler<Telerik.Windows.Input.MouseButtonEventArgs> handler)
        {
            element.MouseRightButtonDown -= new MouseButtonEventHandler(Mouse.OnElementMouseRightButtonDown);
            EventHandler<Telerik.Windows.Input.MouseButtonEventArgs> attachedHandler = GetMouseDownHandler(element) as EventHandler<Telerik.Windows.Input.MouseButtonEventArgs>;
            if (attachedHandler == handler)
            {
                SetMouseDownHandler(element, null);
            }
            element.RemoveHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(Mouse.OnElementMouseLeftButtonDown));
        }

        public static void RemoveMouseUpHandler(UIElement element, EventHandler<Telerik.Windows.Input.MouseButtonEventArgs> handler)
        {
            element.MouseRightButtonUp -= new MouseButtonEventHandler(Mouse.OnElementMouseRightButtonUp);
            EventHandler<Telerik.Windows.Input.MouseButtonEventArgs> attachedHandler = GetMouseUpHandler(element) as EventHandler<Telerik.Windows.Input.MouseButtonEventArgs>;
            if (attachedHandler == handler)
            {
                SetMouseUpHandler(element, null);
            }
            element.RemoveHandler(UIElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Mouse.OnElementMouseLeftButtonUp));
        }

        public static void RemoveMouseWheelHandler(DependencyObject element, EventHandler<Telerik.Windows.Input.MouseWheelEventArgs> handler)
        {
            element.RemoveHandler(MouseWheelEvent, handler);
        }

        private static void SetMouseDownHandler(DependencyObject obj, object value)
        {
            obj.SetValue(MouseDownHandlerProperty, value);
        }

        private static void SetMouseUpHandler(DependencyObject obj, object value)
        {
            obj.SetValue(MouseUpHandlerProperty, value);
        }

        public static TimeSpan DoubleClickDuration{get;set;}

        public static bool IsMouseWheelSupported
        {
            get
            {
                if (Application.Current.Host.Settings.Windowless)
                {
                    return Application.Current.IsRunningOutOfBrowser;
                }
                return true;
            }
        }
    }
}

