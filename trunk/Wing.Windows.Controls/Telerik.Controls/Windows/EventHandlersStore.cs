namespace Telerik.Windows
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    internal class EventHandlersStore
    {
        private Dictionary<int, object> entries;

        public EventHandlersStore()
        {
            this.entries = new Dictionary<int, object>();
        }

        public EventHandlersStore(EventHandlersStore source)
        {
            this.entries = source.entries;
        }

        public void AddRoutedEventHandler(RoutedEvent routedEvent, Delegate handler, bool handledEventsToo)
        {
            EventManager.CheckParameters(routedEvent, handler);
            RoutedEventHandlerInfo info = new RoutedEventHandlerInfo(handler, handledEventsToo);
            List<RoutedEventHandlerInfo> list = this[routedEvent];
            if (list == null)
            {
                this.entries[routedEvent.GlobalIndex] = list = new List<RoutedEventHandlerInfo>();
            }
            list.Add(info);
        }

        public void RemoveRoutedEventHandler(RoutedEvent routedEvent, Delegate handler)
        {
            EventManager.CheckParameters(routedEvent, handler);
            List<RoutedEventHandlerInfo> list = this[routedEvent];
            if ((list != null) && (list.Count > 0))
            {
                if (list.Count == 1)
                {
                    RoutedEventHandlerInfo info2 = list[0];
                    if (info2.Handler == handler)
                    {
                        this.entries[routedEvent.GlobalIndex] = null;
                        return;
                    }
                }
                for (int i = 0; i < list.Count; i++)
                {
                    RoutedEventHandlerInfo info = list[i];
                    if (info.Handler == handler)
                    {
                        list.RemoveAt(i);
                        return;
                    }
                }
            }
        }

        public int Count
        {
            get
            {
                return this.entries.Count;
            }
        }

        public List<RoutedEventHandlerInfo> this[RoutedEvent key]
        {
            get
            {
                if (this.entries.ContainsKey(key.GlobalIndex))
                {
                    return (List<RoutedEventHandlerInfo>) this.entries[key.GlobalIndex];
                }
                return null;
            }
        }
    }
}

