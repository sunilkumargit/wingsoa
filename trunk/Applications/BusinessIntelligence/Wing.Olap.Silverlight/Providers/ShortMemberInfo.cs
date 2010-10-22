using System;

namespace Wing.Olap.Providers
{
    public class ShortMemberInfo
    {
        String m_HierarchyUniqueName = String.Empty;
        public String HierarchyUniqueName
        {
            get { return m_HierarchyUniqueName; }
            set { m_HierarchyUniqueName = value; }
        }

        String m_UniqueName = String.Empty;
        public String UniqueName
        {
            get { return m_UniqueName; }
            set { m_UniqueName = value; }
        }

        public ShortMemberInfo()
        { }

        public ShortMemberInfo(String hierarchyUniqueName, String uniqueName)
        {
            m_HierarchyUniqueName = hierarchyUniqueName;
            m_UniqueName = uniqueName;
        }
    }
}
