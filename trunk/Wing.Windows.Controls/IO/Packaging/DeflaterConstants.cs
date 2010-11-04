namespace Telerik.IO.Packaging
{
    using System;

    internal class DeflaterConstants
    {
        public static readonly int[] CompressionFunction = new int[] { 0, 1, 1, 1, 1, 2, 2, 2, 2, 2 };
        public const int DefaultMemoryLevel = 8;
        public const int DeflateFastCompression = 1;
        public const int DeflateSlowCompression = 2;
        public const int DeflateStoredCompression = 0;
        public const int DynamicTrees = 2;
        public static readonly int[] GoodLength = new int[] { 0, 4, 4, 4, 4, 8, 8, 8, 0x20, 0x20 };
        public const int HashBits = 15;
        public const int HashMask = 0x7fff;
        public const int HashShift = 5;
        public const int HashSize = 0x8000;
        public static readonly int MaxBlockSize = Math.Min(0xffff, 0xfffb);
        public static readonly int[] MaxChain = new int[] { 0, 4, 8, 0x20, 0x10, 0x20, 0x80, 0x100, 0x400, 0x1000 };
        public const int MaxDist = 0x7efa;
        public static readonly int[] MaxLazy = new int[] { 0, 4, 5, 6, 4, 0x10, 0x10, 0x20, 0x80, 0x102 };
        public const int MaxMatch = 0x102;
        public const int MaxWBits = 15;
        public const int MinLookAhead = 0x106;
        public const int MinMatch = 3;
        public static readonly int[] NiceLength = new int[] { 0, 8, 0x10, 0x20, 0x10, 0x20, 0x80, 0x80, 0x102, 0x102 };
        public const int PendingBufferSize = 0x10000;
        public const int PresetDictionary = 0x20;
        public const int StaticTrees = 1;
        public const int StoredBlock = 0;
        public const int WMask = 0x7fff;
        public const int WSize = 0x8000;
    }
}

