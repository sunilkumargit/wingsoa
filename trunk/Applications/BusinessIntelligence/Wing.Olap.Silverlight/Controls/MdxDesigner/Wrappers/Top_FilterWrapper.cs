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
using Wing.AgOlap.Controls.MdxDesigner.Filters;

namespace Wing.AgOlap.Controls.MdxDesigner.Wrappers
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
