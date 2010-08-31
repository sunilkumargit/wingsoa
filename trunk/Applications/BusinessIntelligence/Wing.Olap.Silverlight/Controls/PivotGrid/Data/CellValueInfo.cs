/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wing.Olap.Core.Providers;

namespace Wing.AgOlap.Controls.PivotGrid.Data
{
    public class CellValueInfo : IProperties
    {
        public CellValueInfo(object value, string displayValue)
        {
            m_Value = value;
            m_DisplayValue = displayValue;
        }

        public void AcceptChanges()
        {
            m_IsModified = false;
        }

        private static CellValueInfo m_Empty;
        public static CellValueInfo Empty
        {
            get
            {
                if (m_Empty == null)
                {
                    m_Empty = new CellValueInfo(null, string.Empty);
                }

                return m_Empty;
            }
        }

        private bool m_IsModified = false;
        public bool IsModified
        {
            get
            {
                return m_IsModified;
            }
        }

        private object m_Value;
        public object Value
        {
            get
            {
                return m_Value;
            }
        }

        private string m_DisplayValue;
        public string DisplayValue
        {
            get
            {
                return m_DisplayValue;
            }
            set
            {
//                if (m_DisplayValue != value)
//                {
                    m_IsModified = true;
                    m_DisplayValue = value;
//                }
            }
        }

        private readonly Dictionary<string, object> m_Properties = new Dictionary<string, object>();
        public Dictionary<string, object> Properties
        {
            get
            {
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
                    return (GetUpdateStatus(this.Properties) & MD_MASK_NOT_ENABLED) == 0;
                }
                return false;
            }
        }

        private uint GetUpdateStatus(IDictionary<string, object> properties)
        {
            if (properties.ContainsKey("UPDATEABLE"))
            {
                object prop = properties["UPDATEABLE"];
                if (prop != null)
                {
                    return Convert.ToUInt32(prop);
                }
            }

            return 0xFFFFFFFF;
        }

        public object GetPropertyValue(String propertyName)
        {
            if (this.Properties != null && !String.IsNullOrEmpty(propertyName))
            {
                object ret;
                this.Properties.TryGetValue(propertyName, out ret);
                return ret;
            }
            return null;
        }

        #region IProperties Members

        public Dictionary<string, object> PropertiesDictionary
        {
            get
            {
                return m_Properties;
            }
        }
        #endregion
    }
}
