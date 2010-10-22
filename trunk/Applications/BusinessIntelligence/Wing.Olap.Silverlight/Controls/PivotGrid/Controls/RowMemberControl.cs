/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System.Windows;
using System.Windows.Media.Imaging;
using Wing.Olap.Core.Providers;

namespace Wing.Olap.Controls.PivotGrid.Controls
{
    public class RowMemberControl : MemberControl
    {
        public RowMemberControl(PivotGridControl owner, MemberInfo info, int drillDownDepth) 
            : base(owner, info)
        {
            if (owner.RowsViewMode == ViewModeTypes.Tree)
                Margin = new Thickness(drillDownDepth * owner.DRILLDOWN_SPACE_WIDTH * Scale, 0, 0, 0);

//            Border border = LayoutRoot;
//            border.BorderThickness = new Thickness(0, 0, 1 + MembersAreaContol.SPLITTER_SIZE, 1 + MembersAreaContol.SPLITTER_SIZE);

            //if(Member.DrilledDownChildren.Count > 0)
            //{
            //    RootPanel.VerticalAlignment = VerticalAlignment.Top;
            //}

            if (drillDownDepth > 0)
            {
                ShowLeftBorder = true;
            }
        }

        protected override bool IsInteractive
        {
            get
            {
                return Owner.Rows_IsInteractive;
            }
        }

        protected override bool UseHint
        {
            get
            {
                return Owner.Rows_UseHint;
            }
        }

        protected override BitmapImage SortAsc_Image
        {
            get { return UriResources.Images.RowSortAsc16; }
        }

        protected override BitmapImage SortDesc_Image
        {
            get { return UriResources.Images.RowSortDesc16; }
        }
    }
}
