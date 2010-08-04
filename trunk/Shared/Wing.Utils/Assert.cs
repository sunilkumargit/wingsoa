using System;

namespace Wing.Utils
{
    public static class Assert
    {
        public static void NullArgument(Object value, String argName)
        {
            if (value == null || (value.ToString() == ""))
                throw new ArgumentNullException(argName);
        }
    }
}
