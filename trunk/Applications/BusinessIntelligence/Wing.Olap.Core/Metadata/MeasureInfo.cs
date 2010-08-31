/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;

namespace Wing.Olap.Core.Metadata
{
    public class MeasureInfo : InfoBase
    {
        private String m_ParentCubeId = string.Empty;
        public String ParentCubeId
        {
            get { return m_ParentCubeId; }
            set { m_ParentCubeId = value; }
        }

        private String m_UniqueName = String.Empty;
        public virtual String UniqueName
        {
            get { return m_UniqueName; }
            set { m_UniqueName = value; }
        }

        private String m_DisplayFolder = String.Empty;
        public String DisplayFolder
        {
            get { return m_DisplayFolder; }
            set { m_DisplayFolder = value; }
        }

        private String m_Expression = String.Empty;
        public String Expression
        {
            get { return m_Expression; }
            set { m_Expression = value; }
        }

        private int m_NumericPrecision = 0;
        public int NumericPrecision
        {
            get { return m_NumericPrecision; }
            set { m_NumericPrecision = value; }
        }

        private short m_NumericScale = 0;
        public short NumericScale
        {
            get { return m_NumericScale; }
            set { m_NumericScale = value; }
        }
        
        private String m_Units = String.Empty;
        public String Units
        {
            get { return m_Units; }
            set { m_Units = value; }
        }

        public String MeasureGroup
        {
            get {
                PropertyInfo pi = GetProperty("MEASUREGROUP_NAME");
                if (pi != null && pi.Value != null)
                {
                    return pi.Value.ToString();
                }
                return null;
            }
        }
    }
}
