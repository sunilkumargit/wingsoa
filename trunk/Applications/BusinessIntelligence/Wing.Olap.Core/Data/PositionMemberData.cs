/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/


namespace Wing.Olap.Core.Data
{
    public class PositionMemberData
    {
        public PositionMemberData()
        { 
        
        }
    
        public PositionMemberData(int id)
        {
            m_Id = id;
        }

        private int m_Id = -1;
        public int Id
        {
            get { return m_Id; }
            set { m_Id = value; }
        }

        private bool m_DrilledDown = false;
        public bool DrilledDown
        {
            get { return m_DrilledDown; }
            set { m_DrilledDown = value; }
        }
    }
}
