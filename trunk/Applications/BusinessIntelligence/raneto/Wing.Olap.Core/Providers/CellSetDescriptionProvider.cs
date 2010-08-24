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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AnalysisServices.AdomdClient;
using Ranet.Olap.Core.Data;
using System.Xml;
using System.Globalization;

namespace Ranet.Olap.Core.Providers
{
    public class CellSetDescriptionProvider
    {
        ConnectionInfo m_Connection = new ConnectionInfo();
        /// <summary>
        /// Information on Connection
        /// </summary>
        public ConnectionInfo Connection
        {
            get
            {
                if (m_Connection == null)
                    m_Connection = new ConnectionInfo();
                return m_Connection;
            }
            set { m_Connection = value; }
        }

        public readonly CellSet CS = null;

        public CellSetDescriptionProvider(CellSet cs)
        {
            CS = cs;
        }

        CellSetData m_CellSet = null;
        public CellSetData CellSet
        {
            get {
                if (m_CellSet == null)
                {
                    m_CellSet = new CellSetData();
                    DateTime start = DateTime.Now;
                    BuildDescription(CS);
                    System.Diagnostics.Debug.WriteLine("CellSetData building time: " + (DateTime.Now - start).ToString());
                }
                return m_CellSet; 
            }
        }

        private void BuildDescription(CellSet cs)
        {
            if (cs != null)
            {
                if (cs.OlapInfo != null &&
                    cs.OlapInfo.CubeInfo != null &&
                    cs.OlapInfo.CubeInfo.Cubes != null &&
                    cs.OlapInfo.CubeInfo.Cubes.Count > 0)
                {
                    m_CellSet.CubeName = cs.OlapInfo.CubeInfo.Cubes[0].CubeName;
                }

                DateTime start = DateTime.Now;
                int i = 0;
                // Оси
                foreach(Axis axis in cs.Axes)
                {
                    AxisData axis_data = BuildAxisDescription(axis);
                    axis_data.AxisNum = i;
                    m_CellSet.Axes.Add(axis_data);
                    i++;
                }
                System.Diagnostics.Debug.WriteLine("CellSetData building AXES time: " + (DateTime.Now - start).ToString());

                // Ось фильтров - пока эта информация не нужна
                // cs_descr.FilterAxis = BuildAxisDescription(cs.FilterAxis); 

                DateTime start1 = DateTime.Now;
                if (cs.Axes.Count == 1)
                {
                    for (int col = 0; col < cs.Axes[0].Positions.Count; col++)
                    {
                        CellData cell_descr = new CellData();
                        cell_descr.Axis0_Coord = col;
                        cell_descr.Axis1_Coord = -1;
                        cell_descr.Value = GetValue(col);
                        m_CellSet.Cells.Add(cell_descr);
                    }
                }

                if (cs.Axes.Count >= 2)
                { 
                    for(int col = 0; col < cs.Axes[0].Positions.Count; col++)
                    {
                        for (int row = 0; row < cs.Axes[1].Positions.Count; row++)
                        {
                            CellData cell_descr = new CellData();
                            cell_descr.Axis0_Coord = col;
                            cell_descr.Axis1_Coord = row;
                            cell_descr.Value = GetValue(col, row);
                            m_CellSet.Cells.Add(cell_descr);
                        }
                    }
                }

                System.Diagnostics.Debug.WriteLine("CellSetData building CELLS time: " + (DateTime.Now - start1).ToString());
            }
        }

        private AxisData BuildAxisDescription(Axis axis)
        {
            // Список созданных описателей для элементов
            Dictionary<String, int> list = new Dictionary<string, int>();

            AxisData axis_descr = null;
            if (axis != null)
            {
                axis_descr = new AxisData();
                axis_descr.Name = axis.Name;

                foreach (Position pos in axis.Positions)
                {
                    PositionData pos_desc = new PositionData();
                    foreach (Member member in pos.Members)
                    {
                        int id = -1;
                        if (!list.ContainsKey(member.UniqueName))
                        {
                            MemberData member_desc = CreateMemberDescription(member);

                            // Добавляем в список элементов на оси
                            id = axis_descr.Members.Count;
                            if (!axis_descr.Members.ContainsKey(id))
                            {
                                axis_descr.Members.Add(id, member_desc);
                            }
                            else
                            {
                                axis_descr.Members[id] = member_desc;
                            }

                            list.Add(member.UniqueName, id);
                        }
                        else
                        {
                            id = list[member.UniqueName];
                        }

                        pos_desc.Members.Add(new PositionMemberData(id) { DrilledDown = member.DrilledDown });
                    }
                    axis_descr.Positions.Add(pos_desc);
                }
            }

            list.Clear();
            list = null;
            return axis_descr;
        }


