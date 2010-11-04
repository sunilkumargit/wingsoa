namespace Telerik.Windows.Controls.QuickStart
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;

    [SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces")]
    public static class QuickStart
    {
        private static readonly DependencyProperty ConfigurationPanelProperty = DependencyProperty.RegisterAttached("ConfigurationPanel", typeof(Panel), typeof(Telerik.Windows.Controls.QuickStart.QuickStart), new PropertyMetadata(null));

        public static Panel GetConfigurationPanel(DependencyObject obj)
        {
            return (Panel) obj.GetValue(ConfigurationPanelProperty);
        }

        public static void SetConfigurationPanel(DependencyObject obj, Panel value)
        {
            obj.SetValue(ConfigurationPanelProperty, value);
        }
    }
}

