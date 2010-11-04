namespace Telerik.Windows.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Media.Animation;
    using System.Windows.Resources;

    internal static class ResourceStorage
    {
        private static List<ResourceDictionary> generics = new List<ResourceDictionary>();
        private static Dictionary<Uri, ResourceKey> resources = new Dictionary<Uri, ResourceKey>();

        public static ResourceDictionary GetResourceDictionary(Uri source)
        {
            ResourceKey resource = GetResourceKeyDictionary(source);
            if (resource != null)
            {
                return resource.Dictionary;
            }
            return null;
        }

        private static ResourceKey GetResourceKeyDictionary(Uri source)
        {
            if (resources.ContainsKey(source))
            {
                return resources[source];
            }
            ResourceKey resource = LoadResourceDictionary(source);
            if (resource != null)
            {
                resources[source] = resource;
            }
            return resource;
        }

        internal static void LoadAndMergeDictionaries(ResourceDictionary generic, Theme theme)
        {
            if (!generics.Contains(generic))
            {
                generics.Add(generic);
            }
            if ((theme != null) && (theme.Source != null))
            {
                ResourceKey resourceKey = GetResourceKeyDictionary(theme.Source);
                ReplaceKeys(generic, resourceKey);
            }
        }

        private static ResourceKey LoadResourceDictionary(Uri source)
        {
            StreamResourceInfo sri = null;
            try
            {
                sri = Application.GetResourceStream(source);
                if ((sri == null) || (sri.Stream == null))
                {
                    return null;
                }
            }
            catch (KeyNotFoundException)
            {
                return null;
            }
            List<string> keys = new List<string>();
            ResourceDictionary newDict = ResourceParser.Parse(sri.Stream, true, keys);
            if (newDict != null)
            {
                return new ResourceKey { Dictionary = newDict, Keys = keys };
            }
            return null;
        }

        private static void ReplaceKeys(ResourceDictionary generic, ResourceKey resourceKey)
        {
            if (resourceKey != null)
            {
                foreach (string key in resourceKey.Keys)
                {
                    if (!string.IsNullOrEmpty(key))
                    {
                        object resource = resourceKey.Dictionary[key];
                        if (resource != null)
                        {
                            if (generic.Contains(key))
                            {
                                generic.Remove(key);
                            }
                            if (resource is Timeline)
                            {
                                resourceKey.Dictionary.Remove(key);
                            }
                            generic.Add(key, resource);
                        }
                    }
                }
            }
        }

        internal static void UpdateGenericDictionaries(Uri uri)
        {
            ResourceKey newResourceKey = GetResourceKeyDictionary(uri);
            foreach (ResourceDictionary generic in generics)
            {
                ReplaceKeys(generic, newResourceKey);
            }
        }

        private class ResourceKey
        {
            public ResourceKey()
            {
                this.Keys = new List<string>();
            }

            public ResourceDictionary Dictionary { get; set; }

            public List<string> Keys { get; set; }
        }
    }
}

