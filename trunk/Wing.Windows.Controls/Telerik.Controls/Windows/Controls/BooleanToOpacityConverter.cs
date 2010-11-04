﻿namespace Telerik.Windows.Controls
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public class BooleanToOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (((bool) value) ? 1 : 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}

