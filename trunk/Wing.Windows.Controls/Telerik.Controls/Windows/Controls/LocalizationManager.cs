namespace Telerik.Windows.Controls
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Resources;
    using System.Runtime.CompilerServices;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media.Animation;

    public class LocalizationManager
    {
        private static System.Resources.ResourceManager defaultResourceManager = Strings.ResourceManager;
        public static readonly DependencyProperty ResourceKeyProperty = DependencyProperty.RegisterAttached("ResourceKey", typeof(string), typeof(LocalizationManager), new PropertyMetadata(null, new PropertyChangedCallback(LocalizationManager.OnResourceKeyChanged)));

        public static string GetResourceKey(DependencyObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return (element.GetValue(ResourceKeyProperty) as string);
        }

        public static string GetString(string key)
        {
            LocalizationManager localizationManager = Manager;
            if (localizationManager != null)
            {
                return localizationManager.GetStringOverride(key);
            }
            return GetString(key, DefaultCulture, DefaultResourceManager);
        }

        private static string GetString(string key, CultureInfo culture, System.Resources.ResourceManager resourceManager)
        {
            string data = null;
            if (resourceManager != null)
            {
                try
                {
                    data = (culture == null) ? resourceManager.GetString(key) : resourceManager.GetString(key, culture);
                }
                catch (InvalidOperationException)
                {
                }
                catch (MissingManifestResourceException)
                {
                }
            }
            return (data ?? Telerik.Windows.Controls.SR.GetString(key));
        }

        public virtual string GetStringOverride(string key)
        {
            return GetString(key, this.Culture ?? DefaultCulture, this.ResourceManager ?? DefaultResourceManager);
        }

        [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        private static void OnResourceKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            string resourceKey = e.NewValue as string;
            if (resourceKey != null)
            {
                string resourceData = GetString(resourceKey);
                if (resourceData != null)
                {
                    resourceData = Regex.Replace(resourceData, "_(?!_)", string.Empty);
                    ILocalizable localizable = d as ILocalizable;
                    if (localizable != null)
                    {
                        localizable.SetString(resourceData);
                    }
                    else if (d is TextBox)
                    {
                        (d as TextBox).Text = resourceData;
                    }
                    else if (d is TextBlock)
                    {
                        (d as TextBlock).Text = resourceData;
                    }
                    else if (d is ContentControl)
                    {
                        (d as ContentControl).Content = resourceData;
                    }
                    else if (d is ContentPresenter)
                    {
                        (d as ContentPresenter).Content = resourceData;
                    }
                    else if (d is HeaderedContentControl)
                    {
                        (d as HeaderedContentControl).Header = resourceData;
                    }
                    else if (d is HeaderedItemsControl)
                    {
                        (d as HeaderedItemsControl).Header = resourceData;
                    }
                    else if (d is Setter)
                    {
                        (d as Setter).Value = resourceData;
                    }
                    else if (d is DiscreteObjectKeyFrame)
                    {
                        (d as DiscreteObjectKeyFrame).Value = resourceData;
                    }
                }
            }
        }

        public static void SetResourceKey(DependencyObject element, string value)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            element.SetValue(ResourceKeyProperty, value);
        }

        public CultureInfo Culture { get; set; }

        public static CultureInfo DefaultCulture { get; set; }

        public static System.Resources.ResourceManager DefaultResourceManager
        {
            get
            {
                return defaultResourceManager;
            }
            set
            {
                defaultResourceManager = value;
            }
        }

        public static LocalizationManager Manager { get; set; }

        public System.Resources.ResourceManager ResourceManager { get; set; }
    }
}

