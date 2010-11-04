namespace Telerik.Windows.Controls.Design
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using Telerik.Windows;
    using Telerik.Windows.Controls;

    public class MouseActionConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return (sourceType == typeof(string));
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return ((((destinationType == typeof(string)) && (context != null)) && (context.Instance != null)) && IsDefinedMouseAction((MouseAction) context.Instance));
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            value.TestNotNull("value");
            string str = value as string;
            if (str == null)
            {
                throw new NotSupportedException("'value' should be of type String");
            }
            str = str.Trim().ToUpper(CultureInfo.InvariantCulture);
            if (string.IsNullOrEmpty(str))
            {
                return MouseAction.None;
            }
            switch (str)
            {
                case "LEFTCLICK":
                    return MouseAction.LeftClick;

                case "RIGHTCLICK":
                    return MouseAction.RightClick;

                case "MIDDLECLICK":
                    return MouseAction.MiddleClick;

                case "WHEELCLICK":
                    return MouseAction.WheelClick;

                case "LEFTDOUBLECLICK":
                    return MouseAction.LeftDoubleClick;

                case "RIGHTDOUBLECLICK":
                    return MouseAction.RightDoubleClick;

                case "MIDDLEDOUBLECLICK":
                    return MouseAction.MiddleDoubleClick;
            }
            throw new NotSupportedException("Unsupported_MouseAction");
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            destinationType.TestNotNull("destinationType");
            if ((destinationType != typeof(string)) || (value == null))
            {
                throw new NotSupportedException(destinationType.ToString());
            }
            MouseAction mouseAction = (MouseAction) value;
            if (IsDefinedMouseAction(mouseAction))
            {
                string str = null;
                switch (mouseAction)
                {
                    case MouseAction.None:
                        str = string.Empty;
                        break;

                    case MouseAction.LeftClick:
                        str = "LeftClick";
                        break;

                    case MouseAction.RightClick:
                        str = "RightClick";
                        break;

                    case MouseAction.MiddleClick:
                        str = "MiddleClick";
                        break;

                    case MouseAction.WheelClick:
                        str = "WheelClick";
                        break;

                    case MouseAction.LeftDoubleClick:
                        str = "LeftDoubleClick";
                        break;

                    case MouseAction.RightDoubleClick:
                        str = "RightDoubleClick";
                        break;

                    case MouseAction.MiddleDoubleClick:
                        str = "MiddleDoubleClick";
                        break;
                }
                if (str != null)
                {
                    return str;
                }
            }
            throw new NotSupportedException("value");
        }

        internal static bool IsDefinedMouseAction(MouseAction mouseAction)
        {
            return ((mouseAction >= MouseAction.None) && (mouseAction <= MouseAction.MiddleDoubleClick));
        }
    }
}

