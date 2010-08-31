/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System.Windows;
using System.Windows.Controls;

namespace Wing.Olap.Controls.General
{
    public partial class CustomPanel : UserControl
    {
        public new UIElement Content
        {
            get
            {
                if (grdContent.Children.Count > 0)
                    return grdContent.Children[0];
                return null;
            }
            set
            {
                grdContent.Children.Clear();
                grdContent.Children.Add(value);
            }
        }

        public CustomPanel()
        {
            InitializeComponent();
        }

        public new bool IsEnabled
        {
            get { return base.IsEnabled; }
            set
            {
                if (value)
                {
                    brdEnabled.Visibility = Visibility.Collapsed;
                }
                else
                {
                    brdEnabled.Visibility = Visibility.Visible;
                }
                base.IsEnabled = value;
            }
        }
    }
}
