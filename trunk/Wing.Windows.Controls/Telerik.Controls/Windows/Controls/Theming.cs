namespace Telerik.Windows.Controls
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;

    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId="Theming")]
    public static class Theming
    {
        [Obsolete("This property will be removed in the next release. Please use StyleManager.ThemeProperty instead", true)]
        public static readonly DependencyProperty ThemeProperty = DependencyProperty.RegisterAttached("Theme", typeof(Theme), typeof(Theming), new PropertyMetadata(null, new PropertyChangedCallback(Theming.ThemeChanged)));

        [Obsolete("This mthod will be removed in the next release. Please use StyleManager.GetTheme instead", true), Category("Appearance")]
        public static Theme GetTheme(DependencyObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return (element.GetValue(ThemeProperty) as Theme);
        }

        [Obsolete("This mthod will be removed in the next release. Please use StyleManager.SetTheme instead", true)]
        public static void SetTheme(DependencyObject element, Theme value)
        {
            element.SetValue(ThemeProperty, value);
        }

        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId="element"), SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId="parent"), Obsolete("This mthod will be removed in the next release. Please use StyleManager.SetThemeFromParent instead", true)]
        public static void SetThemeFromParent(DependencyObject element, DependencyObject parent)
        {
            StyleManager.SetTheme(element, StyleManager.GetTheme(parent));
        }

        private static void ThemeChanged(DependencyObject target, DependencyPropertyChangedEventArgs changedEventArgs)
        {
            StyleManager.SetTheme(target, changedEventArgs.NewValue as Theme);
        }
    }
}

