namespace Telerik.IO.Packaging
{
    using System;
    using System.IO;

    internal class ZipHelperStream : Stream
    {
        private bool isOwner;
        private Stream stream;

        public ZipHelperStream(Stream stream)
        {
            this.stream = stream;
        }

        public override void Close()
        {
            Stream streamToClose = this.stream;
            this.stream = null;
            if (this.isOwner && (streamToClose != null))
            {
                this.isOwner = false;
                streamToClose.Close();
            }
        }

        public override void Flush()
        {
            this.stream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return this.stream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return this.stream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            this.stream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            this.stream.Write(buffer, offset, count);
        }

        public void WriteEndOfCentralDirectory(long numberOfDirectoryEntries, long sizeEntries, long startOfCentralDirectory)
        {
            this.WriteLEInt(0x6054b50);
            this.WriteLEShort(0);
            this.WriteLEShort(0);
            if (numberOfDirectoryEntries >= 0xffffL)
            {
                this.WriteLEUshort(0xffff);
                this.WriteLEUshort(0xffff);
            }
            else
            {
                this.WriteLEShort((short) numberOfDirectoryEntries);
                this.WriteLEShort((short) numberOfDirectoryEntries);
            }
            if (sizeEntries >= 0xffffffffL)
            {
                this.WriteLEUint(uint.MaxValue);
            }
            else
            {
                this.WriteLEInt((int) sizeEntries);
            }
            if (startOfCentralDirectory >= 0xffffffffL)
            {
                this.WriteLEUint(uint.MaxValue);
            }
            else
            {
                this.WriteLEInt((int) startOfCentralDirectory);
            }
            this.WriteLEShort(0);
        }

        public void WriteLEInt(int value)
        {
            this.WriteLEShort(value);
            this.WriteLEShort(value >> 0x10);
        }

        public void WriteLEShort(int value)
        {
            this.stream.WriteByte((byte) (value & 0xff));
            this.stream.WriteByte((byte) ((value >> 8) & 0xff));
        }

        public void WriteLEUint(uint value)
        {
            this.WriteLEUshort((ushort) (value & 0xffff));
            this.WriteLEUshort((ushort) (value >> 0x10));
        }

        public void WriteLEUshort(ushort value)
        {
            this.stream.WriteByte((byte) (value & 0xff));
            this.stream.WriteByte((byte) (value >> 8));
        }

        public override bool CanRead
        {
            get
            {
                return this.stream.CanRead;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return this.stream.CanSeek;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return this.stream.CanWrite;
            }
        }

        public override long Length
        {
            get
            {
                return this.stream.Length;
            }
        }

        public override long Position
        {
            get
            {
                return this.stream.Position;
            }
            set
            {
                this.stream.Position = value;
            }
        }
    }
}

