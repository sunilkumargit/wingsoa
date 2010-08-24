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
using Ranet.Olap.Core.Metadata;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Ranet.AgOlap.Controls.MdxDesigner.Wrappers
{
    [XmlInclude(typeof(NamedSet_AreaItemWrapper))]
    [XmlInclude(typeof(Calculated_AreaItemWrapper))]
    [XmlInclude(typeof(CalculatedNamedSet_AreaItemWrapper))]
    [XmlInclude(typeof(CalculatedMember_AreaItemWrapper))]
    [XmlInclude(typeof(Level_AreaItemWrapper))]
    [XmlInclude(typeof(Hierarchy_AreaItemWrapper))]
    [XmlInclude(typeof(Measure_AreaItemWrapper))]
    [XmlInclude(typeof(Kpi_AreaItemWrapper))]
    [XmlInclude(typeof(Values_AreaItemWrapper))]
    [XmlInclude(typeof(Filtered_AreaItemWrapper))]
    public class AreaItemWrapper
    {
        private List<PropertyInfo> m_CustomProperties = null;
        /// <summary>
        /// Свойства
        /// </summary>
        public List<PropertyInfo> CustomProperties
        {
            set { m_CustomProperties = value; }
            get {
                if (m_CustomProperties == null)
                {
                    m_CustomProperties = new List<PropertyInfo>();
                }
                return m_CustomProperties; 
            }
        }

        String m_Caption = String.Empty;
        public String Caption
        {
            get { return m_Caption; }
            set { m_Caption = value; }
        }

        public AreaItemWrapper()
        { 
        }

        public AreaItemWrapper(InfoBase info)
        {
            Caption = info.Caption;
            foreach (PropertyInfo prop in info.CustomProperties)
            { 
                this.CustomProperties.Add(prop);
            }
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
    }
}
