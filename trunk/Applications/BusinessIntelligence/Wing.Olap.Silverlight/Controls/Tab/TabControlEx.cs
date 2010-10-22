/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Windows;
using System.Windows.Controls;

namespace Wing.Olap.Controls.Tab
{
    public class TabControlEx : TabControl
    {
        public TabControlEx()
        {
            DefaultStyleKey = typeof(TabControlEx);
        }

        public FrameworkElement GetControlFromTemplate(String nameFromTemplate)
        {
            return base.GetTemplateChild(nameFromTemplate) as FrameworkElement;
        }
    }
}
