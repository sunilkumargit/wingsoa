/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Globalization;
using Wing.Json;

namespace Wing.Olap.Core.Data
{
    public class AxisData
    {
        public AxisData()
        {
        }

        public int AxisNum = -1;

        String m_Name = String.Empty;
        /// <summary>
        /// Название
        /// </summary>
        public String Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        List<PositionData> m_Positions = null;
        /// <summary>
        /// Позиции на оси
        /// </summary>
        public List<PositionData> Positions
        {
            get
            {
                if (m_Positions == null)
                    m_Positions = new List<PositionData>();
                return m_Positions;
            }
            set { m_Positions = value; }
        }

        Dictionary<int, MemberData> m_Members;
        public Dictionary<int, MemberData> Members
        {
            get
            {
                if (m_Members == null)
                {
                    m_Members = new Dictionary<int, MemberData>();
                }
                return m_Members;
            }
        }

        Dictionary<int, String> m_MembersPropertiesNames;
        /// <summary>
        /// Названия свойств элементов
        /// </summary>
        public Dictionary<int, String> MembersPropertiesNames
        {
            get
            {
                if (m_MembersPropertiesNames == null)
                {
                    m_MembersPropertiesNames = new Dictionary<int, String>();
                }
                return m_MembersPropertiesNames;
            }
        }

        Dictionary<int, String> m_CustomMembersPropertiesNames;
        /// <summary>
        /// Названия пользовательских свойств элементов
        /// </summary>
        public Dictionary<int, String> CustomMembersPropertiesNames
        {
            get
            {
                if (m_CustomMembersPropertiesNames == null)
                {
                    m_CustomMembersPropertiesNames = new Dictionary<int, String>();
                }
                return m_CustomMembersPropertiesNames;
            }
        }
    }
}
