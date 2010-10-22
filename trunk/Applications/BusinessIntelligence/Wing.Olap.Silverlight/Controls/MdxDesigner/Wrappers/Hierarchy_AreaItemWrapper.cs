/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using Wing.Olap.Core.Metadata;

namespace Wing.Olap.Controls.MdxDesigner.Wrappers
{
    public class Hierarchy_AreaItemWrapper : Filtered_AreaItemWrapper
    {
        String m_UniqueName = String.Empty;
        public String UniqueName
        {
            get { return m_UniqueName; }
            set { m_UniqueName = value; }
        }

        public Hierarchy_AreaItemWrapper()
        {
        }

        public Hierarchy_AreaItemWrapper(HierarchyInfo info)
            : base(info)
        {
            UniqueName = info.UniqueName;
        }
    }
}
