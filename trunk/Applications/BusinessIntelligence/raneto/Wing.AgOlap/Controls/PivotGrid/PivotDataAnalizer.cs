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
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Ranet.AgOlap.Controls.PivotGrid.Controls;
using Ranet.AgOlap.Controls.PivotGrid.Layout;
using System.Collections.Generic;
using Ranet.Olap.Core.Providers;
using Ranet.AgOlap.Features;

namespace Ranet.AgOlap.Controls.PivotGrid
{
    /// <summary>
    /// Класс предназначен для сбора информации о многомерных данных (макс. и мин. длина, макс. и мин. значение и т.д.)
    /// </summary>
    public class PivotDataAnalizer
    {
        PivotGridControl m_PivotGrid;
        public PivotDataAnalizer(PivotGridControl pivotGrid)
        {
            if (pivotGrid == null)
                throw new ArgumentNullException("pivotGrid");
            m_PivotGrid = pivotGrid;

            BuildCellsAnalytic();
        }

        internal void ClearMembersAnalytic()
        {
            m_ColumnsArea_ColumnsSize.Clear();
            m_RowsArea_ColumnsSize.Clear();
        }

        Dictionary<int, double> m_ColumnsArea_ColumnsSize = new Dictionary<int, double>();
        Dictionary<int, double> m_RowsArea_ColumnsSize = new Dictionary<int, double>();

        internal double GetEstimatedColumnSizeForColumnsArea(int columnIndex)
        {
            if(m_ColumnsArea_ColumnsSize.ContainsKey(columnIndex))
            {
                return m_ColumnsArea_ColumnsSize[columnIndex];
            }
            if (m_PivotGrid.m_LayoutProvider != null && columnIndex >= 0 && columnIndex < m_PivotGrid.m_LayoutProvider.ColumnsLayout.Columns_Size)
            {
                try
                {
                    // Описатели, попадающие в данную колонку
                    List<LayoutCellWrapper> column_items = new List<LayoutCellWrapper>();
                    for (int j = 0; j < m_PivotGrid.m_LayoutProvider.ColumnsLayout.Rows_Size; j++)
                    {
                        var cell_wrapper = m_PivotGrid.m_LayoutProvider.ColumnsLayout[columnIndex, j];
                        if (cell_wrapper != null)
                        {
                            column_items.Add(cell_wrapper);
                        }
                    }

                    // Анализируем только те элементы, которые не являются расширениями (описатель может содержать несколько элементов)
                    List<MemberLayoutItem> columns_members = new List<MemberLayoutItem>();
                    foreach (var item in column_items)
                    {
                        columns_members.AddRange(from p in item.Items where ((p.ColumnSpan == 1 && m_PivotGrid.ColumnsViewMode == ViewModeTypes.Tree) || p.IsExtension == false) select p as MemberLayoutItem);
                    }

                    // Сортируем элементы по длине текста с учетом размера шрифта
                    if (columns_members.Count > 0)
                    {
                        // Сортируем колонку по длине DisplayValue ячейки
                        var sorted = from p in columns_members orderby StringExtensions.Measure(p.PivotMember.Member.GetText(m_PivotGrid.MemberVisualizationType), m_PivotGrid.DefaultFontSize, null).Width select p;
                        double max = StringExtensions.Measure(sorted.Last().PivotMember.Member.GetText(m_PivotGrid.MemberVisualizationType), m_PivotGrid.DefaultFontSize, null).Width;
                        m_ColumnsArea_ColumnsSize[columnIndex] = max;
                        return max;
                    }
                }
                catch { }
            }
            return m_PivotGrid.DEFAULT_WIDTH;
        }

