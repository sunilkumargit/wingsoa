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
using System.Windows.Media.Imaging;
using Ranet.AgOlap.Controls.Buttons;
using Ranet.AgOlap.Controls.General;
using Ranet.AgOlap.Controls.ValueCopy;
using Ranet.AgOlap.Controls.ContextMenu;
using Ranet.Olap.Core.Metadata;
using Ranet.AgOlap.Controls.MdxDesigner.Wrappers;
using Ranet.AgOlap.Controls.Forms;
using Ranet.AgOlap.Controls.MdxDesigner.Filters;

namespace Ranet.AgOlap.Controls.MdxDesigner
{
    public class FilteredItemControl : InfoItemControl
    {
        RanetHotButton m_FilterButton;

        public FilteredItemControl(Filtered_AreaItemWrapper wrapper, BitmapImage icon)
            : base(wrapper, icon)
        {
            m_FilterButton = new RanetHotButton();
            m_FilterButton.Click += new RoutedEventHandler(m_FilterButton_Click);
            m_FilterButton.Width = 18;
            m_FilterButton.Height = 18;
            m_FilterButton.Content = UiHelper.CreateIcon(UriResources.Images.FiltersArea16);
            LayoutRoot.Children.Add(m_FilterButton);
            Grid.SetColumn(m_FilterButton, 1);

            Refresh();
        }


        public FilteredItemControl(Filtered_AreaItemWrapper wrapper)
            : this (wrapper, null)
        {
        }

        internal Filtered_AreaItemWrapper FilteredWrapper
        {
            get {
                return Wrapper as Filtered_AreaItemWrapper;
            }
        }
        //bool m_UseTopFilter = false;
        ///// <summary>
        ///// Использовать Top-фильтр
        ///// </summary>
        //public bool UseTopFilter
        //{
        //    get { return m_UseTopFilter; }
        //    set { 
        //        m_UseTopFilter = value;
        //        RefreshFilterItemsImages();
        //    }
        //}

        //bool m_UseMembersFilter = false;
        ///// <summary>
        ///// Использовать фильтр на элементы
        ///// </summary>
        //public bool UseMembersFilter
        //{
        //    get { return m_UseMembersFilter; }
        //    set
        //    {
        //        m_UseMembersFilter = value;
        //        RefreshFilterItemsImages();
        //    }
        //}

        public void Refresh()
        {
            if (FilteredWrapper != null)
            {
                bool isFiltered = FilteredWrapper.CompositeFilter.IsUsed;
                // Иконка для самого элемента списка - "Наложен фильтр"
                if (isFiltered)
                {
                    ItemCtrl.TransparentIcon = UriResources.Images.MiniFilter16;
                }
                else
                {
                    ItemCtrl.TransparentIcon = null;
                }

                // Доступность кнопки "Очистить фильтр"
                if (m_Cancel_Filter_MenuItem != null)
                {
                    m_Cancel_Filter_MenuItem.IsEnabled = isFiltered;
                }
            }
            else
            {
                // Иконка для самого элемента списка - убираем иконку "Наложен фильтр"
                ItemCtrl.TransparentIcon = null;

                //if (m_MembersFilter_MenuItem != null)
                //    m_MembersFilter_MenuItem.Icon = null;
                // Доступность кнопки "Очистить фильтр"
                if (m_Cancel_Filter_MenuItem != null)
                {
                    m_Cancel_Filter_MenuItem.IsEnabled = false;
                }
            }
        }

        ContextMenuItem m_Cancel_Filter_MenuItem = null;
        ContextMenuItem m_MembersFilter_MenuItem = null;

        protected override Ranet.AgOlap.Controls.ContextMenu.CustomContextMenu CreateContextMenu()
        {
            CustomContextMenu contextMenu = base.CreateContextMenu();

            if (contextMenu.Items.Count > 0)
            {
                contextMenu.AddMenuSplitter();
            }

            m_Cancel_Filter_MenuItem = new ContextMenuItem(Localization.MdxDesigner_ContextMenu_CancelFilter);
            m_Cancel_Filter_MenuItem.Icon = UriResources.Images.CancelFilter16;
            contextMenu.AddMenuItem(m_Cancel_Filter_MenuItem);
            m_Cancel_Filter_MenuItem.ItemClick += new EventHandler(m_Cancel_Filter_MenuItem_ItemClick);

            m_MembersFilter_MenuItem = new ContextMenuItem(Localization.MdxDesigner_ContextMenu_ChangeFilter);
            m_MembersFilter_MenuItem.Icon = UriResources.Images.ChangeFilter16;
            contextMenu.AddMenuItem(m_MembersFilter_MenuItem);
            m_MembersFilter_MenuItem.ItemClick += new EventHandler(m_Members_Filter_MenuItem_ItemClick);

            Refresh();
            return contextMenu;
        }

        void m_Cancel_Filter_MenuItem_ItemClick(object sender, EventArgs e)
        {
            m_Cancel_Filter_MenuItem.IsEnabled = false;
            if (FilteredWrapper != null)
            {
                FilteredWrapper.CompositeFilter.IsUsed = false;
            }
            Refresh();
            Raise_CancelFilter();
        }

        void m_Members_Filter_MenuItem_ItemClick(object sender, EventArgs e)
        {
            Raise_ShowFilter();
        }

        public event EventHandler CancelFilter;
        void Raise_CancelFilter()
        {
            EventHandler handler = CancelFilter;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        public event EventHandler ShowFilter;
        void Raise_ShowFilter()
        {
            EventHandler handler = ShowFilter;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        void m_FilterButton_Click(object sender, RoutedEventArgs e)
        {
            Raise_ShowFilter();
        }

        //bool m_IsFiltered = false;
        ///// <summary>
        ///// Определяет, установлен ли для элемента какой-либо фильтр
        ///// </summary>
        //internal bool IsFiltered
        //{
        //    get { return m_IsFiltered; }
        //    set
        //    {
        //        m_IsFiltered = value;
        //        if (value)
        //        {
        //            ItemCtrl.TransparentIcon = UriResources.Images.MiniFilter16;
        //        }
        //        else
        //        {
        //            ItemCtrl.TransparentIcon = null;
        //        }
        //        // Доступность кнопки "Очистить фильтр"
        //        if (m_Cancel_Filter_MenuItem != null)
        //        {
        //            m_Cancel_Filter_MenuItem.IsEnabled = value;
        //        }
        //    }
        //}

        //String m_FilterSet = String.Empty;
        //public virtual String FilterSet
        //{
        //    get { return m_FilterSet; }
        //    set
        //    {
        //        m_FilterSet = value;
        //        m_FilterSet = m_FilterSet.Trim();
        //        if (m_FilterSet == "{}")
        //            m_FilterSet = String.Empty;

        //        IsFiltered = !String.IsNullOrEmpty(m_FilterSet);
        //    }
        //}
    }
}
