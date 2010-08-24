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
using Ranet.Olap.Core.Providers;

namespace Ranet.AgOlap.Controls.PivotGrid.Layout
{
    public class PivotLayoutProvider
    {
        LayoutGridWrapper m_ColumnsLayout = null;
        public LayoutGridWrapper ColumnsLayout
        {
            get {
                if (m_ColumnsLayout == null)
                {
                    m_ColumnsLayout = new LayoutGridWrapper();
                    FillColumnsLayout();   
                }
                return m_ColumnsLayout;
            }
        }

        LayoutGridWrapper m_RowsLayout = null;
        public LayoutGridWrapper RowsLayout
        {
            get
            {
                if (m_RowsLayout == null)
                {
                    m_RowsLayout = new LayoutGridWrapper();
                    FillRowsLayout();
                }
                return m_RowsLayout;
            }
        }

        PivotDataProvider m_Provider = null;
        public PivotDataProvider PivotProvider
        {
            get { return m_Provider; }
        }

        public PivotLayoutProvider(PivotDataProvider provider)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }

            m_Provider = provider;
        }

        void FillColumnsLayout()
        {
            foreach (PivotMemberItem member in m_Provider.ColumnsArea.Members)
            {
                AddToColumnsLayout(member);
            }
        }

        void FillRowsLayout()
        {
            foreach (PivotMemberItem member in m_Provider.RowsArea.Members)
            {
                AddToRowsLayout(member);
            }
        }

        void AddToColumnsLayout(PivotMemberItem member)
        {
            if (member != null)
            {
                // Для данного элемента пытаемся получить по координатам LayoutCellWrapper
                LayoutCellWrapper itemWrapper = ColumnsLayout[member.ColumnIndex, member.RowIndex];
                if (itemWrapper == null)
                {
                    itemWrapper = new LayoutCellWrapper();
                    ColumnsLayout.Add(itemWrapper, member.ColumnIndex, member.RowIndex);
                }

                // Создаем описатель для данного элемента и добавляем его в коллекцию объектов ячейки сетки
                MemberLayoutItem item = new MemberLayoutItem(member);
                itemWrapper.Items.Add(item);
                item.RowSpan = member.RowSpan;
                item.ColumnSpan = member.ColumnSpan;

                // Если элемент в ширину больше чем 1, то добавляем фиктивные элементы на сетку
                for (int i = 1; i < member.ColumnSpan; i++)
                {
                    LayoutCellWrapper ext_itemWrapper = ColumnsLayout[member.ColumnIndex + i, member.RowIndex];
                    if (ext_itemWrapper == null)
                    {
                        ext_itemWrapper = new LayoutCellWrapper();
                        ColumnsLayout.Add(ext_itemWrapper, member.ColumnIndex + i, member.RowIndex);
                    }

                    // Создаем описатель для фиктивного элемента
                    MemberLayoutItem ext_item = new MemberLayoutItem(member);
                    ext_item.IsExtension = true;
                    ext_itemWrapper.Items.Add(ext_item);
                    ext_item.ColumnSpan = member.ColumnSpan - i;
                    ext_item.RowSpan = member.RowSpan;
                }

                foreach (PivotMemberItem dd_item in member.DrillDownChildren)
                {
                    AddToColumnsLayout(dd_item);
                }
                foreach (PivotMemberItem child_item in member.Children)
                {
                    AddToColumnsLayout(child_item);
                }
            }
        }

        void AddToRowsLayout(PivotMemberItem member)
        {
            if (member != null)
            {
                // Для данного элемента пытаемся получить по координатам LayoutCellWrapper
                LayoutCellWrapper itemWrapper = RowsLayout[member.ColumnIndex, member.RowIndex];
                if (itemWrapper == null)
                {
                    itemWrapper = new LayoutCellWrapper();
                    RowsLayout.Add(itemWrapper, member.ColumnIndex, member.RowIndex);
                }

                // Создаем описатель для данного элемента и добавляем его в коллекцию объектов ячейки сетки
                MemberLayoutItem item = new MemberLayoutItem(member);
                itemWrapper.Items.Add(item);
                item.RowSpan = member.RowSpan;
                item.ColumnSpan = member.ColumnSpan;

                // Если элемент в высоту больше чем 1, то добавляем фиктивные элементы на сетку
                for (int i = 1; i < member.RowSpan; i++)
                {
                    LayoutCellWrapper ext_itemWrapper = RowsLayout[member.ColumnIndex, member.RowIndex + i];
                    if (ext_itemWrapper == null)
                    {
                        ext_itemWrapper = new LayoutCellWrapper();
                        RowsLayout.Add(ext_itemWrapper, member.ColumnIndex, member.RowIndex + i);
                    }

                    // Создаем описатель для фиктивного элемента
                    MemberLayoutItem ext_item = new MemberLayoutItem(member);
                    ext_item.IsExtension = true;
                    ext_itemWrapper.Items.Add(ext_item);
                    ext_item.ColumnSpan = member.ColumnSpan;
                    ext_item.RowSpan = member.RowSpan - i;
                }

                foreach (PivotMemberItem dd_item in member.DrillDownChildren)
                {
                    AddToRowsLayout(dd_item);
                }
                foreach (PivotMemberItem child_item in member.Children)
                {
                    AddToRowsLayout(child_item);
                }
            }
        }
    }
}
