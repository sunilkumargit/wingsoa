/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Net;

namespace Wing.Olap.Core
{
    public class ObjectDescription
    {
        String m_Caption = String.Empty;
        public String Caption
        {
            get { return m_Caption; }
            set { m_Caption = value; }
        }

        String m_Name = String.Empty;
        public String Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        String m_Description = String.Empty;
        public String Description
        {
            get { return m_Description; }
            set { m_Description = value; }
        }
    }
}
