namespace Telerik.IO.Packaging
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    internal class Package : IDisposable
    {
        private Dictionary<string, string> contentTypeDefaults;
        private Dictionary<string, string> contentTypeOverrides;
        private ZipOutputStream zipStream;

        protected Package(Stream stream)
        {
            this.zipStream = new ZipOutputStream(stream);
            this.contentTypeDefaults = new Dictionary<string, string>();
            this.contentTypeOverrides = new Dictionary<string, string>();
        }

        public void Close()
        {
            PackagePart.CreateContentTypesPart(this);
            this.WriteXMLString("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>\r\n");
            this.WriteXMLString("<Types xmlns=\"http://schemas.openxmlformats.org/package/2006/content-types\">");
            foreach (string key in this.contentTypeDefaults.Keys)
            {
                this.WriteXMLString(string.Format("<Default Extension=\"{0}\" ContentType=\"{1}\" />", new object[] { key, this.contentTypeDefaults[key] }));
            }
            foreach (string key in this.contentTypeOverrides.Keys)
            {
                this.WriteXMLString(string.Format("<Override PartName=\"{0}\" ContentType=\"{1}\" />", new object[] { key, this.contentTypeOverrides[key] }));
            }
            this.WriteXMLString("</Types>");
            this.zipStream.Finish();
            this.zipStream.Close();
            this.zipStream.Dispose();
        }

        public PackagePart CreatePart(Uri uri, string mimeType)
        {
            string path = uri.OriginalString;
            string extension = Path.GetExtension(path).Substring(1);
            if (this.contentTypeDefaults.ContainsKey(extension))
            {
                if (this.contentTypeDefaults[extension] != mimeType)
                {
                    this.contentTypeOverrides.Add(path, mimeType);
                }
            }
            else
            {
                this.contentTypeDefaults.Add(extension, mimeType);
            }
            return new PackagePart(this, path);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            if (this.zipStream != null)
            {
                this.zipStream.Finish();
                this.zipStream.Close();
                this.zipStream.Dispose();
            }
        }

        public void Flush()
        {
            this.zipStream.Flush();
        }

        internal ZipOutputStream GetStream()
        {
            return this.zipStream;
        }

        public static Package Open(Stream stream, FileMode fileMode)
        {
            if (fileMode != FileMode.Create)
            {
                throw new ArgumentOutOfRangeException("fileMode", "File mode must be FileMode.Create.");
            }
            return new Package(stream);
        }

        private void WriteXMLString(string partString)
        {
            this.zipStream.Write(Encoding.UTF8.GetBytes(partString), 0, Encoding.UTF8.GetByteCount(partString));
        }
    }
}

