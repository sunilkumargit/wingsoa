namespace Telerik.Windows
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Windows;

    public static class DependencyPropertyExtensions
    {
        private static Dictionary<DependencyProperty, CoerceValueCallback> coerceValueCallbackCache = new Dictionary<DependencyProperty, CoerceValueCallback>();
        private static List<DependencyProperty> readonlyDP = new List<DependencyProperty>();

        internal static bool IsDependencyPropertyReadOnly(DependencyProperty property)
        {
            return ((property != null) && readonlyDP.Contains(property));
        }

        public static DependencyProperty Register(string name, Type propertyType, Type ownerType, Telerik.Windows.PropertyMetadata typeMetadata)
        {
            DependencyProperty dp = DependencyProperty.Register(name, propertyType, ownerType, typeMetadata);
            if ((typeMetadata != null) && (typeMetadata.CoerceCallback != null))
            {
                coerceValueCallbackCache.Add(dp, typeMetadata.CoerceCallback);
            }
            return dp;
        }

        public static DependencyProperty Register(string name, Type propertyType, Type ownerType, Telerik.Windows.PropertyMetadata typeMetadata, ValidateValueCallback validateValueCallback)
        {
            DependencyProperty dp = null;
            if (validateValueCallback == null)
            {
                dp = DependencyProperty.Register(name, propertyType, ownerType, typeMetadata);
            }
            else if (typeMetadata != null)
            {
                if (!validateValueCallback(typeMetadata.DefaultValue))
                {
                    throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Default value for '{0}' property is not valid because ValidateValueCallback failed.", new object[] { name }));
                }
                Telerik.Windows.PropertyMetadata metadata = new Telerik.Windows.PropertyMetadata(typeMetadata.DefaultValue, typeMetadata.PropertyChangedCallback, typeMetadata.CoerceCallback, validateValueCallback);
                dp = DependencyProperty.Register(name, propertyType, ownerType, metadata);
            }
            else
            {
                Telerik.Windows.PropertyMetadata metadata = new Telerik.Windows.PropertyMetadata(null, null, null, validateValueCallback);
                if (!validateValueCallback(metadata.DefaultValue))
                {
                    throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Cannot automatically generate a valid default value for property '{0}'. Specify a default value explicitly when owner type '{1}' is registering this DependencyProperty.", new object[] { name, ownerType }));
                }
                dp = DependencyProperty.Register(name, propertyType, ownerType, metadata);
            }
            if ((typeMetadata != null) && (typeMetadata.CoerceCallback != null))
            {
                coerceValueCallbackCache.Add(dp, typeMetadata.CoerceCallback);
            }
            return dp;
        }

        public static DependencyProperty RegisterAttached(string name, Type propertyType, Type ownerType, Telerik.Windows.PropertyMetadata typeMetadata)
        {
            DependencyProperty dp = DependencyProperty.RegisterAttached(name, propertyType, ownerType, typeMetadata);
            if ((typeMetadata != null) && (typeMetadata.CoerceCallback != null))
            {
                coerceValueCallbackCache.Add(dp, typeMetadata.CoerceCallback);
            }
            return dp;
        }

        public static DependencyPropertyKey RegisterAttachedReadOnly(string name, Type propertyType, Type ownerType, Telerik.Windows.PropertyMetadata typeMetadata)
        {
            DependencyPropertyKey key = new DependencyPropertyKey();
            if (typeMetadata == null)
            {
                typeMetadata = new Telerik.Windows.PropertyMetadata();
            }
            key.DependencyProperty = DependencyProperty.RegisterAttached(name, propertyType, ownerType, typeMetadata);
            readonlyDP.Add(key.DependencyProperty);
            if ((typeMetadata != null) && (typeMetadata.CoerceCallback != null))
            {
                coerceValueCallbackCache.Add(key.DependencyProperty, typeMetadata.CoerceCallback);
            }
            return key;
        }

        public static DependencyPropertyKey RegisterReadOnly(string name, Type propertyType, Type ownerType, Telerik.Windows.PropertyMetadata typeMetadata)
        {
            DependencyPropertyKey key = new DependencyPropertyKey();
            if (typeMetadata == null)
            {
                typeMetadata = new Telerik.Windows.PropertyMetadata();
            }
            key.DependencyProperty = DependencyProperty.Register(name, propertyType, ownerType, typeMetadata);
            readonlyDP.Add(key.DependencyProperty);
            if ((typeMetadata != null) && (typeMetadata.CoerceCallback != null))
            {
                coerceValueCallbackCache.Add(key.DependencyProperty, typeMetadata.CoerceCallback);
            }
            return key;
        }

        internal static bool TryGetValue(DependencyProperty dp, out CoerceValueCallback coerceCallback)
        {
            return coerceValueCallbackCache.TryGetValue(dp, out coerceCallback);
        }
    }
}

