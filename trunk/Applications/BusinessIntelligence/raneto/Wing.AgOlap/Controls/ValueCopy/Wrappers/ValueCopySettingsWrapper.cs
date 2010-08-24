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
using System.Collections.Generic;

namespace Ranet.AgOlap.Controls.ValueCopy.Wrappers
{
    public class ValueCopySettingsWrapper
    {
        public ValueCopySettingsWrapper()
        { 
        }

        ValueCopyTypes m_CopyType = ValueCopyTypes.CopyValueFromSource;
        /// <summary>
        /// Тип копирования
        /// </summary>
        public ValueCopyTypes CopyType
        {
            get { return m_CopyType; }
            set { m_CopyType = value; }
        }

        String m_Value = "0";
        /// <summary>
        /// Значение
        /// </summary>
        public String Value
        {
            get { return m_Value; }
            set { m_Value = value; }
        }

        double m_Coefficient = 1;
        /// <summary>
        /// Коэффициент
        /// </summary>
        public double Coefficient
        {
            get { return m_Coefficient; }
            set { m_Coefficient = value; }
        }

        List<CoordinateItem_Wrapper> m_CoordinatesList = null;
        /// <summary>
        /// Координаты
        /// </summary>
        public List<CoordinateItem_Wrapper> CoordinatesList
        {
            get
            {
                if (m_CoordinatesList == null)
                {
                    m_CoordinatesList = new List<CoordinateItem_Wrapper>();
                }
                return m_CoordinatesList;
            }
            set { m_CoordinatesList = value; }
        }

        bool m_ShowNotEmptyCoordinates = false;
        public bool ShowNotEmptyCoordinates
        {
            get { return m_ShowNotEmptyCoordinates; }
            set { m_ShowNotEmptyCoordinates = value; }
        }

    }
}
