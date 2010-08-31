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
    public enum KpiControlType
    { 
        Goal,
        Value,
        Trend,
        Status
    }

    public class Kpi_AreaItemControl : InfoItemControl
    {
        public readonly KpiControlType Type = KpiControlType.Value;

        public Kpi_AreaItemControl(Kpi_AreaItemWrapper wrapper)
            : this(wrapper, null)
        {
            
        }

        public Kpi_AreaItemControl(Kpi_AreaItemWrapper wrapper, BitmapImage icon)
            : base(wrapper, icon)
        {
            Type = wrapper.Type;
        }

        public Kpi_AreaItemWrapper Kpi
        {
            get { return Wrapper as Kpi_AreaItemWrapper; }
        }
    }
}
