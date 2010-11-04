namespace Telerik.Windows.Controls
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Markup;

    [ContentProperty("ContainerStyles")]
    public class ToolBarContainerStyleSelector : StyleSelector
    {
        public ToolBarContainerStyleSelector()
        {
            this.ContainerStyles = new List<ToolBarContainerStyle>();
        }

        internal Style GetStyle(string typeName)
        {
            foreach (object contentItem in this.ContainerStyles)
            {
                ToolBarContainerStyle containerStyle = contentItem as ToolBarContainerStyle;
                if ((containerStyle != null) && (containerStyle.TypeName == typeName))
                {
                    return containerStyle.ContainerStyle;
                }
            }
            return null;
        }

        public override Style SelectStyle(object item, DependencyObject container)
        {
            string typeName = item.GetType().Name;
            Style style = this.GetStyle(typeName);
            if ((style == null) && !object.ReferenceEquals(item, container))
            {
                typeName = container.GetType().Name;
                style = this.GetStyle(typeName);
            }
            return style;
        }

        public IList ContainerStyles { get; private set; }
    }
}

