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

namespace Wing.AgOlap.Controls.General
{
    public class AddButton : HotButton
    {
        public AddButton() 
        {
            this.Height = 22;
            this.Width = 22;

            LeaveImage = CreateImage();
            LeaveImage.Source = UriResources.Images.Add16;

            EnterImage = CreateImage();
            EnterImage.Source = UriResources.Images.AddHot16;

            this.Content = LeaveImage;
        }

        Image CreateImage()
        {
            Image image = new Image();
            image.Height = 16;
            image.Width = 16;
            return image;
        }

    }
}
