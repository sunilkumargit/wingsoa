/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System.Windows.Controls;

namespace Wing.Client.Sdk.Controls
{
    public class HotButton : Button
    {
        public HotButton()
        {
            DefaultStyleKey = typeof(HotButton);
            this.Height = 22;
            this.Width = 22;
        }
    }

}
