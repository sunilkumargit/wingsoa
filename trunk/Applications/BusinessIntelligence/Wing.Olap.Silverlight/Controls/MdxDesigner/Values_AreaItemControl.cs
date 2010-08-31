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

namespace Wing.Olap.Controls.MdxDesigner
{
    public class Values_AreaItemControl : AreaItemControl
    {
        public Values_AreaItemControl()
            : base (UriResources.Images.DataArea16, Localization.MdxDesigner_ValuesNode_Caption)
        {
            UseDragDrop = true;
        }
    }
}
