/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System.Windows.Media.Imaging;
using Wing.Olap.Controls.MdxDesigner.Wrappers;

namespace Wing.Olap.Controls.MdxDesigner
{
    public class CalculateNamedSet_AreaItemControl : InfoItemControl
    {
        public CalculateNamedSet_AreaItemControl(CalculatedNamedSet_AreaItemWrapper wrapper)
            : base(wrapper)
        {

        }

        public CalculateNamedSet_AreaItemControl(CalculatedNamedSet_AreaItemWrapper wrapper, BitmapImage icon)
            : base(wrapper, icon)
        {

        }

        public CalculatedNamedSet_AreaItemWrapper CalculatedNamedSet
        {
            get { return Wrapper as CalculatedNamedSet_AreaItemWrapper; }
        }
    }
}
