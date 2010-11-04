namespace Telerik.Windows.Controls
{
    using System;
    using System.ComponentModel;
    using System.Globalization;

    public class NullableDoubleConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType != typeof(double))
            {
                return (sourceType == typeof(string));
            }
            return true;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
            {
                return null;
            }
            string val = value as string;
            if (val == null)
            {
                throw new NotSupportedException(string.Format(CultureInfo.CurrentUICulture, "Cannot convert the value {0} to an instance of {1}", new object[] { value.ToString(), typeof(double?).Name }));
            }
            return (string.IsNullOrEmpty(val) ? null : new double?(double.Parse(val, culture)));
        }
    }
}

