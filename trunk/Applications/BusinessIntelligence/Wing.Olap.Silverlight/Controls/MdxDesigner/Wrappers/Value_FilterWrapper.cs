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
