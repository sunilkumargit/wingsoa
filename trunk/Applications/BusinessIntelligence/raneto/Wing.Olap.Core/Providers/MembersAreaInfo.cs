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

namespace Ranet.Olap.Core.Providers
{
    public enum AreaType
    { 
        RowsArea,
        ColumnsArea,
        CellsArea
    }
    
    public class MembersAreaInfo
    {
        public readonly AreaType Type = AreaType.RowsArea;

        public MembersAreaInfo(AreaType type)
        { 
            Type = type;
        }

        /// <summary>
        /// Область строк: Ключ - номер колонки; значение - максимальная глубина DrillDown
        /// Область столбцов: Ключ - номер строки; значение - максимальная глубина DrillDown
        /// </summary>
        Dictionary<int, int> m_DrillDepth = new Dictionary<int, int>();
        public Dictionary<int, int> DrillDepth
        {
            get {
                return m_DrillDepth;
            }
        }

        public int ColumnsCount = 0;
        public int RowsCount = 0;

        List<PivotMemberItem> m_Members = new List<PivotMemberItem>();
        public List<PivotMemberItem> Members
        {
            get
            {
                return m_Members;
            }
        }

        public void Initialize(MemberInfoCollection members)
        {
            Members.Clear();
            if (members != null)
            {
                int size = 0;

                int rowIndex = 0;
                int columnIndex = 0;

                foreach (MemberInfo member in members)
                {
                    if (Type == AreaType.RowsArea)
                    {
                        this.AddRowMember(Members, member, columnIndex, rowIndex, out size, 0);
                        rowIndex += size;
                    }
                    if (Type == AreaType.ColumnsArea)
                    {
                        this.AddColumnMember(Members, member, columnIndex, rowIndex, out size, 0);
                        columnIndex += size;
                    }
                }
            }
        }

        private PivotMemberItem AddRowMember(List<PivotMemberItem> container, MemberInfo member, int columnIndex, int rowIndex, out int memberSize, int drillDepth)
        {
            // Добавляем в случае необходимости строку и столбец
            ColumnsCount = Math.Max(ColumnsCount, columnIndex + 1);
            RowsCount = Math.Max(RowsCount, rowIndex + 1);
            
            // Сохраняем максимальной глубины DrillDown
            if (m_DrillDepth.ContainsKey(columnIndex))
                m_DrillDepth[columnIndex] = Math.Max(m_DrillDepth[columnIndex], drillDepth);
            else
                m_DrillDepth[columnIndex] = drillDepth;

            // Элемент сводной таблицы
            PivotMemberItem item = new PivotMemberItem(member);
            item.PivotDrillDepth = drillDepth;
            item.ColumnIndex = columnIndex;
            item.RowIndex = rowIndex;

            container.Add(item);

            // Дочерние элементы начинаются отображаться с той же строки. только в соседней колонке
            // Номер строки с которой начнется отображение элемента
            int start_Child_RowIndex = rowIndex;
            // Текущий индекс строки
            int current_Child_RowIndex = start_Child_RowIndex;
            // Размер добавляемого контрола (в строках)
            int size = 0;
            foreach (MemberInfo child in member.Children)
            {
                AddRowMember(item.Children, child, columnIndex + 1, current_Child_RowIndex, out size, 0);
                current_Child_RowIndex += size;
            }
            // Номер строки, на которой закончится отображение элемента
            int end_Child_RowIndex = current_Child_RowIndex;
            int ChildrenSize = end_Child_RowIndex - start_Child_RowIndex;

            // Дрилл-даун дочерние отображаются в этой же колонке, но со строки следующей после самого элемента и всех дочерних
            // Номер строки с которой начнется отображение элемента
            int start_DrillDown_RowIndex = ChildrenSize > 0 ? end_Child_RowIndex : rowIndex + 1;
            // Текущий индекс строки
            int current_DrillDown_RowIndex = start_DrillDown_RowIndex;

            // Размер добавляемого контрола (в строках)
            size = 0;
            int i = 0;
            foreach (MemberInfo child in member.DrilledDownChildren)
            {
                PivotMemberItem dd_item = AddRowMember(item.DrillDownChildren, child, columnIndex, current_DrillDown_RowIndex, out size, drillDepth + 1);
                if (i == 0)
                    dd_item.IsFirstDrillDownChild = true;
                i++;
                current_DrillDown_RowIndex += size;
            }
            // Номер строки, на которой закончится отображение элемента
            int end_DrillDown_RowIndex = current_DrillDown_RowIndex;
            int DrillDownChildrenSize = end_DrillDown_RowIndex - start_DrillDown_RowIndex;

            // Растягиваем элемент если есть дочерние
            int span_Size = 1;
            int span_Size_WithChildren = 1;
            if (ChildrenSize > 1)
            {
                span_Size_WithChildren = ChildrenSize;
            }
            if (DrillDownChildrenSize > 0)
            {
                span_Size = span_Size_WithChildren + DrillDownChildrenSize;
            }
            else
            {
                span_Size = span_Size_WithChildren;
            }

            if (span_Size > 1)
            {
                item.RowSpan = span_Size;
            }

            item.ChildrenSize = ChildrenSize;
            item.DrillDownChildrenSize = DrillDownChildrenSize;
            memberSize = span_Size;
            return item;
        }

