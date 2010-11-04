namespace Telerik.Windows.Controls
{
    using System;
    using System.Collections;
    using System.Runtime.CompilerServices;
    using Telerik.Windows;

    public class DropDownEventArgs : RadRoutedEventArgs
    {
        public DropDownEventArgs(RoutedEvent routedEvent, object source, IEnumerable dropDownItems) : base(routedEvent, source)
        {
            this.DropDownItemsSource = dropDownItems;
        }

        protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget)
        {
            (genericHandler as DropDownEventHandler)(genericTarget, this);
        }

        public IEnumerable DropDownItemsSource { get; set; }
    }
}

