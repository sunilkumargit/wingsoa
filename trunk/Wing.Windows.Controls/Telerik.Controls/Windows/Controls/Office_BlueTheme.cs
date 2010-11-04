namespace Telerik.Windows.Controls
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores"), ThemeLocation(ThemeLocation.BuiltIn)]
    public class Office_BlueTheme : Theme
    {
        public Office_BlueTheme()
        {
            base.Source = new Uri("/Wing.Windows.Controls;component/themes/Generic.xaml", UriKind.RelativeOrAbsolute);
        }
    }
}

