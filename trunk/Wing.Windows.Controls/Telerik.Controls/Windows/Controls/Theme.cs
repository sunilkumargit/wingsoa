namespace Telerik.Windows.Controls
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Controls;

    [TypeConverter(typeof(ThemeConverter))]
    public class Theme
    {
        public static readonly DependencyProperty ApplicationThemeSetterProperty = DependencyProperty.RegisterAttached("ApplicationThemeSetter", typeof(string), typeof(Theme), new PropertyMetadata(new PropertyChangedCallback(Theme.OnApplicationThemeSetterChanged)));
        private static readonly Dictionary<string, ResourceDictionary> cachedDictionaries = new Dictionary<string, ResourceDictionary>();
        internal static readonly Dictionary<ResourceDictionary, string> GenericResourceDictionaries = new Dictionary<ResourceDictionary, string>();
        private bool isApplicationTheme;
        private static Dictionary<string, bool> isSourceValid = new Dictionary<string, bool>();
        private static Dictionary<string, ThemeLocationAttribute> themeLocationCache = new Dictionary<string, ThemeLocationAttribute>();

        public Theme()
        {
        }

        public Theme(Uri source)
            : this()
        {
            this.Source = source;
        }

        private static void AddMergedDictionaryFromApplicationTheme(ResourceDictionary generic, string assemblyName)
        {
            if (AppThemeIsNotDefault)
            {
                ResourceDictionary themeResources = CreateResourceDictionaryForApplicationTheme(StyleManager.ApplicationTheme, assemblyName);
                if (themeResources != null)
                {
                    generic.MergedDictionaries.Add(themeResources);
                }
            }
        }

        internal void Apply(FrameworkElement element, Theme oldTheme)
        {
            if (this.Source != null)
            {
                Type defaultStyleKey = null;
                Control control = element as Control;
                if (control != null)
                {
                    defaultStyleKey = DefaultStyleKeyHelper.GetControlDefaultStyleKey(control);
                }
                if (defaultStyleKey == null)
                {
                    defaultStyleKey = element.GetType();
                }
                Style themeStyle = this.GetThemeStyle(oldTheme, defaultStyleKey);
                if (themeStyle != null)
                {
                    element.Style = themeStyle;
                }
            }
        }

        private static bool ContainsKey(ResourceDictionary resource, Type defaultStyleKeyType)
        {
            if ((resource == null) || (defaultStyleKeyType == null))
            {
                return false;
            }
            if (!resource.Contains(defaultStyleKeyType))
            {
                return resource.Contains(defaultStyleKeyType.FullName);
            }
            return true;
        }

        internal static ResourceDictionary CreateResourceDictionaryForApplicationTheme(Theme theme, string controlAssembly)
        {
            ThemeLocationAttribute themeLocation = GetThemeLocationAttribute(theme);
            Uri uri = null;
            if ((themeLocation != null) && (themeLocation.Location == ThemeLocation.BuiltIn))
            {
                uri = GetUriForBuiltInTheme(theme, controlAssembly);
            }
            else
            {
                uri = theme.Source;
            }
            return new ResourceDictionary { Source = uri };
        }

        public static string GetApplicationThemeSetter(DependencyObject obj)
        {
            return (string)obj.GetValue(ApplicationThemeSetterProperty);
        }

        internal static ResourceDictionary GetCachedResourceDictionary(Uri uri)
        {
            bool sourceValid;
            if (!isSourceValid.TryGetValue(uri.OriginalString, out sourceValid))
            {
                if (Application.GetResourceStream(uri) == null)
                {
                    isSourceValid.Add(uri.OriginalString, false);
                    sourceValid = false;
                }
                else
                {
                    isSourceValid.Add(uri.OriginalString, true);
                    sourceValid = true;
                }
            }
            if (!sourceValid)
            {
                return null;
            }
            ResourceDictionary resource = null;
            if (cachedDictionaries.ContainsKey(uri.OriginalString))
            {
                return cachedDictionaries[uri.OriginalString];
            }
            try
            {
                resource = new ResourceDictionary
                {
                    Source = uri
                };
                cachedDictionaries.Add(uri.OriginalString, resource);
            }
            catch (Exception keyNotFound)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "A ResourceDictionary '{0}' cannot be found. Please make sure that references to the needed theme assemblies have been added to the project.", new object[] { uri.ToString() }), keyNotFound);
            }
            return resource;
        }

        private static object GetResource(ResourceDictionary resources, Type defaultStyleKey)
        {
            if ((resources != null) && (defaultStyleKey != null))
            {
                if (resources.Contains(defaultStyleKey))
                {
                    return resources[defaultStyleKey];
                }
                if (resources.Contains(defaultStyleKey.FullName))
                {
                    return resources[defaultStyleKey.FullName];
                }
            }
            return null;
        }

        private static object GetResourceFromDefaultTheme(Type defaultStyleKeyType)
        {
            object value = null;
            Dictionary<ResourceDictionary, ResourceDictionary> cachedApplicationResourceDictionary = new Dictionary<ResourceDictionary, ResourceDictionary>(GenericResourceDictionaries.Count);
            if (AppThemeIsNotDefault)
            {
                foreach (KeyValuePair<ResourceDictionary, string> item in GenericResourceDictionaries)
                {
                    ResourceDictionary generic = item.Key;
                    int lastRDIndex = generic.MergedDictionaries.Count - 1;
                    ResourceDictionary lastRD = generic.MergedDictionaries[lastRDIndex];
                    cachedApplicationResourceDictionary.Add(generic, lastRD);
                    generic.MergedDictionaries.RemoveAt(lastRDIndex);
                }
            }
            foreach (KeyValuePair<ResourceDictionary, string> item in GenericResourceDictionaries)
            {
                ResourceDictionary rd = item.Key;
                if (ContainsKey(rd, defaultStyleKeyType))
                {
                    value = GetResource(rd, defaultStyleKeyType);
                    break;
                }
            }
            foreach (KeyValuePair<ResourceDictionary, ResourceDictionary> item in cachedApplicationResourceDictionary)
            {
                item.Key.MergedDictionaries.Add(item.Value);
            }
            cachedApplicationResourceDictionary.Clear();
            return value;
        }

        private static object GetResourceFromTheme(Theme theme, string controlAssembly, Type defaultStyleKeyType, Theme controlTheme)
        {
            Uri uri;
            ThemeLocationAttribute themeLocation = GetThemeLocationAttribute(theme);
            if ((themeLocation != null) && (themeLocation.Location == ThemeLocation.BuiltIn))
            {
                uri = GetUriForBuiltInTheme(theme, controlAssembly);
                if (theme.IsDefault && (controlAssembly != "System.Windows"))
                {
                    if (!AppThemeIsNotDefault && ((controlTheme == null) || controlTheme.IsDefault))
                    {
                        return null;
                    }
                    LoadGenericIfNeeded(controlAssembly);
                    return GetResourceFromDefaultTheme(defaultStyleKeyType);
                }
            }
            else
            {
                uri = theme.Source;
            }
            return GetResource(GetCachedResourceDictionary(uri), defaultStyleKeyType);
        }

        private static object GetResourceValue(Type defaultStyleKey, Theme theme, Theme controlTheme)
        {
            if (defaultStyleKey == null)
            {
                throw new ArgumentNullException("defaultStyleKey");
            }
            if ((theme == null) || (theme.Source == null))
            {
                throw new ArgumentNullException("theme");
            }
            string typeAssemblyName = defaultStyleKey.Assembly.FullName.Split(new char[] { ',' })[0];
            return GetResourceFromTheme(theme, typeAssemblyName, defaultStyleKey, controlTheme);
        }

        private static string GetThemeAssemblyName(Theme theme)
        {
            return theme.Source.OriginalString.Split(new char[] { ';' })[0].Replace("/", string.Empty);
        }

        private static ThemeLocationAttribute GetThemeLocationAttribute(Theme theme)
        {
            ThemeLocationAttribute themeLocationAttribute = null;
            string source = theme.Source.OriginalString;
            if (themeLocationCache.ContainsKey(source))
            {
                return themeLocationCache[source];
            }
            foreach (object item in theme.GetType().GetCustomAttributes(typeof(ThemeLocationAttribute), false))
            {
                themeLocationAttribute = item as ThemeLocationAttribute;
                if (themeLocationAttribute != null)
                {
                    break;
                }
            }
            if (themeLocationAttribute != null)
            {
                themeLocationCache.Add(source, themeLocationAttribute);
            }
            return themeLocationAttribute;
        }

        internal Style GetThemeStyle(Theme oldTheme, Type defaultStyleKey)
        {
            Style themeStyle = null;
            object style = GetResourceValue(defaultStyleKey, this, oldTheme);
            if (style != null)
            {
                return (style as Style);
            }
            ThemeLocationAttribute themeLocation = GetThemeLocationAttribute(this);
            if ((themeLocation != null) && (themeLocation.Location != ThemeLocation.External))
            {
                return themeStyle;
            }
            return (GetResource(GetCachedResourceDictionary(this.Source), defaultStyleKey) as Style);
        }

        internal static Uri GetUriForBuiltInTheme(Theme theme, string controlAssembly)
        {
            string themeAssembly = GetThemeAssemblyName(theme);
            return new Uri(string.Format(CultureInfo.InvariantCulture, "/{0};component/Themes/{1}.xaml", new object[] { themeAssembly, controlAssembly }), UriKind.RelativeOrAbsolute);
        }

        private static void LoadGenericIfNeeded(string controlAssembly)
        {
            bool shouldLoadGeneric = true;
            foreach (KeyValuePair<ResourceDictionary, string> item in GenericResourceDictionaries)
            {
                if (item.Value == controlAssembly)
                {
                    shouldLoadGeneric = false;
                    break;
                }
            }
            if (shouldLoadGeneric)
            {
                Uri uri = new Uri(string.Format(CultureInfo.InvariantCulture, "/{0};component/Themes/generic.xaml", new object[] { controlAssembly }), UriKind.Relative);
                ResourceDictionary local = new ResourceDictionary
                {
                    Source = uri
                };
            }
        }

        private static void OnApplicationThemeSetterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ResourceDictionary generic = d as ResourceDictionary;
            if (generic != null)
            {
                string assemblyName = Convert.ToString(e.NewValue, CultureInfo.InvariantCulture);
                if (!GenericResourceDictionaries.ContainsKey(generic))
                {
                    GenericResourceDictionaries.Add(generic, assemblyName);
                }
                AddMergedDictionaryFromApplicationTheme(generic, assemblyName);
            }
        }

        public static void SetApplicationThemeSetter(DependencyObject obj, string value)
        {
            obj.SetValue(ApplicationThemeSetterProperty, value);
        }

        public override string ToString()
        {
            return base.GetType().Name.Replace("Theme", string.Empty);
        }

        internal static bool AppThemeIsNotDefault
        {
            get
            {
                return ((StyleManager.ApplicationTheme != null) && !StyleManager.ApplicationTheme.IsDefault);
            }
        }

        public bool IsApplicationTheme
        {
            get
            {
                return this.isApplicationTheme;
            }
            set
            {
                if (value)
                {
                    StyleManager.ApplicationTheme = this;
                }
                this.isApplicationTheme = value;
            }
        }

        internal bool IsDefault
        {
            get
            {
                return (base.GetType() == ThemeManager.FromName(ThemeManager.DefaultThemeName).GetType());
            }
        }

        [TypeConverter(typeof(UriTypeConverter))]
        public Uri Source { get; set; }

        internal class DefaultStyleKeyHelper : Control
        {
            public static Type GetControlDefaultStyleKey(Control control)
            {
                return (Type)control.GetValue(Control.DefaultStyleKeyProperty);
            }

            public static void SetDefaultStyleKey(Control control, object value)
            {
                control.SetValue(Control.DefaultStyleKeyProperty, value);
            }
        }
    }
}

