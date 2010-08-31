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
using Wing.AgOlap.Controls.PivotGrid.Data;
using Wing.Olap.Core.Providers;

namespace Wing.AgOlap.Controls.ValueCopy
{
    public class MemberWrap
    {
        String m_UniqueName = String.Empty;
        public String UniqueName
        {
            get { return m_UniqueName; }
            set { m_UniqueName = value; }
        }

        String m_Caption = String.Empty;
        public String Caption
        {
            get { return m_Caption; }
            set { m_Caption = value; }
        }

        String m_HierarchyUniqueName = String.Empty;
        public String HierarchyUniqueName
        {
            get { return m_HierarchyUniqueName; }
            set { m_HierarchyUniqueName = value; }
        }

        bool m_IsUnknownMember = false;
        public bool IsUnknownMember
        {
            get { return m_IsUnknownMember; }
            set { m_IsUnknownMember = value; }
        }

        bool m_IsDefaultMember = false;
        public bool IsDefaultMember
        {
            get { return m_IsDefaultMember; }
            set { m_IsDefaultMember = value; }
        }

        public MemberWrap() { }
        public MemberWrap(MemberInfo member) 
        {
            if (member == null)
                throw new ArgumentNullException("member");
            UniqueName = member.UniqueName;
            Caption = member.Caption;
            HierarchyUniqueName = member.HierarchyUniqueName;
        }
    }
}
