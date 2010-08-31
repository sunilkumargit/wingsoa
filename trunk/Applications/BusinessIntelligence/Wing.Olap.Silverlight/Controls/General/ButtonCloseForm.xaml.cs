/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System.Windows;
using System.Windows.Controls;

namespace Wing.Olap.Controls.General
{
    public partial class ButtonCloseForm : UserControl
	{
		public ButtonCloseForm()
		{
			// Required to initialize variables
			InitializeComponent();

            ToolTipService.SetToolTip(this, Localization.Dialog_CloseButton_ToolTip);
		}

        public event RoutedEventHandler Click;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            RoutedEventHandler handler = Click;
            if (handler != null)
            {
                handler(this, e);
            }
        }
	}
}