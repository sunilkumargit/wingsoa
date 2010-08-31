/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Wing.Olap.Controls.Tab
{
	public partial class TabToolBar : UserControl
	{
        public TabToolBar()
		{
			// Required to initialize variables
			InitializeComponent();
		}

        public StackPanel Stack
        {
            get {
                return LayoutRoot;
            }
        }
	}
}