/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System.Collections.Generic;

namespace Wing.Olap.Core.Data
{
    public class PositionData
    {
        public PositionData()
        { 
        
        }

        List<PositionMemberData> m_Members = null;
        /// <summary>
        /// Элементы для данной позиции
        /// </summary>
        public List<PositionMemberData> Members
        {
            get
            {
                if (m_Members == null)
                    m_Members = new List<PositionMemberData>();
                return m_Members;
            }
            set { m_Members = value; }
        }

    }
}
