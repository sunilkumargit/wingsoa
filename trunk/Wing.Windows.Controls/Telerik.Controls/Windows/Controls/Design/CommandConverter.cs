namespace Telerik.Windows.Controls.Design
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Windows.Input;
    using Telerik.Windows;
    using Telerik.Windows.Controls;

    public sealed class CommandConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return (sourceType == typeof(string));
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return (destinationType == typeof(string));
        }

        [SuppressMessage("Microsoft.Performance", "CA1820:TestForEmptyStringsUsingStringLength")]
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string ownerNamsepace;
            string ownerTypeName;
            string commandName;
            string stringValue = value as string;
            switch (stringValue)
            {
                case null:
                    throw new NotImplementedException();

                case "":
                    return null;
            }
            ParseUri(stringValue, out ownerNamsepace, out ownerTypeName, out commandName);
            if (!string.IsNullOrEmpty(ownerTypeName))
            {
                commandName = ownerTypeName + "." + commandName;
            }
            return RoutedCommand.GetRegisteredCommand(commandName);
        }

        internal static ICommand ConvertFromHelper(Type ownerType, string commandName)
        {
            ICommand command = null;
            PropertyInfo commandProperty = ownerType.GetProperty(commandName, BindingFlags.Public | BindingFlags.Static);
            if (commandProperty != null)
            {
                command = commandProperty.GetValue(null, null) as ICommand;
            }
            if (command == null)
            {
                FieldInfo commandField = ownerType.GetField(commandName, BindingFlags.Public | BindingFlags.Static);
                if (commandField != null)
                {
                    command = commandField.GetValue(null) as ICommand;
                }
            }
            return command;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            destinationType.TestNotNull("destinationType");
            if ((destinationType != typeof(string)) || (value != null))
            {
                throw new NotImplementedException();
            }
            return string.Empty;
        }

        private static void ParseUri(string source, out string ownerNamsepace, out string typeName, out string commandName)
        {
            ownerNamsepace = null;
            typeName = null;
            commandName = source.Trim();
            int typePlusNamespaceLength = commandName.LastIndexOf(".", StringComparison.Ordinal);
            if (typePlusNamespaceLength >= 0)
            {
                typeName = commandName.Substring(0, typePlusNamespaceLength);
                commandName = commandName.Substring(typePlusNamespaceLength + 1);
                int namespaceLength = typeName.LastIndexOfAny(new char[] { ':', '.' });
                if (namespaceLength >= 0)
                {
                    ownerNamsepace = typeName.Substring(0, namespaceLength);
                    typeName = typeName.Substring(namespaceLength + 1);
                }
            }
        }
    }
}

