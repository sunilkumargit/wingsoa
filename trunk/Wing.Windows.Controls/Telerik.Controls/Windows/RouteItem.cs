namespace Telerik.Windows
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct RouteItem
    {
        private RoutedEventHandlerInfo routedEventHandlerInfo;
        internal RouteItem(object target, RoutedEventHandlerInfo routedEventHandlerInfo)
        {
            this = new RouteItem();
            this.Target = target;
            this.routedEventHandlerInfo = routedEventHandlerInfo;
        }

        internal object Target { get; private set; }
        public static bool operator ==(RouteItem routeItem1, RouteItem routeItem2)
        {
            return routeItem1.Equals(routeItem2);
        }

        public static bool operator !=(RouteItem routeItem1, RouteItem routeItem2)
        {
            return !routeItem1.Equals(routeItem2);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object o)
        {
            return this.Equals((RouteItem) o);
        }

        public bool Equals(RouteItem routeItem)
        {
            return ((routeItem.Target == this.Target) && (routeItem.routedEventHandlerInfo == this.routedEventHandlerInfo));
        }

        internal void InvokeHandler(RadRoutedEventArgs routedEventArgs)
        {
            this.routedEventHandlerInfo.InvokeHandler(this.Target, routedEventArgs);
        }
    }
}

