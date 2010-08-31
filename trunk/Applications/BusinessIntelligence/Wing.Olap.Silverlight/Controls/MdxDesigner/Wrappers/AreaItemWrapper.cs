/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
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
using Wing.Olap.Core.Metadata;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Wing.Olap.Controls.MdxDesigner.Wrappers
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
