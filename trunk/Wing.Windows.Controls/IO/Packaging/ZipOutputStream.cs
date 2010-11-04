namespace Telerik.IO.Packaging
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    internal class ZipOutputStream : DeflaterOutputStream
    {
        private Crc32 crc;
        private long crcPatchPos;
        private ZipEntry curEntry;
        private CompressionMethod curMethod;
        private int defaultCompressionLevel;
        private List<ZipEntry> entries;
        private long offset;
        private bool patchEntryHeader;
        private long size;

        public ZipOutputStream(Stream baseOutputStream) : base(baseOutputStream, new Deflater(-1, true))
        {
            this.entries = new List<ZipEntry>();
            this.crc = new Crc32();
            this.defaultCompressionLevel = -1;
            this.curMethod = CompressionMethod.Deflated;
            this.crcPatchPos = -1L;
        }

        public void CloseEntry()
        {
            if (this.curEntry == null)
            {
                throw new InvalidOperationException("No open entry");
            }
            if (this.curMethod == CompressionMethod.Deflated)
            {
                base.Finish();
            }
            long csize = (this.curMethod == CompressionMethod.Deflated) ? base.Deflater.TotalOut : this.size;
            if (this.curEntry.Size < 0L)
            {
                this.curEntry.Size = this.size;
            }
            else if (this.curEntry.Size != this.size)
            {
                throw new InvalidOperationException(string.Concat(new object[] { "size was ", this.size, ", but I expected ", this.curEntry.Size }));
            }
            if (this.curEntry.CompressedSize < 0L)
            {
                this.curEntry.CompressedSize = csize;
            }
            else if (this.curEntry.CompressedSize != csize)
            {
                throw new InvalidOperationException(string.Concat(new object[] { "compressed size was ", csize, ", but I expected ", this.curEntry.CompressedSize }));
            }
            if (this.curEntry.Crc < 0L)
            {
                this.curEntry.Crc = this.crc.Value;
            }
            else if (this.curEntry.Crc != this.crc.Value)
            {
                throw new InvalidOperationException(string.Concat(new object[] { "crc was ", this.crc.Value, ", but I expected ", this.curEntry.Crc }));
            }
            this.offset += csize;
            if (this.patchEntryHeader)
            {
                this.patchEntryHeader = false;
                long curPos = base.BaseOutputStream.Position;
                base.BaseOutputStream.Seek(this.crcPatchPos, SeekOrigin.Begin);
                this.WriteLeInt((int) this.curEntry.Crc);
                this.WriteLeInt((int) this.curEntry.CompressedSize);
                this.WriteLeInt((int) this.curEntry.Size);
                base.BaseOutputStream.Seek(curPos, SeekOrigin.Begin);
            }
            if ((this.curEntry.Flags & 8) != 0)
            {
                this.WriteLeInt(0x8074b50);
                this.WriteLeInt((int) this.curEntry.Crc);
                this.WriteLeInt((int) this.curEntry.CompressedSize);
                this.WriteLeInt((int) this.curEntry.Size);
                this.offset += 0x10L;
            }
            this.entries.Add(this.curEntry);
            this.curEntry = null;
        }

        public override void Finish()
        {
            if (this.entries != null)
            {
                if (this.curEntry != null)
                {
                    this.CloseEntry();
                }
                long numEntries = this.entries.Count;
                long sizeEntries = 0L;
                foreach (ZipEntry entry in this.entries)
                {
                    this.WriteLeInt(0x2014b50);
                    this.WriteLeShort(0x2d);
                    this.WriteLeShort(entry.Version);
                    this.WriteLeShort(entry.Flags);
                    this.WriteLeShort((short) entry.CompressionMethod);
                    this.WriteLeInt((int) entry.DosTime);
                    this.WriteLeInt((int) entry.Crc);
                    if (entry.CompressedSize >= 0xffffffffL)
                    {
                        this.WriteLeInt(-1);
                    }
                    else
                    {
                        this.WriteLeInt((int) entry.CompressedSize);
                    }
                    if (entry.Size >= 0xffffffffL)
                    {
                        this.WriteLeInt(-1);
                    }
                    else
                    {
                        this.WriteLeInt((int) entry.Size);
                    }
                    byte[] name = ZipConstants.ConvertToArray(entry.Flags, entry.Name);
                    if (name.Length > 0xffff)
                    {
                        throw new InvalidOperationException("Entry name too long.");
                    }
                    this.WriteLeShort(name.Length);
                    this.WriteLeShort(0);
                    this.WriteLeShort(0);
                    this.WriteLeShort(0);
                    this.WriteLeShort(0);
                    this.WriteLeInt(0);
                    if (entry.Offset >= 0xffffffffL)
                    {
                        this.WriteLeInt(-1);
                    }
                    else
                    {
                        this.WriteLeInt((int) entry.Offset);
                    }
                    if (name.Length > 0)
                    {
                        base.BaseOutputStream.Write(name, 0, name.Length);
                    }
                    sizeEntries += 0x2e + name.Length;
                }
                using (ZipHelperStream zipHelperStream = new ZipHelperStream(base.BaseOutputStream))
                {
                    zipHelperStream.WriteEndOfCentralDirectory(numEntries, sizeEntries, this.offset);
                }
                this.entries = null;
            }
        }

        public void PutNextEntry(ZipEntry entry)
        {
            if (entry == null)
            {
                throw new ArgumentNullException("entry");
            }
            if (this.entries == null)
            {
                throw new InvalidOperationException("ZipOutputStream was finished");
            }
            if (this.curEntry != null)
            {
                this.CloseEntry();
            }
            if (this.entries.Count == 0x7fffffff)
            {
                throw new InvalidOperationException("Too many entries for Zip file");
            }
            CompressionMethod method = entry.CompressionMethod;
            int compressionLevel = this.defaultCompressionLevel;
            entry.Flags &= 0x800;
            this.patchEntryHeader = false;
            bool headerInfoAvailable = true;
            if (method == CompressionMethod.Stored)
            {
                entry.Flags &= -9;
                if (entry.CompressedSize >= 0L)
                {
                    if (entry.Size >= 0L)
                    {
                        if (entry.Size != entry.CompressedSize)
                        {
                            throw new InvalidOperationException("Method STORED, but compressed size != size");
                        }
                    }
                    else
                    {
                        entry.Size = entry.CompressedSize;
                    }
                }
                else if (entry.Size >= 0L)
                {
                    entry.CompressedSize = entry.Size;
                }
                if ((entry.Size < 0L) || (entry.Crc < 0L))
                {
                    if (base.CanPatchEntries)
                    {
                        headerInfoAvailable = false;
                    }
                    else
                    {
                        method = CompressionMethod.Deflated;
                        compressionLevel = 0;
                    }
                }
            }
            else if (entry.Size == 0L)
            {
                entry.CompressedSize = entry.Size;
                entry.Crc = 0L;
                method = CompressionMethod.Stored;
            }
            else if (((entry.CompressedSize < 0L) || (entry.Size < 0L)) || (entry.Crc < 0L))
            {
                headerInfoAvailable = false;
            }
            if (!headerInfoAvailable)
            {
                if (!base.CanPatchEntries)
                {
                    entry.Flags |= 8;
                }
                else
                {
                    this.patchEntryHeader = true;
                }
            }
            entry.Offset = this.offset;
            entry.CompressionMethod = method;
            this.curMethod = method;
            this.WriteLeInt(0x4034b50);
            this.WriteLeShort(entry.Version);
            this.WriteLeShort(entry.Flags);
            this.WriteLeShort((byte) method);
            this.WriteLeInt((int) entry.DosTime);
            if (headerInfoAvailable)
            {
                this.WriteLeInt((int) entry.Crc);
                this.WriteLeInt((int) entry.CompressedSize);
                this.WriteLeInt((int) entry.Size);
            }
            else
            {
                if (this.patchEntryHeader)
                {
                    this.crcPatchPos = base.BaseOutputStream.Position;
                }
                this.WriteLeInt(0);
                this.WriteLeInt(0);
                this.WriteLeInt(0);
            }
            byte[] name = ZipConstants.ConvertToArray(entry.Flags, entry.Name);
            if (name.Length > 0xffff)
            {
                throw new InvalidOperationException("Entry name too long.");
            }
            this.WriteLeShort(name.Length);
            this.WriteLeShort(0);
            if (name.Length > 0)
            {
                base.BaseOutputStream.Write(name, 0, name.Length);
            }
            this.offset += 30 + name.Length;
            this.curEntry = entry;
            this.crc.Reset();
            if (method == CompressionMethod.Deflated)
            {
                base.Deflater.Reset();
                base.Deflater.SetLevel(compressionLevel);
            }
            this.size = 0L;
        }

        public override void Write(byte[] buffer, int bufferOffset, int count)
        {
            if (this.curEntry == null)
            {
                throw new InvalidOperationException("No open entry.");
            }
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }
            if (bufferOffset < 0)
            {
                throw new ArgumentOutOfRangeException("bufferOffset", "Cannot be negative");
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count", "Cannot be negative");
            }
            if ((buffer.Length - bufferOffset) < count)
            {
                throw new ArgumentException("Invalid offset/count combination");
            }
            this.crc.Update(buffer, bufferOffset, count);
            this.size += count;
            switch (this.curMethod)
            {
                case CompressionMethod.Stored:
                    base.BaseOutputStream.Write(buffer, bufferOffset, count);
                    break;

                case CompressionMethod.Deflated:
                    base.Write(buffer, bufferOffset, count);
                    break;
            }
        }

        private void WriteLeInt(int value)
        {
            this.WriteLeShort(value);
            this.WriteLeShort(value >> 0x10);
        }

        private void WriteLeShort(int value)
        {
            base.BaseOutputStream.WriteByte((byte) (value & 0xff));
            base.BaseOutputStream.WriteByte((byte) ((value >> 8) & 0xff));
        }
    }
}

