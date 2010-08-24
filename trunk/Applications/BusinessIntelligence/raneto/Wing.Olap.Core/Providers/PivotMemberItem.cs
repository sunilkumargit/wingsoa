/*   
    Copyright (C) 2009 Galaktika Corporation ZAO

    This file is a part of Ranet.UILibrary.Olap
 
    Ranet.UILibrary.Olap is a free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
      
    You should have received a copy of the GNU General Public License
    along with Ranet.UILibrary.Olap.  If not, see <http://www.gnu.org/licenses/>.
  
    If GPL v.3 is not suitable for your products or company,
    Galaktika Corp provides Ranet.UILibrary.Olap under a flexible commercial license
    designed to meet your specific usage and distribution requirements.
    If you have already obtained a commercial license from Galaktika Corp,
    you can use this file under those license terms.
*/

using System;
using System.Net;
using System.Collections.Generic;

namespace Ranet.Olap.Core.Providers
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
