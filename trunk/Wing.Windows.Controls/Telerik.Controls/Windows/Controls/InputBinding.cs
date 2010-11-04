namespace Telerik.Windows.Controls
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Input;
    using Telerik.Windows;
    using Telerik.Windows.Controls.Design;

    public class InputBinding : DependencyObject, ICommandSource
    {
        private ICommand command;
        private object commandParameter;
        private UIElement commandTarget;
        private InputGesture gesture;

        static InputBinding()
        {
            DataLock = new object();
        }

        protected InputBinding()
        {
        }

        public InputBinding(ICommand command, InputGesture inputGesture)
        {
            command.TestNotNull("command");
            inputGesture.TestNotNull("inputGesture");
            this.command = command;
            this.gesture = inputGesture;
        }

        [TypeConverter(typeof(CommandConverter))]
        public ICommand Command
        {
            get
            {
                return this.command;
            }
            set
            {
                value.TestNotNull("value");
                lock (DataLock)
                {
                    this.command = value;
                }
            }
        }

        public object CommandParameter
        {
            get
            {
                return this.commandParameter;
            }
            set
            {
                lock (DataLock)
                {
                    this.commandParameter = value;
                }
            }
        }

        public UIElement CommandTarget
        {
            get
            {
                return this.commandTarget;
            }
            set
            {
                lock (DataLock)
                {
                    this.commandTarget = value;
                }
            }
        }

        internal static object DataLock{get;set;}

        public virtual InputGesture Gesture
        {
            get
            {
                return this.gesture;
            }
            set
            {
                value.TestNotNull("value");
                lock (DataLock)
                {
                    this.gesture = value;
                }
            }
        }
    }
}

