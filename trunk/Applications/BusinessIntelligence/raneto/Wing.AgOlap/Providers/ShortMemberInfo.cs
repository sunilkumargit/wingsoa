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

namespace Ranet.AgOlap.Providers
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
