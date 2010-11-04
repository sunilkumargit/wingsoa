namespace Telerik.Windows.Controls
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Windows.Input;
    using Telerik.Windows;
    using Telerik.Windows.Controls.Design;

    public class CommandBinding
    {
        public event CanExecuteRoutedEventHandler CanExecute;

        public event ExecutedRoutedEventHandler Executed;

        public event CanExecuteRoutedEventHandler PreviewCanExecute;

        public event ExecutedRoutedEventHandler PreviewExecuted;

        public CommandBinding()
        {
        }

        public CommandBinding(ICommand command) : this(command, null, null)
        {
        }

        public CommandBinding(ICommand command, ExecutedRoutedEventHandler executed) : this(command, executed, null)
        {
        }

        public CommandBinding(ICommand command, ExecutedRoutedEventHandler executed, CanExecuteRoutedEventHandler canExecute)
        {
            command.TestNotNull("command");
            this.Command = command;
            if (executed != null)
            {
                this.Executed = (ExecutedRoutedEventHandler) Delegate.Combine(this.Executed, executed);
            }
            if (canExecute != null)
            {
                this.CanExecute = (CanExecuteRoutedEventHandler) Delegate.Combine(this.CanExecute, canExecute);
            }
        }

        private bool CheckCanExecute(object sender, ExecutedRoutedEventArgs e)
        {
            CanExecuteRoutedEventArgs args = new CanExecuteRoutedEventArgs(e.Command, e.Parameter) {
                RoutedEvent = CommandManager.CanExecuteEvent,
                Source = e.Source
            };
            this.OnCanExecute(sender, args);
            return args.CanExecute;
        }

        internal void OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (!e.Handled)
            {
                if (e.RoutedEvent == CommandManager.CanExecuteEvent)
                {
                    if (this.CanExecute != null)
                    {
                        this.CanExecute(sender, e);
                        if (e.CanExecute)
                        {
                            e.Handled = true;
                        }
                    }
                    else if (!e.CanExecute && (this.Executed != null))
                    {
                        e.CanExecute = true;
                        e.Handled = true;
                    }
                }
                else if (this.PreviewCanExecute != null)
                {
                    this.PreviewCanExecute(sender, e);
                    if (e.CanExecute)
                    {
                        e.Handled = true;
                    }
                }
            }
        }

        internal void OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (!e.Handled)
            {
                if (e.RoutedEvent == CommandManager.ExecutedEvent)
                {
                    if ((this.Executed != null) && this.CheckCanExecute(sender, e))
                    {
                        this.Executed(sender, e);
                        e.Handled = true;
                    }
                }
                else if ((this.PreviewExecuted != null) && this.CheckCanExecute(sender, e))
                {
                    this.PreviewExecuted(sender, e);
                    e.Handled = true;
                }
            }
        }

        [TypeConverter(typeof(CommandConverter))]
        public ICommand Command { get; set; }
    }
}

