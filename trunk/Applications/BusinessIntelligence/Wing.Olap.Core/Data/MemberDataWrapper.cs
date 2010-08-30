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

namespace Ranet.Olap.Core.Data
{
    public class MemberDataWrapper
    {
        public const String CHILDREN_COUNT_PROPERTY = "-Special_ChildrenCount-";

        public const String PARENT_UNIQUE_NAME_PROPERTY = "-PARENT_UNIQUE_NAME-";
        public const String KEY0_PROPERTY = "-KEY0-";
        public const String HIERARCHY_UNIQUE_NAME_PROPERTY = "-HIERARCHY_UNIQUE_NAME-";

        MemberData m_Member = null;
        public MemberData Member
        {
            get {
                return m_Member;
            }
            set {
                m_Member = value;
            }
        }

        public MemberDataWrapper()
        { 
        
        }

        public MemberDataWrapper(MemberData member)
        {
            Member = member;
        }

        private static MemberDataWrapper m_Empty;
        public static MemberDataWrapper Empty
        {
            get
            {
                if (m_Empty == null)
                {
                    m_Empty = new MemberDataWrapper(null);
                }

                return m_Empty;
            }
        }

        public long RealChildrenCount
        {
            get
            {
                //Количество дочерних узлов в кубе
                long childrenCount = 0;
                if (Member != null)
                {
                    PropertyData prop = Member.GetMemberProperty(MemberDataWrapper.CHILDREN_COUNT_PROPERTY);
                    if (prop != null)
                    {
                        try
                        {
                            childrenCount = (int)(prop.Value);
                        }
                        catch (Exception)
                        {
                            childrenCount = 0;
                        }
                    }
                }
                return childrenCount;
            }
        }

        public String ParentUniqueName
        {
            get
            {
                String res = String.Empty;
                if (Member != null)
                {
                    PropertyData prop = Member.GetMemberProperty(PARENT_UNIQUE_NAME_PROPERTY);
                    if (prop != null && prop.Value != null)
                    {
                        res = prop.Value.ToString();
                    }
                }
                return res;
            }
        }

        public String Key0
        {
            get
            {
                String res = String.Empty;
                if (Member != null)
                {
                    PropertyData prop = Member.GetMemberProperty(KEY0_PROPERTY);
                    if (prop != null)
                    {
                        if (prop.Value != null)
                            res = prop.Value.ToString();
                        else
                            res = "null";
                    }
                    else
                    {
                        res = null;
                    }
                }
                return res;
            }
        }
        public String HierarchyUniqueName
        {
            get
            {
                String res = String.Empty;
                if (Member != null)
                {
                    PropertyData prop = Member.GetMemberProperty(HIERARCHY_UNIQUE_NAME_PROPERTY);
                    if (prop != null && prop.Value != null)
                    {
                        res = prop.Value.ToString();
                    }
                }
                return res;
            }
        }
    }
}
