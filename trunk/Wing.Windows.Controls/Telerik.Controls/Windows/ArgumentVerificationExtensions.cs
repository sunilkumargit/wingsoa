namespace Telerik.Windows
{
    using System;
    using System.Runtime.CompilerServices;

    internal static class ArgumentVerificationExtensions
    {
        public static void TestNotEmptyString(this string parameter, string parameterName)
        {
            if (string.IsNullOrEmpty(parameter))
            {
                throw new ArgumentException(string.Format("The parameter '{0}' should not be empty string.", parameterName), parameterName);
            }
        }

        public static void TestNotNull(this object parameter, string parameterName)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException(parameterName);
            }
        }
    }
}

