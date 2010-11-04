namespace Telerik.Windows.Media.Imaging
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Windows.Media.Imaging;
    using Telerik.IO.Packaging;

    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Png")]
    public class PngBitmapEncoder : BitmapEncoder, IDisposable
    {
        private PngCodecInfo codecInfo = new PngCodecInfo();
        private const int MaxBlockSize = 0xffff;
        private byte[] pixels;
        private Stream pngStream;
        private WriteableBitmap writableImage;

        private void CreateWritableData()
        {
            int width = this.writableImage.PixelWidth;
            int height = this.writableImage.PixelHeight;
            this.pixels = new byte[(height * width) * 4];
            int i = 0;
            foreach (int pixel in this.writableImage.Pixels)
            {
                byte[] bpixel = BitConverter.GetBytes(pixel);
                byte tmp = bpixel[0];
                bpixel[0] = bpixel[2];
                bpixel[2] = tmp;
                bpixel.CopyTo(this.pixels, i);
                i += 4;
            }
        }

        public sealed override void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool cleanUpBoth)
        {
            if (cleanUpBoth && (this.pngStream != null))
            {
                this.pngStream.Dispose();
            }
        }

        private Stream Encode(WriteableBitmap image)
        {
            this.writableImage = image;
            this.pngStream = new MemoryStream();
            this.WriteHeader();
            this.CreateWritableData();
            byte[] fourByteData = new byte[4];
            byte[] size = BitConverter.GetBytes((double)75000.0);
            fourByteData[0] = size[3];
            fourByteData[1] = size[2];
            fourByteData[2] = size[1];
            fourByteData[3] = size[0];
            this.WriteChunk("gAMA", fourByteData);
            this.WriteDataChunks();
            this.WriteEndChunk();
            this.pngStream.Flush();
            return this.pngStream;
        }

        public override void Save(Stream stream)
        {
            base.Save(stream);
            using (Stream encoderStream = this.Encode(base.Frames[0]))
            {
                encoderStream.Position = 0L;
                encoderStream.WriteToStream(stream);
            }
        }

        private void WriteChunk(string type, byte[] data)
        {
            this.WriteChunk(type, data, 0, (data != null) ? data.Length : 0);
        }

        private void WriteChunk(string type, byte[] data, int offset, int length)
        {
            byte[] lengthArray = BitConverter.GetBytes(length);
            Array.Reverse(lengthArray);
            this.pngStream.Write(lengthArray, 0, 4);
            byte[] typeArray = new byte[] { (byte)type[0], (byte)type[1], (byte)type[2], (byte)type[3] };
            this.pngStream.Write(typeArray, 0, 4);
            if (data != null)
            {
                this.pngStream.Write(data, offset, length);
            }
            Crc32 crc = new Crc32();
            crc.Update(typeArray);
            if (data != null)
            {
                crc.Update(data, offset, length);
            }
            byte[] crcArray = BitConverter.GetBytes((uint)crc.Value);
            Array.Reverse(crcArray);
            this.pngStream.Write(crcArray, 0, 4);
        }

        private void WriteDataChunks()
        {
            byte[] data = new byte[((this.writableImage.PixelWidth * this.writableImage.PixelHeight) * 4) + this.writableImage.PixelHeight];
            int rowLength = (this.writableImage.PixelWidth * 4) + 1;
            for (int y = 0; y < this.writableImage.PixelHeight; y++)
            {
                byte compression = 0;
                if (y > 0)
                {
                    compression = 2;
                }
                data[y * rowLength] = compression;
                for (int x = 0; x < this.writableImage.PixelWidth; x++)
                {
                    int dataOffset = ((y * rowLength) + (x * 4)) + 1;
                    int pixelOffset = ((y * this.writableImage.PixelWidth) + x) * 4;
                    data[dataOffset] = this.pixels[pixelOffset];
                    data[dataOffset + 1] = this.pixels[pixelOffset + 1];
                    data[dataOffset + 2] = this.pixels[pixelOffset + 2];
                    data[dataOffset + 3] = this.pixels[pixelOffset + 3];
                    if (y > 0)
                    {
                        int lastOffset = (((y - 1) * this.writableImage.PixelWidth) + x) * 4;
                        data[dataOffset] = (byte)(data[dataOffset] - this.pixels[lastOffset]);
                        data[dataOffset + 1] = (byte)(data[dataOffset + 1] - this.pixels[lastOffset + 1]);
                        data[dataOffset + 2] = (byte)(data[dataOffset + 2] - this.pixels[lastOffset + 2]);
                        data[dataOffset + 3] = (byte)(data[dataOffset + 3] - this.pixels[lastOffset + 3]);
                    }
                }
            }
            byte[] buffer = null;
            int bufferLength = 0;
            using (MemoryStream tempStream = new MemoryStream())
            {
                using (DeflaterOutputStream zStream = new DeflaterOutputStream(tempStream))
                {
                    zStream.Write(data, 0, data.Length);
                    zStream.Flush();
                    zStream.Finish();
                    bufferLength = (int)tempStream.Length;
                    buffer = tempStream.GetBuffer();
                }
            }
            int numChunks = bufferLength / 0xffff;
            if ((bufferLength % 0xffff) != 0)
            {
                numChunks++;
            }
            for (int i = 0; i < numChunks; i++)
            {
                int length = bufferLength - (i * 0xffff);
                if (length > 0xffff)
                {
                    length = 0xffff;
                }
                this.WriteChunk("IDAT", buffer, i * 0xffff, length);
            }
        }

        private void WriteEndChunk()
        {
            this.WriteChunk("IEND", null);
        }

        private void WriteHeader()
        {
            byte[] pngHeader = new byte[] { 0x89, 80, 0x4e, 0x47, 13, 10, 0x1a, 10 };
            this.pngStream.Write(pngHeader, 0, 8);
            PngHeader header = new PngHeader
            {
                Width = this.writableImage.PixelWidth,
                Height = this.writableImage.PixelHeight,
                ColorType = 6,
                BitDepth = 8,
                FilterMethod = 0,
                CompressionMethod = 0,
                InterlaceMethod = 0
            };
            this.WriteHeaderChunk(header);
        }

        private void WriteHeaderChunk(PngHeader header)
        {
            byte[] chunkData = new byte[13];
            byte[] buffer = null;
            buffer = BitConverter.GetBytes(header.Width);
            Array.Reverse(buffer);
            Array.Copy(buffer, 0, chunkData, 0, 4);
            buffer = BitConverter.GetBytes(header.Height);
            Array.Reverse(buffer);
            Array.Copy(buffer, 0, chunkData, 4, 4);
            chunkData[8] = header.BitDepth;
            chunkData[9] = header.ColorType;
            chunkData[10] = header.CompressionMethod;
            chunkData[11] = header.FilterMethod;
            chunkData[12] = header.InterlaceMethod;
            this.WriteChunk("IHDR", chunkData);
        }

        public override BitmapCodecInfo CodecInfo
        {
            get
            {
                return this.codecInfo;
            }
        }

        private static class PngChunkTypes
        {
            public const string Data = "IDAT";
            public const string End = "IEND";
            public const string Gamma = "gAMA";
            public const string Header = "IHDR";
            public const string Palette = "PLTE";
            public const string PaletteAlpha = "tRNS";
            public const string Text = "tEXt";
        }

        private class PngHeader
        {
            public byte BitDepth { get; set; }

            public byte ColorType { get; set; }

            public byte CompressionMethod { get; set; }

            public byte FilterMethod { get; set; }

            public int Height { get; set; }

            public byte InterlaceMethod { get; set; }

            public int Width { get; set; }
        }
    }
}

