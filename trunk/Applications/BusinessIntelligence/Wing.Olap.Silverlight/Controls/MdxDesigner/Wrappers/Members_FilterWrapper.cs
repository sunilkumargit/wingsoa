/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Collections.Generic;
using Wing.Olap.Controls.MemberChoice.Info;

namespace Wing.Olap.Controls.MdxDesigner.Wrappers
{
    public class Members_FilterWrapper : FilterWrapperBase
    {
        public Members_FilterWrapper()
        {
        }

        public bool IsModified { get; private set; }

        /// <summary>
        /// Коллекция для хранения информации о выбранных элементах
        /// </summary>
        private List<MemberChoiceSettings> m_SelectedInfo = null;
        /// <summary>
        /// Информация о выбранных элементах
        /// </summary>
        public List<MemberChoiceSettings> SelectedInfo
        {
            get
            {
                if (m_SelectedInfo == null)
                {
                    m_SelectedInfo = new List<MemberChoiceSettings>();
                }
                return m_SelectedInfo;
            }
            set
            {
                IsModified = true;
                m_SelectedInfo = value;
            }
        }

        String m_FilterSet = String.Empty;
        /// <summary>
        /// Set, сформированный из выбранных элементов
        /// </summary>
        public String FilterSet
        {
            get { return m_FilterSet; }
            set {
                IsModified = true;
                m_FilterSet = value; 
            }
        }
    }
}
