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
using Ranet.AgOlap.Controls.List;

namespace Ranet.AgOlap.Controls.MdxDesigner
{
    public class AreaItemArgs : EventArgs
    {
        public readonly AreaItemControl ItemControl = null;

        public AreaItemArgs(AreaItemControl itemControl)
        {
            if (itemControl == null)
                throw new ArgumentNullException("itemControl");
            ItemControl = itemControl;
        }
    }

    public class AreaItemsList : UserControl
    {
        RanetListBox m_List;

        public AreaItemsList()
        {
            Grid LayoutRoot = new Grid();

            m_List = new RanetListBox();
            LayoutRoot.Children.Add(m_List);

            this.Content = LayoutRoot;

            this.KeyDown += new KeyEventHandler(AreaItemsList_KeyDown);
        }

        void AreaItemsList_KeyDown(object sender, KeyEventArgs e)
        {
            var item = m_List.SelectedItem as ListBoxItem;
            if (item != null && item.Content is AreaItemControl)
            {
                RemoveItem(item.Content as AreaItemControl);
            }
        }

        public List<AreaItemControl> Items
        {
            get
            {
                List<AreaItemControl> m_Items = new List<AreaItemControl>();
                foreach (ListBoxItem list_Item in m_List.Items)
                { 
                    AreaItemControl item = list_Item.Content as AreaItemControl;
                    if (item != null)
                        m_Items.Add(item);
                }
                return m_Items;
            }
        }

        //void RefreshVisualState()
        //{
        //    m_List.Items.Clear();
        //    foreach (AreaItemControl item in m_Items)
        //    {
        //        //AreaItemControl item = x.Clone();
        //        ListBoxItem list_Item = new ListBoxItem();
        //        list_Item.HorizontalContentAlignment = HorizontalAlignment.Stretch;
         
        //        item.Remove += new EventHandler(item_Remove);
        //        list_Item.Content = item;
        //        m_List.Items.Add(list_Item);
        //    }
        //}

        #region События
        public event EventHandler<AreaItemArgs> ItemRemoved;
        public event EventHandler<AreaItemArgs> ContextMenuCreated;
        public event EventHandler<AreaItemArgs> BeforeShowContextMenu;
        public event EventHandler ItemsListChanged;

        void Raise_BeforeShowContextMenu(AreaItemControl item)
        {
            EventHandler<AreaItemArgs> handler = BeforeShowContextMenu;
            if (handler != null)
            {
                handler(this, new AreaItemArgs(item));
            }
        }

        void Raise_ContextMenuCreated(AreaItemControl item)
        {
            EventHandler<AreaItemArgs> handler = ContextMenuCreated;
            if (handler != null)
            {
                handler(this, new AreaItemArgs(item));
            }
        }

        void Raise_ItemRemoved(AreaItemControl item)
        {
            EventHandler<AreaItemArgs> handler = ItemRemoved;
            if (handler != null)
            {
                handler(this, new AreaItemArgs(item));
            }
        }

