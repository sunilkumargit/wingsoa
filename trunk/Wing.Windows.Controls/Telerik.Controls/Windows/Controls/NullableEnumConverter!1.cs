namespace Telerik.Windows.Controls
{
    using System;
    using System.ComponentModel;
    using System.Globalization;

    public class NullableEnumConverter<T> : TypeConverter where T: struct
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return (sourceType == typeof(string));
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
            {
                return null;
            }
            string stringValue = value as string;
            if (stringValue == null)
            {
                throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, "Cannot convert the value {0} to an instance of {1}", new object[] { value.ToString(), typeof(T).Name }));
            }
            return Enum.Parse(typeof(T), stringValue, true);
        }
    }
}

