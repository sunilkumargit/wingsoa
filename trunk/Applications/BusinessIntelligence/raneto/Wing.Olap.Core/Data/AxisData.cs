/*   
    Copyright (C) 2009 Galaktika Corporation ZAO

    This file is a part of Ranet.UILibrary.Olap
 
    Ranet.UILibrary.Olap is a free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
      
    You should have received a copy of the GNU General Public License
    along with Ranet.UILibrary.Olap.  If not, see <http://www.gnu.org/licenses/>.
  
    If GPL v.3 is not suitable for your products or company,
    Galaktika Corp provides Ranet.UILibrary.Olap under a flexible commercial license
    designed to meet your specific usage and distribution requirements.
    If you have already obtained a commercial license from Galaktika Corp,
    you can use this file under those license terms.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Globalization;
using Jayrock.Json;

namespace Ranet.Olap.Core.Data
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
            get {
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
