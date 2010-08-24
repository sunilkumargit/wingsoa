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
using Ranet.AgOlap.Controls.General.Tree;
using Ranet.AgOlap.Controls.Buttons;
using Ranet.AgOlap.Controls.General;
using System.Windows.Media.Imaging;
using Ranet.AgOlap.Controls.ContextMenu;
using System.Windows.Browser;
using Ranet.AgOlap.Controls.PivotGrid.Controls;

namespace Ranet.AgOlap.Controls.MdxDesigner
{
    public class PositionArgs : EventArgs
    { 
        public readonly Point Position = new Point(0, 0);

        public PositionArgs(Point position)
        {
            Position = position;
        }
    }

    public class AreaItemControl : DragDropControl
    {
        RanetHotButton m_RemoveButton;
        RanetHotButton m_MoveUpButton;
        RanetHotButton m_MoveDownButton;

        HeaderControl m_ItemCtrl;
        protected HeaderControl ItemCtrl
        {
            get { return m_ItemCtrl; }
        }

        public PivotAreaContainer Area = null;

        #region События 
        public event EventHandler Remove;
        public event EventHandler MoveUp;
        public event EventHandler MoveDown;
        public event EventHandler<PositionArgs> ShowContextMenu;
        public event EventHandler ContextMenuCreated;

