using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wing.Utils
{
    public static class Converter
    {
        public static string ByteArrayToHexString(byte[] bytes)
        {
            StringBuilder result = new StringBuilder();
            for (var i = 0; i < bytes.Length; i++)
                result.Append(bytes[i].ToString("X2"));
            return result.ToString();
        }
    }

}
