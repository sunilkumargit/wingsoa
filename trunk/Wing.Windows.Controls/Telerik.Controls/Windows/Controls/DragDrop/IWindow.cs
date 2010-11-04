namespace Telerik.Windows.Controls.DragDrop
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("Microsoft.Design", "CA1064:ExceptionsShouldBePublic")]
    internal interface IWindow
    {
        bool IsOpen { get; }

        int Z { get; }
    }
}

