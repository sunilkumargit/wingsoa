namespace Telerik.IO.Packaging
{
    using System;

    internal class PendingBuffer
    {
        private int bitCount;
        private uint bits;
        private byte[] buffer;
        private int end;
        private int start;

        public PendingBuffer() : this(0x10000)
        {
        }

        public PendingBuffer(int bufferSize)
        {
            this.buffer = new byte[bufferSize];
        }

        public void AlignToByte()
        {
            if (this.bitCount > 0)
            {
                this.buffer[this.end++] = (byte) this.bits;
                if (this.bitCount > 8)
                {
                    this.buffer[this.end++] = (byte) (this.bits >> 8);
                }
            }
            this.bits = 0;
            this.bitCount = 0;
        }

        public int Flush(byte[] output, int offset, int length)
        {
            if (this.bitCount >= 8)
            {
                this.buffer[this.end++] = (byte) this.bits;
                this.bits = this.bits >> 8;
                this.bitCount -= 8;
            }
            if (length > (this.end - this.start))
            {
                length = this.end - this.start;
                Array.Copy(this.buffer, this.start, output, offset, length);
                this.start = 0;
                this.end = 0;
                return length;
            }
            Array.Copy(this.buffer, this.start, output, offset, length);
            this.start += length;
            return length;
        }

        public void Reset()
        {
            this.start = this.end = this.bitCount = 0;
        }

        public byte[] ToByteArray()
        {
            byte[] result = new byte[this.end - this.start];
            Array.Copy(this.buffer, this.start, result, 0, result.Length);
            this.start = 0;
            this.end = 0;
            return result;
        }

        public void WriteBits(int b, int count)
        {
            this.bits |= (uint) (b << this.bitCount);
            this.bitCount += count;
            if (this.bitCount >= 0x10)
            {
                this.buffer[this.end++] = (byte) this.bits;
                this.buffer[this.end++] = (byte) (this.bits >> 8);
                this.bits = this.bits >> 0x10;
                this.bitCount -= 0x10;
            }
        }

        public void WriteBlock(byte[] block, int offset, int length)
        {
            Array.Copy(block, offset, this.buffer, this.end, length);
            this.end += length;
        }

        public void WriteByte(int value)
        {
            this.buffer[this.end++] = (byte) value;
        }

        public void WriteInt(int value)
        {
            this.buffer[this.end++] = (byte) value;
            this.buffer[this.end++] = (byte) (value >> 8);
            this.buffer[this.end++] = (byte) (value >> 0x10);
            this.buffer[this.end++] = (byte) (value >> 0x18);
        }

        public void WriteShort(int value)
        {
            this.buffer[this.end++] = (byte) value;
            this.buffer[this.end++] = (byte) (value >> 8);
        }

        public void WriteShortMSB(int s)
        {
            this.buffer[this.end++] = (byte) (s >> 8);
            this.buffer[this.end++] = (byte) s;
        }

        public int BitCount
        {
            get
            {
                return this.bitCount;
            }
        }

        public bool IsFlushed
        {
            get
            {
                return (this.end == 0);
            }
        }
    }
}

