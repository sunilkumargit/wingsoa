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
using Wing.AgOlap.Controls.Buttons;
using Wing.AgOlap.Controls.General;

namespace Wing.AgOlap.Controls.ValueCopy
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
