/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/


namespace Wing.Olap.Controls.MdxDesigner.Wrappers
{
    public class FilterWrapperBase
    {
        bool m_IsUsed = false;
        public bool IsUsed
        {
            get { return m_IsUsed; }
            set { m_IsUsed = value; }
        }

        public FilterWrapperBase()
        { }
    }
}
