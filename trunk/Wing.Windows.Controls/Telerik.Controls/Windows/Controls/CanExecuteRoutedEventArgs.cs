namespace Telerik.Windows.Controls
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Input;
    using Telerik.Windows;

    public sealed class CanExecuteRoutedEventArgs : RadRoutedEventArgs
    {
        internal CanExecuteRoutedEventArgs(ICommand command, object parameter)
        {
            command.TestNotNull("command");
            this.Command = command;
            this.Parameter = parameter;
        }

        protected override void InvokeEventHandler(Delegate genericHandler, object target)
        {
            CanExecuteRoutedEventHandler handler = (CanExecuteRoutedEventHandler) genericHandler;
            handler(target as DependencyObject, this);
        }

        public bool CanExecute { get; set; }

        public ICommand Command { get; private set; }

        public object Parameter { get; private set; }
    }
}