        /// <summary>
        /// Внутренний кэш. Ключ - уник. имя уровня, значение - уник. имя иерархии
        /// </summary>
        Dictionary<String, String> m_HierarchiesToLevelsCache = new Dictionary<string, string>();

        /// <summary>
        /// Формирует описание для элемента
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public MemberData CreateMemberDescription(Member member)
        {
            if (member == null)
                return null;
            MemberData info = new MemberData();
            info.Caption = member.Caption;
            info.ChildCount = member.ChildCount;
            info.Description = member.Description;
            info.DrilledDown = member.DrilledDown;
            info.LevelDepth = member.LevelDepth;
            info.LevelName = member.LevelName;
            info.Name = member.Name;
            info.ParentSameAsPrevious = member.ParentSameAsPrevious;

            String hierarchyUniqueName = GetMemberPropertyValue(member, "HIERARCHY_UNIQUE_NAME");
            if (hierarchyUniqueName != null)
            {
                info.HierarchyUniqueName = hierarchyUniqueName;
            }
            else
            {
                if (m_HierarchiesToLevelsCache.ContainsKey(info.LevelName))
                {
                    info.HierarchyUniqueName = m_HierarchiesToLevelsCache[info.LevelName];
                }
                else
                {
                    try
                    {
                        info.HierarchyUniqueName = member.ParentLevel.ParentHierarchy.UniqueName;
                    }
                    catch (System.InvalidOperationException)
                    {
                        info.HierarchyUniqueName = String.Empty;
                    }

                    m_HierarchiesToLevelsCache[info.LevelName] = info.HierarchyUniqueName;
                }
            }

            info.UniqueName = member.UniqueName;

            // Свойства
            foreach (Property prop in member.Properties)
            {
                PropertyData property = new PropertyData(prop.Name, null);
                try
                {
                    property.Value = prop.Value;
                }
                catch (Microsoft.AnalysisServices.AdomdClient.AdomdErrorResponseException ex)
                {
                    property.Value = ex.ToString();
                }
                info.Properties.Add(property);
            }

            foreach (MemberProperty mp in member.MemberProperties)
            {
                PropertyData property = new PropertyData(mp.Name, null);
                try
                {
                    property.Value = mp.Value;
                }
                catch (Microsoft.AnalysisServices.AdomdClient.AdomdErrorResponseException ex)
                {
                    property.Value = ex.ToString();
                }
                info.MemberProperties.Add(property);
            }
            return info;
        }

        /// <summary>
        /// Возвращает значение свойства для элемента
        /// </summary>
        /// <param name="member"></param>
        /// <param name="propName"></param>
        /// <returns></returns>
        private String GetMemberPropertyValue(Member member, String propName)
        {
            if (member == null || member.MemberProperties == null || String.IsNullOrEmpty(propName))
                return null;
            MemberProperty prop = member.MemberProperties.Find(propName);
            if (prop != null && prop.Value != null)
            {
                return prop.Value.ToString();
            }
            return null;
        }

        public CellValueData GetValue(params int[] index)
        {
            if (CS == null)
            {
                return CellValueData.Empty;
            }

            int[] indexVector = new int[CS.Axes.Count];
            for (int i = 0; i < indexVector.Length; i++)
            {
                indexVector[i] = 0;
            }
            index.CopyTo(indexVector, 0);

            Cell cell = null;
            try
            {
                cell = CS[indexVector];
            }
            catch (ArgumentOutOfRangeException)
            {
                return CellValueData.Empty;
            }
            if (cell != null)
            {
                object value = null;
                string displayName = string.Empty;

                try
                {
                    displayName = cell.FormattedValue;
                }
                catch (Exception exc)
                {
                    displayName = CellValueData.ERROR;
                }

                try
                {
                    value = cell.Value;
                }
                catch (Exception exc)
                {
                    value = exc.ToString();
                }

                if (string.IsNullOrEmpty(displayName))
                {
                    if (value == null)
                    {
                        displayName = String.Empty;
                    }
                    else
                    {
                        displayName = value.ToString();
                    }
                }
                
                CellValueData res = new CellValueData(value, displayName);

                foreach (CellProperty prop in cell.CellProperties)
                {
                    PropertyData property = new PropertyData(prop.Name, null);
                    try
                    {
                        property.Value = prop.Value;
                    }
                    catch (Microsoft.AnalysisServices.AdomdClient.AdomdErrorResponseException ex)
                    {
                        property.Value = ex.ToString();
                    }
                    res.Properties.Add(property);
                }

                return res;
            }

            return null;
        }

