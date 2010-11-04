namespace Telerik.Windows.Controls
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public class InvertedBooleanToOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (((bool) value) ? 0 : 1);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}

