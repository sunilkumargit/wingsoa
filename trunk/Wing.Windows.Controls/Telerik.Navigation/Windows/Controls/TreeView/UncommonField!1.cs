namespace Telerik.Windows.Controls.TreeView
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;

    internal class UncommonField<T>
    {
        private T defaultValue;
        private static readonly DependencyProperty HolderProperty;

        static UncommonField()
        {
            UncommonField<T>.HolderProperty = DependencyProperty.RegisterAttached("Holder", typeof(object), typeof(UncommonField<T>), null);
        }

        public UncommonField() : this(default(T))
        {
        }

        public UncommonField(T defaultValue)
        {
            this.defaultValue = defaultValue;
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public void ClearValue(DependencyObject instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            instance.ClearValue(UncommonField<T>.HolderProperty);
        }

        public T GetValue(DependencyObject instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            if (instance.ReadLocalValue(UncommonField<T>.HolderProperty) == DependencyProperty.UnsetValue)
            {
                return this.defaultValue;
            }
            return (T) instance.GetValue(UncommonField<T>.HolderProperty);
        }

        public void SetValue(DependencyObject instance, T value)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            if (!object.ReferenceEquals(value, this.defaultValue))
            {
                instance.SetValue(UncommonField<T>.HolderProperty, value);
            }
            else
            {
                instance.ClearValue(UncommonField<T>.HolderProperty);
            }
        }
    }
}

