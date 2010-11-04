namespace Telerik.Windows
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Media;
    using Telerik.Windows.Controls;

    internal class RadRoutedEventHelper
    {
        public static readonly DependencyProperty DependencyObjectTypeProperty = DependencyProperty.RegisterAttached("DependencyObjectType", typeof(DependencyObjectType), typeof(RadRoutedEventHelper), null);
        public static readonly DependencyProperty EventHandlersStoreProperty = DependencyProperty.RegisterAttached("EventHandlersStore", typeof(EventHandlersStore), typeof(RadRoutedEventHelper), null);

        private RadRoutedEventHelper()
        {
        }

        public static void AddHandler(DependencyObject element, Telerik.Windows.RoutedEvent routedEvent, Delegate handler)
        {
            AddHandler(element, routedEvent, handler, false);
        }

        public static void AddHandler(DependencyObject element, Telerik.Windows.RoutedEvent routedEvent, Delegate handler, bool handledEventsToo)
        {
            EventManager.CheckParameters(routedEvent, handler);
            EnsureEventHandlersStore(element).AddRoutedEventHandler(routedEvent, handler, handledEventsToo);
        }

        private static void AddToEventRoute(DependencyObject element, EventRoute route, RadRoutedEventArgs e)
        {
            CheckParameters(element, route, e);
            DependencyObjectType dependencyType = element.GetDependencyObjectType();
            if (dependencyType == null)
            {
                dependencyType = DependencyObjectType.FromSystemTypeInternal(element.GetType());
                element.SetDependencyObjectType(dependencyType);
            }
            for (RoutedEventHandlerInfoList list = GlobalEventManager.GetDTypedClassListeners(dependencyType, e.RoutedEvent); list != null; list = list.Next)
            {
                for (int i = 0; i < list.Handlers.Length; i++)
                {
                    route.Add(element, list.Handlers[i].Handler, list.Handlers[i].InvokeHandledEventsToo);
                }
            }
            EventHandlersStore eventHandlersStore = GetEventHandlersStore(element);
            if (eventHandlersStore != null)
            {
                List<RoutedEventHandlerInfo> list2 = eventHandlersStore[e.RoutedEvent];
                if (list2 != null)
                {
                    for (int j = 0; j < list2.Count; j++)
                    {
                        RoutedEventHandlerInfo info = list2[j];
                        route.Add(element, info.Handler, info.InvokeHandledEventsToo);
                    }
                }
            }
        }

        private static void BuildRoute(DependencyObject element, EventRoute route, RadRoutedEventArgs args)
        {
            CheckParameters(route, args);
            if (args.RoutedEvent.RoutingStrategy == RoutingStrategy.Direct)
            {
                AddToEventRoute(element, route, args);
            }
            else
            {
                int nestLevel = 0;
                while (element != null)
                {
                    if (nestLevel++ > 0x100)
                    {
                        throw new InvalidOperationException(Telerik.Windows.Controls.SR.Get("TreeLoop"));
                    }
                    object source = null;
                    if (source != null)
                    {
                        route.AddSource(source);
                    }
                    if (element != null)
                    {
                        AddToEventRoute(element, route, args);
                        element = FindParent(element as FrameworkElement);
                    }
                    if (element == args.Source)
                    {
                        route.AddSource(element);
                    }
                }
            }
        }

        private static void CheckParameters(EventRoute route, RadRoutedEventArgs args)
        {
            if (route == null)
            {
                throw new ArgumentNullException("route");
            }
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }
            if (args.Source == null)
            {
                throw new ArgumentException(Telerik.Windows.Controls.SR.Get("SourceNotSet", new object[0]));
            }
            if (args.RoutedEvent != route.RoutedEvent)
            {
                throw new ArgumentException(Telerik.Windows.Controls.SR.Get("Mismatched_RoutedEvent", new object[0]));
            }
        }

        private static void CheckParameters(DependencyObject element, EventRoute route, RadRoutedEventArgs e)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            if (route == null)
            {
                throw new ArgumentNullException("route");
            }
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }
        }

        private static void ClearEventHandlersStore(DependencyObject element)
        {
            element.SetValue(EventHandlersStoreProperty, null);
        }

        private static EventHandlersStore EnsureEventHandlersStore(DependencyObject element)
        {
            EventHandlersStore store = element.GetValue(EventHandlersStoreProperty) as EventHandlersStore;
            if (store == null)
            {
                store = new EventHandlersStore();
                element.SetValue(EventHandlersStoreProperty, store);
            }
            return store;
        }

        private static DependencyObject FindParent(FrameworkElement item)
        {
            if (item == null)
            {
                return item;
            }
            FrameworkElement parent = Telerik.Windows.RoutedEvent.GetLogicalParent(item) as FrameworkElement;
            if (parent == null)
            {
                parent = VisualTreeHelper.GetParent(item) as FrameworkElement;
                if (parent == null)
                {
                    parent = item.Parent as FrameworkElement;
                }
            }
            if (parent != null)
            {
                return parent;
            }
            return null;
        }

        private static EventHandlersStore GetEventHandlersStore(DependencyObject element)
        {
            return (element.GetValue(EventHandlersStoreProperty) as EventHandlersStore);
        }

        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification="WPF code port")]
        public static void RaiseEvent(DependencyObject element, RadRoutedEventArgs args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }
            EventRoute route = new EventRoute(args.RoutedEvent);
            args.Source = element;
            BuildRoute(element, route, args);
            route.InvokeHandlers(element, args);
            args.Source = args.OriginalSource;
            route.Clear();
        }

        public static void RemoveHandler(DependencyObject element, Telerik.Windows.RoutedEvent routedEvent, Delegate handler)
        {
            EventManager.CheckParameters(routedEvent, handler);
            EventHandlersStore eventHandlersStore = GetEventHandlersStore(element);
            if (eventHandlersStore != null)
            {
                eventHandlersStore.RemoveRoutedEventHandler(routedEvent, handler);
                if (eventHandlersStore.Count == 0)
                {
                    ClearEventHandlersStore(element);
                }
            }
        }
    }
}

