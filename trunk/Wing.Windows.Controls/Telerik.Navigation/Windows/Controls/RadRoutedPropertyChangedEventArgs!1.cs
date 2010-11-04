namespace Telerik.Windows.Controls
{
    using System;
    using Telerik.Windows;

    internal class RadRoutedPropertyChangedEventArgs<T> : RadRoutedEventArgs
    {
        private T newValue;
        private T oldValue;

        public RadRoutedPropertyChangedEventArgs(T oldValue, T newValue)
        {
            this.oldValue = oldValue;
            this.newValue = newValue;
        }

        public RadRoutedPropertyChangedEventArgs(T oldValue, T newValue, RoutedEvent routedEvent) : this(oldValue, newValue)
        {
            base.RoutedEvent = routedEvent;
        }

        protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget)
        {
            RadRoutedPropertyChangedEventHandler<T> handler = (RadRoutedPropertyChangedEventHandler<T>) genericHandler;
            handler(genericTarget, (Telerik.Windows.Controls.RadRoutedPropertyChangedEventArgs<T>) this);
        }

        public T NewValue
        {
            get
            {
                return this.newValue;
            }
        }

        public T OldValue
        {
            get
            {
                return this.oldValue;
            }
        }
    }
}

