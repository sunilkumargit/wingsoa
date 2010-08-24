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
using System.Windows.Controls.Primitives;

namespace Ranet.AgOlap.Features
{
   
    /// <summary>
    /// Utility class for adding mouse wheel scrolling to ScrollBar controls
    /// </summary>
    public class ScrollBarMouseWheelSupport
    {
        public bool IsHorizontal { get; set; } 

        public bool ScrollAlways 
        {
            get {
                if (mouseWheelHelper != null)
                    return mouseWheelHelper.ScrollAlways;
                return false; 
            }
            set
            {
                if (mouseWheelHelper != null)
                    mouseWheelHelper.ScrollAlways = value;
            }
        }

        public int m_ScrollAmount = 1;
        public int ScrollAmount
        {
            get
            {
                if (m_ScrollAmount < 1)
                    return 1;
                return m_ScrollAmount;
            }
            set
            {
                m_ScrollAmount = value;
            }
        }
        private CustomMouseWheelSupport mouseWheelHelper;

        /// <summary>
        /// Adds mouse wheel support to a <see cref="ScrollViewer"/>.
        /// As long as the <see cref="ScrollViewer"/> has focus,
        /// the mouse wheel can be used to scroll up and down.
        /// </summary>
        /// <param name="scrollViewer">The scroll viewer.</param>
        /// <returns>The <see cref="ScrollViewer"/>.</returns>
        public ScrollBar AddMouseWheelSupport(ScrollBar scrollBar)
        {
            mouseWheelHelper = new CustomMouseWheelSupport(scrollBar, null);
            mouseWheelHelper.MouseWheelMoved += new EventHandler<MouseWheelEventArgs>(mouseWheelHelper_MouseWheelMoved);
            return scrollBar;
        }

        void mouseWheelHelper_MouseWheelMoved(object source, MouseWheelEventArgs eventArgs)
        {
            // Ctrl+колесо - масштабирование в IE
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                return;
            // Shift+колесо - скроллинг по горизонтали
            if (IsHorizontal)
            {
                if ((Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.Shift)
                    return;
            }
            else
            {
                if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                    return;
            }

            CustomMouseWheelSupport support = source as CustomMouseWheelSupport;
            if (support != null)
            {
                ScrollBar scroller = support.ElementToAddMouseWheelSupportTo as ScrollBar;
                if (scroller != null)
                {
                    var delta = eventArgs.WheelDelta;

                    delta *= ScrollAmount;

                    var newOffset = scroller.Value - delta;

                    if (newOffset > scroller.Maximum)
                        newOffset = scroller.Maximum;
                    else if (newOffset < scroller.Minimum)
                        newOffset = scroller.Minimum;

                    scroller.Value = newOffset;
                }
            }
            eventArgs.BrowserEventHandled = true;
        }

        public bool RemoveMouseWheelSupport(ScrollBar scrollBar)
        {
           if (mouseWheelHelper!= null)
           {
               try
               {
                   mouseWheelHelper.RemoveWheelSupport(scrollBar);
                   mouseWheelHelper = null;
                   return true;
               }
               catch 
               {
                   return false;
               }               
           }
           return false;
        }
    }
}

