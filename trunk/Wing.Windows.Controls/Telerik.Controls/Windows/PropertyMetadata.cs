namespace Telerik.Windows
{
    using System;
    using System.Globalization;
    using System.Windows;

    public class PropertyMetadata : System.Windows.PropertyMetadata
    {
        internal static readonly DependencyProperty CanCoerceReadOnlyPropertyProperty = DependencyProperty.RegisterAttached("CanCoerceReadOnlyProperty", typeof(bool), typeof(DependencyObject), null);
        private CoerceValueCallback coerceCallback;
        internal static readonly DependencyProperty IsCoercingProperty = DependencyProperty.RegisterAttached("IsCoercing", typeof(bool), typeof(DependencyObject), null);
        private System.Windows.PropertyChangedCallback propertyChangedCallback;

        internal PropertyMetadata() : base(Create(null, null, null))
        {
        }

        public PropertyMetadata(object defaultValue) : base(defaultValue, Create(null, null, null))
        {
        }

        public PropertyMetadata(System.Windows.PropertyChangedCallback propertyChangedCallback) : base(Create(propertyChangedCallback, null, null))
        {
            this.propertyChangedCallback = propertyChangedCallback;
        }

        public PropertyMetadata(object defaultValue, System.Windows.PropertyChangedCallback propertyChangedCallback) : base(defaultValue, Create(propertyChangedCallback, null, null))
        {
            this.propertyChangedCallback = propertyChangedCallback;
        }

        public PropertyMetadata(object defaultValue, System.Windows.PropertyChangedCallback propertyChangedCallback, CoerceValueCallback coerceValueCallback) : base(defaultValue, Create(propertyChangedCallback, coerceValueCallback, null))
        {
            this.propertyChangedCallback = propertyChangedCallback;
            this.coerceCallback = coerceValueCallback;
        }

        internal PropertyMetadata(object defaultValue, System.Windows.PropertyChangedCallback propertyChangedCallback, CoerceValueCallback coerceValueCallback, ValidateValueCallback validateValueCallback) : base(defaultValue, Create(propertyChangedCallback, coerceValueCallback, validateValueCallback))
        {
            this.propertyChangedCallback = propertyChangedCallback;
            this.coerceCallback = coerceValueCallback;
        }

        internal static bool CoerceValue(DependencyObject d, DependencyPropertyChangedEventArgs e, CoerceValueCallback callback)
        {
            if (callback == null)
            {
                return true;
            }
            if ((bool) d.GetValue(IsCoercingProperty))
            {
                return false;
            }
            object coercedValue = callback(d, e.NewValue);
            if (!object.Equals(coercedValue, e.NewValue))
            {
                d.SetValue(IsCoercingProperty, true);
                d.SetValue(e.Property, coercedValue);
                d.SetValue(IsCoercingProperty, false);
            }
            return !object.Equals(coercedValue, e.OldValue);
        }

        private static System.Windows.PropertyChangedCallback Create(System.Windows.PropertyChangedCallback propertyChangedCallback, CoerceValueCallback coerceValueCallback, ValidateValueCallback validateValueCallback)
        {
            return delegate (DependencyObject d, DependencyPropertyChangedEventArgs e) {
                if ((!((bool) d.GetValue(CanCoerceReadOnlyPropertyProperty)) && DependencyPropertyExtensions.IsDependencyPropertyReadOnly(e.Property)) && !((bool) d.GetValue(DependencyObjectExtensions.PropertyChangesAllowedProperty)))
                {
                    throw new InvalidOperationException("Cannot change read-only DependencyProperty");
                }
                if ((validateValueCallback != null) && !validateValueCallback(e.NewValue))
                {
                    throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "'{0}' is not a valid value for property '{1}'.", new object[] { e.NewValue, e.Property }));
                }
                bool shouldCallPropertyChanged = propertyChangedCallback != null;
                if (coerceValueCallback != null)
                {
                    shouldCallPropertyChanged = CoerceValue(d, e, coerceValueCallback);
                }
                if ((propertyChangedCallback != null) && shouldCallPropertyChanged)
                {
                    propertyChangedCallback(d, e);
                }
            };
        }

        internal CoerceValueCallback CoerceCallback
        {
            get
            {
                return this.coerceCallback;
            }
        }

        internal System.Windows.PropertyChangedCallback PropertyChangedCallback
        {
            get
            {
                return this.propertyChangedCallback;
            }
        }
    }
}

