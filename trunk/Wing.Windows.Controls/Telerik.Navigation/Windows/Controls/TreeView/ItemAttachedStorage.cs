namespace Telerik.Windows.Controls.TreeView
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Windows;

    internal class ItemAttachedStorage
    {
        private Dictionary<WeakReferenceKey<object>, Dictionary<DependencyProperty, object>> itemStorageMap;
        internal static readonly DependencyProperty ItemStorageProperty = DependencyProperty.RegisterAttached("ItemStorage", typeof(ItemAttachedStorage), typeof(ItemAttachedStorage), null);

        internal void Clear()
        {
            this.itemStorageMap = null;
        }

        internal void ClearItem(object item)
        {
            this.EnsureItemStorageMap();
            WeakReferenceKey<object> key = new WeakReferenceKey<object>(item);
            this.itemStorageMap.Remove(key);
        }

        internal void ClearProperty(DependencyProperty property)
        {
            this.EnsureItemStorageMap();
            foreach (Dictionary<DependencyProperty, object> dictionary in this.itemStorageMap.Values)
            {
                if (dictionary.ContainsKey(property))
                {
                    dictionary.Remove(property);
                }
            }
        }

        internal void ClearValue(object item, DependencyProperty property)
        {
            Dictionary<DependencyProperty, object> map;
            this.EnsureItemStorageMap();
            WeakReferenceKey<object> key = new WeakReferenceKey<object>(item);
            if (this.itemStorageMap.TryGetValue(key, out map))
            {
                map.Remove(property);
            }
        }

        private Dictionary<DependencyProperty, object> EnsureItem(WeakReferenceKey<object> itemKeyableReference)
        {
            Dictionary<DependencyProperty, object> map;
            this.EnsureItemStorageMap();
            if (!this.itemStorageMap.TryGetValue(itemKeyableReference, out map))
            {
                map = new Dictionary<DependencyProperty, object>();
                this.itemStorageMap[itemKeyableReference] = map;
            }
            return map;
        }

        private void EnsureItemStorageMap()
        {
            if (this.itemStorageMap == null)
            {
                this.itemStorageMap = new Dictionary<WeakReferenceKey<object>, Dictionary<DependencyProperty, object>>();
            }
        }

        internal T ExtractValue<T>(DependencyObject container, DependencyProperty property, object item, T defaultValue)
        {
            object result;
            if (container != null)
            {
                return (T) container.GetValue(property);
            }
            if (this.TryGetValue(item, property, out result))
            {
                return (T) result;
            }
            return defaultValue;
        }

        internal static ItemAttachedStorage GetItemStorage(FrameworkElement target)
        {
            return (ItemAttachedStorage) target.GetValue(ItemStorageProperty);
        }

        internal T GetValue<T>(object item, DependencyProperty dependencyProperty, T defaultValue)
        {
            object result;
            if (this.TryGetValue(item, dependencyProperty, out result))
            {
                return (T) result;
            }
            return defaultValue;
        }

        internal void RestoreValue(FrameworkElement container, DependencyProperty dependencyProperty, object item, object defaultValue)
        {
            if (container.GetBindingExpression(dependencyProperty) == null)
            {
                object storedValue;
                if (this.TryGetValue(item, dependencyProperty, out storedValue))
                {
                    container.SetValue(dependencyProperty, storedValue);
                }
                else
                {
                    container.SetValue(dependencyProperty, defaultValue);
                }
            }
        }

        internal static void SetItemStorage(FrameworkElement target, ItemAttachedStorage storage)
        {
            target.SetValue(ItemStorageProperty, storage);
        }

        internal void SetValue(object item, DependencyProperty property, object value)
        {
            WeakReferenceKey<object> key = new WeakReferenceKey<object>(item);
            this.EnsureItem(key)[property] = value;
        }

        internal void StoreValue(FrameworkElement container, DependencyProperty property, object item, object defaultValue)
        {
            if ((item != null) && (container.GetBindingExpression(property) == null))
            {
                object propertyValue = container.GetValue(property);
                if (object.Equals(defaultValue, propertyValue))
                {
                    this.ClearValue(item, property);
                }
                else
                {
                    this.SetValue(item, property, propertyValue);
                }
            }
        }

        internal void StoreValueAndClearContainer(FrameworkElement container, DependencyProperty property, object item, object defaultValue)
        {
            if ((item != null) && (container.GetBindingExpression(property) == null))
            {
                object propertyValue = container.GetValue(property);
                if (object.Equals(propertyValue, defaultValue))
                {
                    this.ClearValue(item, property);
                }
                else
                {
                    this.SetValue(item, property, propertyValue);
                }
                container.ClearValue(property);
            }
        }

        internal bool TryGetValue(object item, DependencyProperty property, out object value)
        {
            Dictionary<DependencyProperty, object> map;
            value = null;
            this.EnsureItemStorageMap();
            WeakReferenceKey<object> key = new WeakReferenceKey<object>(item);
            return (this.itemStorageMap.TryGetValue(key, out map) && map.TryGetValue(property, out value));
        }
    }
}

