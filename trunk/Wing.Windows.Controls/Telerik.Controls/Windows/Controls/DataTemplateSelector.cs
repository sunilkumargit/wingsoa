namespace Telerik.Windows.Controls
{
    using System;
    using System.Windows;

    public class DataTemplateSelector
    {
        public virtual DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return null;
        }
    }
}

