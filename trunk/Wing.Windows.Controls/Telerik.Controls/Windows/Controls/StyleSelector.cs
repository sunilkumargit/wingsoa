namespace Telerik.Windows.Controls
{
    using System;
    using System.Windows;

    public class StyleSelector
    {
        public virtual Style SelectStyle(object item, DependencyObject container)
        {
            return null;
        }
    }
}

