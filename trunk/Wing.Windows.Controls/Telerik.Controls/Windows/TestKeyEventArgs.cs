namespace Telerik.Windows
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Input;

    internal class TestKeyEventArgs : RoutedEventArgs
    {
        public TestKeyEventArgs(System.Windows.Input.Key key, ModifierKeys modifiers)
        {
            this.Key = key;
            this.Modifiers = modifiers;
        }

        public System.Windows.Input.Key Key { get; set; }

        public ModifierKeys Modifiers { get; set; }
    }
}

