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
    /// Фильтр по значению
    /// </summary>
    public class Value_FilterWrapper
    {
        public Value_FilterWrapper()
        {
        }

        ValueFilterTypes m_FilterType = ValueFilterTypes.Equal;
        /// <summary>
        /// Тип фильтра
        /// </summary>
        public ValueFilterTypes FilterType
        {
            get { return m_FilterType; }
            set { m_FilterType = value; }
        }

        int m_Num1 = 0;
        /// <summary>
        /// Число - 1
        /// </summary>
        public int Num1
        {
            get { return m_Num1; }
            set { m_Num1 = value; }
        }

        int m_Num2 = 1;
        /// <summary>
        /// Число - 2
        /// </summary>
        public int Num2
        {
            get { return m_Num2; }
            set { m_Num2 = value; }
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
