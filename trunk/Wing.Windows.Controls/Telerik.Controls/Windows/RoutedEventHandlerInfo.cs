namespace Telerik.Windows
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Windows;

    [StructLayout(LayoutKind.Sequential)]
    internal struct RoutedEventHandlerInfo
    {
        internal RoutedEventHandlerInfo(Delegate handler, bool handledEventsToo)
        {
            this = new RoutedEventHandlerInfo();
            this.Handler = handler;
            this.InvokeHandledEventsToo = handledEventsToo;
        }

        public Delegate Handler { get; private set; }
        public bool InvokeHandledEventsToo { get; private set; }
        public static bool operator ==(RoutedEventHandlerInfo handlerInfo1, RoutedEventHandlerInfo handlerInfo2)
        {
            return handlerInfo1.Equals(handlerInfo2);
        }

        public static bool operator !=(RoutedEventHandlerInfo handlerInfo1, RoutedEventHandlerInfo handlerInfo2)
        {
            return !handlerInfo1.Equals(handlerInfo2);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return (((obj != null) && (obj is RoutedEventHandlerInfo)) && this.Equals((RoutedEventHandlerInfo) obj));
        }

        public bool Equals(RoutedEventHandlerInfo handlerInfo)
        {
            return ((this.Handler == handlerInfo.Handler) && (this.InvokeHandledEventsToo == handlerInfo.InvokeHandledEventsToo));
        }

        internal void InvokeHandler(object target, RadRoutedEventArgs routedEventArgs)
        {
            if (!routedEventArgs.Handled || this.InvokeHandledEventsToo)
            {
                if (this.Handler is RoutedEventHandler)
                {
                    ((RoutedEventHandler) this.Handler)(target, routedEventArgs);
                }
                else
                {
                    routedEventArgs.InvokeHandler(this.Handler, target);
                }
            }
        }
    }
}

