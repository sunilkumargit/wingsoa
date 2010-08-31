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
using System.Windows.Media.Imaging;

namespace Wing.AgOlap.Controls.Combo
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
