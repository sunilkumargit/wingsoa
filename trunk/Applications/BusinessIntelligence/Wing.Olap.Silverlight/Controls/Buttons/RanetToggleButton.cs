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
using System.Windows.Controls.Primitives;

namespace Wing.Olap.Controls.Buttons
{
	public class RanetToggleButton : ToggleButton
	{
		internal int ButtonId;

		public RanetToggleButton()
		{
			DefaultStyleKey = typeof(RanetToggleButton);
			this.Height = 22;
			this.Width = 22;
			this.Margin = new Thickness(2, 0, 0, 0);
		}
	}
}
