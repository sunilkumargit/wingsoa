﻿/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Wing.Olap.Controls.General
{
    public static class UiHelper
    {
        public static Image CreateIcon(BitmapImage source)
        {
            Image image = new Image();
            image.Stretch = Stretch.None;
            image.Height = 16;
            image.Width = 16;
            image.Source = source;
            return image;
        }
    }
}
