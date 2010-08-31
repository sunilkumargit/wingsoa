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
using System.Collections.Generic;

namespace Wing.AgOlap.Controls.PivotGrid.Layout
{
    /// <summary>
    /// Описатель для ячейки сетки расположения элементов. В ячейке может быть коллекция элементов
    /// </summary>
    public class LayoutCellWrapper
    {
        List<LayoutItem> m_Items = null;
        public List<LayoutItem> Items
        {
            get {
                if (m_Items == null)
                {
                    m_Items = new List<LayoutItem>();
                }
                return m_Items;
            }
        }

        public LayoutCellWrapper()
        {
        }
    }
}