        const string XML_CellSetData = "csd";

        public String Serialize()
        {
            StringBuilder sb = new StringBuilder();

            XmlWriter writer = XmlWriter.Create(sb);
            Serialize(writer);
            writer.Close();
            return sb.ToString();
        }

        void Serialize(XmlWriter writer)
        {
            if (writer == null || CS == null)
                return;

            var data = new List<object>();

            // Начало
            writer.WriteStartElement(XML_CellSetData);

            String cubeName = String.Empty;
            if (CS.OlapInfo != null &&
                 CS.OlapInfo.CubeInfo != null &&
                 CS.OlapInfo.CubeInfo.Cubes != null &&
                 CS.OlapInfo.CubeInfo.Cubes.Count > 0)
            {
                cubeName = CS.OlapInfo.CubeInfo.Cubes[0].CubeName;
            }

            // Имя куба
            data.Add(cubeName);

            // Соединение
            var connection = new List<object>();
            connection.Add(Connection.ConnectionID.ToString(CultureInfo.InvariantCulture));
            connection.Add(Connection.ConnectionString.ToString(CultureInfo.InvariantCulture));
            data.Add(connection.ToArray());

            var axes = new List<object>();
            // Оси
            foreach (Axis axis in CS.Axes)
            {
                var axis_data = new List<object>();

                axis_data.Add(axis.Name);

                List<Member> axis_members = new List<Member>();

                // Список созданных описателей для элементов
                Dictionary<String, int> list = new Dictionary<string, int>();
                var positions = new List<object>();
                foreach (Position pos in axis.Positions)
                {
                    var position_data = new List<object>();
                    foreach (Member member in pos.Members)
                    {
                        var member_data = new List<object>();

                        int id = -1;
                        if (!list.ContainsKey(member.UniqueName))
                        {
                            // Добавляем в список элементов на оси
                            id = axis_members.Count;
                            axis_members.Add(member);
                            list.Add(member.UniqueName, id);
                        }
                        else
                        {
                            id = list[member.UniqueName];
                        }
                        member_data.Add(id.ToString(CultureInfo.InvariantCulture));
                        member_data.Add(member.DrilledDown.ToString(CultureInfo.InvariantCulture));

                        position_data.Add(member_data.ToArray());
                    }
                    positions.Add(position_data.ToArray());
                }
                axis_data.Add(positions.ToArray());

                // Элементы

                //   - Members Count
                //   - Properties Names
                //   - Member Properties Names

                //   - Settings for Member[x]
                //   - Properties Values for Member[x]
                //   - Member Properties Values for Member[x]
                //   - Member Properties Style
                // .....

                //   - Member Properties Style Values
                // ....

                //   - Member Properties Style Names

                var tmp = new List<object>();

                var equalsMemberPropertiesList = new List<AdomdCustomMemberPropsObj>();
                var equalsMemberPropertiesHash = new Dictionary<AdomdCustomMemberPropsObj, int>();

                //! var props = new int[Members.Count];
                //! var memberProps = new int[Members.Count];

                var propNames = new List<string>();
                var propNamesHash = new Dictionary<string, int>();

                var memberPropNames = new List<string>();
                var memberPropNamesHash = new Dictionary<string, int>();

                foreach (var member in axis_members)
                {
                    var member_data = new List<object>();
                    // Member Settings
                    var settings = new List<String>();
                    settings.Add(member.Caption.ToString(CultureInfo.InvariantCulture));
                    settings.Add(member.Description.ToString(CultureInfo.InvariantCulture));
                    settings.Add(member.Name.ToString(CultureInfo.InvariantCulture));
                    settings.Add(member.UniqueName.ToString(CultureInfo.InvariantCulture));
                    settings.Add(member.ChildCount.ToString(CultureInfo.InvariantCulture));
                    settings.Add(member.DrilledDown.ToString(CultureInfo.InvariantCulture));
                    settings.Add(member.LevelDepth.ToString(CultureInfo.InvariantCulture));
                    settings.Add(member.LevelName.ToString(CultureInfo.InvariantCulture));
                    String hierarchyUniqueName = GetMemberPropertyValue(member, "HIERARCHY_UNIQUE_NAME");
                    if (hierarchyUniqueName == null)
                    {
                        if (m_HierarchiesToLevelsCache.ContainsKey(member.LevelName))
                        {
                            hierarchyUniqueName = m_HierarchiesToLevelsCache[member.LevelName];
                        }
                        else
                        {
                            try
                            {
                                hierarchyUniqueName = member.ParentLevel.ParentHierarchy.UniqueName;
                            }
                            catch (System.InvalidOperationException)
                            {
                                hierarchyUniqueName = String.Empty;
                            }

                            m_HierarchiesToLevelsCache[member.LevelName] = hierarchyUniqueName;
                        }
                    }
                    settings.Add(hierarchyUniqueName.ToString(CultureInfo.InvariantCulture));
                    settings.Add(member.ParentSameAsPrevious.ToString(CultureInfo.InvariantCulture));
                    // Описание элемента
                    member_data.Add(settings.ToArray());

                    // Properties
                    var propObj = new AdomdMemberPropsObj(member.Properties);

                    var propVals = new object[propNames.Count];
                    foreach (var p in propObj.props)
                    {
                        switch (p.Name)
                        {
                            default:
                                int indx;
                                if (!propNamesHash.TryGetValue(p.Name, out indx))
                                {
                                    indx = propNames.Count;
                                    propNames.Add(p.Name);
                                    propNamesHash[p.Name] = indx;
                                    var newPropVals = new object[propNames.Count];
                                    Array.Copy(propVals, newPropVals, propVals.Length);
                                    propVals = newPropVals;
                                }
                                propVals[indx] = p.Value;
                                break;
                        }
                    }
                    // Значения свойств
                    member_data.Add(propVals.ToArray());

                    // Member Properties
                    var memberPropObj = new AdomdCustomMemberPropsObj(member.MemberProperties);
                    int indMemberProp = 0;
                    if (!equalsMemberPropertiesHash.TryGetValue(memberPropObj, out indMemberProp))
                    {
                        indMemberProp = equalsMemberPropertiesList.Count;
                        equalsMemberPropertiesHash[memberPropObj] = indMemberProp;
                        equalsMemberPropertiesList.Add(memberPropObj);
                    }

                    var memberPropVals = new object[memberPropNames.Count];
                    foreach (var p in memberPropObj.props)
                    {
                        switch (p.Name)
                        {
                            case "CUBE_NAME":
                            case "DIMENSION_UNIQUE_NAME":
                            case "HIERARCHY_UNIQUE_NAME":
                            case "LEVEL_UNIQUE_NAME":
                            case "LEVEL_NUMBER":
                            case "PARENT_COUNT":
                            case "PARENT_LEVEL":
                            case "PARENT_UNIQUE_NAME":
                            case "SKIPPED_LEVELS":
                                continue;
                            default:
                                int indx;
                                if (!memberPropNamesHash.TryGetValue(p.Name, out indx))
                                {
                                    indx = memberPropNames.Count;
                                    memberPropNames.Add(p.Name);
                                    memberPropNamesHash[p.Name] = indx;
                                    var newPropVals = new object[memberPropNames.Count];
                                    Array.Copy(memberPropVals, newPropVals, memberPropVals.Length);
                                    memberPropVals = newPropVals;
                                }
                                memberPropVals[indx] = p.Value;
                                break;
                        }
                    }
                    // Значения пользовательских свойств
                    member_data.Add(memberPropVals.ToArray());
                    // Id стиля
                    member_data.Add(indMemberProp);

                    tmp.Add(member_data.ToArray());
                }

                // Назвнаия свойств
                axis_data.Add(propNames.ToArray());
                // Назвнаия пользовательских свойств
                axis_data.Add(memberPropNames.ToArray());
                // Элементы
                axis_data.Add(tmp.ToArray());

                var equalsMemberPropNames = new List<string>();
                var equalsMemberPropNamesHash = new Dictionary<string, int>();

                var equalsPropertiesValues = new List<object>();
                for (int i = 0; i < equalsMemberPropertiesList.Count; i++)
                {
                    var po = equalsMemberPropertiesList[i];
                    var propVals = new object[equalsMemberPropNames.Count];
                    foreach (var p in po.props)
                    {
                        switch (p.Name)
                        {
                            case "CUBE_NAME":
                            case "DIMENSION_UNIQUE_NAME":
                            case "HIERARCHY_UNIQUE_NAME":
                            case "LEVEL_UNIQUE_NAME":
                            case "LEVEL_NUMBER":
                            case "PARENT_COUNT":
                            case "PARENT_LEVEL":
                            case "PARENT_UNIQUE_NAME":
                            case "SKIPPED_LEVELS":
                                int indx;
                                if (!equalsMemberPropNamesHash.TryGetValue(p.Name, out indx))
                                {
                                    indx = equalsMemberPropNames.Count;
                                    equalsMemberPropNames.Add(p.Name);
                                    equalsMemberPropNamesHash[p.Name] = indx;
                                    var newPropVals = new object[equalsMemberPropNames.Count];
                                    Array.Copy(propVals, newPropVals, propVals.Length);
                                    propVals = newPropVals;
                                }
                                propVals[indx] = p.Value;
                                break;
                            default:
                                continue;
                        }
                    }
                    // Значения для стиля
                    equalsPropertiesValues.Add(propVals.ToArray());
                }
                // Значения для стиля
                axis_data.Add(equalsPropertiesValues.ToArray());
                // стиль
                axis_data.Add(equalsMemberPropNames.ToArray());

                axes.Add(axis_data.ToArray());
            }

            writer.WriteString(Jayrock.Json.Conversion.JsonConvert.ExportToString(data.ToArray()));

            // Оси - начало
            writer.WriteStartElement("Axes");

            var result = Jayrock.Json.Conversion.JsonConvert.ExportToString(axes.ToArray());
            writer.WriteString(result);

            // Оси - конец
            writer.WriteEndElement();

            // Ячейки - начало
            writer.WriteStartElement("Cells");

            var cells = SerializeCells(CS);
            writer.WriteString(cells);

            // Ячейки - конец
            writer.WriteEndElement();

            // Конец
            writer.WriteEndElement();
        }

