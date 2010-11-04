namespace Telerik.IO.Packaging
{
    using System;
    using System.Text;

    internal sealed class ZipConstants
    {
        public const int CentralHeaderBaseSize = 0x2e;
        public const int CentralHeaderSignature = 0x2014b50;
        public const int DataDescriptorSignature = 0x8074b50;
        public const int DataDescriptorSize = 0x10;
        public const int EndOfCentralDirectorySignature = 0x6054b50;
        public const int LocalHeaderBaseSize = 30;
        public const int LocalHeaderSignature = 0x4034b50;
        public const int VersionMadeBy = 0x2d;

        public static byte[] ConvertToArray(string str)
        {
            if (str == null)
            {
                return new byte[0];
            }
            return Encoding.GetEncoding(DefaultCodePage.WebName).GetBytes(str);
        }

        public static byte[] ConvertToArray(int flags, string str)
        {
            if (str == null)
            {
                return new byte[0];
            }
            if ((flags & 0x800) != 0)
            {
                return Encoding.UTF8.GetBytes(str);
            }
            return ConvertToArray(str);
        }

        public static Encoding DefaultCodePage
        {
            get
            {
                return Encoding.UTF8;
            }
        }
    }
}

