/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Collections.Generic;

namespace Wing.Olap.Core.Providers
{
    public class PivotMemberItem
    {
        public readonly MemberInfo Member;

        public PivotMemberItem(MemberInfo member)
        {
            if (member == null)
                throw new ArgumentNullException("member");

            Member = member;
        }

        public int RowSpan = 1;
        public int ColumnSpan = 1;
        public int PivotDrillDepth = 0;
        public int ColumnIndex = 0;
        public int RowIndex = 0;
        public bool IsFirstDrillDownChild = false;

        public int ChildrenSize = 0;
        public int DrillDownChildrenSize = 0;

        List<PivotMemberItem> m_Children = new List<PivotMemberItem>();
        public List<PivotMemberItem> Children
        {
            get {
                return m_Children;
            }
        }

        List<PivotMemberItem> m_DrillDownChildren = new List<PivotMemberItem>();
        public List<PivotMemberItem> DrillDownChildren
        {
            get
            {
                return m_DrillDownChildren;
            }
        }
    }
}
