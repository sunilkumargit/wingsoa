namespace Telerik.Windows.Controls
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using Telerik.Windows;
    using Telerik.Windows.Controls.Design;

    [TypeConverter(typeof(CommandConverter))]
    public class RoutedUICommand : RoutedCommand
    {
        protected RoutedUICommand()
        {
            this.Text = string.Empty;
        }

        public RoutedUICommand(string text, string name, Type ownerType) : this(text, name, ownerType, null)
        {
        }

        public RoutedUICommand(string text, string name, Type ownerType, InputGestureCollection inputGestures) : base(name, ownerType, inputGestures)
        {
            text.TestNotNull("text");
            this.Text = text;
        }

        public string Text { get; set; }
    }
}

