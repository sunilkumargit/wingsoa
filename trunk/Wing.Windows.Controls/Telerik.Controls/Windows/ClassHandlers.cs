namespace Telerik.Windows
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct ClassHandlers
    {
        internal Telerik.Windows.RoutedEvent RoutedEvent;
        internal RoutedEventHandlerInfoList Handlers;
        internal bool HasSelfHandlers;
        public static bool operator ==(ClassHandlers classHandlers1, ClassHandlers classHandlers2)
        {
            return classHandlers1.Equals(classHandlers2);
        }

        public static bool operator !=(ClassHandlers classHandlers1, ClassHandlers classHandlers2)
        {
            return !classHandlers1.Equals(classHandlers2);
        }

        public override bool Equals(object o)
        {
            return this.Equals((ClassHandlers) o);
        }

        public bool Equals(ClassHandlers classHandlers)
        {
            return ((classHandlers.RoutedEvent == this.RoutedEvent) && (classHandlers.Handlers == this.Handlers));
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}

