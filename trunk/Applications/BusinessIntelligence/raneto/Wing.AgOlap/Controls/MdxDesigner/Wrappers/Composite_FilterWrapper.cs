/*   
    Copyright (C) 2009 Galaktika Corporation ZAO

    This file is a part of Ranet.UILibrary.Olap
 
    Ranet.UILibrary.Olap is a free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
      
    You should have received a copy of the GNU General Public License
    along with Ranet.UILibrary.Olap.  If not, see
  	<http://www.gnu.org/licenses/> 
  
    If GPL v.3 is not suitable for your products or company,
    Galaktika Corp provides Ranet.UILibrary.Olap under a flexible commercial license
    designed to meet your specific usage and distribution requirements.
    If you have already obtained a commercial license from Galaktika Corp,
    you can use this file under those license terms.
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
using Ranet.AgOlap.Controls.MdxDesigner.Filters;

namespace Ranet.AgOlap.Controls.MdxDesigner.Wrappers
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
