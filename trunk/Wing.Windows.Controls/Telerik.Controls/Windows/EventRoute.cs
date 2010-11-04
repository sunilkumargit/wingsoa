namespace Telerik.Windows
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using Telerik.Windows.Controls;

    internal sealed class EventRoute
    {
        private List<RouteItem> routeItemList;
        private List<SourceItem> sourceItemList;

        public EventRoute(Telerik.Windows.RoutedEvent routedEvent)
        {
            CheckParameters(routedEvent);
            this.RoutedEvent = routedEvent;
            this.routeItemList = new List<RouteItem>(0x10);
            this.sourceItemList = new List<SourceItem>(0x10);
        }

        public void Add(object target, Delegate handler, bool handledEventsToo)
        {
            CheckParameters(target, handler);
            RouteItem item = new RouteItem(target, new RoutedEventHandlerInfo(handler, handledEventsToo));
            this.routeItemList.Add(item);
        }

        internal void AddSource(object source)
        {
            int count = this.routeItemList.Count;
            this.sourceItemList.Add(new SourceItem(count, source));
        }

        private static void CheckParameters(Telerik.Windows.RoutedEvent routedEvent)
        {
            if (routedEvent == null)
            {
                throw new ArgumentNullException("routedEvent");
            }
        }

        private static void CheckParameters(object target, Delegate handler)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }
        }

        private void CheckParameters(object source, RadRoutedEventArgs args)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }
            if (args.Source == null)
            {
                throw new ArgumentException(Telerik.Windows.Controls.SR.Get("SourceNotSet", new object[0]));
            }
            if (args.RoutedEvent != this.RoutedEvent)
            {
                throw new ArgumentException(Telerik.Windows.Controls.SR.Get("Mismatched_RoutedEvent", new object[0]));
            }
        }

        internal void Clear()
        {
            this.RoutedEvent = null;
            this.routeItemList.Clear();
            this.sourceItemList.Clear();
        }

        private object GetBubbleSource(int index, out int endIndex)
        {
            if (this.sourceItemList.Count == 0)
            {
                endIndex = this.routeItemList.Count;
                return null;
            }
            SourceItem item7 = this.sourceItemList[0];
            if (index < item7.StartIndex)
            {
                SourceItem item6 = this.sourceItemList[0];
                endIndex = item6.StartIndex;
                return null;
            }
            for (int i = 0; i < (this.sourceItemList.Count - 1); i++)
            {
                SourceItem item5 = this.sourceItemList[i];
                if (index >= item5.StartIndex)
                {
                    SourceItem item4 = this.sourceItemList[i + 1];
                    if (index < item4.StartIndex)
                    {
                        SourceItem item3 = this.sourceItemList[i + 1];
                        endIndex = item3.StartIndex;
                        SourceItem item2 = this.sourceItemList[i];
                        return item2.Source;
                    }
                }
            }
            endIndex = this.routeItemList.Count;
            SourceItem item = this.sourceItemList[this.sourceItemList.Count - 1];
            return item.Source;
        }

        private object GetTunnelSource(int index, out int startIndex)
        {
            if (this.sourceItemList.Count == 0)
            {
                startIndex = 0;
                return null;
            }
            SourceItem item7 = this.sourceItemList[0];
            if (index < item7.StartIndex)
            {
                startIndex = 0;
                return null;
            }
            for (int i = 0; i < (this.sourceItemList.Count - 1); i++)
            {
                SourceItem item6 = this.sourceItemList[i];
                if (index >= item6.StartIndex)
                {
                    SourceItem item5 = this.sourceItemList[i + 1];
                    if (index < item5.StartIndex)
                    {
                        SourceItem item4 = this.sourceItemList[i];
                        startIndex = item4.StartIndex;
                        SourceItem item3 = this.sourceItemList[i];
                        return item3.Source;
                    }
                }
            }
            SourceItem item2 = this.sourceItemList[this.sourceItemList.Count - 1];
            startIndex = item2.StartIndex;
            SourceItem item = this.sourceItemList[this.sourceItemList.Count - 1];
            return item.Source;
        }

        internal void InvokeHandlers(object source, RadRoutedEventArgs args)
        {
            this.InvokeHandlersImpl(source, args, false);
        }

        private void InvokeHandlersImpl(object source, RadRoutedEventArgs args, bool raisedAgain)
        {
            this.CheckParameters(source, args);
            if ((args.RoutedEvent.RoutingStrategy == RoutingStrategy.Bubble) || (args.RoutedEvent.RoutingStrategy == RoutingStrategy.Direct))
            {
                int endIndex = 0;
                for (int i = 0; i < this.routeItemList.Count; i++)
                {
                    if ((i >= endIndex) && !raisedAgain)
                    {
                        args.Source = this.GetBubbleSource(i, out endIndex) ?? source;
                    }
                    this.routeItemList[i].InvokeHandler(args);
                }
            }
            else
            {
                int num3;
                int count = this.routeItemList.Count;
                for (int j = this.routeItemList.Count - 1; j >= 0; j = num3)
                {
                    RouteItem item5 = this.routeItemList[j];
                    object target = item5.Target;
                    num3 = j;
                    while (num3 >= 0)
                    {
                        RouteItem item4 = this.routeItemList[num3];
                        if (item4.Target != target)
                        {
                            break;
                        }
                        num3--;
                    }
                    for (int k = num3 + 1; k <= j; k++)
                    {
                        if (k < count)
                        {
                            args.Source = this.GetTunnelSource(k, out count) ?? source;
                        }
                        this.routeItemList[k].InvokeHandler(args);
                    }
                }
            }
        }

        internal Telerik.Windows.RoutedEvent RoutedEvent { get; set; }

        [StructLayout(LayoutKind.Sequential)]
        private struct BranchNode
        {
            public object Node;
            public object Source;
        }
    }
}

