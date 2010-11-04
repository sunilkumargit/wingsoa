namespace Telerik.Windows.Controls
{
    using System;
    using System.Windows;

    public static class RadControl
    {
        private static bool? isInDesignMode = null;
        [Obsolete("This property will be removed in the next release. Please use StyleManager.ThemeProperty instead", true)]
        public static readonly DependencyProperty ThemeProperty = DependencyProperty.RegisterAttached("Theme", typeof(Theme), typeof(RadControl), new PropertyMetadata(null, new PropertyChangedCallback(RadControl.OnThemeChanged)));

        [Obsolete("This mthod will be removed in the next release. Please use StyleManager.GetTheme instead", true)]
        public static Theme GetTheme(DependencyObject obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }
            return (Theme) obj.GetValue(ThemeProperty);
        }

        private static void OnThemeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            StyleManager.SetTheme(sender, e.NewValue as Theme);
        }

        [Obsolete("This mthod will be removed in the next release. Please use StyleManager.SetTheme instead", true)]
        public static void SetTheme(DependencyObject obj, Theme value)
        {
            obj.SetValue(ThemeProperty, value);
        }

        public static bool IsInDesignMode
        {
            get
            {
                if (isInDesignMode.HasValue)
                {
                    return isInDesignMode.Value;
                }
                return ((Application.Current == null) || (Application.Current.GetType() == typeof(Application)));
            }
            internal set
            {
                isInDesignMode = new bool?(value);
            }
        }
    }
}