        void Raise_ItemsListChanged()
        {
            EventHandler handler = ItemsListChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
        #endregion События

        public void RemoveItem(AreaItemControl item, bool raise_ListChanged)
        {
            if (item != null && item.Tag != null)
            {
                if (m_List.Items.Contains(item.Tag))
                {
                    m_List.Items.Remove(item.Tag);

                    item.Remove -= new EventHandler(item_Remove);
                    item.MoveUp -= new EventHandler(item_MoveUp);
                    item.MoveDown -= new EventHandler(item_MoveDown);
                    item.ShowContextMenu -= new EventHandler<PositionArgs>(item_ShowContextMenu);
                    item.ContextMenuCreated -= new EventHandler(item_ContextMenuCreated);

                    Raise_ItemRemoved(item);
                    if (raise_ListChanged)
                    {
                        Raise_ItemsListChanged();
                    }
                }
            }
        }

        public void RemoveItem(AreaItemControl item)
        {
            RemoveItem(item, true);
        }

        public void Clear()
        {
            List<AreaItemControl> list = new List<AreaItemControl>();
            foreach (ListBoxItem list_Item in m_List.Items)
            {
                AreaItemControl item = list_Item.Content as AreaItemControl;
                if (item != null)
                    list.Add(item);
            }

            foreach (AreaItemControl item in list)
                RemoveItem(item, false);
            
            if (list.Count > 0)
                Raise_ItemsListChanged();
        }

        public void AddItem(AreaItemControl item)
        {
            AddItem(item, true);
        }

        public void AddItem(AreaItemControl item, bool raise_ItemsListChanged)
        {
            // Если ListBoxItem ранее был удален из списка (при перетаскивании например), то item.Tag содержит тот ListBoxItem, который удалялся. И попытка создать новый и присвоить в Content текущий item приведет к ошибке, т.к. Parent остался старый
            // В этом случае просто восстановим ListBoxItem из св-ва Tag
            ListBoxItem list_Item = null;
            list_Item = item.Tag as ListBoxItem;
            if (list_Item == null)
            {
                list_Item = new ListBoxItem();
            }
            
            list_Item.HorizontalContentAlignment = HorizontalAlignment.Stretch;

            item.Remove += new EventHandler(item_Remove);
            item.MoveUp += new EventHandler(item_MoveUp);
            item.MoveDown += new EventHandler(item_MoveDown);
            item.ShowContextMenu += new EventHandler<PositionArgs>(item_ShowContextMenu);
            item.ContextMenuCreated += new EventHandler(item_ContextMenuCreated);
            list_Item.Content = item;
            item.Tag = list_Item;

            m_List.Items.Add(list_Item);

            if(raise_ItemsListChanged)
                Raise_ItemsListChanged();
        }


        void item_ContextMenuCreated(object sender, EventArgs e)
        {
            AreaItemControl item = sender as AreaItemControl;
            if (item != null)
            {
                Raise_ContextMenuCreated(item);
            }
        }

        void item_ShowContextMenu(object sender, PositionArgs e)
        {
            AreaItemControl item = sender as AreaItemControl;
            if (item != null)
            {
                if (item.ContextMenu.IsDropDownOpen)
                    item.ContextMenu.IsDropDownOpen = false;

                Raise_BeforeShowContextMenu(item);

                // Управляем доступностью кнопок Вверх/Вниз
                ListBoxItem list_Item = item.Tag as ListBoxItem;
                if (list_Item != null)
                {
                    int indx = m_List.Items.IndexOf(list_Item);
                    item.CanMoveUp = indx > 0;
                    item.CanMoveDown = (indx > -1 && indx < m_List.Items.Count - 1);
                }
                else
                {
                    item.CanMoveUp = false;
                    item.CanMoveDown = false;
                }

                item.ContextMenu.SetLocation(e.Position);
                item.ContextMenu.Tag = item;
                item.ContextMenu.IsDropDownOpen = true;
            }
        }

        void item_MoveDown(object sender, EventArgs e)
        {
            AreaItemControl item = sender as AreaItemControl;
            if (item != null)
            {
                ListBoxItem list_Item = item.Tag as ListBoxItem;
                if (list_Item != null)
                {
                    int indx = m_List.Items.IndexOf(list_Item);
                    if (indx > -1 && indx < m_List.Items.Count - 1)
                    {
                        m_List.Items.RemoveAt(indx);
                        // Контент убивается при удалении
                        list_Item.Content = item;
                        m_List.Items.Insert(indx + 1, list_Item);
                        Raise_ItemsListChanged();
                    }
                }
            }
        }

        void item_MoveUp(object sender, EventArgs e)
        {
            AreaItemControl item = sender as AreaItemControl;
            if (item != null)
            {
                ListBoxItem list_Item = item.Tag as ListBoxItem;
                if (list_Item != null)
                {
                    int indx = m_List.Items.IndexOf(list_Item);
                    if (indx > 0)
                    {
                        m_List.Items.RemoveAt(indx);
                        // Контент убивается при удалении
                        list_Item.Content = item;
                        m_List.Items.Insert(indx - 1, list_Item);
                        Raise_ItemsListChanged();
                    }
                }
            }
        }

        void item_Remove(object sender, EventArgs e)
        {
            RemoveItem(sender as AreaItemControl);
        }
    }
}
