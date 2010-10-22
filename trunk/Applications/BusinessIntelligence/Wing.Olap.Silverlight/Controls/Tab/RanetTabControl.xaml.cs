/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System.Windows.Controls;

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
