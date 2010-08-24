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
    public class PropertiesBase
    {
        public const String DIMENSION_CAPTION = "DIMENSION_CAPTION";
        public const String HIERARCHY_CAPTION = "HIERARCHY_CAPTION";
        public const String LEVEL_CAPTION = "LEVEL_CAPTION";
        public const String CUBE_CAPTION = "CUBE_CAPTION";

        private List<PropertyInfo> m_Properties = new List<PropertyInfo>();
        public List<PropertyInfo> Properties
        {
            get { return m_Properties; }
        }

        public PropertyInfo GetProperty(String propName)
        {
            if (Properties != null)
            {
                foreach (PropertyInfo propInfo in Properties)
                {
                    if (propInfo.Name == propName)
                        return propInfo;
                }
            }
            return null;
        }

        public object GetPropertyValue(String propName)
        {
            PropertyInfo propInfo = GetProperty(propName);
            if (propInfo != null)
            {
                return propInfo.Value;
            }
            return null;
        }

        private List<PropertyInfo> m_CustomProperties = new List<PropertyInfo>();
        public List<PropertyInfo> CustomProperties
        {
            get { return m_CustomProperties; }
        }

        public PropertyInfo GetCustomProperty(String propName)
        {
            if (CustomProperties != null)
            {
                foreach (PropertyInfo propInfo in CustomProperties)
                {
                    if (propInfo.Name == propName)
                        return propInfo;
                }
            }
            return null;
        }

        public object GetCustomPropertyValue(String propName)
        {
            PropertyInfo propInfo = GetCustomProperty(propName);
            if (propInfo != null)
            {
                return propInfo.Value;
            }
            return null;
        }
    }

    public class InfoBase : PropertiesBase
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
    }
}
