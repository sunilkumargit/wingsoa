namespace Telerik.Windows.Controls.TreeView
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.CompilerServices;

    [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances", Justification="We have agreed to use custom event handlers for brevity and readability.")]
    public delegate void RequestBringIntoViewEventHandler(object sender, RequestBringIntoViewEventArgs e);
}

