namespace Telerik.Windows.Controls.Design
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Windows.Input;
    using Telerik.Windows;
    using Telerik.Windows.Controls;

    public class MouseGestureConverter : TypeConverter
    {
        private const char ModifiersDelimiter = '+';

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return (sourceType == typeof(string));
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (((destinationType == typeof(string)) && (context != null)) && (context.Instance != null))
            {
                MouseGesture instance = context.Instance as MouseGesture;
                if (instance != null)
                {
                    return (ModifierKeysConverter.IsDefinedModifierKeys(instance.Modifiers) && MouseActionConverter.IsDefinedMouseAction(instance.MouseAction));
                }
            }
            return false;
        }

        [SuppressMessage("Microsoft.Performance", "CA1820:TestForEmptyStringsUsingStringLength")]
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string stringValue = value as string;
            if (stringValue != null)
            {
                string str2;
                string str3;
                string str = stringValue.Trim();
                if (string.IsNullOrEmpty(str))
                {
                    return new MouseGesture(MouseAction.None, ModifierKeys.None);
                }
                int length = str.LastIndexOf('+');
                if (length >= 0)
                {
                    str2 = str.Substring(0, length);
                    str3 = str.Substring(length + 1);
                }
                else
                {
                    str2 = string.Empty;
                    str3 = str;
                }
                TypeConverter converter2 = new MouseActionConverter();
                if (converter2 != null)
                {
                    object obj3 = converter2.ConvertFrom(context, culture, str3);
                    if (obj3 != null)
                    {
                        if (str2 != string.Empty)
                        {
                            TypeConverter converter = new ModifierKeysConverter();
                            if (converter != null)
                            {
                                object obj2 = converter.ConvertFrom(context, culture, str2);
                                if ((obj2 != null) && (obj2 is ModifierKeys))
                                {
                                    return new MouseGesture((MouseAction) obj3, (ModifierKeys) obj2);
                                }
                            }
                        }
                        else
                        {
                            return new MouseGesture((MouseAction) obj3);
                        }
                    }
                }
            }
            throw new NotImplementedException();
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            destinationType.TestNotNull("destinationType");
            if (destinationType == typeof(string))
            {
                if (value == null)
                {
                    return string.Empty;
                }
                MouseGesture gesture = value as MouseGesture;
                if (gesture != null)
                {
                    string str = string.Empty;
                    TypeConverter modifierKeysConverter = new ModifierKeysConverter();
                    if (modifierKeysConverter != null)
                    {
                        str = str + (modifierKeysConverter.ConvertTo(context, culture, gesture.Modifiers, destinationType) as string);
                        if (!string.IsNullOrEmpty(str))
                        {
                            str = str + '+';
                        }
                    }
                    TypeConverter mouseActionConverter = new MouseActionConverter();
                    if (mouseActionConverter != null)
                    {
                        str = str + (mouseActionConverter.ConvertTo(context, culture, gesture.MouseAction, destinationType) as string);
                    }
                    return str;
                }
            }
            throw new NotImplementedException();
        }
    }
}

