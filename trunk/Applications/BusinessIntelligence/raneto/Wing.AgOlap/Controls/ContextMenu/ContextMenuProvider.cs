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
using System.Net;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Ranet.AgOlap.Controls.ContextMenu
{
    /// <summary>
    /// Класс содержит extension-методы для централизованного подключения
    /// контекстного меню. 
    /// </summary>
    public static class ContextMenuProvider
    {
        // Для подключения нужно в aspx: 		
        //  <param name="enableHtmlAccess" value="true" />
        //  <param name="windowless" value="true" />
        // и в Application_Startup:
        //  ContextMenuProvider.EnableContextMenuSupport(this);

        /// <summary>
        /// Делегат, описывающий метод, возвращающий текущее контекстное меню для контрола.
        /// </summary>
        /// <returns></returns>
        public delegate Ranet.AgOlap.Controls.ContextMenu.CustomContextMenu GetContextMenuDelegate(Point point);

        /// <summary>
        /// Attached-свойство, используемое для назначения контекстного меню контролу.
        /// </summary>
        public static readonly DependencyProperty ContextMenuProperty =
            DependencyProperty.RegisterAttached("ContextMenu",
            typeof(GetContextMenuDelegate),
            typeof(ContextMenuProvider),
            null);


        /// <summary>
        /// Возвращает контекстное меню контрола. Если контекстное меню не назначено - возвращает null.
        /// </summary>
        /// <param name="element">Контрол.</param>
        /// <returns></returns>
        public static GetContextMenuDelegate GetContextMenu(UIElement element)
        {
            return (GetContextMenuDelegate)element.GetValue(ContextMenuProperty);
        }

        /// <summary>
        /// Устанавливает контестное меню у контрола.
        /// </summary>
        /// <param name="element">Контрол.</param>
        /// <param name="getMenu">Метод, возвращающий текущее контекстное меню для контрола.</param>
        public static void SetContextMenu(UIElement element, GetContextMenuDelegate getMenu)
        {
            element.SetValue(ContextMenuProperty, getMenu);
        }

        /// <summary>
        /// Присваивает контекстное меню контролу.
        /// </summary>
        /// <param name="element">Контрол.</param>
        /// <param name="getMenu">Метод, возвращающий текущее контекстное меню для контрола.</param>
        /// <param name="mode">Режим отображения контекстного меню.</param>
        public static void AttachContextMenu(
            this UIElement element,
            GetContextMenuDelegate getMenu,
            ContextMenuSupportMode mode)
        {
            SetContextMenu(element, getMenu);
            if ((mode & ContextMenuSupportMode.OnRightMouseButtonClick) != ContextMenuSupportMode.None)
            {
                //element.MouseEnter += element_MouseEnter;
                //element.MouseLeave += element_MouseLeave;
                MouseRightClick.Instance.RightClick -= Instance_RightClick;
                MouseRightClick.Instance.RightClick += Instance_RightClick;
            }
            if ((mode & ContextMenuSupportMode.OnCtrlLeftMouseButtonClick) != ContextMenuSupportMode.None)
            {
                element.MouseLeftButtonDown += element_MouseLeftButtonDown;
            }
        }

        static void Instance_RightClick(object sender, MouseRightClickEventArgs e)
        {
            foreach (UIElement element in e.GetElementsInPosition(Application.Current.RootVisual))
            {
                var getMenu = GetContextMenu(element);
                if (getMenu == null) continue;

                HideCurrentMenu();
                var menu = getMenu(e.Position);
                if (menu != null)
                {
                    m_CurrentMenu = menu;
                    menu.LayoutUpdated += menu_LayoutUpdated;
                    menu.SetLocation(e.Position);
                    menu.IsDropDownOpen = true;
                }
                break;
            }
        }

        static void menu_LayoutUpdated(object sender, EventArgs e)
        {
            m_CurrentMenu.LayoutUpdated -= menu_LayoutUpdated;
            m_CurrentMenu.SetLocation(GetMenuPosition(m_CurrentMenu, m_CurrentMenu.GetLocation()));
        }

        static Point GetMenuPosition(FrameworkElement menu, Point point)
        {
            var deltaX = 0.0;
            var deltaY = 0.0;
            var right = point.X + menu.ActualWidth;
            var bottom = point.Y + menu.ActualHeight;
            var content = Application.Current.Host.Content;

            if (right > content.ActualWidth)
            {
                deltaX = right - content.ActualWidth;
            }
            if (bottom > content.ActualHeight)
            {
                deltaY = bottom - content.ActualHeight;
            }

            return new Point(point.X - deltaX, point.Y - deltaY);
        }

        /// <summary>
        /// Присваивает контекстное меню контролу.
        /// </summary>
        /// <param name="element">Контрол.</param>
        /// <param name="getMenu">Метод, возвращающий текущее контекстное меню для контрола.</param>
        public static void AttachContextMenu(
            this UIElement element,
            GetContextMenuDelegate getMenu)
        {
            AttachContextMenu(element, getMenu, ContextMenuSupportMode.All);
        }

        /// <summary>
        /// Удаляет контекстное меню у контрола.
        /// </summary>
        /// <param name="element">Контрол.</param>
        public static void DetachContextMenu(this UIElement element)
        {
            SetContextMenu(element, null);
        }


        /// <summary>
        /// Включает централизованную обработку контекстных меню в приложении.
        /// </summary>
        /// <param name="app"></param>
        public static void EnableContextMenuSupport(this Application app)
        {
            if (app.RootVisual == null)
            {
                throw new Exception("RootVisual element must be set before enabling context menu support");
            }

            app.RootVisual.MouseLeftButtonDown += app_HideContexMenu;
        }

        static void app_HideContexMenu(object sender, MouseButtonEventArgs e)
        {
            HideCurrentMenu();
        }

        static void HideCurrentMenu()
        {
            if (m_CurrentMenu != null)
            {
                m_CurrentMenu.IsDropDownOpen = false;
                m_CurrentMenu = null;
            }
        }

        static CustomContextMenu m_CurrentMenu;

        static void element_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.None)
            {
                UIElement element = (UIElement)sender;
                var getMenu = GetContextMenu(element);
                if (getMenu != null)
                {
                    Point p = e.GetPosition(null);
                    var menu = getMenu(p);
                    HideCurrentMenu();
                    if (menu != null)
                    {
                        m_CurrentMenu = menu;
                        menu.SetLocation(p);
                        menu.IsDropDownOpen = true;
                        e.Handled = true;
                    }
                }
            }
        }

        //static void element_MouseLeave(object sender, MouseEventArgs e)
        //{
        //    if (System.Windows.Browser.HtmlPage.IsEnabled)
        //    {
        //        HtmlPage.Document.DetachEvent("oncontextmenu", new EventHandler<HtmlEventArgs>(ContentMenu_EventHandler));
        //    }
        //}

        //static void element_MouseEnter(object sender, MouseEventArgs e)
        //{
        //    if (System.Windows.Browser.HtmlPage.IsEnabled)
        //    {
        //        HtmlPage.Document.AttachEvent("oncontextmenu", new EventHandler<HtmlEventArgs>(ContentMenu_EventHandler));
        //    }
        //}

        //static void ContentMenu_EventHandler(object sender, HtmlEventArgs e)
        //{
        //    e.PreventDefault();
        //    e.StopPropagation();
        //    var p = new Point(e.OffsetX, e.OffsetY);
        //    foreach (UIElement element in
        //        VisualTreeHelper.FindElementsInHostCoordinates(p, Application.Current.RootVisual))
        //    {
        //        var getMenu = GetContextMenu(element);
        //        if (getMenu == null) continue;

        //        HideCurrentMenu();
        //        var menu = getMenu(p);
        //        if (menu != null)
        //        {
        //            m_CurrentMenu = menu;
        //            menu.SetLocation(p);
        //            menu.IsDropDownOpen = true;
        //        }
        //        break;
        //    }
        //}
    }

    /// <summary>
    /// Режим отображения контекстного меню.
    /// </summary>
    [Flags]
    public enum ContextMenuSupportMode
    {
        /// <summary>
        /// Не отображать.
        /// </summary>
        None = 0x00,

        /// <summary>
        /// Отображать по правому клику мыши (если доступен html bridge).
        /// </summary>
        OnRightMouseButtonClick = 0x01,

        /// <summary>
        /// Отображать по нажатию Ctrl+левый клик мыши.
        /// </summary>
        OnCtrlLeftMouseButtonClick = 0x02,

        /// <summary>
        /// Отображать по правому клику мыши (если доступен html bridge) и
        /// по нажатию Ctrl+левый клик мыши.
        /// </summary>
        All = OnCtrlLeftMouseButtonClick | OnRightMouseButtonClick
    }
}
