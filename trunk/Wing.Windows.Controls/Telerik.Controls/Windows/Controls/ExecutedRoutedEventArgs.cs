namespace Telerik.Windows.Controls
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Input;
    using Telerik.Windows;

    public sealed class ExecutedRoutedEventArgs : RadRoutedEventArgs
    {
        internal ExecutedRoutedEventArgs(ICommand command, object parameter)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }
            this.Command = command;
            this.Parameter = parameter;
        }

        protected override void InvokeEventHandler(Delegate genericHandler, object target)
        {
            ExecutedRoutedEventHandler handler = (ExecutedRoutedEventHandler) genericHandler;
            handler(target as DependencyObject, this);
        }

        public ICommand Command { get; private set; }

        public object Parameter { get; private set; }
    }
}

