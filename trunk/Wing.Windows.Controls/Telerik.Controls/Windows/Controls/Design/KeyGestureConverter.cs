namespace Telerik.Windows.Controls.Design
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Windows.Input;
    using Telerik.Windows;
    using Telerik.Windows.Controls;

    public class KeyGestureConverter : TypeConverter
    {
        internal const char DisplayStringSeparator = ',';
        private static KeyConverter keyConverter = new KeyConverter();
        private static ModifierKeysConverter modifierKeysConverter = new ModifierKeysConverter();
        private const char ModifiersDelimiter = '+';

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return (sourceType == typeof(string));
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (((destinationType == typeof(string)) && (context != null)) && (context.Instance != null))
            {
                KeyGesture instance = context.Instance as KeyGesture;
                if (instance != null)
                {
                    return (ModifierKeysConverter.IsDefinedModifierKeys(instance.Modifiers) && IsDefinedKey(instance.Key));
                }
            }
            return false;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string stringValue = value as string;
            if (!string.IsNullOrEmpty(stringValue))
            {
                string str2;
                string str3;
                string str4;
                string str = stringValue.Trim();
                if (string.IsNullOrEmpty(str))
                {
                    return new KeyGesture(Key.None);
                }
                int index = str.IndexOf(',');
                if (index >= 0)
                {
                    str4 = str.Substring(index + 1).Trim();
                    str = str.Substring(0, index).Trim();
                }
                else
                {
                    str4 = string.Empty;
                }
                index = str.LastIndexOf('+');
                if (index >= 0)
                {
                    str3 = str.Substring(0, index);
                    str2 = str.Substring(index + 1);
                }
                else
                {
                    str3 = string.Empty;
                    str2 = str;
                }
                ModifierKeys none = ModifierKeys.None;
                object obj3 = keyConverter.ConvertFrom(context, culture, str2);
                if (obj3 != null)
                {
                    object obj2 = modifierKeysConverter.ConvertFrom(context, culture, str3);
                    if (obj2 != null)
                    {
                        none = (ModifierKeys) obj2;
                    }
                    return new KeyGesture((Key) obj3, none, str4);
                }
            }
            return null;
        }

        [SuppressMessage("Microsoft.Performance", "CA1820:TestForEmptyStringsUsingStringLength")]
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            destinationType.TestNotNull("destinationType");
            if (destinationType == typeof(string))
            {
                if (value == null)
                {
                    return string.Empty;
                }
                KeyGesture gesture = value as KeyGesture;
                if (gesture != null)
                {
                    if (gesture.Key == Key.None)
                    {
                        return string.Empty;
                    }
                    string str = string.Empty;
                    string str2 = (string) keyConverter.ConvertTo(context, culture, gesture.Key, destinationType);
                    if (!string.IsNullOrEmpty(str2))
                    {
                        str = str + (modifierKeysConverter.ConvertTo(context, culture, gesture.Modifiers, destinationType) as string);
                        if (str != string.Empty)
                        {
                            str = str + '+';
                        }
                        str = str + str2;
                        if (!string.IsNullOrEmpty(gesture.DisplayString))
                        {
                            str = str + ',' + gesture.DisplayString;
                        }
                    }
                    return str;
                }
            }
            throw new NotImplementedException();
        }

        internal static bool IsDefinedKey(Key key)
        {
            return ((key >= Key.None) && (key <= Key.Divide));
        }
    }
}

