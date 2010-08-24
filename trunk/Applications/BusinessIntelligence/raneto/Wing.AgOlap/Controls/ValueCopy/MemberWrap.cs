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

namespace Ranet.AgOlap.Controls.ValueCopy
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
