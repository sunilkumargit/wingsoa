using System;

namespace Wing
{
    [System.Diagnostics.DebuggerStepThrough]
    public static class Assert
    {
        public static void NullArgument(Object value, String name)
        {
            if (value == null)
                throw new ArgumentNullException(name);
        }

        public static void EmptyString(String value, String name)
        {
            if (String.IsNullOrEmpty(value))
                throw new ArgumentException("Parametro nulo ou em branco.", name);
        }

        public static void EmptyNumber(double? value, String name)
        {
            if (!value.HasValue || value.Value == 0)
                throw new ArgumentException("Parametro nulo ou em branco.", name);
        }

        public static void EmptyNumber(double value, String name)
        {
            if (value == 0)
                throw new ArgumentException("Parametro não deve ser zero.", name);
        }
    }
}
