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

namespace Ranet.AgOlap.Controls.PivotGrid.Controls
{
    public class ColumnMemberControl : MemberControl
    {
        public ColumnMemberControl(PivotGridControl owner, MemberInfo info, int drillDownDepth)
            : base(owner, info)
        {
            if(owner.ColumnsViewMode == ViewModeTypes.Tree)
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
