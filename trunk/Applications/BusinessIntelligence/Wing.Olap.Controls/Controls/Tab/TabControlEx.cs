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

namespace Wing.AgOlap.Controls.Tab
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
