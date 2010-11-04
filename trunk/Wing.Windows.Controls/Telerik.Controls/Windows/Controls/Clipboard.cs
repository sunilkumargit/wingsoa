namespace Telerik.Windows.Controls
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Security;
    using System.Windows;
    using System.Windows.Input;

    public static class Clipboard
    {
        public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(Telerik.Windows.Controls.Clipboard), new PropertyMetadata(true));

        [Obsolete("This method is obsolete. Please, use the GetText method."), SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId="e")]
        public static string GetData(KeyEventArgs e)
        {
            return GetText();
        }

        public static bool GetIsEnabled(DependencyObject dependencyObject)
        {
            return (bool) dependencyObject.GetValue(IsEnabledProperty);
        }

        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public static string GetText()
        {
            try
            {
                return System.Windows.Clipboard.GetText();
            }
            catch (SecurityException)
            {
                return string.Empty;
            }
        }

        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId="e"), Obsolete("This method is obsolete. Please, use the SetText method.")]
        public static void SetData(KeyEventArgs e, string data)
        {
            SetText(data);
        }

        public static void SetIsEnabled(DependencyObject dependencyObject, bool enabled)
        {
            dependencyObject.SetValue(IsEnabledProperty, enabled);
        }

        public static void SetText(string text)
        {
            try
            {
                System.Windows.Clipboard.SetText(text);
            }
            catch (SecurityException)
            {
            }
        }
    }
}

