/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Media;

namespace Wing.Olap.Controls.ContextMenu
{
    public class MouseRightClick
    {
        private MouseRightClick()
        {
            this.Initialize();
        }

        public event EventHandler<MouseRightClickEventArgs> RightClick;

        public void OnContextMenu(string xStr, string yStr)
        {
            double num = Convert.ToDouble(xStr, CultureInfo.InvariantCulture);
            double num2 = Convert.ToDouble(yStr, CultureInfo.InvariantCulture);
            Point pluginPosition = this.GetPluginPosition();
            this.OnRightClick(new Point(num - pluginPosition.X, num2 - pluginPosition.Y));
        }

        private void OnRightClick(Point cp)
        {
            var rightClick = this.RightClick;
            if (rightClick != null)
            {
                var mce = new MouseRightClickEventArgs(cp);
                rightClick(this, mce);
            }
        }

        private Point GetPluginPosition()
        {
            Point point = new Point(0.0, 0.0);
            try
            {
                for (HtmlElement element = HtmlPage.Plugin; element != null; element = (HtmlElement)element.GetProperty("offsetParent"))
                {
                    point.X += (double)element.GetProperty("offsetLeft");
                    point.Y += (double)element.GetProperty("offsetTop");
                }
            }
            catch
            {
            }
            return point;
        }

        private bool m_IsInjected = false;

        private void MouseDownHandler(object sender, HtmlEventArgs e)
        {
            if (e.MouseButton == MouseButtons.Right)
            {
            }
        }

        private static MouseRightClick m_Instance;
        public static MouseRightClick Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new MouseRightClick();
                }

                return m_Instance;
            }
        }

        private void Initialize()
        {
        }

    }
}
