namespace Telerik.Windows.Controls.Common
{
    using System;
    using System.Windows;

    internal interface IMouseEventArgs
    {
        Point GetPosition(UIElement relativeTo);

        object OriginalSource { get; }
    }
}

