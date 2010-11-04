namespace Telerik.Windows.Controls
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.CompilerServices;

    [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances", Justification="In WPF most events have an own handler, we follow this")]
    public delegate void EditorPrepareEventHandler(object sender, EditorPrepareEventArgs e);
}

