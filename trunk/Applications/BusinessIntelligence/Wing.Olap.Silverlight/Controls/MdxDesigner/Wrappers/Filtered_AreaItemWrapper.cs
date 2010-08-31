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
using Wing.Olap.Core.Metadata;

namespace Wing.Olap.Controls.MdxDesigner.Wrappers
{
    public class Filtered_AreaItemWrapper : AreaItemWrapper
    {
        public Filtered_AreaItemWrapper(InfoBase info)
            : base (info)
        {
        
        }

        public Filtered_AreaItemWrapper()
        { }

        Composite_FilterWrapper m_CompositeFilter = null;
        /// <summary>
        /// Фильтр
        /// </summary>
        public Composite_FilterWrapper CompositeFilter
        {
            get
            {
                if (m_CompositeFilter == null)
                    m_CompositeFilter = new Composite_FilterWrapper();
                return m_CompositeFilter;
            }
            set { m_CompositeFilter = value; }
        }
        
    }
}
