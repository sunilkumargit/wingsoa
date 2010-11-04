namespace Telerik.Windows.Media.Imaging
{
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;

    internal static class StreamExtension
    {
        public static void WriteInt(this Stream stream, int value)
        {
            stream.Write(BitConverter.GetBytes(value), 0, 4);
        }

        public static void WriteShort(this Stream stream, short value)
        {
            stream.Write(BitConverter.GetBytes(value), 0, 2);
        }

        public static void WriteToStream(this Stream source, Stream destination)
        {
            int bytesRead;
            byte[] buf = new byte[0x1000];
            while ((bytesRead = source.Read(buf, 0, 0x1000)) > 0)
            {
                destination.Write(buf, 0, bytesRead);
            }
        }
    }
}

