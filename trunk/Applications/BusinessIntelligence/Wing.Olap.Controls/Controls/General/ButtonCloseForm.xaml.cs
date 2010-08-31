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

namespace Wing.AgOlap.Controls.General
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