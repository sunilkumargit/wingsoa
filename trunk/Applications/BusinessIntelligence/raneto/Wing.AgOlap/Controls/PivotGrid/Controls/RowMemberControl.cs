/*   
    Copyright (C) 2009 Galaktika Corporation ZAO

    This file is a part of Ranet.UILibrary.Olap
 
    Ranet.UILibrary.Olap is a free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
      
    You should have received a copy of the GNU General Public License
    along with Ranet.UILibrary.Olap.  If not, see
  	<http://www.gnu.org/licenses/> 
  
    If GPL v.3 is not suitable for your products or company,
    Galaktika Corp provides Ranet.UILibrary.Olap under a flexible commercial license
    designed to meet your specific usage and distribution requirements.
    If you have already obtained a commercial license from Galaktika Corp,
    you can use this file under those license terms.
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
using Ranet.AgOlap.Controls.PivotGrid.Data;
using Ranet.Olap.Core.Providers;
using System.Windows.Media.Imaging;

namespace Ranet.AgOlap.Controls.PivotGrid.Controls
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
