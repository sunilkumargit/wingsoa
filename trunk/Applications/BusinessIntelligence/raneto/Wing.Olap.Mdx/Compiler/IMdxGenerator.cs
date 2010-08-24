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

namespace Wing.Olap.Mdx.Compiler
{
    public interface IMdxGenerator
    {
        void GenerateMdxFromDom(MdxObject mdx, TextWriter writer, MdxGeneratorOptions options);
    }
}
