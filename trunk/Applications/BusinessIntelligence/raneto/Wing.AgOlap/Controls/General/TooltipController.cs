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
using Ranet.AgOlap.Controls.PivotGrid.Controls;

namespace Ranet.AgOlap.Controls.General
{
    public class TooltipController
    {
        UIElement m_Element = null;
        Storyboard m_TooltipTimer = null;
        Storyboard m_AutoHideTimer = null;
        ToolTip m_ToolTip = null;

        object m_ToolTipContent = null;
        public object ToolTipContent
        {
            get { return m_ToolTipContent; }
            set 
            {
                m_ToolTipContent = value;
                m_ToolTip.Content = value;
            }
        }

        public TooltipController(UIElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");
            m_Element = element;

            element.MouseEnter += new MouseEventHandler(element_MouseEnter);
            element.MouseLeave += new MouseEventHandler(element_MouseLeave);
            element.MouseMove += new MouseEventHandler(element_MouseMove);

            m_AutoHideTimer = new Storyboard();
            m_AutoHideTimer.Duration = new Duration(new TimeSpan(0, 0, 0, 5, 0));
            m_AutoHideTimer.Completed += new EventHandler(m_AutoHideTimer_Completed);

            m_TooltipTimer = new Storyboard();
            m_TooltipTimer.Duration = new Duration(new TimeSpan(0, 0, 0, 1, 0));
            m_TooltipTimer.Completed += new EventHandler(m_TooltipTimer_Completed);

            m_ToolTip = new ToolTip();
            m_ToolTip.Padding = new Thickness(0);
            m_ToolTip.Opened += new RoutedEventHandler(m_ToolTip_Opened);
            m_ToolTip.Closed += new RoutedEventHandler(m_ToolTip_Closed);

            //m_ToolTip.VerticalOffset = 10;
            //m_ToolTip.HorizontalOffset = 10;
        }

        #region Таймер автозакрытия
        void m_ToolTip_Closed(object sender, RoutedEventArgs e)
        {
            m_AutoHideTimer.Stop();
        }

        void m_AutoHideTimer_Completed(object sender, EventArgs e)
        {
            m_ToolTip.IsOpen = false;
        }

        void m_ToolTip_Opened(object sender, RoutedEventArgs e)
        {
            m_AutoHideTimer.Begin();
        }
        #endregion Таймер автозакрытия

        public event EventHandler<CustomEventArgs<Point>> BeforeOpen;

        void m_TooltipTimer_Completed(object sender, EventArgs e)
        {
            EventHandler<CustomEventArgs<Point>> handler = BeforeOpen;
            if (handler != null)
            {
                CustomEventArgs<Point> args = new CustomEventArgs<Point>(m_CurrentPosition);
                handler(this, args);
                // Пользователь отменил отображение тултипа
                if (args.Cancel)
                {
                    m_ToolTip.IsOpen = false;
                    return;
                }
            }

            if (m_ToolTip.Content != null)
            {
                m_ToolTip.IsOpen = true;
                return;
            }
            m_ToolTip.IsOpen = false;
        }

        Point m_CurrentPosition = new Point(0, 0);

        void element_MouseMove(object sender, MouseEventArgs e)
        {
            if (!IsPaused)
            {
                m_ToolTip.IsOpen = false;
                m_CurrentPosition = e.GetPosition(null);
                m_TooltipTimer.Stop();
                m_TooltipTimer.Begin();
            }
        }

        void element_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!IsPaused)
            {
                m_TooltipTimer.Stop();
            }
        }

        public void Restart()
        {
            m_TooltipTimer.Stop();
            m_TooltipTimer.Begin();
            m_IsPaused = false;
        }

        void element_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!IsPaused)
            {
                m_TooltipTimer.Stop();
                m_TooltipTimer.Begin();
            }
        }

        public void Hide()
        {
            if (!IsPaused)
            {
                m_ToolTip.IsOpen = false;
                m_TooltipTimer.Stop();
            }
        }

        bool m_IsPaused = false;
        public bool IsPaused
        {
            get { return m_IsPaused; }
            set
            {
                m_IsPaused = value;
                if (value)
                {
                    m_ToolTip.IsOpen = false;
                    m_TooltipTimer.Stop();
                }
            }
        }
    }
}
