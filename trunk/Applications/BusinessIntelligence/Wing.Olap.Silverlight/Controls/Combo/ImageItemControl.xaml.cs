/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Wing.Olap.Controls.Combo
{
    public partial class ImageItemControl : UserControl
    {
        public ImageItemControl()
        {
            InitializeComponent();
        }

        public ImageItemControl(BitmapImage image, String text)
            : this()
        {
            this.Image = image;
            ImageCtrl.Source = image;
            TextCtrl.Text = text;
        }

        public readonly BitmapImage Image;
    }
}
