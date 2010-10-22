/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using Wing.Olap.Controls.MdxDesigner.Filters;

namespace Wing.Olap.Controls.MdxDesigner.Wrappers
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
