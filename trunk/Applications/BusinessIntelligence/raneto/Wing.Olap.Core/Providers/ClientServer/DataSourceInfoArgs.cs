/*   
    Copyright (C) 2009 Galaktika Corporation ZAO

    This file is a part of Ranet.UILibrary.Olap
 
    Ranet.UILibrary.Olap is a free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
      
    You should have received a copy of the GNU General Public License
    along with Ranet.UILibrary.Olap.  If not, see <http://www.gnu.org/licenses/>.
  
    If GPL v.3 is not suitable for your products or company,
    Galaktika Corp provides Ranet.UILibrary.Olap under a flexible commercial license
    designed to meet your specific usage and distribution requirements.
    If you have already obtained a commercial license from Galaktika Corp,
    you can use this file under those license terms.
*/

using System;
using System.Net;

namespace Ranet.Olap.Core.Providers.ClientServer
{
    public class DataSourceInfoArgs
    {
        public String ConnectionString = String.Empty;

        public String MDXQuery = String.Empty;
        public String Parsed_MDXQuery = String.Empty;

        public String UpdateScript = String.Empty;
        public String Parsed_UpdateScript = String.Empty;

        public String MovedAxes_MDXQuery = String.Empty;
        public String Parsed_MovedAxes_MDXQuery = String.Empty;

        public DataSourceInfoArgs()
        { 
        }
    }
}
