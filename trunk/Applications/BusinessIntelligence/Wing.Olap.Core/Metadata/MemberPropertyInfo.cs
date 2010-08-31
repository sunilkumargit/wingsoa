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
    public class MemberPropertyInfo
    {
        private String m_Name = String.Empty;
        public String Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        private String m_UniqueName = String.Empty;
        public String UniqueName
        {
            get { return m_UniqueName; }
            set { m_UniqueName = value; }
        }

        private object m_Value = null;
        public object Value
        {
            get { return m_Value; }
            set { m_Value = value; }
        }

        private bool m_IsErrorValue = false;
        public bool IsErrorValue
        {
            get { return m_IsErrorValue; }
            set { m_IsErrorValue = value; }
        }

        public MemberPropertyInfo()
        {
        }

        public MemberPropertyInfo(String name, String uniqueName)
        {
            m_Name = name;
            m_UniqueName = uniqueName;
        }
    }
}
