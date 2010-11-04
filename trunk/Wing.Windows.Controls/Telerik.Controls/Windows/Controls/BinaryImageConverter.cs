namespace Telerik.Windows.Controls
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Windows.Data;
    using System.Windows.Media.Imaging;

    public class BinaryImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                MethodInfo toArrayMethod = value.GetType().GetMethod("ToArray", BindingFlags.Public | BindingFlags.Instance);
                if (toArrayMethod != null)
                {
                    value = toArrayMethod.Invoke(value, new object[0]);
                }
                byte[] bytes = value as byte[];
                if (bytes != null)
                {
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.SetSource(new MemoryStream(bytes));
                    return bitmapImage;
                }
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}

