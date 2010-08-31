/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
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
using System.Windows.Browser;
using System.Collections.Generic;
using Wing.Olap.Controls.General;

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
