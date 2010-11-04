namespace Telerik.Windows.Controls
{
    using System;
    using System.Windows;
    using System.Windows.Input;

    public interface ICommandSource
    {
        ICommand Command { get; }

        object CommandParameter { get; }

        UIElement CommandTarget { get; }
    }
}

