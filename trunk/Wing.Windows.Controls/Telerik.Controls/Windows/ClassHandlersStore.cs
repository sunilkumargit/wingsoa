namespace Telerik.Windows
{
    using MS.Utility;
    using System;

    internal class ClassHandlersStore
    {
        private ItemStructList<ClassHandlers> eventHandlersList;

        internal ClassHandlersStore(int size)
        {
            this.eventHandlersList = new ItemStructList<ClassHandlers>(size);
        }

        internal RoutedEventHandlerInfoList AddToExistingHandlers(int index, Delegate handler, bool handledEventsToo)
        {
            RoutedEventHandlerInfo info = new RoutedEventHandlerInfo(handler, handledEventsToo);
            RoutedEventHandlerInfoList handlers = this.eventHandlersList.List[index].Handlers;
            if ((handlers == null) || !this.eventHandlersList.List[index].HasSelfHandlers)
            {
                handlers = new RoutedEventHandlerInfoList();
                handlers.Handlers = new RoutedEventHandlerInfo[] { info };
                handlers.Next = this.eventHandlersList.List[index].Handlers;
                this.eventHandlersList.List[index].Handlers = handlers;
                this.eventHandlersList.List[index].HasSelfHandlers = true;
                return handlers;
            }
            int length = handlers.Handlers.Length;
            RoutedEventHandlerInfo[] destinationArray = new RoutedEventHandlerInfo[length + 1];
            Array.Copy(handlers.Handlers, 0, destinationArray, 0, length);
            destinationArray[length] = info;
            handlers.Handlers = destinationArray;
            return handlers;
        }

        internal int CreateHandlersLink(RoutedEvent routedEvent, RoutedEventHandlerInfoList handlers)
        {
            ClassHandlers item = new ClassHandlers {
                RoutedEvent = routedEvent,
                Handlers = handlers,
                HasSelfHandlers = false
            };
            this.eventHandlersList.Add(item);
            return (this.eventHandlersList.Count - 1);
        }

        internal RoutedEventHandlerInfoList GetExistingHandlers(int index)
        {
            return this.eventHandlersList.List[index].Handlers;
        }

        internal int GetHandlersIndex(RoutedEvent routedEvent)
        {
            for (int i = 0; i < this.eventHandlersList.Count; i++)
            {
                if (this.eventHandlersList.List[i].RoutedEvent == routedEvent)
                {
                    return i;
                }
            }
            return -1;
        }

        internal void UpdateSubClassHandlers(RoutedEvent routedEvent, RoutedEventHandlerInfoList baseClassListeners)
        {
            int handlersIndex = this.GetHandlersIndex(routedEvent);
            if (handlersIndex != -1)
            {
                bool hasSelfHandlers = this.eventHandlersList.List[handlersIndex].HasSelfHandlers;
                RoutedEventHandlerInfoList handlers = hasSelfHandlers ? this.eventHandlersList.List[handlersIndex].Handlers.Next : this.eventHandlersList.List[handlersIndex].Handlers;
                bool flag = false;
                if (handlers != null)
                {
                    if ((baseClassListeners.Next != null) && baseClassListeners.Next.Contains(handlers))
                    {
                        flag = true;
                    }
                }
                else
                {
                    flag = true;
                }
                if (flag)
                {
                    if (hasSelfHandlers)
                    {
                        this.eventHandlersList.List[handlersIndex].Handlers.Next = baseClassListeners;
                    }
                    else
                    {
                        this.eventHandlersList.List[handlersIndex].Handlers = baseClassListeners;
                    }
                }
            }
        }
    }
}