        internal double GetEstimatedColumnSizeForRowsArea(int columnIndex)
        {
            if (m_RowsArea_ColumnsSize.ContainsKey(columnIndex))
            {
                return m_RowsArea_ColumnsSize[columnIndex];
            }
            if (m_PivotGrid.m_LayoutProvider != null && columnIndex >= 0 && columnIndex < m_PivotGrid.m_LayoutProvider.RowsLayout.Columns_Size)
            {
                try
                {
                    // Описатели, попадающие в данную колонку
                    List<LayoutCellWrapper> column_items = new List<LayoutCellWrapper>();
                    for (int j = 0; j < m_PivotGrid.m_LayoutProvider.RowsLayout.Rows_Size; j++)
                    {
                        var cell_wrapper = m_PivotGrid.m_LayoutProvider.RowsLayout[columnIndex, j];
                        if (cell_wrapper != null)
                        {
                            column_items.Add(cell_wrapper);
                        }
                    }

                    // Анализируем только те элементы, которые не являются расширениями (описатель может содержать несколько элементов)
                    List<MemberLayoutItem> columns_members = new List<MemberLayoutItem>();
                    foreach (var item in column_items)
                    {
                        columns_members.AddRange(from p in item.Items where ((p.RowSpan == 1 && m_PivotGrid.ColumnsViewMode == ViewModeTypes.Tree) || p.IsExtension == false) select p as MemberLayoutItem);
                    }

                    // Сортируем элементы по длине текста с учетом размера шрифта
                    if (columns_members.Count > 0)
                    {
                        // Сортируем колонку по длине DisplayValue ячейки + отступ слева в случае древовидного представления
                        var sorted = from p in columns_members orderby (StringExtensions.Measure(p.PivotMember.Member.GetText(m_PivotGrid.MemberVisualizationType), m_PivotGrid.DefaultFontSize, null).Width + (m_PivotGrid.ColumnsViewMode == ViewModeTypes.Tree ? p.PivotMember.PivotDrillDepth : 0)) select p;
                        double max = StringExtensions.Measure(sorted.Last().PivotMember.Member.GetText(m_PivotGrid.MemberVisualizationType), m_PivotGrid.DefaultFontSize, null).Width;
                        m_RowsArea_ColumnsSize[columnIndex] = max;
                        return max;
                    }
                }
                catch { }
            }
            return m_PivotGrid.DEFAULT_WIDTH;
        }

        Dictionary<MemberInfo, MinMaxDescriptor<CellInfo>> m_Cells_DisplayValueLength_MinMax;
        /// <summary>
        /// Информация о мин. и макс. по длине строки значениях ячеек для элементов
        /// </summary>
        public Dictionary<MemberInfo, MinMaxDescriptor<CellInfo>> Cells_DisplayValueLength_MinMax
        {
            get
            {
                if (m_Cells_DisplayValueLength_MinMax == null)
                    m_Cells_DisplayValueLength_MinMax = new Dictionary<MemberInfo, MinMaxDescriptor<CellInfo>>();
                return m_Cells_DisplayValueLength_MinMax;
            }
        }

        /// <summary>
        /// Метод собирает аналитику по ячекам (мин. и макс. значение для строк и колонок, мин. и макс. по длине строки и т.д.)
        /// </summary>
        public void BuildCellsAnalytic()
        {
            Cells_DisplayValueLength_MinMax.Clear();
            CellSetDataProvider cs;
            if (m_PivotGrid.m_LayoutProvider != null &&
                m_PivotGrid.m_LayoutProvider.PivotProvider.Provider != null &&
                m_PivotGrid.m_LayoutProvider.PivotProvider.Provider.CellSet_Description != null)
            {
                cs = m_PivotGrid.m_LayoutProvider.PivotProvider.Provider;

                // Сбор аналитики для колонок

                int start_col = 0;
                // Если колонок нет, то нужно проверить колонку с индексом -1 (Ячейка есть, а оси пустые)
                if (cs.Columns_Size == 0 && cs.CellSet_Description.Cells.Count > 0)
                    start_col = -1;
                for (int col = start_col; col < cs.Columns_Size; col++)
                {
                    // колонка ячеек
                    List<CellInfo> column_cells = new List<CellInfo>();
                    int start_row = 0;
                    // Если колонок нет, то нужно проверить колонку с индексом -1 (Ячейка есть, а оси пустые)
                    if (cs.Rows_Size == 0 && cs.CellSet_Description.Cells.Count > 0)
                        start_row = -1;
                    for (int row = start_row; row < cs.Rows_Size; row++)
                    {
                        var cell = cs.GetCellInfo(col, row);
                        if (cell != null)
                        {
                            column_cells.Add(cell);
                        }
                    }

                    if (column_cells.Count > 0)
                    {
                        // Сортируем колонку по длине DisplayValue ячейки
                        var sorted = from p in column_cells orderby StringExtensions.Measure(p.DisplayValue, m_PivotGrid.DefaultFontSize, null).Width select p;
                        MinMaxDescriptor<CellInfo> displayValueLength_MinMax = new MinMaxDescriptor<CellInfo>(sorted.First(), sorted.Last());

                        if (cs.Columns_LowestMembers.ContainsKey(col))
                        {
                            Cells_DisplayValueLength_MinMax[cs.Columns_LowestMembers[col]] = displayValueLength_MinMax;
                        }
                        else
                        {
                            if (col == -1)
                                Cells_DisplayValueLength_MinMax[MemberInfo.Empty] = displayValueLength_MinMax;
                        }
                    }

                }
            }
        }
    }
}
