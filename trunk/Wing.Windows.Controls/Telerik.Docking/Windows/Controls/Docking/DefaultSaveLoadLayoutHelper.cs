namespace Telerik.Windows.Controls.Docking
{
    using System;
    using System.Collections.Generic;
    using System.Windows;

    public class DefaultSaveLoadLayoutHelper : SaveLoadLayoutHelper
    {
        private Dictionary<string, DependencyObject> cache = new Dictionary<string, DependencyObject>();

        private void AddToCache(string serializationTag, DependencyObject element)
        {
            if (!this.cache.ContainsKey(serializationTag) && !this.cache.ContainsValue(element))
            {
                this.cache.Add(serializationTag, element);
            }
        }

        protected override void ElementCleanedOverride(string serializationTag, DependencyObject element)
        {
            this.AddToCache(serializationTag, element);
        }

        protected override DependencyObject ElementLoadingOverride(string serializationTag)
        {
            DependencyObject result = this.GetFromCache(serializationTag);
            if (result != null)
            {
                this.RemoveFromCache(serializationTag);
                return result;
            }
            return base.ElementLoadingOverride(serializationTag);
        }

        private DependencyObject GetFromCache(string serializationTag)
        {
            if (this.cache.ContainsKey(serializationTag))
            {
                return this.cache[serializationTag];
            }
            return null;
        }

        private void RemoveFromCache(string serializationTag)
        {
            this.cache.Remove(serializationTag);
        }
    }
}

