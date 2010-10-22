/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System.Windows;
using Wing.Olap.Core.Providers;

namespace Wing.Olap.Controls.PivotGrid.Controls
{
    public class ColumnMemberControl : MemberControl
    {
        public ColumnMemberControl(PivotGridControl owner, MemberInfo info, int drillDownDepth)
            : base(owner, info)
        {
            if (owner.ColumnsViewMode == ViewModeTypes.Tree)
                Margin = new Thickness(0, drillDownDepth * owner.DRILLDOWN_SPACE_HEIGHT * Scale, 0, 0);

            //            Border border = LayoutRoot;

            //            int right_Offs = Member.DrilledDownChildren.Count > 0 ? 0 : MembersAreaContol.SPLITTER_SIZE;    
            //            border.BorderThickness = new Thickness(0, 0, 1 + right_Offs, 1 + MembersAreaContol.SPLITTER_SIZE);


            if (drillDownDepth > 0)
            {
                ShowUpBorder = true;
            }
        }

        protected override bool IsInteractive
        {
            get
            {
                return Owner.Columns_IsInteractive;
            }
        }

        protected override bool UseHint
        {
            get
            {
                return Owner.Columns_UseHint;
            }
        }
    }
}
