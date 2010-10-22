/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using Wing.Olap.Core.Providers;

namespace Wing.Olap.Controls.PivotGrid.Layout
{
    public class MemberLayoutItem : LayoutItem
    {
        public readonly PivotMemberItem PivotMember = null;

        public MemberLayoutItem(PivotMemberItem pivotMember)
        {
            if (pivotMember == null)
                throw new ArgumentNullException("pivotMember");
            PivotMember = pivotMember;
        }
    }
}