        private PivotMemberItem AddColumnMember(List<PivotMemberItem> container, MemberInfo member, int columnIndex, int rowIndex, out int memberSize, int drillDepth)
        {
            // Добавляем в случае необходимости строку и столбец
            ColumnsCount = Math.Max(ColumnsCount, columnIndex + 1);
            RowsCount = Math.Max(RowsCount, rowIndex + 1);

            // Сохраняем максимальной глубины DrillDown
            if (m_DrillDepth.ContainsKey(rowIndex))
                m_DrillDepth[rowIndex] = Math.Max(m_DrillDepth[rowIndex], drillDepth);
            else
                m_DrillDepth[rowIndex] = drillDepth;

            // Элемент сводной таблицы
            PivotMemberItem item = new PivotMemberItem(member);
            item.PivotDrillDepth = drillDepth;
            item.ColumnIndex = columnIndex;
            item.RowIndex = rowIndex;

            container.Add(item);

            // Дочерние элементы начинаются отображаться с той же колонки. только строкой ниже
            // Номер колонки с которой начнется отображение элемента
            int start_Child_ColumnIndex = columnIndex;
            // Текущий индекс колонки
            int current_Child_columnIndex = start_Child_ColumnIndex;
            // Размер добавляемого контрола (в колонках)
            int size = 0;
            foreach (MemberInfo child in member.Children)
            {
                AddColumnMember(item.Children, child, current_Child_columnIndex, rowIndex + 1, out size, 0);
                current_Child_columnIndex = current_Child_columnIndex + size;
            }
            // Номер колонки, на которой закончится отображение элемента
            int end_Child_ColumnIndex = current_Child_columnIndex;
            int ChildrenSize = end_Child_ColumnIndex - start_Child_ColumnIndex;

            // Дрилл-даун дочерние отображаются в этой же строке, но с колонки следующей после самого элемента и всех дочерних
            // Номер колонки с которой начнется отображение элемента
            int start_DrillDown_ColumnIndex = ChildrenSize > 0 ? end_Child_ColumnIndex : columnIndex + 1;
            // Текущий индекс колонки
            int current_DrillDown_columnIndex = start_DrillDown_ColumnIndex;

            // Размер добавляемого контрола (в строках)
            size = 0;
            int i = 0;
            foreach (MemberInfo child in member.DrilledDownChildren)
            {
                PivotMemberItem dd_item = AddColumnMember(item.DrillDownChildren, child, current_DrillDown_columnIndex, rowIndex, out size, drillDepth + 1);
                if (i == 0)
                    dd_item.IsFirstDrillDownChild = true;
                i++;
                current_DrillDown_columnIndex += size;
            }
            // Номер колонки, на которой закончится отображение элемента
            int end_DrillDown_ColumnIndex = current_DrillDown_columnIndex;
            int DrillDownChildrenSize = end_DrillDown_ColumnIndex - start_DrillDown_ColumnIndex;


            // Растягиваем элемент если есть дочерние
            int span_Size = 1;
            int span_Size_WithChildren = 1;
            if (ChildrenSize > 1)
            {
                span_Size_WithChildren = ChildrenSize;
            }
            if (member.DrilledDownChildren.Count > 0)
            {
                span_Size = span_Size_WithChildren + DrillDownChildrenSize;
            }
            else
            {
                span_Size = span_Size_WithChildren;
            }

            if (span_Size > 1)
            {
                item.ColumnSpan = span_Size;
            }

            item.ChildrenSize = ChildrenSize;
            item.DrillDownChildrenSize = DrillDownChildrenSize;
            memberSize = span_Size;
            return item;
        }
    }
}
