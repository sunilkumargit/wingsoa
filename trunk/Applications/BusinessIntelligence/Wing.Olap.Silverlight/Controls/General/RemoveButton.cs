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

namespace Wing.Olap.Controls.General
{
    public class RemoveButton : HotButton
    {
        public RemoveButton() 
        {
            this.Height = 22;
            this.Width = 22;

            LeaveImage = CreateImage();
            LeaveImage.Source = UriResources.Images.Remove16;

            EnterImage = CreateImage();
            EnterImage.Source = UriResources.Images.RemoveHot16;

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