        private void Raise_ContextMenuCreated()
        {
            EventHandler handler = this.ContextMenuCreated;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
        
        private void Raise_ShowContextMenu(PositionArgs args)
        {
            EventHandler<PositionArgs> handler = this.ShowContextMenu;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        void Raise_MoveDown()
        {
            EventHandler handler = MoveDown;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        void Raise_MoveUp()
        {
            EventHandler handler = MoveUp;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        void Raise_Remove()
        {
            EventHandler handler = Remove;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
        #endregion События

        public bool CanMoveUp
        {
            get { return m_MoveUp_MenuItem.IsEnabled; }
            set { m_MoveUp_MenuItem.IsEnabled = value; }
        }

        public bool CanMoveDown
        {
            get { return m_MoveDown_MenuItem.IsEnabled; }
            set { m_MoveDown_MenuItem.IsEnabled = value; }
        }

        public String Caption
        {
            get { return m_ItemCtrl.Caption; }
            set { m_ItemCtrl.Caption = value; }
        }

        public BitmapImage Icon
        {
            get { return m_ItemCtrl.Icon; }
            set { m_ItemCtrl.Icon = value; }
        }

        protected Grid LayoutRoot;

        public AreaItemControl(BitmapImage icon, String caption)
            :this ( )
        {
            Icon = icon;
            Caption = caption;
        }

        TooltipController TooltipManager;
        protected readonly ToolTipControl ToolTip = new ToolTipControl();

        public AreaItemControl()
        {
            LayoutRoot = new Grid();
            LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition());
            LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

            Border border = new Border();
            border.BorderBrush = new SolidColorBrush(Colors.DarkGray);
            border.BorderThickness = new Thickness(1);
            border.Padding = new Thickness(2);
            border.Child = LayoutRoot;

            m_ItemCtrl = new HeaderControl();
            LayoutRoot.Children.Add(m_ItemCtrl);

            m_MoveUpButton = new RanetHotButton();
            m_MoveUpButton.Click += new RoutedEventHandler(m_MoveUpButton_Click);
            m_MoveUpButton.Width = 18;
            m_MoveUpButton.Height = 18;
            m_MoveUpButton.Content = UiHelper.CreateIcon(UriResources.Images.Up16);
            LayoutRoot.Children.Add(m_MoveUpButton);
            Grid.SetColumn(m_MoveUpButton, 2); 

            m_MoveDownButton = new RanetHotButton();
            m_MoveDownButton.Click += new RoutedEventHandler(m_MoveDownButton_Click);
            m_MoveDownButton.Width = 18;
            m_MoveDownButton.Height = 18;
            m_MoveDownButton.Content = UiHelper.CreateIcon(UriResources.Images.Down16);
            LayoutRoot.Children.Add(m_MoveDownButton);
            Grid.SetColumn(m_MoveDownButton, 3); 

            m_RemoveButton = new RanetHotButton();
            m_RemoveButton.Click += new RoutedEventHandler(m_RemoveButton_Click);
            m_RemoveButton.Width = 18;
            m_RemoveButton.Height = 18;
            m_RemoveButton.Content = UiHelper.CreateIcon(UriResources.Images.RemoveHot16);
            LayoutRoot.Children.Add(m_RemoveButton);
            Grid.SetColumn(m_RemoveButton, 4); 

            this.Content = border;

            LayoutRoot.AttachContextMenu(p => GetCurrentContextMenu(p));

            TooltipManager = new TooltipController(this);
            TooltipManager.BeforeOpen += new EventHandler<CustomEventArgs<Point>>(TooltipManager_BeforeOpen);
            TooltipManager.ToolTipContent = ToolTip;
        }

        void TooltipManager_BeforeOpen(object sender, CustomEventArgs<Point> e)
        {
            // Проверяем чтобы в тултипе было хоть что-нибудь задано
            var tooltip = TooltipManager.ToolTipContent as ToolTipControl;
            if (tooltip != null)
            {
                if (!String.IsNullOrEmpty(tooltip.Text) ||
                !String.IsNullOrEmpty(tooltip.Caption))
                    return;
            }
            e.Cancel = true;
        }

        CustomContextMenu GetCurrentContextMenu(Point p)
        {
            if (AgControlBase.GetSLBounds(this).Contains(p))
            {
                return ContextMenu;
            }
            return null;
        }

        public Rect SLBounds
        {
            get
            {
                Point pos = AgControlBase.GetSilverlightPos(this);
                return new Rect(pos, new Size(this.ActualWidth, this.ActualHeight));
            }
        }

        void m_MoveDownButton_Click(object sender, RoutedEventArgs e)
        {
            Raise_MoveDown();
        }

        void m_MoveUpButton_Click(object sender, RoutedEventArgs e)
        {
            Raise_MoveUp();
        }

        void m_RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            Raise_Remove();
        }

        public object UserData;

        #region Контекстное меню
        CustomContextMenu m_ContextMenu = null;
        public CustomContextMenu ContextMenu
        {
            get
            {
                if (m_ContextMenu == null)
                {
                    m_ContextMenu = CreateContextMenu();
                    Raise_ContextMenuCreated();
                }
                return m_ContextMenu;
            }
        }

        ContextMenuItem m_MoveUp_MenuItem;
        ContextMenuItem m_MoveDown_MenuItem;

        protected virtual CustomContextMenu CreateContextMenu()
        {
            CustomContextMenu contextMenu = new CustomContextMenu();
            ContextMenuItem item;

            m_MoveUp_MenuItem = new ContextMenuItem(Localization.MdxDesigner_ContextMenu_MoveUp);
            m_MoveUp_MenuItem.Icon = UriResources.Images.Up16;
            contextMenu.AddMenuItem(m_MoveUp_MenuItem);
            m_MoveUp_MenuItem.ItemClick += new EventHandler(MoveUp_ItemClick);

            m_MoveDown_MenuItem = new ContextMenuItem(Localization.MdxDesigner_ContextMenu_MoveDown);
            m_MoveDown_MenuItem.Icon = UriResources.Images.Down16;
            contextMenu.AddMenuItem(m_MoveDown_MenuItem);
            m_MoveDown_MenuItem.ItemClick += new EventHandler(MoveDown_ItemClick);

            //contextMenu.AddMenuSplitter();

            item = new ContextMenuItem(Localization.MdxDesigner_ContextMenu_RemoveField);
            item.Icon = UriResources.Images.RemoveHot16;
            contextMenu.AddMenuItem(item);
            item.ItemClick += new EventHandler(Remove_ItemClick);

            // Используем открытие и закрытие меню для работы с тултипом
            contextMenu.PopupControl.Closed -= new EventHandler(PopupControl_Closed);
            contextMenu.PopupControl.Closed += new EventHandler(PopupControl_Closed);
            contextMenu.PopupControl.Opened -= new EventHandler(PopupControl_Opened);
            contextMenu.PopupControl.Opened += new EventHandler(PopupControl_Opened);
            return contextMenu;
        }

        void PopupControl_Opened(object sender, EventArgs e)
        {
            // Прячем тултип
            TooltipManager.IsPaused = true;
        }

        void PopupControl_Closed(object sender, EventArgs e)
        {
            // Возвращаем тултип
            TooltipManager.IsPaused = false;
        }

        void MoveUp_ItemClick(object sender, EventArgs e)
        {
            Raise_MoveUp();
        }

        void MoveDown_ItemClick(object sender, EventArgs e)
        {
            Raise_MoveDown();
        }

        void Remove_ItemClick(object sender, EventArgs e)
        {
            Raise_Remove();
        }
        
        #endregion Контекстное меню
    }
}
