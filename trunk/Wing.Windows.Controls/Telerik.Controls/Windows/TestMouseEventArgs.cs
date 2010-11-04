namespace Telerik.Windows
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Input;
    using Telerik.Windows.Input;

    internal class TestMouseEventArgs : RoutedEventArgs
    {
        public TestMouseEventArgs(MouseButton button, ModifierKeys modifiers) : this(button, modifiers, null)
        {
        }

        public TestMouseEventArgs(MouseButton button, ModifierKeys modifiers, object customValue)
        {
            this.Button = button;
            this.Modifiers = modifiers;
            this.CustomValue = customValue;
        }

        public MouseButton Button { get; set; }

        public object CustomValue { get; set; }

        public ModifierKeys Modifiers { get; set; }
    }
}

