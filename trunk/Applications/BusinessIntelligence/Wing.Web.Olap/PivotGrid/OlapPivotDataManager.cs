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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ranet.Olap.Core.Data;
using Ranet.Olap.Mdx.Compiler;
using Ranet.Olap.Mdx;
using Ranet.Olap.Core;
using Ranet.Olap.Core.Providers;

namespace Ranet.Web.Olap.PivotGrid
{
    public class OlapPivotDataManager : PivotDataManagerBase
    {
        public OlapPivotDataManager(ConnectionInfo connection, String query, String updateScript)
            : base(connection, query, updateScript)
        { 
        
        }

        protected override IQueryExecuter CreateQueryExecutor(ConnectionInfo connection)
        {
            return new DefaultQueryExecuter(connection);
        }

        public override String ExportToExcel()
        {
            String result = String.Empty;
            try
            {
                CellSetData res = RefreshQuery();
                PivotDataProvider pivotProvider = new PivotDataProvider(new CellSetDataProvider(res));
                result = ExportHelper.ExportToExcel(pivotProvider);
            }
            catch
            {
                result = string.Empty;
            }
            return result;
        }
    }
}
