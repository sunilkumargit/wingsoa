/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Collections.Generic;

namespace Wing.Olap.Core.Data
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
