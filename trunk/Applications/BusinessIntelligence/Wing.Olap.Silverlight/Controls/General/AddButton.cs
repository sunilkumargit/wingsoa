/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System.Windows.Controls;

namespace Wing.Olap.Controls.General
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
