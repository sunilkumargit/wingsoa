/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Windows;

namespace Wing.Olap.Features
{
    internal class CustomMouseWheelSupport
    {
        public bool ScrollAlways { get; set; }
        private static Point currentMousePosition;

        public event EventHandler<MouseWheelEventArgs> MouseWheelMoved;

        public FrameworkElement ElementToAddMouseWheelSupportTo { get; private set; }

        public CustomMouseWheelSupport(FrameworkElement elementToAddMouseWheelSupportTo, FrameworkElement parentElementWithMouseWheelSupport)
        {
            ElementToAddMouseWheelSupportTo = elementToAddMouseWheelSupportTo;
        }

        bool IsActive()
        {
            if (ElementToAddMouseWheelSupportTo != null)
            {
                if (ScrollAlways)
                    return true;
            }

            return false;
        }
    }
}
