/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Wing.Olap.Controls.ContextMenu
{
    public class MouseRightClickEventArgs : EventArgs
    {
        public Point Position { get; private set; }
        public IEnumerable<UIElement> GetElementsInPosition(UIElement relativeTo)
        {
            return VisualTreeHelper.FindElementsInHostCoordinates(this.Position, relativeTo);
        }

        public MouseRightClickEventArgs(Point cp)
        {
            this.Position = cp;
        }
    }
}
