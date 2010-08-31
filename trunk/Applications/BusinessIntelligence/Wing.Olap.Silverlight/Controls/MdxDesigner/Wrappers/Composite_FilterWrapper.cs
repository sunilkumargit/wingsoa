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
using Wing.Olap.Controls.MdxDesigner.Filters;

namespace Wing.Olap.Controls.MdxDesigner.Wrappers
{
    public class Composite_FilterWrapper : FilterWrapperBase
    {
        public Composite_FilterWrapper()
        { 
        
        }

        Members_FilterWrapper m_MembersFilter = null;
        /// <summary>
        /// Фильтр по элементам
        /// </summary>
        public Members_FilterWrapper MembersFilter
        {
            get
            {
                if (m_MembersFilter == null)
                    m_MembersFilter = new Members_FilterWrapper();
                return m_MembersFilter;
            }
            set { m_MembersFilter = value; }
        }

        Filter_FilterWrapper m_Filter = null;
        /// <summary>
        /// Фильтр (Top, по значению либо по подписи)
        /// </summary>
        public Filter_FilterWrapper Filter
        {
            get
            {
                if (m_Filter == null)
                    m_Filter = new Filter_FilterWrapper();
                return m_Filter;
            }
            set { m_Filter = value; }
        }

        public new bool IsUsed
        {
            get
            {
                return Filter.IsUsed | MembersFilter.IsUsed;
            }
            set
            {
                if (!value)
                {
                    MembersFilter.IsUsed = value;
                    Filter.IsUsed = value;
                }
            }
        }
    }
}
