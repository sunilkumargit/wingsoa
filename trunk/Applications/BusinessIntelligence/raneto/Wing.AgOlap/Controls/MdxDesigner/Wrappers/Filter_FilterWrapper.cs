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
    public class Filter_FilterWrapper : FilterWrapperBase
    {
        public Filter_FilterWrapper()
        { 
        
        }

        FilterFamilyTypes m_CurrentFilter = FilterFamilyTypes.LabelFilter;
        /// <summary>
        /// Тип текущего фильтра
        /// </summary>
        public FilterFamilyTypes CurrentFilter
        {
            get { return m_CurrentFilter; }
            set { m_CurrentFilter = value; }
        }
        
        Top_FilterWrapper m_TopFilter = null;
        /// <summary>
        /// Фильтр Top
        /// </summary>
        public Top_FilterWrapper TopFilter
        {
            get
            {
                if (m_TopFilter == null)
                    m_TopFilter = new Top_FilterWrapper();
                return m_TopFilter;
            }
            set { m_TopFilter = value; }
        }

        Value_FilterWrapper m_ValueFilter = null;
        /// <summary>
        /// Фильтр по значению
        /// </summary>
        public Value_FilterWrapper ValueFilter
        {
            get
            {
                if (m_ValueFilter == null)
                    m_ValueFilter = new Value_FilterWrapper();
                return m_ValueFilter;
            }
            set { m_ValueFilter = value; }
        }

        Label_FilterWrapper m_LabelFilter = null;
        /// <summary>
        /// Фильтр по подписи
        /// </summary>
        public Label_FilterWrapper LabelFilter
        {
            get
            {
                if (m_LabelFilter == null)
                    m_LabelFilter = new Label_FilterWrapper();
                return m_LabelFilter;
            }
            set { m_LabelFilter = value; }
        }
    }
}
