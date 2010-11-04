namespace Telerik.IO.Packaging
{
    using System;
    using System.IO;

    internal class DeflaterOutputStream : Stream
    {
        private Stream baseOutputStream;
        private Telerik.IO.Packaging.Deflater deflater;
        private bool isClosed;
        private byte[] outputBuffer;

        public DeflaterOutputStream(Stream stream) : this(stream, new Telerik.IO.Packaging.Deflater(), 0x200)
        {
        }

        public DeflaterOutputStream(Stream stream, Telerik.IO.Packaging.Deflater deflater) : this(stream, deflater, 0x200)
        {
        }

        protected DeflaterOutputStream(Stream stream, Telerik.IO.Packaging.Deflater deflater, int bufferSize)
        {
            this.baseOutputStream = stream;
            this.outputBuffer = new byte[bufferSize];
            this.deflater = deflater;
        }

        public override void Close()
        {
            if (!this.isClosed)
            {
                this.isClosed = true;
                try
                {
                    this.Finish();
                }
                finally
                {
                    this.baseOutputStream.Close();
                }
            }
        }

        protected void Deflate()
        {
            while (!this.deflater.IsNeedingInput)
            {
                int deflateCount = this.deflater.Deflate(this.outputBuffer, 0, this.outputBuffer.Length);
                if (deflateCount <= 0)
                {
                    break;
                }
                this.baseOutputStream.Write(this.outputBuffer, 0, deflateCount);
            }
            if (!this.deflater.IsNeedingInput)
            {
                throw new InvalidOperationException("DeflaterOutputStream can't deflate all input?");
            }
        }

        public virtual void Finish()
        {
            this.deflater.Finish();
            while (!this.deflater.IsFinished)
            {
                int len = this.deflater.Deflate(this.outputBuffer, 0, this.outputBuffer.Length);
                if (len <= 0)
                {
                    break;
                }
                this.baseOutputStream.Write(this.outputBuffer, 0, len);
            }
            if (!this.deflater.IsFinished)
            {
                throw new InvalidOperationException("Can't deflate all input?");
            }
            this.baseOutputStream.Flush();
        }

        public override void Flush()
        {
            this.deflater.Flush();
            this.Deflate();
            this.baseOutputStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException("DeflaterOutputStream Read not supported");
        }

        public override int ReadByte()
        {
            throw new NotSupportedException("DeflaterOutputStream ReadByte not supported");
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException("DeflaterOutputStream Seek not supported");
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException("DeflaterOutputStream SetLength not supported");
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            this.deflater.SetInput(buffer, offset, count);
            this.Deflate();
        }

        public override void WriteByte(byte value)
        {
            byte[] b = new byte[] { value };
            this.Write(b, 0, 1);
        }

        protected Stream BaseOutputStream
        {
            get
            {
                return this.baseOutputStream;
            }
        }

        public bool CanPatchEntries
        {
            get
            {
                return this.baseOutputStream.CanSeek;
            }
        }

        public override bool CanRead
        {
            get
            {
                return false;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return false;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return this.baseOutputStream.CanWrite;
            }
        }

        protected Telerik.IO.Packaging.Deflater Deflater
        {
            get
            {
                return this.deflater;
            }
        }

        public override long Length
        {
            get
            {
                return this.baseOutputStream.Length;
            }
        }

        public override long Position
        {
            get
            {
                return this.baseOutputStream.Position;
            }
            set
            {
                throw new NotSupportedException("Position property not supported");
            }
        }
    }
}

