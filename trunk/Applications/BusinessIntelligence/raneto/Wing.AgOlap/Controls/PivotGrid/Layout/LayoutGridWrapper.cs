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

namespace Ranet.AgOlap.Controls.PivotGrid.Layout
{
    /// <summary>
    /// Враппер, описывающий сетку расположения элементов
    /// </summary>
    public class LayoutGridWrapper
    {
        //public readonly MembersAreaInfo PivotArea = null;
        //public LayoutGridWrapper(MembersAreaInfo area)
        //{
        //    PivotArea = area;
        //}

        //  Ключ - номер строки. Значение - элементы для данной строки с ключом: номер колонки
        IDictionary<int, Dictionary<int, LayoutCellWrapper>> m_CellsCache = new Dictionary<int, Dictionary<int, LayoutCellWrapper>>();

        int m_Columns_Size = 0;
        /// <summary>
        /// Размер по горизонтали (не все колонки могут быть инициализированы!)
        /// </summary>
        public int Columns_Size
        {
            get { return m_Columns_Size; }
        }

        int m_Rows_Size = 0;
        /// <summary>
        /// Размер по вертикали (не все строки могут быть инициализированы!)
        /// </summary>
        public int Rows_Size
        {
            get { return m_Rows_Size; }
        }

        public LayoutCellWrapper this[
                int columnIndex,
                int rowIndex]
        {
            get
            {
                LayoutCellWrapper res = null;
                Dictionary<int, LayoutCellWrapper> columnDict = null;
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

        public void Add(LayoutCellWrapper cell,
            int columnIndex,
            int rowIndex)
        {
            m_Rows_Size = Math.Max(rowIndex + 1, m_Rows_Size);
            m_Columns_Size = Math.Max(columnIndex + 1, m_Columns_Size);

            Dictionary<int, LayoutCellWrapper> columnDict = null;
            if (this.m_CellsCache.ContainsKey(rowIndex))
            {
                columnDict = this.m_CellsCache[rowIndex];
            }

            if (columnDict == null)
            {
                columnDict = new Dictionary<int, LayoutCellWrapper>();
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
