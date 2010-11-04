namespace Telerik.IO.Packaging
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;

    internal class PackagePart
    {
        private Package packagePartZip;

        [SuppressMessage("Microsoft.Globalization", "CA1307:SpecifyStringComparison", MessageId="System.String.StartsWith(System.String)")]
        public PackagePart(Package zip, string path)
        {
            this.packagePartZip = zip;
            if (path.StartsWith("/"))
            {
                path = path.Substring(1);
            }
            ZipEntry entry = new ZipEntry(path) {
                DateTime = DateTime.Now
            };
            this.packagePartZip.GetStream().PutNextEntry(entry);
        }

        public static PackagePart CreateContentTypesPart(Package zip)
        {
            return new PackagePart(zip, "[Content_Types].xml");
        }

        public Stream GetStream()
        {
            return this.packagePartZip.GetStream();
        }
    }
}

