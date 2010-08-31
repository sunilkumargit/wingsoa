/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Collections.Generic;

namespace Wing.Olap.Core.Data
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
