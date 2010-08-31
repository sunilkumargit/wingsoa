/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;

using System.IO;
using System.Text;

namespace Wing.Olap.Mdx.Compiler
{
    public abstract class MdxDomProvider : IDisposable
    {
        protected MdxDomProvider() { }
        public readonly StringBuilder Errors = new StringBuilder();

        public static MdxDomProvider CreateProvider()
        {
            // TODO: ability to replace default provider
            return new DefaultMdxDomProvider();
        }

        public void GenerateMdxFromDom(MdxObject mdx, TextWriter writer, MdxGeneratorOptions options)
        {
            this.CreateMdxGenerator().GenerateMdxFromDom(mdx, writer, options);
        }

        public void GenerateMdxFromDom(MdxObject mdx, Stream outputStream, MdxGeneratorOptions options)
        {
            using (StreamWriter sw = new StreamWriter(outputStream))
            {
                this.CreateMdxGenerator().GenerateMdxFromDom(mdx, sw, options);
                sw.Flush();
            }
        }

        public void GenerateMdxFromDom(MdxObject mdx, string fileName, MdxGeneratorOptions options)
        {
            using (StreamWriter sw = new StreamWriter(fileName))
            {
                this.CreateMdxGenerator().GenerateMdxFromDom(mdx, sw, options);
                sw.Flush();
            }
        }

        public void GenerateMdxFromDom(MdxObject mdx, StringBuilder builder, MdxGeneratorOptions options)
        {
            using (StringWriter sw = new StringWriter(builder))
            {
                try
                {
                    this.CreateMdxGenerator().GenerateMdxFromDom(mdx, sw, options);
                }
                finally
                {
                    sw.Flush();
                }
            }
        }

        public MdxObject ParseMdx(TextReader tr)
        {
            return this.CreateMdxParser().Parse(tr);
        }

        public MdxObject ParseMdx(Stream inputStream)
        {
            using (StreamReader sr = new StreamReader(inputStream))
            {
                return this.CreateMdxParser().Parse(sr);
            }
        }

        public MdxObject ParseMdx(string mdx)
        {
            using (StringReader sr = new StringReader(mdx))
            {
                return this.CreateMdxParser().Parse(sr);
            }
        }
        protected virtual IMdxGenerator CreateMdxGenerator()
        {
            return new DefaultMdxGenerator();
        }

        protected virtual IMdxParser CreateMdxParser()
        {
            var p = new DefaultMdxParser();
            var Error = new StringWriter(Errors);
            p.Error = Error;
            return p;
        }

        #region IDisposable Members

        public virtual void Dispose()
        {
        }

        #endregion
    }
}
