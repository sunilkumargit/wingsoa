/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Wing.Olap.Features
{

    /// <summary>
    /// Utility class for adding mouse wheel scrolling to ScrollBar controls
    /// </summary>
    public class ScrollBarMouseWheelSupport
    {
        public bool IsHorizontal { get; set; }

        public bool ScrollAlways
        {
            get
            {
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
            if (mouseWheelHelper != null)
            {
                try
                {
                    //mouseWheelHelper.RemoveWheelSupport(scrollBar);
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

