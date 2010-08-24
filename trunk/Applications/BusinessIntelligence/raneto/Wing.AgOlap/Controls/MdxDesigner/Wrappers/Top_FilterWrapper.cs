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
    /// <summary>
    /// Фильтр "Top N"
    /// </summary>
    public class Top_FilterWrapper
    {
        public Top_FilterWrapper()
        {
        }

        TopFilterTypes m_FilterType = TopFilterTypes.Top;
        /// <summary>
        /// Тип фильтра
        /// </summary>
        public TopFilterTypes FilterType
        {
            get { return m_FilterType; }
            set { m_FilterType = value; }
        }

        int m_Count = 10;
        /// <summary>
        /// Число элементов
        /// </summary>
        public int Count
        {
            get { return m_Count; }
            set { m_Count = value; }
        }

        TopFilterTargetTypes m_FilterTarget = TopFilterTargetTypes.Items;
        /// <summary>
        /// Отбираемые объекты
        /// </summary>
        public TopFilterTargetTypes FilterTarget
        {
            get { return m_FilterTarget; }
            set { m_FilterTarget = value; }
        }

        String m_MeasureUniqueName = String.Empty;
        /// <summary>
        /// Уникальное имя меры
        /// </summary>
        public String MeasureUniqueName
        {
            get { return m_MeasureUniqueName; }
            set { m_MeasureUniqueName = value; }
        }
    }
}
