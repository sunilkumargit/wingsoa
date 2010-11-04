namespace Telerik.Windows.Controls
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Input;
    using Telerik.Windows;
    using Telerik.Windows.Controls.Design;

    [TypeConverter(typeof(CommandConverter))]
    public class RoutedCommand : ICommand
    {
        private InputGestureCollection inputGestureCollection;
        private string name;
        private Type ownerType;
        private static Dictionary<string, RoutedCommand> registeredCommands = new Dictionary<string, RoutedCommand>();

        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        protected RoutedCommand()
        {
            this.Name = string.Empty;
        }

        public RoutedCommand(string name, Type ownerType) : this(name, ownerType, null)
        {
        }

        public RoutedCommand(string name, Type ownerType, InputGestureCollection inputGestures)
        {
            name.TestNotNull("name");
            name.TestNotEmptyString("name");
            ownerType.TestNotNull("ownerType");
            this.Name = name;
            this.OwnerType = ownerType;
            this.inputGestureCollection = inputGestures;
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public bool CanExecute(object parameter, UIElement target)
        {
            if (target == null)
            {
                return false;
            }
            CanExecuteRoutedEventArgs args = new CanExecuteRoutedEventArgs(this, parameter) {
                RoutedEvent = CommandManager.PreviewCanExecuteEvent
            };
            target.RaiseEvent(args);
            if (!args.Handled)
            {
                args.RoutedEvent = CommandManager.CanExecuteEvent;
                target.RaiseEvent(args);
            }
            return args.CanExecute;
        }

        public void Execute(object parameter, UIElement target)
        {
            if (target == null)
            {
                target = FocusManager.GetFocusedElement() as UIElement;
            }
            this.ExecuteImpl(parameter, target);
        }

        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId="userInitiated")]
        private bool ExecuteImpl(object parameter, UIElement target)
        {
            if (target == null)
            {
                return false;
            }
            UIElement element2 = target;
            ExecutedRoutedEventArgs args = new ExecutedRoutedEventArgs(this, parameter) {
                RoutedEvent = CommandManager.PreviewExecutedEvent
            };
            if (element2 != null)
            {
                element2.RaiseEvent(args);
            }
            if (!args.Handled)
            {
                args.RoutedEvent = CommandManager.ExecutedEvent;
                if (element2 != null)
                {
                    element2.RaiseEvent(args);
                }
            }
            return args.Handled;
        }

        internal static RoutedCommand GetRegisteredCommand(string name)
        {
            if (registeredCommands.ContainsKey(name))
            {
                return registeredCommands[name];
            }
            return null;
        }

        private static void Register(RoutedCommand command)
        {
            if (!string.IsNullOrEmpty(command.TypeQualifiedName))
            {
                registeredCommands[command.TypeQualifiedName] = command;
            }
        }

        bool ICommand.CanExecute(object parameter)
        {
            return this.CanExecute(parameter, Keyboard.FocusedElement);
        }

        void ICommand.Execute(object parameter)
        {
            this.Execute(parameter, Keyboard.FocusedElement);
        }

        private static void Unregister(RoutedCommand command)
        {
            if (!string.IsNullOrEmpty(command.TypeQualifiedName) && registeredCommands.ContainsKey(command.TypeQualifiedName))
            {
                registeredCommands.Remove(command.TypeQualifiedName);
            }
        }

        public InputGestureCollection InputGestures
        {
            get
            {
                if (this.inputGestureCollection == null)
                {
                    this.inputGestureCollection = new InputGestureCollection();
                }
                return this.inputGestureCollection;
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                if (this.name != value)
                {
                    Unregister(this);
                    this.name = value;
                    Register(this);
                }
            }
        }

        public Type OwnerType
        {
            get
            {
                return this.ownerType;
            }
            private set
            {
                if (this.ownerType != value)
                {
                    Unregister(this);
                    this.ownerType = value;
                    Register(this);
                }
            }
        }

        private string TypeQualifiedName
        {
            get
            {
                if (this.OwnerType == null)
                {
                    return this.Name;
                }
                return (this.OwnerType.Name + "." + this.Name);
            }
        }

        private static class Keyboard
        {
            public static UIElement FocusedElement
            {
                get
                {
                    return (FocusManager.GetFocusedElement() as UIElement);
                }
            }
        }
    }
}

