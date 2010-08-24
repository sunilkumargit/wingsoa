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
using System.Xml;
using System.Globalization;

namespace Ranet.Olap.Core.Data
{
    public class CellValueData : IPropertiesData
    {
        public static String ERROR = "#Error";

        public bool IsError 
        { 
            get
            {
                return m_DisplayValue == ERROR;
                     
            }
        }

        public CellValueData()
        {
        }

        public CellValueData(object value, string displayValue)
        {
            m_Value = value;
            m_DisplayValue = displayValue;
        }

        private static CellValueData m_Empty;
        public static CellValueData Empty
        {
            get
            {
                if (m_Empty == null)
                {
                    m_Empty = new CellValueData();
                }

                return m_Empty;
            }
        }

        private object m_Value = null;
        public object Value
        {
            get
            {
                return m_Value;
            }
            set 
            {
                m_Value = value;
            }
        }

        private string m_DisplayValue = String.Empty;
        public string DisplayValue
        {
            get
            {
                return m_DisplayValue;
            }
            set
            {
                m_DisplayValue = value;
            }
        }

        List<PropertyData> m_Properties = null;
        public List<PropertyData> Properties
        {
            get
            {
                if (m_Properties == null)
                {
                    m_Properties = new List<PropertyData>();
                }
                return m_Properties;
            }
        }

        private const uint MD_MASK_ENABLED = 0x00000000;
        private const uint MD_MASK_NOT_ENABLED = 0x10000000;

        public bool CanUpdate
        {
            get
            {
                if (this.Properties != null)
                {
                    return (GetUpdateStatus() & MD_MASK_NOT_ENABLED) == 0;
                }
                return false;
            }
        }

        private uint GetUpdateStatus()
        {
            Object val = GetPropertyValue("UPDATEABLE");
            if (val != null)
            {
                return Convert.ToUInt32(val);
            }

            return 0xFFFFFFFF;
        }

        public object GetPropertyValue(String propertyName)
        {
            foreach(PropertyData pd in Properties)
            {
                if (pd.Name == propertyName)
                    return pd.Value;
            
            }
            return null;
        }

        public PropertyData GetProperty(string name)
        {
            foreach (PropertyData prop in Properties)
            {
                if (prop.Name == name)
                    return prop;
            }
            return null;
        }
    }
}
