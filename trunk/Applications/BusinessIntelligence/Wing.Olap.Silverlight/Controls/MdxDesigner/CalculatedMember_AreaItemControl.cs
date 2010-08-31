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
using Wing.Olap.Core.Metadata;
using System.Windows.Media.Imaging;
using Wing.Olap.Controls.MdxDesigner.Wrappers;

namespace Wing.Olap.Controls.MdxDesigner
{
    public class CalculatedMember_AreaItemControl : InfoItemControl
    {
        public CalculatedMember_AreaItemControl(CalculatedMember_AreaItemWrapper wrapper)
            : base(wrapper)
        {

        }

        public CalculatedMember_AreaItemControl(CalculatedMember_AreaItemWrapper wrapper, BitmapImage icon)
            : base(wrapper, icon)
        {

        }

        public CalculatedMember_AreaItemWrapper CalculatedMember
        {
            get { return Wrapper as CalculatedMember_AreaItemWrapper; }
        }
    }
}
