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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ranet.Olap.Core.Metadata
{
    public class MemberPropertyInfo
    {
        private String m_Name = String.Empty;
        public String Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        private String m_UniqueName = String.Empty;
        public String UniqueName
        {
            get { return m_UniqueName; }
            set { m_UniqueName = value; }
        }

        private object m_Value = null;
        public object Value
        {
            get { return m_Value; }
            set { m_Value = value; }
        }

        private bool m_IsErrorValue = false;
        public bool IsErrorValue
        {
            get { return m_IsErrorValue; }
            set { m_IsErrorValue = value; }
        }

        public MemberPropertyInfo()
        {
        }

        public MemberPropertyInfo(String name, String uniqueName)
        {
            m_Name = name;
            m_UniqueName = uniqueName;
        }
    }
}
