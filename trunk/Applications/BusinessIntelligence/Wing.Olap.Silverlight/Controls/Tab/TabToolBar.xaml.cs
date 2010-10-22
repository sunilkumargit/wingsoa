/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System.Windows.Controls;

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