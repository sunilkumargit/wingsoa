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
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using Ranet.AgOlap.Features;
using System.Windows.Browser;
using Ranet.AgOlap.Controls.General;

namespace Ranet.AgOlap.Controls.ContextMenu
{
    public partial class CustomContextMenu : UserControl
    {
        public CustomContextMenu()
        {
            InitializeComponent();

            this.MouseEnter += new MouseEventHandler(CustomContextMenu_MouseEnter);
            this.MouseLeave += new MouseEventHandler(CustomContextMenu_MouseLeave);
        }

        public event EventHandler Closed;
        public event EventHandler Opened;

        bool m_IsMouseOver = false;
        void CustomContextMenu_MouseLeave(object sender, MouseEventArgs e)
        {
            resTimer.Begin();
            m_IsMouseOver = false;
        }

        void CustomContextMenu_MouseEnter(object sender, MouseEventArgs e)
        {
            resTimer.Stop();
            m_IsMouseOver = true;
        }

        Popup m_PopupControl = null;
        public Popup PopupControl
        {
            get {
                if (m_PopupControl == null)
                {
                    m_PopupControl = new Popup();
                    m_PopupControl.Opened += new EventHandler(m_PopupControl_Opened);
                    m_PopupControl.Closed += new EventHandler(m_PopupControl_Closed);
                    m_PopupControl.Child = this;

                    PopupControl.VerticalOffset = 0;
                    PopupControl.HorizontalOffset = 0;

                    //this.LostFocus += new RoutedEventHandler(CustomContextMenu_LostFocus);
                }
                return m_PopupControl;
            }
        }

        void m_PopupControl_Closed(object sender, EventArgs e)
        {
            if (BrowserHelper.IsMozilla)
            {
                HtmlPage.Document.DetachEvent("onkeydown", new EventHandler<HtmlEventArgs>(Document_OnKeyDown));
            }
            else
            {
                HtmlPage.Document.DetachEvent("onkeypress", new EventHandler<HtmlEventArgs>(Document_OnKeyDown));
            }

            EventHandler handler = Closed;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        void m_PopupControl_Opened(object sender, EventArgs e)
        {
            if (BrowserHelper.IsMozilla)
            {
                HtmlPage.Document.AttachEvent("onkeydown", new EventHandler<HtmlEventArgs>(Document_OnKeyDown));
            }
            else
            {
                HtmlPage.Document.AttachEvent("onkeypress", new EventHandler<HtmlEventArgs>(Document_OnKeyDown));
            }
            
            EventHandler handler = Opened;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        void Document_OnKeyDown(object sender, HtmlEventArgs e)
        {
            if (e != null && e.CharacterCode == 27) //Escape
            {
                IsDropDownOpen = false;
            }
        }

        //void CustomContextMenu_LostFocus(object sender, RoutedEventArgs e)
        //{
        //    IsDropDownOpen = false;
        //}

        public event EventHandler ItemClicked;
        public event EventHandler Collapsed;
        public event EventHandler BeforeOpen;

        public bool IsDropDownOpen
        {
            get
            {
                return PopupControl.IsOpen;
            }
            set
            {
                if (value)
                {
                    if (Items.Count > 0)
                    {
                        EventHandler handler = BeforeOpen;
                        if (handler != null)
                        {
                            handler(this, EventArgs.Empty);
                        }
                        PopupControl.IsOpen = value;
                    }
                }
                else
                {
                    // Меню закрывается, значит таймер останавливаем
                    resTimer.Stop();

                    if (CurrentSubMenu != null)
                        CurrentSubMenu.IsDropDownOpen = false;

                    PopupControl.IsOpen = value;
                }
            }
        }

        public void SetLocation(Point pos)
        {
            // Сдвижка чтобы курсор попал в область меню
            PopupControl.VerticalOffset = pos.Y - 5;
            PopupControl.HorizontalOffset = pos.X - 5;
        }

        public Point GetLocation()
        {
            return new Point(PopupControl.HorizontalOffset, PopupControl.VerticalOffset);
        }

        public void AddMenuItem(ContextMenuItem item)
        {
            if(item != null)
            {
                itemsPanel.Children.Add(item);

                item.ItemClick += new EventHandler(item_ItemClick);

                item.MouseEnter += new MouseEventHandler(item_MouseEnter);
            }
        }

        void item_MouseEnter(object sender, MouseEventArgs e)
        {
            if (CurrentSubMenu != null)
            {
                CurrentSubMenu.IsDropDownOpen = false;
                CurrentSubMenu = null;
            }
            ShowSubmenu(sender as ContextMenuItem);
        }

        void ShowSubmenu(ContextMenuItem item)
        {
            if (item != null && item.HasSubMenu)
            {
                Rect p = AgControlBase.GetSLBounds(item);
                item.SubMenu.SetLocation(new Point(p.Right, p.Top + 6));
                item.SubMenu.Opened -= new EventHandler(SubMenu_Opened);
                item.SubMenu.Opened += new EventHandler(SubMenu_Opened);
                item.SubMenu.Closed -= new EventHandler(SubMenu_Closed);
                item.SubMenu.Closed += new EventHandler(SubMenu_Closed);
                item.SubMenu.MouseEnter -= new MouseEventHandler(SubMenu_MouseEnter);
                item.SubMenu.MouseEnter += new MouseEventHandler(SubMenu_MouseEnter);
                item.SubMenu.Collapsed -= new EventHandler(SubMenu_Collapsed);
                item.SubMenu.Collapsed += new EventHandler(SubMenu_Collapsed);
                item.SubMenu.ItemClicked -= new EventHandler(SubMenu_ItemClicked);
                item.SubMenu.ItemClicked += new EventHandler(SubMenu_ItemClicked);

                item.SubMenu.IsDropDownOpen = true;
                return;
            }
        }

        void SubMenu_ItemClicked(object sender, EventArgs e)
        {
            IsDropDownOpen = false;
            EventHandler handler = this.ItemClicked;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        void SubMenu_Collapsed(object sender, EventArgs e)
        {
            if (!m_IsMouseOver)
            {
                IsDropDownOpen = false;

                EventHandler handler = this.Collapsed;
                if (handler != null)
                {
                    handler(this, EventArgs.Empty);
                }
            }
        }

        void SubMenu_MouseEnter(object sender, MouseEventArgs e)
        {
            resTimer.Stop();
        }

        void SubMenu_Opened(object sender, EventArgs e)
        {
            var subMenu = sender as CustomContextMenu;
            CurrentSubMenu = subMenu;
        }

        void SubMenu_Closed(object sender, EventArgs e)
        {
            CurrentSubMenu = null;
        }

        internal CustomContextMenu CurrentSubMenu;

        public UIElementCollection Items
        {
            get {
                return itemsPanel.Children;
            }
        }

        public ContextMenuSplitter AddMenuSplitter()
        {
            ContextMenuSplitter splitter = new ContextMenuSplitter();
            itemsPanel.Children.Add(splitter);
            return splitter;
        }

        void item_ItemClick(object sender, EventArgs e)
        {
            var item = sender as ContextMenuItem;
            if (item != null && item.HasSubMenu)
            {
                if (CurrentSubMenu != null)
                {
                    CurrentSubMenu.IsDropDownOpen = false;
                    CurrentSubMenu = null;
                }
                else
                {
                    ShowSubmenu(item);
                }
                return;
            }

            IsDropDownOpen = false;

            EventHandler handler = this.ItemClicked;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        private void resTimer_Completed(object sender, EventArgs e)
        {
            IsDropDownOpen = false;

            EventHandler handler = this.Collapsed;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}
