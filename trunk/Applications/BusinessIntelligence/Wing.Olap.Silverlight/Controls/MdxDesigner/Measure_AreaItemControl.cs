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