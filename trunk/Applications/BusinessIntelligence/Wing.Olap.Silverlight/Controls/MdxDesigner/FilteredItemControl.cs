/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Wing.Olap.Controls.Buttons;
using Wing.Olap.Controls.ContextMenu;
using Wing.Olap.Controls.General;
using Wing.Olap.Controls.MdxDesigner.Wrappers;

namespace Wing.Olap.Controls.MdxDesigner
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

        protected override Wing.Olap.Controls.ContextMenu.CustomContextMenu CreateContextMenu()
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
