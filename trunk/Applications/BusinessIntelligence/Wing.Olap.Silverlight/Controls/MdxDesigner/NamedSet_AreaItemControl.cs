/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System.Windows.Media.Imaging;
using Wing.Olap.Controls.MdxDesigner.Wrappers;

namespace Wing.Olap.Controls.MdxDesigner
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


