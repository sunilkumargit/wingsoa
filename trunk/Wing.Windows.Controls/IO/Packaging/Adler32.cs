namespace Telerik.IO.Packaging
{
    using System;

    internal sealed class Adler32
    {
        private const uint BASE = 0xfff1;
        private uint checksum;

        public Adler32()
        {
            this.Reset();
        }

        public void Reset()
        {
            this.checksum = 1;
        }

        public void Update(int value)
        {
            uint s1 = this.checksum & 0xffff;
            uint s2 = this.checksum >> 0x10;
            s1 = (uint) ((s1 + (value & 0xff)) % 0xfff1);
            s2 = (s1 + s2) % 0xfff1;
            this.checksum = (s2 << 0x10) + s1;
        }

        public void Update(byte[] buffer)
        {
            this.Update(buffer, 0, buffer.Length);
        }

        public void Update(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }
            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException("offset", "cannot be negative");
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count", "cannot be negative");
            }
            if (offset >= buffer.Length)
            {
                throw new ArgumentOutOfRangeException("offset", "not a valid index into buffer");
            }
            if ((offset + count) > buffer.Length)
            {
                throw new ArgumentOutOfRangeException("count", "exceeds buffer size");
            }
            uint s1 = this.checksum & 0xffff;
            uint s2 = this.checksum >> 0x10;
            while (count > 0)
            {
                int n = 0xed8;
                if (n > count)
                {
                    n = count;
                }
                count -= n;
                while (--n >= 0)
                {
                    s1 += (uint) (buffer[offset++] & 0xff);
                    s2 += s1;
                }
                s1 = s1 % 0xfff1;
                s2 = s2 % 0xfff1;
            }
            this.checksum = (s2 << 0x10) | s1;
        }

        public long Value
        {
            get
            {
                return (long) this.checksum;
            }
        }
    }
}

