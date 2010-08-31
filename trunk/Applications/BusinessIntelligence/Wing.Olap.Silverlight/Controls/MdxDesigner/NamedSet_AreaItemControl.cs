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
using Wing.AgOlap.Controls.MdxDesigner.Wrappers;

namespace Wing.AgOlap.Controls.MdxDesigner
{
    public class NamedSet_AreaItemControl : InfoItemControl
    {
        public NamedSet_AreaItemControl(NamedSet_AreaItemWrapper wrapper)
            : base(wrapper)
        {

        }

        public NamedSet_AreaItemControl(NamedSet_AreaItemWrapper wrapper, BitmapImage icon)
            : base(wrapper, icon)
        {

        }

        public NamedSet_AreaItemWrapper NamedSet
        {
            get { return Wrapper as NamedSet_AreaItemWrapper; }
        }
    }
}


