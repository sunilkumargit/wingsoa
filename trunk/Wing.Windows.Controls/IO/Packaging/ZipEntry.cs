namespace Telerik.IO.Packaging
{
    using System;

    internal class ZipEntry
    {
        private ulong compressedSize;
        private uint crc;
        private uint dosTime;
        private int flags;
        private Known known;
        private Telerik.IO.Packaging.CompressionMethod method;
        private string name;
        private long offset;
        private ulong size;
        private ushort versionToExtract;

        public ZipEntry(string name) : this(name, 0, Telerik.IO.Packaging.CompressionMethod.Deflated)
        {
        }

        internal ZipEntry(string name, int versionRequiredToExtract, Telerik.IO.Packaging.CompressionMethod method)
        {
            this.method = Telerik.IO.Packaging.CompressionMethod.Deflated;
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (name.Length > 0xffff)
            {
                throw new ArgumentException("Name is too long", "name");
            }
            if ((versionRequiredToExtract != 0) && (versionRequiredToExtract < 10))
            {
                throw new ArgumentOutOfRangeException("versionRequiredToExtract");
            }
            this.DateTime = System.DateTime.Now;
            this.name = name;
            this.versionToExtract = (ushort) versionRequiredToExtract;
            this.method = method;
        }

        internal static bool IsCompressionMethodSupported(Telerik.IO.Packaging.CompressionMethod method)
        {
            if (method != Telerik.IO.Packaging.CompressionMethod.Deflated)
            {
                return (method == Telerik.IO.Packaging.CompressionMethod.Stored);
            }
            return true;
        }

        public override string ToString()
        {
            return this.name;
        }

        public long CompressedSize
        {
            get
            {
                if (((byte) (this.known & Known.CompressedSize)) == 0)
                {
                    return -1L;
                }
                return (long) this.compressedSize;
            }
            set
            {
                this.compressedSize = (ulong) value;
                this.known = (Known) ((byte) (this.known | Known.CompressedSize));
            }
        }

        internal Telerik.IO.Packaging.CompressionMethod CompressionMethod
        {
            get
            {
                return this.method;
            }
            set
            {
                if (!IsCompressionMethodSupported(value))
                {
                    throw new NotSupportedException("Compression method not supported");
                }
                this.method = value;
            }
        }

        public long Crc
        {
            get
            {
                if (((byte) (this.known & Known.Crc)) == 0)
                {
                    return -1L;
                }
                return (long) (this.crc & 0xffffffffL);
            }
            set
            {
                if ((this.crc & 18446744069414584320L) != 0L)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                this.crc = (uint) value;
                this.known = (Known) ((byte) (this.known | Known.Crc));
            }
        }

        public System.DateTime DateTime
        {
            get
            {
                uint sec = Math.Min((uint) 0x3b, (uint) (2 * (this.dosTime & 0x1f)));
                uint min = Math.Min((uint) 0x3b, (uint) ((this.dosTime >> 5) & 0x3f));
                uint hrs = Math.Min((uint) 0x17, (uint) ((this.dosTime >> 11) & 0x1f));
                uint mon = Math.Max(1, Math.Min((uint) 12, (uint) ((this.dosTime >> 0x15) & 15)));
                uint year = ((this.dosTime >> 0x19) & 0x7f) + 0x7bc;
                return new System.DateTime((int) year, (int) mon, Math.Max(1, Math.Min(System.DateTime.DaysInMonth((int) year, (int) mon), ((int) (this.dosTime >> 0x10)) & 0x1f)), (int) hrs, (int) min, (int) sec);
            }
            set
            {
                uint year = (uint) value.Year;
                uint month = (uint) value.Month;
                uint day = (uint) value.Day;
                uint hour = (uint) value.Hour;
                uint minute = (uint) value.Minute;
                uint second = (uint) value.Second;
                if (year < 0x7bc)
                {
                    year = 0x7bc;
                    month = 1;
                    day = 1;
                    hour = 0;
                    minute = 0;
                    second = 0;
                }
                else if (year > 0x83b)
                {
                    year = 0x83b;
                    month = 12;
                    day = 0x1f;
                    hour = 0x17;
                    minute = 0x3b;
                    second = 0x3b;
                }
                this.DosTime = (long) ((ulong) ((((((((year - 0x7bc) & 0x7f) << 0x19) | (month << 0x15)) | (day << 0x10)) | (hour << 11)) | (minute << 5)) | (second >> 1)));
            }
        }

        public long DosTime
        {
            get
            {
                if (((byte) (this.known & Known.Time)) == 0)
                {
                    return 0L;
                }
                return (long) this.dosTime;
            }
            set
            {
                this.dosTime = (uint) value;
                this.known = (Known) ((byte) (this.known | Known.Time));
            }
        }

        public int Flags
        {
            get
            {
                return this.flags;
            }
            set
            {
                this.flags = value;
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }
        }

        public long Offset
        {
            get
            {
                return this.offset;
            }
            set
            {
                this.offset = value;
            }
        }

        public long Size
        {
            get
            {
                if (((byte) (this.known & (Known.None | Known.Size))) == 0)
                {
                    return -1L;
                }
                return (long) this.size;
            }
            set
            {
                this.size = (ulong) value;
                this.known = (Known) ((byte) (this.known | Known.None | Known.Size));
            }
        }

        public int Version
        {
            get
            {
                if (this.versionToExtract != 0)
                {
                    return this.versionToExtract;
                }
                int result = 10;
                if (Telerik.IO.Packaging.CompressionMethod.Deflated == this.method)
                {
                    result = 20;
                }
                return result;
            }
        }

        [Flags]
        private enum Known : byte
        {
            CompressedSize = 2,
            Crc = 4,
            ExternalAttributes = 0x10,
            None = 0,
            Size = 1,
            Time = 8
        }
    }
}

