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
    public interface IPropertiesData
    {
        List<PropertyData> Properties { get; }
        PropertyData GetProperty(String name);
    }

    public class PropertyData
    {
        String m_Name = String.Empty;
        public String Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        Object m_Value = null;
        public Object Value
        {
            get { return m_Value; }
            set { m_Value = value; }
        }

        public override bool Equals(object obj)
        {
            var pd = obj as PropertyData;
            if (pd == null)
                return false;

            if (pd.Name != Name)
                return false;

            if (Value == null)
            {
                if (pd.Value == null)
                    return true;

                return false;
            }
            return Value.Equals(pd.Value);
        }
        public override int GetHashCode()
        {
            int hc = 0;
            if (Name != null)
                hc = Name.GetHashCode();
            if (Value != null)
            {
                unchecked
                {
                    hc *= 3;
                    hc ^= Value.GetHashCode();
                }
            }
            return hc;
        }

        public PropertyData() 
        {
        }

        public PropertyData(String name, object value)
        {
            m_Name = name;
            m_Value = value;
        }
    }
}
