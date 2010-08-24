/*   
    Copyright (C) 2009 Galaktika Corporation ZAO

    This file is a part of Ranet.UILibrary.Olap
 
    Ranet.UILibrary.Olap is a free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
      
    You should have received a copy of the GNU General Public License
    along with Ranet.UILibrary.Olap.  If not, see
  	<http://www.gnu.org/licenses/> 
  
    If GPL v.3 is not suitable for your products or company,
    Galaktika Corp provides Ranet.UILibrary.Olap under a flexible commercial license
    designed to meet your specific usage and distribution requirements.
    If you have already obtained a commercial license from Galaktika Corp,
    you can use this file under those license terms.
*/

using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using Ranet.Olap.Core.Providers;

namespace Ranet.AgOlap.Controls.PivotGrid.Controls
{
    internal class CellControlsCache
    {
        IDictionary<CellInfo, CellControl> m_CellsViewCache = new Dictionary<CellInfo, CellControl>();
        public IDictionary<CellInfo, CellControl> CellsViewCache
        {
            get { return m_CellsViewCache; }
        }

        IDictionary<int, Dictionary<int, CellControl>> m_CellsCache = new Dictionary<int, Dictionary<int, CellControl>>();

        public CellControl this[CellInfo cellInfo]
        {
            get
            {
                CellControl res = null;
                if (this.m_CellsViewCache.ContainsKey(cellInfo))
                {
                    res = this.m_CellsViewCache[cellInfo];
                }
                return res;
            }
        }

        public CellControl this[
                int columnIndex,
                int rowIndex]
        {
            get
            {
                CellControl res = null;
                Dictionary<int, CellControl> columnDict = null;
                if (this.m_CellsCache.ContainsKey(rowIndex))
                {
                    columnDict = this.m_CellsCache[rowIndex];
                }
                if (columnDict != null)
                {
                    if (columnDict.ContainsKey(columnIndex))
                    {
                        res = columnDict[columnIndex];
                    }
                }

                return res;
            }
        }

        public void Add(CellControl cell,
            int columnIndex,
            int rowIndex)
        {
            if (cell != null && cell.Cell != null)
            {
                m_CellsViewCache[cell.Cell] = cell;
            }

            Dictionary<int, CellControl> columnDict = null;
            if (this.m_CellsCache.ContainsKey(rowIndex))
            {
                columnDict = this.m_CellsCache[rowIndex];
            }

            if (columnDict == null)
            {
                columnDict = new Dictionary<int, CellControl>();
                this.m_CellsCache.Add(rowIndex, columnDict);
            }

            if (!columnDict.ContainsKey(columnIndex))
            {
                columnDict.Add(columnIndex, cell);
            }
            else
            {
                columnDict[columnIndex] = cell;
            }
        }
    }
}
