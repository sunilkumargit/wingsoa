/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using Wing.Olap.Core.Metadata;

namespace Wing.Olap.Controls.MdxDesigner.Wrappers
{
    public class Level_AreaItemWrapper : Filtered_AreaItemWrapper
    {
        String m_UniqueName = String.Empty;
        public String UniqueName
        {
            get { return m_UniqueName; }
            set { m_UniqueName = value; }
        }

        String m_HierarchyUniqueName = String.Empty;
        public String HierarchyUniqueName
        {
            get { return m_HierarchyUniqueName; }
            set { m_HierarchyUniqueName = value; }
        }

        public Level_AreaItemWrapper()
        {
        }

        public Level_AreaItemWrapper(LevelInfo info)
            : base(info)
        {
            UniqueName = info.UniqueName;
            HierarchyUniqueName = info.ParentHirerachyId;
        }
    }
}
