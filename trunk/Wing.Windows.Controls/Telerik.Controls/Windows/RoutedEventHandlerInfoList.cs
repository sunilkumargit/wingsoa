namespace Telerik.Windows
{
    using System;
    using System.Runtime.CompilerServices;

    internal class RoutedEventHandlerInfoList
    {
        internal bool Contains(RoutedEventHandlerInfoList handlers)
        {
            for (RoutedEventHandlerInfoList list = this; list != null; list = list.Next)
            {
                if (list == handlers)
                {
                    return true;
                }
            }
            return false;
        }

        internal RoutedEventHandlerInfo[] Handlers { get; set; }

        internal RoutedEventHandlerInfoList Next { get; set; }
    }
}

