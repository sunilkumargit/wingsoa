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
    public class Label_FilterWrapper
    {
        public Label_FilterWrapper()
        {
        }

        LabelFilterTypes m_FilterType = LabelFilterTypes.Equal;
        /// <summary>
        /// Тип фильтра
        /// </summary>
        public LabelFilterTypes FilterType
        {
            get { return m_FilterType; }
            set { m_FilterType = value; }
        }

        String m_Text1 = String.Empty;
        /// <summary>
        /// Значение - 1
        /// </summary>
        public String Text1
        {
            get { return m_Text1; }
            set { m_Text1 = value; }
        }

        String m_Text2 = String.Empty;
        /// <summary>
        /// Значение - 1
        /// </summary>
        public String Text2
        {
            get { return m_Text2; }
            set { m_Text2 = value; }
        }

        String m_LevelPropertyName = String.Empty;
        /// <summary>
        /// Название свойства уровня
        /// </summary>
        public String LevelPropertyName
        {
            get { return m_LevelPropertyName; }
            set { m_LevelPropertyName = value; }
        }
    }
}
