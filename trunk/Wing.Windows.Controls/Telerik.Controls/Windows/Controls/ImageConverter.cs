namespace Telerik.Windows.Controls
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    public class ImageConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == null)
            {
                throw new ArgumentNullException("sourceType");
            }
            if (sourceType != typeof(string))
            {
                return base.CanConvertFrom(context, sourceType);
            }
            return true;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            Uri imageUri;
            if (!(value is string))
            {
                return base.ConvertFrom(context, culture, value);
            }
            if (Uri.TryCreate(Convert.ToString(value, culture), UriKind.RelativeOrAbsolute, out imageUri))
            {
                return new Image { Source = new BitmapImage(imageUri), Stretch = Stretch.None };
            }
            return value;
        }
    }
}

