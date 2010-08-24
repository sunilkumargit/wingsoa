/*   
    Copyright (C) 2009 Galaktika Corporation ZAO

    This file is a part of UILibrary.OLAP
 
    UILibrary.OLAP is a free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
      
    You should have received a copy of the GNU General Public License
    along with UILibrary.OLAP.  If not, see <http://www.gnu.org/licenses/>.
  
    If GPL v.3 is not suitable for your products or company,
    Galaktika Corp provides UILibrary.OLAP under a flexible commercial license
    designed to meet your specific usage and distribution requirements.
    If you have already obtained a commercial license from Galaktika Corp,
    you can use this file under those license terms.
*/

using System;
using System.IO;
using System.Text;

namespace Wing.Olap.Mdx.Compiler
{
    using Parser;

    internal class DefaultMdxParser : IMdxParser
    {
        public System.IO.TextWriter Error = Console.Error;

        public MdxObject Parse(string mdx)
        {
            try
            {
                var ms = new MemoryStream(Encoding.Unicode.GetBytes(mdx));
                ms.Position = 0;

                var lexer = new mdxLexer
                    (new Antlr.Runtime.ANTLRInputStream
                        (ms
                        , Encoding.Unicode
                        )
                    );
                lexer.Error = this.Error;
                var parser = new mdxParser
                (
                    new Antlr.Runtime.CommonTokenStream(lexer)
                );
                parser.Error = this.Error;
                var result = parser.mdx_statement();
                return result;
            }
            catch (Exception e)
            {
                Error.WriteLine(e.ToString());
                throw new Exception("При разборе запроса были ошибки. Смотрите протокол.", e);
            }
        }

        #region IMdxParser Members

        public MdxObject Parse(TextReader reader)
        {
            return this.Parse(reader.ReadToEnd());
        }

        #endregion
    }


    public class ParsingErrorException : Exception
    {
        public ParsingErrorException(int index, int length)
        {
            this.Index = index;
            this.Length = length;
        }

        public readonly int Index;
        public readonly int Length;
    }
}
