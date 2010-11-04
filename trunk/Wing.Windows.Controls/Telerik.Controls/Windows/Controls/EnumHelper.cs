namespace Telerik.Windows.Controls
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Threading;

    internal static class EnumHelper
    {
        internal static string[] GetNames(Type enumType)
        {
            if (enumType == null)
            {
                throw new ArgumentNullException("enumType");
            }
            if (!enumType.IsEnum)
            {
                throw new ArgumentException("Argument must be Enum", "enumType");
            }
            return (from field in enumType.GetFields()
                where field.IsLiteral
                select field.Name).ToArray<string>();
        }

        internal static IEnumerable<T> GetValues<T>() where T: struct
        {
            return GetValues(typeof(T)).Cast<T>();
        }

        internal static IEnumerable GetValues(Type enumType)
        {
            //.TODO.
            return new List<Object>();
        }

    }
}

