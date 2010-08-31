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
