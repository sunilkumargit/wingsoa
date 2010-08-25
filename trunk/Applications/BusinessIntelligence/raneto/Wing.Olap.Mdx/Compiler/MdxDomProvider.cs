/*   
    Copyright (C) 2009 Galaktika Corporation ZAO

    This file is a part of Wing.UILibrary.Olap
 
    Wing.UILibrary.Olap is a free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
      
    You should have received a copy of the GNU General Public License
    along with Wing.UILibrary.Olap.  If not, see <http://www.gnu.org/licenses/>.
  
    If GPL v.3 is not suitable for your products or company,
    Galaktika Corp provides Wing.UILibrary.Olap under a flexible commercial license
    designed to meet your specific usage and distribution requirements.
    If you have already obtained a commercial license from Galaktika Corp,
    you can use this file under those license terms.
*/

using System;
using System.Collections.Generic;

using System.IO;
using System.Security.Permissions;
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
