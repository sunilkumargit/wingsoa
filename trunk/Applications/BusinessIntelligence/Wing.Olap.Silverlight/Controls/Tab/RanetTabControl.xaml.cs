/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Wing.Olap.Controls.Tab
{
    public partial class RanetTabControl : UserControl
    {
        public RanetTabControl()
        {
            InitializeComponent();
            tabCtrl.ApplyTemplate();
        }

        public TabControlEx TabCtrl
        {
            get {
                return tabCtrl;
            }
        }

        public TabToolBar ToolBar
        {
            get {
                return TabCtrl.GetControlFromTemplate("ToolBar") as TabToolBar;
            }
        }
    }
}