        internal string SerializeCells(CellSet cs)
        {
            if (cs == null)
                return String.Empty;

            var data = new List<object>();
            var propsList = new List<AdomdCellPropsObj>();
            var hash = new Dictionary<AdomdCellPropsObj, int>();
            var values = new object[cs.Cells.Count];
            data.Add(values);
            var displayValues = new string[cs.Cells.Count];
            data.Add(displayValues);
            var props = new int[cs.Cells.Count];
            data.Add(props);

            // Ячейки
            for (int indx = 0; indx < cs.Cells.Count; indx++)
            {
                Cell cell = cs.Cells[indx];

                object value = null;
                string displayName = string.Empty;

                if (cell != null)
                {
                    try
                    {
                        displayName = cell.FormattedValue;
                    }
                    catch (Exception)
                    {
                        displayName = CellValueData.ERROR;
                    }

                    try
                    {
                        value = cell.Value;
                    }
                    catch (Exception exc)
                    {
                        value = exc.ToString();
                    }

                    if (string.IsNullOrEmpty(displayName))
                    {
                        if (value == null)
                        {
                            displayName = String.Empty;
                        }
                        else
                        {
                            displayName = value.ToString();
                        }
                    }
                }

                values[indx] = value;
                displayValues[indx] = displayName;
                if (cell != null)
                {
                    var propObj = new AdomdCellPropsObj(cell.CellProperties);
                    int indProp = 0;
                    if (!hash.TryGetValue(propObj, out indProp))
                    {
                        indProp = propsList.Count;
                        hash[propObj] = indProp;
                        propsList.Add(propObj);
                    }
                    props[indx] = indProp;
                }
            }

            // Стили ячеек
            var propNames = new List<string>();
            var propNamesHash = new Dictionary<string, int>();

            for (int i = 0; i < propsList.Count; i++)
            {
                var po = propsList[i];
                var propVals = new object[propNames.Count];
                foreach (var p in po.props)
                {
                    switch (p.Name)
                    {
                        case "CellOrdinal":
                        case "VALUE":
                        case "FORMATTED_VALUE":
                            continue;
                        default:
                            int indx;
                            if (!propNamesHash.TryGetValue(p.Name, out indx))
                            {
                                indx = propNames.Count;
                                propNames.Add(p.Name);
                                propNamesHash[p.Name] = indx;
                                var newPropVals = new object[propNames.Count];
                                Array.Copy(propVals, newPropVals, propVals.Length);
                                propVals = newPropVals;
                            }
                            propVals[indx] = p.Value;
                            break;
                    }
                }
                data.Add(propVals);
            }
            data.Add(propNames.ToArray());

            var result = Jayrock.Json.Conversion.JsonConvert.ExportToString(data.ToArray());
            return result;
        }

        private Cell GetCell(params int[] index)
        {
            if (CS == null)
            {
                return null;
            }

            int[] indexVector = new int[CS.Axes.Count];
            for (int i = 0; i < indexVector.Length; i++)
            {
                indexVector[i] = 0;
            }
            index.CopyTo(indexVector, 0);

            Cell cell = null;
            try
            {
                cell = CS[indexVector];
            }
            catch (ArgumentOutOfRangeException)
            {
                return null;
            }

            return cell;
        }

    }
}
