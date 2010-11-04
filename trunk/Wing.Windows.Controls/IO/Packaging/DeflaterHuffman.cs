namespace Telerik.IO.Packaging
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    internal class DeflaterHuffman
    {
        private static readonly byte[] bit4Reverse = new byte[] { 0, 8, 4, 12, 2, 10, 6, 14, 1, 9, 5, 13, 3, 11, 7, 15 };
        private const int BitLengthNumber = 0x13;
        private static readonly int[] BitLengthOrder = new int[] { 
            0x10, 0x11, 0x12, 0, 8, 7, 9, 6, 10, 5, 11, 4, 12, 3, 13, 2, 
            14, 1, 15
         };
        private Tree bitLengthTree;
        private const int BUFSIZE = 0x4000;
        private short[] distanceBuf;
        private const int DistanceNumber = 30;
        private Tree distTree;
        private const int EofSymbol = 0x100;
        private int extraBits;
        private int lastLiteral;
        private byte[] literalBuf;
        private const int LiteralNumber = 0x11e;
        private Tree literalTree;
        private PendingBuffer pending;
        private const int Repeat11to38 = 0x12;
        private const int Repeat3to10 = 0x11;
        private const int Repeat3to6 = 0x10;
        private static short[] staticDCodes;
        private static byte[] staticDLength;
        private static short[] staticLCodes = new short[0x11e];
        private static byte[] staticLLength = new byte[0x11e];

        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static DeflaterHuffman()
        {
            int i = 0;
            while (i < 0x90)
            {
                staticLCodes[i] = BitReverse((0x30 + i) << 8);
                staticLLength[i++] = 8;
            }
            while (i < 0x100)
            {
                staticLCodes[i] = BitReverse((0x100 + i) << 7);
                staticLLength[i++] = 9;
            }
            while (i < 280)
            {
                staticLCodes[i] = BitReverse((-256 + i) << 9);
                staticLLength[i++] = 7;
            }
            while (i < 0x11e)
            {
                staticLCodes[i] = BitReverse((-88 + i) << 8);
                staticLLength[i++] = 8;
            }
            staticDCodes = new short[30];
            staticDLength = new byte[30];
            for (i = 0; i < 30; i++)
            {
                staticDCodes[i] = BitReverse(i << 11);
                staticDLength[i] = 5;
            }
        }

        public DeflaterHuffman(PendingBuffer pending)
        {
            this.pending = pending;
            this.literalTree = new Tree(this, 0x11e, 0x101, 15);
            this.distTree = new Tree(this, 30, 1, 15);
            this.bitLengthTree = new Tree(this, 0x13, 4, 7);
            this.distanceBuf = new short[0x4000];
            this.literalBuf = new byte[0x4000];
        }

        public static short BitReverse(int valueToReverse)
        {
            return (short) ((((bit4Reverse[valueToReverse & 15] << 12) | (bit4Reverse[(valueToReverse >> 4) & 15] << 8)) | (bit4Reverse[(valueToReverse >> 8) & 15] << 4)) | bit4Reverse[valueToReverse >> 12]);
        }

        public void CompressBlock()
        {
            for (int i = 0; i < this.lastLiteral; i++)
            {
                int litlen = this.literalBuf[i] & 0xff;
                int dist = this.distanceBuf[i];
                if (dist-- != 0)
                {
                    int lc = Lcode(litlen);
                    this.literalTree.WriteSymbol(lc);
                    int bits = (lc - 0x105) / 4;
                    if ((bits > 0) && (bits <= 5))
                    {
                        this.pending.WriteBits(litlen & ((((int) 1) << bits) - 1), bits);
                    }
                    int dc = Dcode(dist);
                    this.distTree.WriteSymbol(dc);
                    bits = (dc / 2) - 1;
                    if (bits > 0)
                    {
                        this.pending.WriteBits(dist & ((((int) 1) << bits) - 1), bits);
                    }
                }
                else
                {
                    this.literalTree.WriteSymbol(litlen);
                }
            }
            this.literalTree.WriteSymbol(0x100);
        }

        private static int Dcode(int distance)
        {
            int code = 0;
            while (distance >= 4)
            {
                code += 2;
                distance = distance >> 1;
            }
            return (code + distance);
        }

        [SuppressMessage("Microsoft.Usage", "CA2233:OperationsShouldNotOverflow", MessageId="storedLength+4")]
        public void FlushBlock(byte[] stored, int storedOffset, int storedLength, bool lastBlock)
        {
            this.literalTree.Freqs[0x100] = (short) (this.literalTree.Freqs[0x100] + 1);
            this.literalTree.BuildTree();
            this.distTree.BuildTree();
            this.literalTree.CalcBLFreq(this.bitLengthTree);
            this.distTree.CalcBLFreq(this.bitLengthTree);
            this.bitLengthTree.BuildTree();
            int bitLengthTreeCodes = 4;
            for (int i = 0x12; i > bitLengthTreeCodes; i--)
            {
                if (this.bitLengthTree.Length[BitLengthOrder[i]] > 0)
                {
                    bitLengthTreeCodes = i + 1;
                }
            }
            int opt_len = ((((14 + (bitLengthTreeCodes * 3)) + this.bitLengthTree.GetEncodedLength()) + this.literalTree.GetEncodedLength()) + this.distTree.GetEncodedLength()) + this.extraBits;
            int static_len = this.extraBits;
            for (int i = 0; i < 0x11e; i++)
            {
                static_len += this.literalTree.Freqs[i] * staticLLength[i];
            }
            for (int i = 0; i < 30; i++)
            {
                static_len += this.distTree.Freqs[i] * staticDLength[i];
            }
            if (opt_len >= static_len)
            {
                opt_len = static_len;
            }
            if ((storedOffset >= 0) && ((storedLength + 4) < (opt_len >> 3)))
            {
                this.FlushStoredBlock(stored, storedOffset, storedLength, lastBlock);
            }
            else if (opt_len == static_len)
            {
                this.pending.WriteBits(2 + (lastBlock ? 1 : 0), 3);
                this.literalTree.SetStaticCodes(staticLCodes, staticLLength);
                this.distTree.SetStaticCodes(staticDCodes, staticDLength);
                this.CompressBlock();
                this.Reset();
            }
            else
            {
                this.pending.WriteBits(4 + (lastBlock ? 1 : 0), 3);
                this.SendAllTrees(bitLengthTreeCodes);
                this.CompressBlock();
                this.Reset();
            }
        }

        public void FlushStoredBlock(byte[] stored, int storedOffset, int storedLength, bool lastBlock)
        {
            this.pending.WriteBits(lastBlock ? 1 : 0, 3);
            this.pending.AlignToByte();
            this.pending.WriteShort(storedLength);
            this.pending.WriteShort(~storedLength);
            this.pending.WriteBlock(stored, storedOffset, storedLength);
            this.Reset();
        }

        public bool IsFull()
        {
            return (this.lastLiteral >= 0x4000);
        }

        private static int Lcode(int length)
        {
            if (length == 0xff)
            {
                return 0x11d;
            }
            int code = 0x101;
            while (length >= 8)
            {
                code += 4;
                length = length >> 1;
            }
            return (code + length);
        }

        public void Reset()
        {
            this.lastLiteral = 0;
            this.extraBits = 0;
            this.literalTree.Reset();
            this.distTree.Reset();
            this.bitLengthTree.Reset();
        }

        public void SendAllTrees(int bitLengthTreeCodes)
        {
            this.bitLengthTree.BuildCodes();
            this.literalTree.BuildCodes();
            this.distTree.BuildCodes();
            this.pending.WriteBits(this.literalTree.NumCodes - 0x101, 5);
            this.pending.WriteBits(this.distTree.NumCodes - 1, 5);
            this.pending.WriteBits(bitLengthTreeCodes - 4, 4);
            for (int rank = 0; rank < bitLengthTreeCodes; rank++)
            {
                this.pending.WriteBits(this.bitLengthTree.Length[BitLengthOrder[rank]], 3);
            }
            this.literalTree.WriteTree(this.bitLengthTree);
            this.distTree.WriteTree(this.bitLengthTree);
        }

        [SuppressMessage("Microsoft.Usage", "CA2233:OperationsShouldNotOverflow", MessageId="length-3"), SuppressMessage("Microsoft.Usage", "CA2233:OperationsShouldNotOverflow", MessageId="distance-1")]
        public bool TallyDist(int distance, int length)
        {
            this.distanceBuf[this.lastLiteral] = (short) distance;
            this.literalBuf[this.lastLiteral++] = (byte) (length - 3);
            int lc = Lcode(length - 3);
            this.literalTree.Freqs[lc] = (short) (this.literalTree.Freqs[lc] + 1);
            if ((lc >= 0x109) && (lc < 0x11d))
            {
                this.extraBits += (lc - 0x105) / 4;
            }
            int dc = Dcode(distance - 1);
            this.distTree.Freqs[dc] = (short) (this.distTree.Freqs[dc] + 1);
            if (dc >= 4)
            {
                this.extraBits += (dc / 2) - 1;
            }
            return this.IsFull();
        }

        public bool TallyLit(int literal)
        {
            this.distanceBuf[this.lastLiteral] = 0;
            this.literalBuf[this.lastLiteral++] = (byte) literal;
            this.literalTree.Freqs[literal] = (short) (this.literalTree.Freqs[literal] + 1);
            return this.IsFull();
        }

        public PendingBuffer Pending
        {
            get
            {
                return this.pending;
            }
        }

        private class Tree
        {
            private int[] bitLengthCounts;
            private short[] codes;
            private DeflaterHuffman dh;
            private short[] freqs;
            private byte[] length;
            private int maxLength;
            private int minNumCodes;
            private int numCodes;

            public Tree(DeflaterHuffman dh, int elems, int minCodes, int maxLength)
            {
                this.dh = dh;
                this.minNumCodes = minCodes;
                this.maxLength = maxLength;
                this.freqs = new short[elems];
                this.bitLengthCounts = new int[maxLength];
            }

            public void BuildCodes()
            {
                int[] nextCode = new int[this.maxLength];
                int code = 0;
                this.codes = new short[this.freqs.Length];
                for (int bits = 0; bits < this.maxLength; bits++)
                {
                    nextCode[bits] = code;
                    code += this.bitLengthCounts[bits] << (15 - bits);
                }
                for (int i = 0; i < this.numCodes; i++)
                {
                    int bits = this.length[i];
                    if (bits > 0)
                    {
                        this.codes[i] = DeflaterHuffman.BitReverse(nextCode[bits - 1]);
                        nextCode[bits - 1] += ((int) 1) << (0x10 - bits);
                    }
                }
            }

            private void BuildLength(int[] childs)
            {
                this.length = new byte[this.freqs.Length];
                int numNodes = childs.Length / 2;
                int numLeafs = (numNodes + 1) / 2;
                int overflow = 0;
                for (int i = 0; i < this.maxLength; i++)
                {
                    this.bitLengthCounts[i] = 0;
                }
                int[] lengths = new int[numNodes];
                lengths[numNodes - 1] = 0;
                for (int i = numNodes - 1; i >= 0; i--)
                {
                    if (childs[(2 * i) + 1] != -1)
                    {
                        int bitLength = lengths[i] + 1;
                        if (bitLength > this.maxLength)
                        {
                            bitLength = this.maxLength;
                            overflow++;
                        }
                        lengths[childs[2 * i]] = lengths[childs[(2 * i) + 1]] = bitLength;
                    }
                    else
                    {
                        int bitLength = lengths[i];
                        this.bitLengthCounts[bitLength - 1]++;
                        this.length[childs[2 * i]] = (byte) lengths[i];
                    }
                }
                if (overflow != 0)
                {
                    int incrBitLen = this.maxLength - 1;
                    do
                    {
                        while (this.bitLengthCounts[--incrBitLen] == 0)
                        {
                        }
                        do
                        {
                            this.bitLengthCounts[incrBitLen]--;
                            this.bitLengthCounts[++incrBitLen]++;
                            overflow -= ((int) 1) << ((this.maxLength - 1) - incrBitLen);
                        }
                        while ((overflow > 0) && (incrBitLen < (this.maxLength - 1)));
                    }
                    while (overflow > 0);
                    this.bitLengthCounts[this.maxLength - 1] += overflow;
                    this.bitLengthCounts[this.maxLength - 2] -= overflow;
                    int nodePtr = 2 * numLeafs;
                    for (int bits = this.maxLength; bits != 0; bits--)
                    {
                        int n = this.bitLengthCounts[bits - 1];
                        while (n > 0)
                        {
                            int childPtr = 2 * childs[nodePtr++];
                            if (childs[childPtr + 1] == -1)
                            {
                                this.length[childs[childPtr]] = (byte) bits;
                                n--;
                            }
                        }
                    }
                }
            }

            public void BuildTree()
            {
                int numSymbols = this.freqs.Length;
                int[] heap = new int[numSymbols];
                int heapLen = 0;
                int maxCode = 0;
                for (int n = 0; n < numSymbols; n++)
                {
                    int freq = this.freqs[n];
                    if (freq != 0)
                    {
                        int ppos;
                        int pos = heapLen++;
                        while ((pos > 0) && (this.freqs[heap[ppos = (pos - 1) / 2]] > freq))
                        {
                            heap[pos] = heap[ppos];
                            pos = ppos;
                        }
                        heap[pos] = n;
                        maxCode = n;
                    }
                }
                while (heapLen < 2)
                {
                    int node = (maxCode < 2) ? ++maxCode : 0;
                    heap[heapLen++] = node;
                }
                this.numCodes = Math.Max(maxCode + 1, this.minNumCodes);
                int numLeafs = heapLen;
                int[] childs = new int[(4 * heapLen) - 2];
                int[] values = new int[(2 * heapLen) - 1];
                int numNodes = numLeafs;
                for (int i = 0; i < heapLen; i++)
                {
                    int node = heap[i];
                    childs[2 * i] = node;
                    childs[(2 * i) + 1] = -1;
                    values[i] = this.freqs[node] << 8;
                    heap[i] = i;
                }
                do
                {
                    int first = heap[0];
                    int last = heap[--heapLen];
                    int ppos = 0;
                    int path = 1;
                    while (path < heapLen)
                    {
                        if (((path + 1) < heapLen) && (values[heap[path]] > values[heap[path + 1]]))
                        {
                            path++;
                        }
                        heap[ppos] = heap[path];
                        ppos = path;
                        path = (path * 2) + 1;
                    }
                    int lastVal = values[last];
                    while (((path = ppos) > 0) && (values[heap[ppos = (path - 1) / 2]] > lastVal))
                    {
                        heap[path] = heap[ppos];
                    }
                    heap[path] = last;
                    int second = heap[0];
                    last = numNodes++;
                    childs[2 * last] = first;
                    childs[(2 * last) + 1] = second;
                    int mindepth = Math.Min((int) (values[first] & 0xff), (int) (values[second] & 0xff));
                    values[last] = lastVal = ((values[first] + values[second]) - mindepth) + 1;
                    ppos = 0;
                    path = 1;
                    while (path < heapLen)
                    {
                        if (((path + 1) < heapLen) && (values[heap[path]] > values[heap[path + 1]]))
                        {
                            path++;
                        }
                        heap[ppos] = heap[path];
                        ppos = path;
                        path = (ppos * 2) + 1;
                    }
                    while (((path = ppos) > 0) && (values[heap[ppos = (path - 1) / 2]] > lastVal))
                    {
                        heap[path] = heap[ppos];
                    }
                    heap[path] = last;
                }
                while (heapLen > 1);
                if (heap[0] != ((childs.Length / 2) - 1))
                {
                    throw new InvalidOperationException("Heap invariant violated");
                }
                this.BuildLength(childs);
            }

            public void CalcBLFreq(DeflaterHuffman.Tree bitLengthTree)
            {
                int curlen = -1;
                int i = 0;
                while (i < this.numCodes)
                {
                    int max_count;
                    int min_count;
                    int count = 1;
                    int nextlen = this.length[i];
                    if (nextlen == 0)
                    {
                        max_count = 0x8a;
                        min_count = 3;
                    }
                    else
                    {
                        max_count = 6;
                        min_count = 3;
                        if (curlen != nextlen)
                        {
                            bitLengthTree.freqs[nextlen] = (short) (bitLengthTree.freqs[nextlen] + 1);
                            count = 0;
                        }
                    }
                    curlen = nextlen;
                    i++;
                    while ((i < this.numCodes) && (curlen == this.length[i]))
                    {
                        i++;
                        if (++count >= max_count)
                        {
                            break;
                        }
                    }
                    if (count < min_count)
                    {
                        bitLengthTree.freqs[curlen] = (short) (bitLengthTree.freqs[curlen] + ((short) count));
                    }
                    else
                    {
                        if (curlen != 0)
                        {
                            bitLengthTree.freqs[0x10] = (short) (bitLengthTree.freqs[0x10] + 1);
                            continue;
                        }
                        if (count <= 10)
                        {
                            bitLengthTree.freqs[0x11] = (short) (bitLengthTree.freqs[0x11] + 1);
                            continue;
                        }
                        bitLengthTree.freqs[0x12] = (short) (bitLengthTree.freqs[0x12] + 1);
                    }
                }
            }

            public int GetEncodedLength()
            {
                int len = 0;
                for (int i = 0; i < this.freqs.Length; i++)
                {
                    len += this.freqs[i] * this.length[i];
                }
                return len;
            }

            public void Reset()
            {
                for (int i = 0; i < this.freqs.Length; i++)
                {
                    this.freqs[i] = 0;
                }
                this.codes = null;
                this.length = null;
            }

            public void SetStaticCodes(short[] staticCodes, byte[] staticLengths)
            {
                this.codes = staticCodes;
                this.length = staticLengths;
            }

            public void WriteSymbol(int code)
            {
                this.dh.Pending.WriteBits(this.codes[code] & 0xffff, this.length[code]);
            }

            public void WriteTree(DeflaterHuffman.Tree bitLengthTree)
            {
                int curlen = -1;
                int i = 0;
                while (i < this.numCodes)
                {
                    int max_count;
                    int min_count;
                    int count = 1;
                    int nextlen = this.length[i];
                    if (nextlen == 0)
                    {
                        max_count = 0x8a;
                        min_count = 3;
                    }
                    else
                    {
                        max_count = 6;
                        min_count = 3;
                        if (curlen != nextlen)
                        {
                            bitLengthTree.WriteSymbol(nextlen);
                            count = 0;
                        }
                    }
                    curlen = nextlen;
                    i++;
                    while ((i < this.numCodes) && (curlen == this.length[i]))
                    {
                        i++;
                        if (++count >= max_count)
                        {
                            break;
                        }
                    }
                    if (count < min_count)
                    {
                        while (count-- > 0)
                        {
                            bitLengthTree.WriteSymbol(curlen);
                        }
                    }
                    else if (curlen != 0)
                    {
                        bitLengthTree.WriteSymbol(0x10);
                        this.dh.Pending.WriteBits(count - 3, 2);
                    }
                    else
                    {
                        if (count <= 10)
                        {
                            bitLengthTree.WriteSymbol(0x11);
                            this.dh.Pending.WriteBits(count - 3, 3);
                            continue;
                        }
                        bitLengthTree.WriteSymbol(0x12);
                        this.dh.Pending.WriteBits(count - 11, 7);
                    }
                }
            }

            public short[] Freqs
            {
                get
                {
                    return this.freqs;
                }
            }

            public byte[] Length
            {
                get
                {
                    return this.length;
                }
            }

            public int NumCodes
            {
                get
                {
                    return this.numCodes;
                }
            }
        }
    }
}

