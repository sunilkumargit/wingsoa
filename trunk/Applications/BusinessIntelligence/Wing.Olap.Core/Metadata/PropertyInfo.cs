/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wing.Olap.Core.Metadata
{
    public class PropertyInfo
    {
        public PropertyInfo()
        { 
        }

        //public PropertyInfo(String name, Type type, object value)
        //{
        //    m_Name = name;
        //    m_Type = type;
        //    m_Value = value;
        //}

        public PropertyInfo(String name, object value)
        {
            m_Name = name;
            m_Value = value;
        }

        private String m_Name = String.Empty;
        public String Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        /*private Type m_Type = typeof(System.Object);
        public Type Type
        {
            get { return m_Type; }
            set { m_Type = value; }
        }*/
        
        private object m_Value = null;
        public object Value
        {
            get { return m_Value; }
            set { m_Value = value; }
        }

    }
}
