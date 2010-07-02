using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
