/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using Wing.Olap.Controls.Buttons;
using Wing.Olap.Controls.General;

namespace Wing.Olap.Controls.ValueCopy
{
    public class DeleteButton : RanetHotButton
    {
        public DeleteButton()
        {
            this.Content = UiHelper.CreateIcon(UriResources.Images.RemoveHot16);
            this.Height = 18;
            this.Width = 18;
        }
    }
}
