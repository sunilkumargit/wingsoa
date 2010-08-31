/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Wing.AgOlap.Controls.PivotGrid.Layout
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
