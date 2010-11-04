namespace Telerik.Windows.Controls
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Markup;

    [ContentProperty("ContainerStyle")]
    public class ToolBarContainerStyle
    {
        public Style ContainerStyle { get; set; }

        public string TypeName { get; set; }
    }
}

