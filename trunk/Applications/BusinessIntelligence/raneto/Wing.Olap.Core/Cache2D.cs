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
using System.Collections.Generic;

namespace Ranet.Olap.Core
{
    public class Cache2D<T>
    {
        //  Ключ - номер строки. Значение - элементы для данной строки с ключом: номер колонки
        IDictionary<int, Dictionary<int, T>> m_Cache = new Dictionary<int, Dictionary<int, T>>();

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

        public T this[
                int columnIndex,
                int rowIndex]
        {
            get
            {
                T res = default(T);
                Dictionary<int, T> columnDict = null;
                if (this.m_Cache.ContainsKey(rowIndex))
                {
                    columnDict = this.m_Cache[rowIndex];
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

        public T RemoveAt(int columnIndex,
            int rowIndex)
        {
            Dictionary<int, T> columnDict = null;
            if (this.m_Cache.ContainsKey(rowIndex))
            {
                columnDict = this.m_Cache[rowIndex];
            }

            if (columnDict != null)
            {
                if (columnDict.ContainsKey(columnIndex))
                {
                    T val = columnDict[columnIndex];
                    columnDict.Remove(columnIndex);
                    return val;
                }
            }

            return default(T);
        }

        public void Add(T cell,
            int columnIndex,
            int rowIndex)
        {
            m_Rows_Size = Math.Max(rowIndex + 1, m_Rows_Size);
            m_Columns_Size = Math.Max(columnIndex + 1, m_Columns_Size);

            Dictionary<int, T> columnDict = null;
            if (this.m_Cache.ContainsKey(rowIndex))
            {
                columnDict = this.m_Cache[rowIndex];
            }

            if (columnDict == null)
            {
                columnDict = new Dictionary<int, T>();
                this.m_Cache.Add(rowIndex, columnDict);
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

        public bool GetCoordinates(T cell, out int columnIndex, out int rowIndex)
        {
            columnIndex = -1;
            rowIndex = -1;

            int i = 0;
            foreach (Dictionary<int, T> columnDict in m_Cache.Values)
            {
                foreach (int key in columnDict.Keys)
                {
                    if (columnDict[key].Equals(cell))
                    {
                        rowIndex = i;
                        columnIndex = key;
                        return true;
                    }
                }
                i++;
            }
            return false;        
        }
    }
}
