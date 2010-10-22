/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/


namespace Wing.Olap.Controls.PivotGrid.Layout
{
    /// <summary>
    /// Описатель элемента для расположения в сетке
    /// </summary>
    public class LayoutItem
    {
        int m_RowSpan = 1;
        public virtual int RowSpan
        {
            get { return m_RowSpan; }
            set { m_RowSpan = value; }
        }
        
        int m_ColumnSpan = 1;
        public virtual int ColumnSpan
        {
            get { return m_ColumnSpan; }
            set { m_ColumnSpan = value; }
        }

        public bool IsExtension = false;

    }
}
