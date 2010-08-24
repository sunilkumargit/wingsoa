/*   
    Copyright (C) 2009 Galaktika Corporation ZAO

    This file is a part of Galaktika.BusinessMonitor
 
    Galaktika.BusinessMonitor is a free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
      
    You should have received a copy of the GNU General Public License
    along with Galaktika.BusinessMonitor.  If not, see <http://www.gnu.org/licenses/>.
  
    If GPL v.3 is not suitable for your products or company,
    Galaktika Corp provides Galaktika.BusinessMonitor under a flexible commercial license
    designed to meet your specific usage and distribution requirements.
    If you have already obtained a commercial license from Galaktika Corp,
    you can use this file under those license terms.
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Data;
using System.Linq;
using System.Web;

namespace Ranet.Olap.Core.Providers
{
    using Ranet.Olap.Core.Data;

    public static class DataTableHelper
    {
        public static DataTableWrapper Create(DataTable table)
        {
            DataTableWrapper res = new DataTableWrapper();
            foreach (DataColumn col in table.Columns)
            {
                res.Columns.Add(new DataTableColumnDefinition(col.ColumnName, col.Caption, col.DataType));
            }

            foreach (DataRow row in table.Rows)
            {
                int i = 0;
                DataTableRowDefinition tableRow = new DataTableRowDefinition();
                foreach (object obj in row.ItemArray)
                {
                    tableRow.Cells.Add(new DataTableCellDefinition(res.Columns[i].Name, obj, res.Columns[i].Type));
                    if (obj == null)
                    {
                        res.Items.Add(null);
                    }
                    else
                    {
                        res.Items.Add(Convert.ToString(obj, CultureInfo.InvariantCulture));
                    }
                    i++;
                }
                res.Rows.Add(tableRow);
            }
            return res;
        }
    }
}
