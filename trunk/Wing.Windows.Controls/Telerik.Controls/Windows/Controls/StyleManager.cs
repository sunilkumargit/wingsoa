namespace Telerik.Windows.Controls
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows;

    public static class StyleManager
    {
        private static Theme applicationTheme;
        public static readonly DependencyProperty BasedOnProperty = DependencyProperty.RegisterAttached("BasedOn", typeof(Theme), typeof(StyleManager), new PropertyMetadata(new PropertyChangedCallback(StyleManager.OnBasedOnChanged)));
        public static readonly DependencyProperty ThemeProperty = DependencyProperty.RegisterAttached("Theme", typeof(Theme), typeof(StyleManager), new PropertyMetadata(new PropertyChangedCallback(StyleManager.OnThemeChanged)));

        public static Theme GetBasedOn(DependencyObject obj)
        {
            return (Theme) obj.GetValue(BasedOnProperty);
        }

        [Category("Appearance")]
        public static Theme GetTheme(DependencyObject element)
        {
            return (Theme) element.GetValue(ThemeProperty);
        }

        private static void OnApplicationThemeChanged(Theme oldValue, Theme newValue)
        {
            if (oldValue != newValue)
            {
                if ((oldValue != null) && !oldValue.IsDefault)
                {
                    foreach (KeyValuePair<ResourceDictionary, string> item in Theme.GenericResourceDictionaries)
                    {
                        ResourceDictionary generic = item.Key;
                        generic.MergedDictionaries.RemoveAt(generic.MergedDictionaries.Count - 1);
                    }
                }
                if ((newValue != null) && !newValue.IsDefault)
                {
                    foreach (KeyValuePair<ResourceDictionary, string> item in Theme.GenericResourceDictionaries)
                    {
                        ResourceDictionary generic = item.Key;
                        string controlAssembly = item.Value;
                        ResourceDictionary appThemeResourceDictionary = Theme.CreateResourceDictionaryForApplicationTheme(newValue, controlAssembly);
                        generic.MergedDictionaries.Add(appThemeResourceDictionary);
                    }
                }
            }
        }

        private static void OnBasedOnChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            Theme theme = (Theme) args.NewValue;
            if (theme != null)
            {
                Style style = d as Style;
                if (((style != null) && !style.IsSealed) && (style.BasedOn == null))
                {
                    Style themeStyle = theme.GetThemeStyle(null, style.TargetType);
                    if (themeStyle != null)
                    {
                        style.BasedOn = themeStyle;
                    }
                }
            }
        }

        private static void OnThemeChanged(DependencyObject target, DependencyPropertyChangedEventArgs changedEventArgs)
        {
            Theme newTheme = changedEventArgs.NewValue as Theme;
            FrameworkElement fe = target as FrameworkElement;
            if (fe != null)
            {
                if ((newTheme == null) && (fe != null))
                {
                    fe.ClearValue(FrameworkElement.StyleProperty);
                    return;
                }
                newTheme.Apply(fe, changedEventArgs.OldValue as Theme);
            }
            IThemable themableControl = target as IThemable;
            if (themableControl != null)
            {
                themableControl.ResetTheme();
            }
        }

        public static void SetBasedOn(DependencyObject obj, Theme value)
        {
            obj.SetValue(BasedOnProperty, value);
        }

        public static void SetTheme(DependencyObject element, Theme value)
        {
            element.SetValue(ThemeProperty, value);
        }

        public static void SetThemeFromParent(DependencyObject element, DependencyObject parent)
        {
            if ((element != null) && (parent != null))
            {
                SetTheme(element, GetTheme(parent));
            }
        }

        public static Theme ApplicationTheme
        {
            get
            {
                return applicationTheme;
            }
            set
            {
                if (applicationTheme != value)
                {
                    Theme oldValue = applicationTheme;
                    applicationTheme = value;
                    OnApplicationThemeChanged(oldValue, value);
                }
            }
        }
    }
}

