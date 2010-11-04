namespace Telerik.Windows.Controls
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.CompilerServices;

    [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances", Justification="We agreed to use custom handlers for brevity.")]
    public delegate void DropDownEventHandler(object sender, DropDownEventArgs e);
}

