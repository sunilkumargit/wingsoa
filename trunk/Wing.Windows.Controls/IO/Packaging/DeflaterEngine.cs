namespace Telerik.IO.Packaging
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    internal class DeflaterEngine : DeflaterConstants
    {
        private Adler32 adler;
        private int blockStart;
        private int compressionFunction;
        private int goodLength;
        private int hashIndex;
        private short[] head;
        private DeflaterHuffman huffman;
        private byte[] inputBuf;
        private int inputEnd;
        private int inputOff;
        private int lookahead;
        private int matchLen;
        private int matchStart;
        private int maxChain;
        private int maxLazy;
        private int niceLength;
        private PendingBuffer pending;
        private bool prevAvailable;
        private short[] previous;
        private DeflateStrategy strategy;
        private int strStart;
        private const int TooFar = 0x1000;
        private int totalIn;
        private byte[] window;

        public DeflaterEngine(PendingBuffer pending)
        {
            this.pending = pending;
            this.huffman = new DeflaterHuffman(this.pending);
            this.adler = new Adler32();
            this.window = new byte[0x10000];
            this.head = new short[0x8000];
            this.previous = new short[0x8000];
            this.blockStart = this.strStart = 1;
        }

        public bool Deflate(bool flush, bool finish)
        {
            bool progress;
            do
            {
                this.FillWindow();
                bool canFlush = flush && (this.inputOff == this.inputEnd);
                switch (this.compressionFunction)
                {
                    case 0:
                        progress = this.DeflateStored(canFlush, finish);
                        break;

                    case 1:
                        progress = this.DeflateFast(canFlush, finish);
                        break;

                    case 2:
                        progress = this.DeflateSlow(canFlush, finish);
                        break;

                    default:
                        throw new InvalidOperationException("unknown compressionFunction");
                }
            }
            while (this.pending.IsFlushed && progress);
            return progress;
        }

        private bool DeflateFast(bool flush, bool finish)
        {
            if ((this.lookahead >= 0x106) || flush)
            {
                goto Label_01EC;
            }
            return false;
        Label_0199:
            if (this.huffman.IsFull())
            {
                bool lastBlock = finish && (this.lookahead == 0);
                this.huffman.FlushBlock(this.window, this.blockStart, this.strStart - this.blockStart, lastBlock);
                this.blockStart = this.strStart;
                return !lastBlock;
            }
        Label_01EC:
            if ((this.lookahead >= 0x106) || flush)
            {
                int hashHead;
                if (this.lookahead == 0)
                {
                    this.huffman.FlushBlock(this.window, this.blockStart, this.strStart - this.blockStart, finish);
                    this.blockStart = this.strStart;
                    return false;
                }
                if (this.strStart > 0xfefa)
                {
                    this.SlideWindow();
                }
                if ((((this.lookahead >= 3) && ((hashHead = this.InsertString()) != 0)) && ((this.strategy != DeflateStrategy.HuffmanOnly) && ((this.strStart - hashHead) <= 0x7efa))) && this.FindLongestMatch(hashHead))
                {
                    bool full = this.huffman.TallyDist(this.strStart - this.matchStart, this.matchLen);
                    this.lookahead -= this.matchLen;
                    if ((this.matchLen <= this.maxLazy) && (this.lookahead >= 3))
                    {
                        while (--this.matchLen > 0)
                        {
                            this.strStart++;
                            this.InsertString();
                        }
                        this.strStart++;
                    }
                    else
                    {
                        this.strStart += this.matchLen;
                        if (this.lookahead >= 2)
                        {
                            this.UpdateHash();
                        }
                    }
                    this.matchLen = 2;
                    if (full)
                    {
                        goto Label_0199;
                    }
                    goto Label_01EC;
                }
                this.huffman.TallyLit(this.window[this.strStart] & 0xff);
                this.strStart++;
                this.lookahead--;
                goto Label_0199;
            }
            return true;
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        private bool DeflateSlow(bool flush, bool finish)
        {
            if ((this.lookahead >= 0x106) || flush)
            {
                while ((this.lookahead >= 0x106) || flush)
                {
                    if (this.lookahead == 0)
                    {
                        if (this.prevAvailable)
                        {
                            this.huffman.TallyLit(this.window[this.strStart - 1] & 0xff);
                        }
                        this.prevAvailable = false;
                        this.huffman.FlushBlock(this.window, this.blockStart, this.strStart - this.blockStart, finish);
                        this.blockStart = this.strStart;
                        return false;
                    }
                    if (this.strStart >= 0xfefa)
                    {
                        this.SlideWindow();
                    }
                    int prevMatch = this.matchStart;
                    int prevLen = this.matchLen;
                    if (this.lookahead >= 3)
                    {
                        int hashHead = this.InsertString();
                        if ((((this.strategy != DeflateStrategy.HuffmanOnly) && (hashHead != 0)) && (((this.strStart - hashHead) <= 0x7efa) && this.FindLongestMatch(hashHead))) && ((this.matchLen <= 5) && ((this.strategy == DeflateStrategy.Filtered) || ((this.matchLen == 3) && ((this.strStart - this.matchStart) > 0x1000)))))
                        {
                            this.matchLen = 2;
                        }
                    }
                    if ((prevLen >= 3) && (this.matchLen <= prevLen))
                    {
                        this.huffman.TallyDist((this.strStart - 1) - prevMatch, prevLen);
                        prevLen -= 2;
                        do
                        {
                            this.strStart++;
                            this.lookahead--;
                            if (this.lookahead >= 3)
                            {
                                this.InsertString();
                            }
                        }
                        while (--prevLen > 0);
                        this.strStart++;
                        this.lookahead--;
                        this.prevAvailable = false;
                        this.matchLen = 2;
                    }
                    else
                    {
                        if (this.prevAvailable)
                        {
                            this.huffman.TallyLit(this.window[this.strStart - 1] & 0xff);
                        }
                        this.prevAvailable = true;
                        this.strStart++;
                        this.lookahead--;
                    }
                    if (this.huffman.IsFull())
                    {
                        int len = this.strStart - this.blockStart;
                        if (this.prevAvailable)
                        {
                            len--;
                        }
                        bool lastBlock = (finish && (this.lookahead == 0)) && !this.prevAvailable;
                        this.huffman.FlushBlock(this.window, this.blockStart, len, lastBlock);
                        this.blockStart += len;
                        return !lastBlock;
                    }
                }
                return true;
            }
            return false;
        }

        private bool DeflateStored(bool flush, bool finish)
        {
            if (!flush && (this.lookahead == 0))
            {
                return false;
            }
            this.strStart += this.lookahead;
            this.lookahead = 0;
            int storedLength = this.strStart - this.blockStart;
            if (((storedLength < DeflaterConstants.MaxBlockSize) && ((this.blockStart >= 0x8000) || (storedLength < 0x7efa))) && !flush)
            {
                return true;
            }
            bool lastBlock = finish;
            if (storedLength > DeflaterConstants.MaxBlockSize)
            {
                storedLength = DeflaterConstants.MaxBlockSize;
                lastBlock = false;
            }
            this.huffman.FlushStoredBlock(this.window, this.blockStart, storedLength, lastBlock);
            this.blockStart += storedLength;
            return !lastBlock;
        }

        public void FillWindow()
        {
            if (this.strStart >= 0xfefa)
            {
                this.SlideWindow();
            }
            while ((this.lookahead < 0x106) && (this.inputOff < this.inputEnd))
            {
                int more = (0x10000 - this.lookahead) - this.strStart;
                if (more > (this.inputEnd - this.inputOff))
                {
                    more = this.inputEnd - this.inputOff;
                }
                Array.Copy(this.inputBuf, this.inputOff, this.window, this.strStart + this.lookahead, more);
                this.adler.Update(this.inputBuf, this.inputOff, more);
                this.inputOff += more;
                this.totalIn += more;
                this.lookahead += more;
            }
            if (this.lookahead >= 3)
            {
                this.UpdateHash();
            }
        }

        private bool FindLongestMatch(int curMatch)
        {
            int chainLength = this.maxChain;
            int length = this.niceLength;
            int scan = this.strStart;
            int best_end = this.strStart + this.matchLen;
            int best_len = Math.Max(this.matchLen, 2);
            int limit = Math.Max(this.strStart - 0x7efa, 0);
            int strend = (this.strStart + 0x102) - 1;
            byte scan_end1 = this.window[best_end - 1];
            byte scan_end = this.window[best_end];
            if (best_len >= this.goodLength)
            {
                chainLength = chainLength >> 2;
            }
            if (length > this.lookahead)
            {
                length = this.lookahead;
            }
            do
            {
                if (((this.window[curMatch + best_len] == scan_end) && (this.window[(curMatch + best_len) - 1] == scan_end1)) && ((this.window[curMatch] == this.window[scan]) && (this.window[curMatch + 1] == this.window[scan + 1])))
                {
                    int match = curMatch + 2;
                    scan += 2;
                    while ((((this.window[++scan] == this.window[++match]) && (this.window[++scan] == this.window[++match])) && ((this.window[++scan] == this.window[++match]) && (this.window[++scan] == this.window[++match]))) && (((this.window[++scan] == this.window[++match]) && (this.window[++scan] == this.window[++match])) && (((this.window[++scan] == this.window[++match]) && (this.window[++scan] == this.window[++match])) && (scan < strend))))
                    {
                    }
                    if (scan > best_end)
                    {
                        this.matchStart = curMatch;
                        best_end = scan;
                        best_len = scan - this.strStart;
                        if (best_len >= length)
                        {
                            break;
                        }
                        scan_end1 = this.window[best_end - 1];
                        scan_end = this.window[best_end];
                    }
                    scan = this.strStart;
                }
            }
            while (((curMatch = this.previous[curMatch & 0x7fff] & 0xffff) > limit) && (--chainLength != 0));
            this.matchLen = Math.Min(best_len, this.lookahead);
            return (this.matchLen >= 3);
        }

        private int InsertString()
        {
            short match;
            int hash = ((this.hashIndex << 5) ^ this.window[this.strStart + 2]) & 0x7fff;
            this.previous[this.strStart & 0x7fff] = match = this.head[hash];
            this.head[hash] = (short) this.strStart;
            this.hashIndex = hash;
            return (match & 0xffff);
        }

        public bool NeedsInput()
        {
            return (this.inputEnd == this.inputOff);
        }

        public void Reset()
        {
            this.huffman.Reset();
            this.adler.Reset();
            this.blockStart = this.strStart = 1;
            this.lookahead = 0;
            this.totalIn = 0;
            this.prevAvailable = false;
            this.matchLen = 2;
            for (int i = 0; i < 0x8000; i++)
            {
                this.head[i] = 0;
            }
            for (int i = 0; i < 0x8000; i++)
            {
                this.previous[i] = 0;
            }
        }

        public void ResetAdler()
        {
            this.adler.Reset();
        }

        public void SetDictionary(byte[] buffer, int offset, int length)
        {
            this.adler.Update(buffer, offset, length);
            if (length >= 3)
            {
                if (length > 0x7efa)
                {
                    offset += length - 0x7efa;
                    length = 0x7efa;
                }
                Array.Copy(buffer, offset, this.window, this.strStart, length);
                this.UpdateHash();
                length--;
                while (--length > 0)
                {
                    this.InsertString();
                    this.strStart++;
                }
                this.strStart += 2;
                this.blockStart = this.strStart;
            }
        }

        public void SetInput(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }
            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException("offset");
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count");
            }
            if (this.inputOff < this.inputEnd)
            {
                throw new InvalidOperationException("Old input was not completely processed");
            }
            int end = offset + count;
            if ((offset > end) || (end > buffer.Length))
            {
                throw new ArgumentOutOfRangeException("count");
            }
            this.inputBuf = buffer;
            this.inputOff = offset;
            this.inputEnd = end;
        }

        public void SetLevel(int level)
        {
            if ((level < 0) || (level > 9))
            {
                throw new ArgumentOutOfRangeException("level");
            }
            this.goodLength = DeflaterConstants.GoodLength[level];
            this.maxLazy = DeflaterConstants.MaxLazy[level];
            this.niceLength = DeflaterConstants.NiceLength[level];
            this.maxChain = DeflaterConstants.MaxChain[level];
            if (DeflaterConstants.CompressionFunction[level] != this.compressionFunction)
            {
                switch (this.compressionFunction)
                {
                    case 0:
                        if (this.strStart > this.blockStart)
                        {
                            this.huffman.FlushStoredBlock(this.window, this.blockStart, this.strStart - this.blockStart, false);
                            this.blockStart = this.strStart;
                        }
                        this.UpdateHash();
                        break;

                    case 1:
                        if (this.strStart > this.blockStart)
                        {
                            this.huffman.FlushBlock(this.window, this.blockStart, this.strStart - this.blockStart, false);
                            this.blockStart = this.strStart;
                        }
                        break;

                    case 2:
                        if (this.prevAvailable)
                        {
                            this.huffman.TallyLit(this.window[this.strStart - 1] & 0xff);
                        }
                        if (this.strStart > this.blockStart)
                        {
                            this.huffman.FlushBlock(this.window, this.blockStart, this.strStart - this.blockStart, false);
                            this.blockStart = this.strStart;
                        }
                        this.prevAvailable = false;
                        this.matchLen = 2;
                        break;
                }
                this.compressionFunction = DeflaterConstants.CompressionFunction[level];
            }
        }

        private void SlideWindow()
        {
            Array.Copy(this.window, 0x8000, this.window, 0, 0x8000);
            this.matchStart -= 0x8000;
            this.strStart -= 0x8000;
            this.blockStart -= 0x8000;
            for (int i = 0; i < 0x8000; i++)
            {
                int m = this.head[i] & 0xffff;
                this.head[i] = (m >= 0x8000) ? ((short) (m - 0x8000)) : ((short) 0);
            }
            for (int i = 0; i < 0x8000; i++)
            {
                int m = this.previous[i] & 0xffff;
                this.previous[i] = (m >= 0x8000) ? ((short) (m - 0x8000)) : ((short) 0);
            }
        }

        private void UpdateHash()
        {
            this.hashIndex = (this.window[this.strStart] << 5) ^ this.window[this.strStart + 1];
        }

        public int Adler
        {
            get
            {
                return (int) this.adler.Value;
            }
        }

        public DeflateStrategy Strategy
        {
            get
            {
                return this.strategy;
            }
            set
            {
                this.strategy = value;
            }
        }

        public int TotalIn
        {
            get
            {
                return this.totalIn;
            }
        }
    }
}

