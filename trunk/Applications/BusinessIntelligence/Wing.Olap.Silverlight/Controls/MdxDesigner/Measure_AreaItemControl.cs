/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System.Windows.Media.Imaging;
using Wing.Olap.Controls.MdxDesigner.Wrappers;

namespace Wing.Olap.Controls.MdxDesigner
{
    public class Measure_AreaItemControl : InfoItemControl
    {
        public Measure_AreaItemControl(Measure_AreaItemWrapper wrapper)
            : base(wrapper)
        {

        }

        public Measure_AreaItemControl(Measure_AreaItemWrapper wrapper, BitmapImage icon)
            : base(wrapper, icon)
        {

        }

        public new Measure_AreaItemWrapper Measure
        {
            get { return Wrapper as Measure_AreaItemWrapper; }
        }
    }
}