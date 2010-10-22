/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using Wing.Olap.Core.Metadata;

namespace Wing.Olap.Controls.MdxDesigner.Wrappers
{
    public class Measure_AreaItemWrapper : AreaItemWrapper
    {
        String m_UniqueName = String.Empty;
        public String UniqueName
        {
            get { return m_UniqueName; }
            set { m_UniqueName = value; }
        }

        public Measure_AreaItemWrapper()
        {
        }

        public Measure_AreaItemWrapper(MeasureInfo info)
            : base(info)
        {
            UniqueName = info.UniqueName;
        }
    }
}