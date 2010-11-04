namespace Telerik.Windows.Media.Imaging
{
    using System;
    using System.IO;
    using System.Windows.Media.Imaging;

    public class BmpBitmapEncoder : BitmapEncoder
    {
        private BmpCodecInfo codecInfo = new BmpCodecInfo();

        private static Stream Encode(WriteableBitmap image)
        {
            int width = image.PixelWidth;
            int height = image.PixelHeight;
            int[] rgb = image.Pixels;
            int pad = (4 - ((width * 3) % 4)) % 4;
            int size = 0x36 + (height * (pad + (width * 3)));
            MemoryStream stream = new MemoryStream(size);
            SaveHeader(stream, width, height, size);
            for (int row = height - 1; row >= 0; row--)
            {
                for (int column = 0; column < width; column++)
                {
                    uint val = (uint) rgb[column + (width * row)];
                    byte byteToWrite = (byte) (val & 0xff);
                    stream.WriteByte(byteToWrite);
                    byteToWrite = (byte) ((val >> 8) & 0xff);
                    stream.WriteByte(byteToWrite);
                    byteToWrite = (byte) ((val >> 0x10) & 0xff);
                    stream.WriteByte(byteToWrite);
                }
                for (int i = 0; i < pad; i++)
                {
                    stream.WriteByte(0);
                }
            }
            return stream;
        }

        public override void Save(Stream stream)
        {
            base.Save(stream);
            using (Stream encoderStream = Encode(base.Frames[0]))
            {
                encoderStream.Position = 0L;
                encoderStream.WriteToStream(stream);
            }
        }

        private static void SaveHeader(Stream stream, int width, int height, int size)
        {
            stream.WriteByte(0x42);
            stream.WriteByte(0x4d);
            stream.WriteInt(size);
            stream.WriteInt(0);
            stream.WriteInt(0x36);
            stream.WriteInt(40);
            stream.WriteInt(width);
            stream.WriteInt(height);
            stream.WriteShort(1);
            stream.WriteShort(0x18);
            stream.WriteInt(0);
            stream.WriteInt(0);
            stream.WriteInt(0);
            stream.WriteInt(0);
            stream.WriteInt(0);
            stream.WriteInt(0);
        }

        public override BitmapCodecInfo CodecInfo
        {
            get
            {
                return this.codecInfo;
            }
        }
    }
}

