namespace Telerik.Windows
{
    using System;

    public class RadRoutedPropertyChangedEventArgs<T> : RadRoutedEventArgs
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

