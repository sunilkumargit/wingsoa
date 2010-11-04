﻿namespace Telerik.Windows.Controls.Design
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Windows.Input;
    using Telerik.Windows;

    public class KeyConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return (sourceType == typeof(string));
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (((destinationType != typeof(string)) || (context == null)) || (context.Instance == null))
            {
                return false;
            }
            Key instance = (Key) context.Instance;
            return ((instance >= Key.None) && (instance <= Key.Divide));
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string stringValue = value as string;
            if (stringValue == null)
            {
                throw new NotImplementedException();
            }
            object key = GetKey(stringValue.Trim(), CultureInfo.InvariantCulture);
            if (key == null)
            {
                throw new NotSupportedException("Unsupported_Key");
            }
            return (Key) key;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            destinationType.TestNotNull("destinationType");
            if ((destinationType == typeof(string)) && (value != null))
            {
                Key key = (Key) value;
                if (key == Key.None)
                {
                    return string.Empty;
                }
                if ((key >= Key.D0) && (key <= Key.D9))
                {
                    return char.ToString((char) ((ushort) ((key - 0x22) + 0x30)));
                }
                if ((key >= Key.A) && (key <= Key.Z))
                {
                    return char.ToString((char) ((ushort) ((key - 0x2c) + 0x41)));
                }
                string str = MatchKey(key);
                if (str != null)
                {
                    return str;
                }
            }
            throw new NotImplementedException();
        }

        private static object GetKey(string keyToken, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(keyToken))
            {
                return Key.None;
            }
            keyToken = keyToken.ToUpper(culture);
            if ((keyToken.Length == 1) && char.IsLetterOrDigit(keyToken[0]))
            {
                if ((char.IsDigit(keyToken[0]) && (keyToken[0] >= '0')) && (keyToken[0] <= '9'))
                {
                    return (('"' + keyToken[0]) - 0x30);
                }
                if ((!char.IsLetter(keyToken[0]) || (keyToken[0] < 'A')) || (keyToken[0] > 'Z'))
                {
                    throw new ArgumentException("CannotConvertStringToType");
                }
                return (((',' + keyToken[0]) - 0x41) - 14);
            }
            Key escape = ~Key.None;
            switch (keyToken)
            {
                case "ESC":
                    escape = Key.Escape;
                    break;

                case "INS":
                    escape = Key.Insert;
                    break;

                case "DEL":
                    escape = Key.Delete;
                    break;

                case "BACKSPACE":
                    escape = Key.Back;
                    break;

                case "BKSP":
                    escape = Key.Back;
                    break;

                case "BS":
                    escape = Key.Back;
                    break;

                default:
                    escape = (Key) Enum.Parse(typeof(Key), keyToken, true);
                    break;
            }
            if (escape != ~Key.None)
            {
                return escape;
            }
            return null;
        }

        private static string MatchKey(Key key)
        {
            switch (key)
            {
                case Key.None:
                    return string.Empty;

                case Key.Back:
                    return "Backspace";

                case Key.Escape:
                    return "Esc";
            }
            if ((key >= Key.None) && (key <= Key.Divide))
            {
                return key.ToString();
            }
            return null;
        }
    }
}

