namespace Telerik.Windows.Controls.Design
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Windows.Input;
    using Telerik.Windows;

    public class ModifierKeysConverter : TypeConverter
    {
        private const char ModifierDelimiter = '+';
        private const ModifierKeys ModifierKeysFlag = (ModifierKeys.Windows | ModifierKeys.Shift | ModifierKeys.Control | ModifierKeys.Alt);

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return (sourceType == typeof(string));
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return ((((destinationType == typeof(string)) && (context != null)) && ((context.Instance != null) && (context.Instance is ModifierKeys))) && IsDefinedModifierKeys((ModifierKeys) context.Instance));
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string stringValue = value as string;
            if (value == null)
            {
                throw new NotImplementedException();
            }
            return GetModifierKeys(stringValue.Trim(), CultureInfo.InvariantCulture);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            destinationType.TestNotNull("destinationType");
            if (destinationType != typeof(string))
            {
                throw new NotImplementedException();
            }
            ModifierKeys modifierKeys = (ModifierKeys) value;
            if (!IsDefinedModifierKeys(modifierKeys))
            {
                throw new ArgumentException("modifiers");
            }
            string str = string.Empty;
            if ((modifierKeys & ModifierKeys.Control) == ModifierKeys.Control)
            {
                str = str + MatchModifiers(ModifierKeys.Control);
            }
            if ((modifierKeys & ModifierKeys.Alt) == ModifierKeys.Alt)
            {
                if (str.Length > 0)
                {
                    str = str + '+';
                }
                str = str + MatchModifiers(ModifierKeys.Alt);
            }
            if ((modifierKeys & ModifierKeys.Windows) == ModifierKeys.Windows)
            {
                if (str.Length > 0)
                {
                    str = str + '+';
                }
                str = str + MatchModifiers(ModifierKeys.Windows);
            }
            if ((modifierKeys & ModifierKeys.Shift) != ModifierKeys.Shift)
            {
                return str;
            }
            if (str.Length > 0)
            {
                str = str + '+';
            }
            return (str + MatchModifiers(ModifierKeys.Shift));
        }

        private static ModifierKeys GetModifierKeys(string modifiersToken, CultureInfo culture)
        {
            ModifierKeys none = ModifierKeys.None;
            if (modifiersToken.Length != 0)
            {
                int length = 0;
                do
                {
                    length = modifiersToken.IndexOf('+');
                    string str = (length < 0) ? modifiersToken : modifiersToken.Substring(0, length);
                    switch (str.Trim().ToUpper(culture))
                    {
                        case "CONTROL":
                        case "CTRL":
                            none |= ModifierKeys.Control;
                            break;

                        case "SHIFT":
                            none |= ModifierKeys.Shift;
                            break;

                        case "ALT":
                            none |= ModifierKeys.Alt;
                            break;

                        case "WINDOWS":
                        case "WIN":
                            none |= ModifierKeys.Windows;
                            break;

                        case "APPLE":
                            none |= ModifierKeys.Windows;
                            break;

                        case "":
                            return none;

                        default:
                            throw new NotSupportedException("Unsupported_Modifier");
                    }
                    modifiersToken = modifiersToken.Substring(length + 1);
                }
                while (length != -1);
            }
            return none;
        }

        internal static bool IsDefinedModifierKeys(ModifierKeys modifierKeys)
        {
            if (modifierKeys != ModifierKeys.None)
            {
                return ((modifierKeys & ~(ModifierKeys.Windows | ModifierKeys.Shift | ModifierKeys.Control | ModifierKeys.Alt)) == ModifierKeys.None);
            }
            return true;
        }

        internal static string MatchModifiers(ModifierKeys modifierKeys)
        {
            string str = string.Empty;
            switch (modifierKeys)
            {
                case ModifierKeys.Alt:
                    return "Alt";

                case ModifierKeys.Control:
                    return "Ctrl";

                case (ModifierKeys.Control | ModifierKeys.Alt):
                    return str;

                case ModifierKeys.Shift:
                    return "Shift";

                case (ModifierKeys.Shift | ModifierKeys.Alt):
                case (ModifierKeys.Shift | ModifierKeys.Control):
                case (ModifierKeys.Shift | ModifierKeys.Control | ModifierKeys.Alt):
                    return str;

                case ModifierKeys.Windows:
                    return "Windows";
            }
            return str;
        }
    }
}

