namespace Telerik.Windows.Controls
{
    using System;
    using System.ComponentModel;
    using System.Globalization;

    public class StringToLongTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return ((sourceType == typeof(string)) || base.CanConvertFrom(context, sourceType));
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return ((destinationType == typeof(string)) || base.CanConvertTo(context, destinationType));
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            long returnValue = 0L;
            if (!long.TryParse(value.ToString(), out returnValue))
            {
                throw new FormatException("Invalid long format");
            }
            return returnValue;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            return value.ToString();
        }
    }
}

