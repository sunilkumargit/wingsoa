﻿/*   
    Copyright (C) 2009 Galaktika Corporation ZAO

    This file is a part of Wing.UILibrary.Olap
 
    Wing.UILibrary.Olap is a free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
      
    You should have received a copy of the GNU General Public License
    along with Wing.UILibrary.Olap.  If not, see <http://www.gnu.org/licenses/>.
  
    If GPL v.3 is not suitable for your products or company,
    Galaktika Corp provides Wing.UILibrary.Olap under a flexible commercial license
    designed to meet your specific usage and distribution requirements.
    If you have already obtained a commercial license from Galaktika Corp,
    you can use this file under those license terms.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wing.Olap.Core.Metadata
{
    public class LevelPropertyInfo
    {
        private String m_Caption = String.Empty;
        public virtual String Caption
        {
            get { return m_Caption; }
            set { m_Caption = value; }
        }

        private String m_Description = String.Empty;
        public virtual String Description
        {
            get { return m_Description; }
            set { m_Description = value; }
        }

        private String m_Name = String.Empty;
        public virtual String Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        /*private String m_UniqueName = String.Empty;
        public virtual String UniqueName
        {
            get { return m_UniqueName; }
            set { m_UniqueName = value; }
        }*/

        private String m_ParentLevelId = null;
        public String ParentLevelId
        {
            get { return m_ParentLevelId; }
            set { m_ParentLevelId = value; }
        }

        /*private Type m_DataType = null;
        public Type DataType
        {
            get { return m_DataType; }
            set { m_DataType = value; }
        }*/

        private int m_PropertyType = 0;
        public int PropertyType
        {
            get { return m_PropertyType; }
            set { m_PropertyType = value; }
        }

        bool m_IsSystem = false;
        public bool IsSystem
        {
            get {
                //if ((m_PropertyType & 0x04) == 0x04)
                //    return true;
                //return false;
                return m_IsSystem;
            }
            set {
                m_IsSystem = value;
            }
        }
        
        public LevelPropertyInfo()
        {
        }

        public LevelPropertyInfo(String caption, String name)
        {
            m_Name = name;
            m_Caption = caption;
        }
    }
}
