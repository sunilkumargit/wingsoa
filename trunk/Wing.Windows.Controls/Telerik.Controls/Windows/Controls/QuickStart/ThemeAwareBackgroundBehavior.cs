namespace Telerik.Windows.Controls.QuickStart
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Telerik.Windows.Controls;

    public static class ThemeAwareBackgroundBehavior
    {
        public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(ThemeAwareBackgroundBehavior), new PropertyMetadata(new PropertyChangedCallback(ThemeAwareBackgroundBehavior.OnIsEnabledPropertyChanged)));

        public static bool GetIsEnabled(DependencyObject obj)
        {
            return (bool) obj.GetValue(IsEnabledProperty);
        }

        private static void OnIsEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            DependencyProperty backgroundProperty = null;
            Control control = d as Control;
            Border border = d as Border;
            FrameworkElement element = null;
            if (control != null)
            {
                backgroundProperty = Control.BackgroundProperty;
                element = control;
            }
            else if (border != null)
            {
                backgroundProperty = Border.BackgroundProperty;
                element = border;
            }
            if (backgroundProperty != null)
            {
                RoutedEventHandler elementLoaded = null;
                elementLoaded = delegate (object sender, RoutedEventArgs e) {
                    element.Loaded -= elementLoaded;
                    FrameworkElement parentUserControl = element.ParentOfType<UserControl>();
                    if ((parentUserControl != null) && (parentUserControl.DataContext != null))
                    {
                        if (parentUserControl.DataContext.GetType().GetProperty("SelectedTheme") == null)
                        {
                            parentUserControl = parentUserControl.Parent as FrameworkElement;
                        }
                        element.SetBinding(backgroundProperty, new Binding("DataContext.SelectedTheme") { Source = parentUserControl, Converter = new TransparentThemeBackgroundConverter(), ConverterParameter = (Brush) d.GetValue(backgroundProperty), Mode = BindingMode.TwoWay });
                    }
                };
                element.Loaded += elementLoaded;
            }
        }

        public static void SetIsEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsEnabledProperty, value);
        }

        internal class TransparentThemeBackgroundConverter : IValueConverter
        {
            private Random random = new Random();

            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                Theme theme = value as Theme;
                if (parameter == null)
                {
                    return DependencyProperty.UnsetValue;
                }
                return parameter;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return value;
            }

            private Uri GetImageUri()
            {
                return new Uri(string.Format("/Telerik.Windows.QuickStartTheme;component/images/bg0{0}.jpg", this.random.Next(1, 7)), UriKind.RelativeOrAbsolute);
            }
        }
    }
}

