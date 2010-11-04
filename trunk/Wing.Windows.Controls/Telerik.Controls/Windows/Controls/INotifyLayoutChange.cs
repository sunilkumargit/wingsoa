namespace Telerik.Windows.Controls
{
    using System;

    public interface INotifyLayoutChange
    {
        event EventHandler LayoutChangeEnded;

        event EventHandler LayoutChangeStarted;

        bool IsLayoutChanging { get; }
    }
}

