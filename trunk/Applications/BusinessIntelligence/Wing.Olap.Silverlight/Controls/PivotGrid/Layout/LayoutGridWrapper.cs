/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
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
using Wing.Olap.Core.Providers;

namespace Wing.Olap.Controls.PivotGrid.Layout
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
