/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Windows.Controls;

namespace Wing.Olap.Controls.General.Tree
{
    public partial class WaitTreeControl : UserControl
    {
        public WaitTreeControl()
        {
            InitializeComponent();
        }

        public String Text
        {
            set
            {
                ItemText.Text = value;
            }
            get
            {
                return ItemText.Text;
            }
        }

    }
}
