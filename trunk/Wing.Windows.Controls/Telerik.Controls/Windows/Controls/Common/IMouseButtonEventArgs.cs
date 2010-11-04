namespace Telerik.Windows.Controls.Common
{
    using System;

    internal interface IMouseButtonEventArgs : IMouseEventArgs
    {
        bool Handled { get; set; }
    }
}

