namespace Telerik.Windows
{
    using System;
    using System.Globalization;
    using System.Windows;
    using Telerik.Windows.Controls;

    public static class EventManager
    {
        private static void CheckParameters(Type ownerType)
        {
            if (ownerType == null)
            {
                throw new ArgumentNullException("ownerType");
            }
        }

        private static void CheckParameters(string name, Type ownerType)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (ownerType == null)
            {
                throw new ArgumentNullException("ownerType");
            }
        }

        internal static void CheckParameters(Telerik.Windows.RoutedEvent routedEvent, Delegate handler)
        {
            if (routedEvent == null)
            {
                throw new ArgumentNullException("routedEvent");
            }
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }
            if (!routedEvent.IsLegalHandler(handler))
            {
                throw new ArgumentException(Telerik.Windows.Controls.SR.Get("HandlerTypeIllegal", new object[0]));
            }
        }

        private static void CheckParameters(Type classType, Telerik.Windows.RoutedEvent routedEvent, Delegate handler)
        {
            if (classType == null)
            {
                throw new ArgumentNullException("classType");
            }
            if (routedEvent == null)
            {
                throw new ArgumentNullException("routedEvent");
            }
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }
            if (!typeof(UIElement).IsAssignableFrom(classType))
            {
                throw new ArgumentException(Telerik.Windows.Controls.SR.Get("ClassTypeIllegal", new object[0]));
            }
            if (!routedEvent.IsLegalHandler(handler))
            {
                throw new ArgumentException(Telerik.Windows.Controls.SR.Get("HandlerTypeIllegal", new object[0]));
            }
        }

        private static void CheckParameters(string name, RoutingStrategy routingStrategy, Type handlerType, Type ownerType)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (((routingStrategy != RoutingStrategy.Tunnel) && (routingStrategy != RoutingStrategy.Bubble)) && (routingStrategy != RoutingStrategy.Direct))
            {
                object[] args = new object[] { "routingStrategy", ((int) routingStrategy).ToString(CultureInfo.CurrentCulture), "RoutingStrategy" };
                throw new ArgumentException(Telerik.Windows.Controls.SR.GetString("InvalidEnumArgument", args), "routingStrategy");
            }
            if (handlerType == null)
            {
                throw new ArgumentNullException("handlerType");
            }
            if (ownerType == null)
            {
                throw new ArgumentNullException("ownerType");
            }
            if (GlobalEventManager.GetRoutedEventFromName(name, ownerType, false) != null)
            {
                throw new ArgumentException(Telerik.Windows.Controls.SR.Get("DuplicateEventName", new object[] { name, ownerType }));
            }
        }

        internal static Telerik.Windows.RoutedEvent GetRoutedEventFromName(string name, Type ownerType)
        {
            CheckParameters(name, ownerType);
            return GlobalEventManager.GetRoutedEventFromName(name, ownerType, true);
        }

        internal static Telerik.Windows.RoutedEvent[] GetRoutedEvents()
        {
            return GlobalEventManager.GetRoutedEvents();
        }

        internal static Telerik.Windows.RoutedEvent[] GetRoutedEventsForOwner(Type ownerType)
        {
            CheckParameters(ownerType);
            return GlobalEventManager.GetRoutedEventsForOwner(ownerType);
        }

        public static void RegisterClassHandler(Type classType, Telerik.Windows.RoutedEvent routedEvent, Delegate handler)
        {
            RegisterClassHandler(classType, routedEvent, handler, false);
        }

        public static void RegisterClassHandler(Type classType, Telerik.Windows.RoutedEvent routedEvent, Delegate handler, bool handledEventsToo)
        {
            CheckParameters(classType, routedEvent, handler);
            GlobalEventManager.RegisterClassHandler(classType, routedEvent, handler, handledEventsToo);
        }

        public static Telerik.Windows.RoutedEvent RegisterRoutedEvent(string name, RoutingStrategy routingStrategy, Type handlerType, Type ownerType)
        {
            CheckParameters(name, routingStrategy, handlerType, ownerType);
            return GlobalEventManager.RegisterRoutedEvent(name, routingStrategy, handlerType, ownerType);
        }
    }
}

