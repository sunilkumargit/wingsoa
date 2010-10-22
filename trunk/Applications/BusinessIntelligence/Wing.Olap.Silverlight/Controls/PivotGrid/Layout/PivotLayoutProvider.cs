/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using Wing.Olap.Core.Providers;

namespace Wing.Olap.Controls.PivotGrid.Layout
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
