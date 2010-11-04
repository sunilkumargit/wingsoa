namespace Telerik.Windows
{
    using MS.Utility;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Windows;
    using Telerik.Windows.Controls;

    internal static class GlobalEventManager
    {
        private static DependencyObjectType dependencyObjectType = DependencyObjectType.FromSystemTypeInternal(typeof(DependencyObject));
        private static DTypeMap dependencyTypedClassListeners = new DTypeMap(100);
        private static DTypeMap dependencyTypedRoutedEventList = new DTypeMap(10);
        private static List<object> globalIndexToEventMap = new List<object>(100);
        private static Dictionary<Type, List<Telerik.Windows.RoutedEvent>> ownerTypedRoutedEventList = new Dictionary<Type, List<Telerik.Windows.RoutedEvent>>(10);
        private static int routedEventsCount;
        private static object synchronized = new object();

        internal static void AddOwner(Telerik.Windows.RoutedEvent routedEvent, Type ownerType)
        {
            if ((ownerType == typeof(DependencyObject)) || ownerType.IsSubclassOf(typeof(DependencyObject)))
            {
                List<Telerik.Windows.RoutedEvent> list2;
                DependencyObjectType type = DependencyObjectType.FromSystemTypeInternal(ownerType);
                object obj3 = dependencyTypedRoutedEventList[type];
                if (obj3 == null)
                {
                    list2 = new List<Telerik.Windows.RoutedEvent>();
                    dependencyTypedRoutedEventList[type] = list2;
                }
                else
                {
                    list2 = (List<Telerik.Windows.RoutedEvent>)obj3;
                }
                if (!list2.Contains(routedEvent))
                {
                    list2.Add(routedEvent);
                }
            }
            else
            {
                List<Telerik.Windows.RoutedEvent> list;
                object obj2 = null;
                if (ownerTypedRoutedEventList.ContainsKey(ownerType))
                {
                    obj2 = ownerTypedRoutedEventList[ownerType];
                }
                if (obj2 == null)
                {
                    list = new List<Telerik.Windows.RoutedEvent>();
                    ownerTypedRoutedEventList[ownerType] = list;
                }
                else
                {
                    list = (List<Telerik.Windows.RoutedEvent>)obj2;
                }
                if (!list.Contains(routedEvent))
                {
                    list.Add(routedEvent);
                }
            }
        }

        internal static object EventFromGlobalIndex(int globalIndex)
        {
            return globalIndexToEventMap[globalIndex];
        }

        internal static RoutedEventHandlerInfoList GetDTypedClassListeners(DependencyObjectType dependencyType, Telerik.Windows.RoutedEvent routedEvent)
        {
            int num;
            ClassHandlersStore store;
            return GetDTypedClassListeners(dependencyType, routedEvent, out store, out num);
        }

        internal static RoutedEventHandlerInfoList GetDTypedClassListeners(DependencyObjectType dependencyType, Telerik.Windows.RoutedEvent routedEvent, out ClassHandlersStore classListenersLists, out int index)
        {
            classListenersLists = (ClassHandlersStore)dependencyTypedClassListeners[dependencyType];
            if (classListenersLists != null)
            {
                index = classListenersLists.GetHandlersIndex(routedEvent);
                if (index != -1)
                {
                    return classListenersLists.GetExistingHandlers(index);
                }
            }
            lock (synchronized)
            {
                return GetUpdatedDTypedClassListeners(dependencyType, routedEvent, out classListenersLists, out index);
            }
        }

        internal static int GetNextAvailableGlobalIndex(object value)
        {
            lock (synchronized)
            {
                if (globalIndexToEventMap.Count >= 0x7fffffff)
                {
                    throw new InvalidOperationException(Telerik.Windows.Controls.SR.Get("TooManyRoutedEvents", new object[0]));
                }
                globalIndexToEventMap.Add(value);
                return globalIndexToEventMap.IndexOf(value);
            }
        }

        internal static Telerik.Windows.RoutedEvent GetRoutedEventFromName(string name, Type ownerType, bool includeSupers)
        {
            if ((ownerType != typeof(DependencyObject)) && !ownerType.IsSubclassOf(typeof(DependencyObject)))
            {
                while (ownerType != null)
                {
                    if (ownerTypedRoutedEventList.ContainsKey(ownerType))
                    {
                        List<Telerik.Windows.RoutedEvent> list = ownerTypedRoutedEventList[ownerType];
                        if (list != null)
                        {
                            for (int i = 0; i < list.Count; i++)
                            {
                                Telerik.Windows.RoutedEvent event2 = list[i];
                                if (event2.Name.Equals(name))
                                {
                                    return event2;
                                }
                            }
                        }
                    }
                    ownerType = includeSupers ? ownerType.BaseType : null;
                }
            }
            else
            {
                for (DependencyObjectType type = DependencyObjectType.FromSystemTypeInternal(ownerType); type != null; type = includeSupers ? type.BaseType : null)
                {
                    List<Telerik.Windows.RoutedEvent> list2 = (List<Telerik.Windows.RoutedEvent>)dependencyTypedRoutedEventList[type];
                    if (list2 != null)
                    {
                        for (int j = 0; j < list2.Count; j++)
                        {
                            Telerik.Windows.RoutedEvent event3 = list2[j];
                            if (event3.Name.Equals(name))
                            {
                                return event3;
                            }
                        }
                    }
                }
            }
            return null;
        }

        internal static Telerik.Windows.RoutedEvent[] GetRoutedEvents()
        {
            Telerik.Windows.RoutedEvent[] eventArray;
            lock (synchronized)
            {
                eventArray = new Telerik.Windows.RoutedEvent[routedEventsCount];
                ItemStructList<DependencyObjectType> activeDTypes = dependencyTypedRoutedEventList.ActiveDTypes;
                int num4 = 0;
                for (int i = 0; i < activeDTypes.Count; i++)
                {
                    List<Telerik.Windows.RoutedEvent> list2 = (List<Telerik.Windows.RoutedEvent>)dependencyTypedRoutedEventList[activeDTypes.List[i]];
                    for (int j = 0; j < list2.Count; j++)
                    {
                        Telerik.Windows.RoutedEvent event3 = list2[j];
                        if (Array.IndexOf<Telerik.Windows.RoutedEvent>(eventArray, event3) < 0)
                        {
                            eventArray[num4++] = event3;
                        }
                    }
                }
                IDictionaryEnumerator enumerator = ownerTypedRoutedEventList.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    List<Telerik.Windows.RoutedEvent> list = (List<Telerik.Windows.RoutedEvent>)enumerator.Value;
                    for (int k = 0; k < list.Count; k++)
                    {
                        Telerik.Windows.RoutedEvent event2 = list[k];
                        if (Array.IndexOf<Telerik.Windows.RoutedEvent>(eventArray, event2) < 0)
                        {
                            eventArray[num4++] = event2;
                        }
                    }
                }
            }
            return eventArray;
        }

        internal static Telerik.Windows.RoutedEvent[] GetRoutedEventsForOwner(Type ownerType)
        {
            if ((ownerType == typeof(DependencyObject)) || ownerType.IsSubclassOf(typeof(DependencyObject)))
            {
                DependencyObjectType type = DependencyObjectType.FromSystemTypeInternal(ownerType);
                List<Telerik.Windows.RoutedEvent> list2 = (List<Telerik.Windows.RoutedEvent>)dependencyTypedRoutedEventList[type];
                if (list2 != null)
                {
                    return list2.ToArray();
                }
            }
            else
            {
                List<Telerik.Windows.RoutedEvent> list = ownerTypedRoutedEventList[ownerType];
                if (list != null)
                {
                    return list.ToArray();
                }
            }
            return null;
        }

        private static RoutedEventHandlerInfoList GetUpdatedDTypedClassListeners(DependencyObjectType dependencyType, Telerik.Windows.RoutedEvent routedEvent, out ClassHandlersStore classListenersLists, out int index)
        {
            classListenersLists = (ClassHandlersStore)dependencyTypedClassListeners[dependencyType];
            if (classListenersLists != null)
            {
                index = classListenersLists.GetHandlersIndex(routedEvent);
                if (index != -1)
                {
                    return classListenersLists.GetExistingHandlers(index);
                }
            }
            DependencyObjectType baseType = dependencyType;
            ClassHandlersStore store = null;
            RoutedEventHandlerInfoList handlers = null;
            int handlersIndex = -1;
            while ((handlersIndex == -1) && (baseType.Id != dependencyObjectType.Id))
            {
                baseType = baseType.BaseType;
                store = (ClassHandlersStore)dependencyTypedClassListeners[baseType];
                if (store != null)
                {
                    handlersIndex = store.GetHandlersIndex(routedEvent);
                    if (handlersIndex != -1)
                    {
                        handlers = store.GetExistingHandlers(handlersIndex);
                    }
                }
            }
            if (classListenersLists == null)
            {
                if (dependencyType.SystemType == typeof(UIElement))
                {
                    classListenersLists = new ClassHandlersStore(80);
                }
                else
                {
                    classListenersLists = new ClassHandlersStore(1);
                }
                dependencyTypedClassListeners[dependencyType] = classListenersLists;
            }
            index = classListenersLists.CreateHandlersLink(routedEvent, handlers);
            return handlers;
        }

        internal static void RegisterClassHandler(Type classType, Telerik.Windows.RoutedEvent routedEvent, Delegate handler, bool handledEventsToo)
        {
            ClassHandlersStore store;
            int num2;
            DependencyObjectType dependencyType = DependencyObjectType.FromSystemTypeInternal(classType);
            GetDTypedClassListeners(dependencyType, routedEvent, out store, out num2);
            lock (synchronized)
            {
                RoutedEventHandlerInfoList baseClassListeners = store.AddToExistingHandlers(num2, handler, handledEventsToo);
                ItemStructList<DependencyObjectType> activeDTypes = dependencyTypedClassListeners.ActiveDTypes;
                for (int i = 0; i < activeDTypes.Count; i++)
                {
                    if (activeDTypes.List[i].IsSubclassOf(dependencyType))
                    {
                        ((ClassHandlersStore)dependencyTypedClassListeners[activeDTypes.List[i]]).UpdateSubClassHandlers(routedEvent, baseClassListeners);
                    }
                }
            }
        }

        internal static Telerik.Windows.RoutedEvent RegisterRoutedEvent(string name, RoutingStrategy routingStrategy, Type handlerType, Type ownerType)
        {
            lock (synchronized)
            {
                Telerik.Windows.RoutedEvent routedEvent = new Telerik.Windows.RoutedEvent(name, routingStrategy, handlerType, ownerType);
                routedEventsCount++;
                AddOwner(routedEvent, ownerType);
                return routedEvent;
            }
        }
    }
}

