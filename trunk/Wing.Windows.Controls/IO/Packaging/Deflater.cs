namespace Telerik.IO.Packaging
{
    using System;

    internal class Deflater
    {
        public const int BestCompression = 9;
        private const int BusyState = 0x10;
        private const int ClosedState = 0x7f;
        public const int DefaultCompression = -1;
        private const int DEFLATED = 8;
        private DeflaterEngine engine;
        private const int FinishedState = 30;
        private const int FinishingState = 0x1c;
        private const int FlushingState = 20;
        private const int InitState = 0;
        private const int IsFinishing = 8;
        private const int IsFlushing = 4;
        private const int IsSetDict = 1;
        private int level;
        public const int NoCompression = 0;
        private PendingBuffer pending;
        private const int SetDictState = 1;
        private int state;
        private bool suppressHeaderOrFooter;
        private long totalOut;

        public Deflater() : this(-1, false)
        {
        }

        public Deflater(int level) : this(level, false)
        {
        }

        public Deflater(int level, bool suppressHeaderOrFooter)
        {
            if (level == -1)
            {
                level = 6;
            }
            else if ((level < 0) || (level > 9))
            {
                throw new ArgumentOutOfRangeException("level");
            }
            this.pending = new PendingBuffer();
            this.engine = new DeflaterEngine(this.pending);
            this.suppressHeaderOrFooter = suppressHeaderOrFooter;
            this.engine.Strategy = DeflateStrategy.Default;
            this.SetLevel(level);
            this.Reset();
        }

        public int Deflate(byte[] output, int offset, int length)
        {
            int origLength = length;
            if (this.state == 0x7f)
            {
                throw new InvalidOperationException("Deflater closed");
            }
            if (this.state < 0x10)
            {
                int header = 0x7800;
                int level_flags = (this.level - 1) >> 1;
                if ((level_flags < 0) || (level_flags > 3))
                {
                    level_flags = 3;
                }
                header |= level_flags << 6;
                if ((this.state & 1) != 0)
                {
                    header |= 0x20;
                }
                header += 0x1f - (header % 0x1f);
                this.pending.WriteShortMSB(header);
                if ((this.state & 1) != 0)
                {
                    int chksum = this.engine.Adler;
                    this.engine.ResetAdler();
                    this.pending.WriteShortMSB(chksum >> 0x10);
                    this.pending.WriteShortMSB(chksum & 0xffff);
                }
                this.state = 0x10 | (this.state & 12);
            }
            while (true)
            {
                do
                {
                    int count = this.pending.Flush(output, offset, length);
                    offset += count;
                    this.totalOut += count;
                    length -= count;
                    if ((length == 0) || (this.state == 30))
                    {
                        return (origLength - length);
                    }
                }
                while (this.engine.Deflate((this.state & 4) != 0, (this.state & 8) != 0));
                if (this.state == 0x10)
                {
                    return (origLength - length);
                }
                if (this.state == 20)
                {
                    if (this.level != 0)
                    {
                        for (int neededbits = 8 + (-this.pending.BitCount & 7); neededbits > 0; neededbits -= 10)
                        {
                            this.pending.WriteBits(2, 10);
                        }
                    }
                    this.state = 0x10;
                }
                else if (this.state == 0x1c)
                {
                    this.pending.AlignToByte();
                    if (!this.suppressHeaderOrFooter)
                    {
                        int adler = this.engine.Adler;
                        this.pending.WriteShortMSB(adler >> 0x10);
                        this.pending.WriteShortMSB(adler & 0xffff);
                    }
                    this.state = 30;
                }
            }
        }

        public void Finish()
        {
            this.state |= 12;
        }

        public void Flush()
        {
            this.state |= 4;
        }

        public void Reset()
        {
            this.state = this.suppressHeaderOrFooter ? 0x10 : 0;
            this.totalOut = 0L;
            this.pending.Reset();
            this.engine.Reset();
        }

        public void SetInput(byte[] input, int offset, int count)
        {
            if ((this.state & 8) != 0)
            {
                throw new InvalidOperationException("Finish() already called");
            }
            this.engine.SetInput(input, offset, count);
        }

        public void SetLevel(int compressionLevel)
        {
            if (compressionLevel == -1)
            {
                compressionLevel = 6;
            }
            else if ((compressionLevel < 0) || (compressionLevel > 9))
            {
                throw new ArgumentOutOfRangeException("compressionLevel");
            }
            if (this.level != compressionLevel)
            {
                this.level = compressionLevel;
                this.engine.SetLevel(compressionLevel);
            }
        }

        public bool IsFinished
        {
            get
            {
                return ((this.state == 30) && this.pending.IsFlushed);
            }
        }

        public bool IsNeedingInput
        {
            get
            {
                return this.engine.NeedsInput();
            }
        }

        public long TotalOut
        {
            get
            {
                return this.totalOut;
            }
        }
    }
}

