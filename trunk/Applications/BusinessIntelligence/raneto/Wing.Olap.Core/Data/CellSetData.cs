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
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using System.Globalization;
using Jayrock.Json.Conversion;
using Jayrock.Json;

namespace Ranet.Olap.Core.Data
{
    public class CellSetData
    {
        const string XML_CellSetData = "csd";

        string m_CubeName = String.Empty;
        public string CubeName
        {
            get { return m_CubeName; }
            set { m_CubeName = value; }
        }

        ConnectionInfo m_Connection = new ConnectionInfo();
        public ConnectionInfo Connection
        {
            get {
                if (m_Connection == null)
                    m_Connection = new ConnectionInfo();
                return m_Connection; 
            }
            set { m_Connection = value; }
        }


        List<AxisData> m_Axes = null;
        /// <summary>
        /// Оси
        /// </summary>
        public List<AxisData> Axes
        {
            get
            {
                if (m_Axes == null)
                    m_Axes = new List<AxisData>();
                return m_Axes;
            }
            set { m_Axes = value; }
        }

        List<CellData> m_Cells = null;
        /// <summary>
        /// Ячейки
        /// </summary>
        public List<CellData> Cells
        {
            get
            {
                if (m_Cells == null)
                    m_Cells = new List<CellData>();
                return m_Cells;
            }
            set { 
                m_Cells = value;
                m_Cells2D = new Cache2D<CellData>();
            }
        }

        // Пока информация о FilterAxis не нужна
        //AxisData m_FilterAxis = null;
        //public AxisData FilterAxis
        //{
        //    get { return m_FilterAxis; }
        //    set { m_FilterAxis = value; }
        //}

        public CellData GetCellDescription(int col)
        {
            //if (col >= 0)
            {
                foreach (CellData cell in Cells)
                {
                    if (cell.Axis0_Coord == col)
                    {
                        return cell;
                    }
                }
            }

            // Ячейка не найдена. Возможно это ячейка, которой не существует в кубе (null)
            // При некоторых настройках безопасности в CellSet есть мемберы а коллекция Cells пустая.
            var cell_empty = new CellData() { Axis0_Coord = col};
            return cell_empty;
        }

        public CellData GetCellDescription(int col, int row)
        {
            CellData cell = m_Cells2D[col, row];
            if (cell == null)
            {
                if (col >= 0 && row >= 0)
                {
                    foreach (CellData c in Cells)
                    {
                        if (c.Axis0_Coord == col &&
                            c.Axis1_Coord == row)
                        {
                            m_Cells2D.Add(c, col, row);
                            return c;
                        }
                    }
                }
            }

            if (cell == null)
            {
                // Ячейка не найдена. Возможно это ячейка, которой не существует в кубе (null)
                // При некоторых настройках безопасности в CellSet есть мемберы а коллекция Cells пустая.
                cell = new CellData() { Axis0_Coord = col, Axis1_Coord = row };
            }
            return cell;
        }

        public static CellSetData Parse(string xml)
        {
            return XmlSerializationUtility.XmlStr2Obj<CellSetData>(xml);
        }

        Cache2D<CellData> m_Cells2D = new Cache2D<CellData>();

        internal void DeserializeData(string DataStr)
        {
            var cellDatas = Jayrock.Json.Conversion.JsonConvert.Import(DataStr) as JsonArray;
            var Values = cellDatas.GetArray(0);
            var DisplayValues = cellDatas.GetArray(1);
            var Styles = cellDatas.GetArray(2);
            int axes0Len = this.Axes.Count > 0 ? this.Axes[0].Positions.Count : 0;
            int axes1Len = this.Axes.Count > 1 ? this.Axes[1].Positions.Count : 0;
            var PropNames = cellDatas[cellDatas.Length - 1] as JsonArray;
            int CellOrdinal = 0;

            int cellsCount = Values.Count;
            for (int j = 0; j < cellsCount; j++)
            {
                var cellData = new CellData();
                cellData.Axis0_Coord = axes0Len > 0 ? j % axes0Len : -1;    // axes0Len может быть 0 и при этом будет одна ячейка (дефолтная). И осей при этом в CellSet нету.
                cellData.Axis1_Coord = axes1Len > 0 ? j / axes0Len : -1;
                var cellValueData = new CellValueData();
                var prop = new PropertyData("CellOrdinal", CellOrdinal);
                cellValueData.Properties.Add(prop);
                object val = ConvertFromJson(Values[CellOrdinal]);
                cellValueData.Value = val;
                prop = new PropertyData("VALUE", val);
                cellValueData.Properties.Add(prop);
                var props = cellDatas.GetArray(3 + Styles.GetInt32(CellOrdinal));
                string FORMAT_STRING = null;
                for (int k = 0; k < PropNames.Length; k++)
                {
                    var propName = PropNames[k].ToString();
                    //if (propName == "FORMAT_STRING")
                    //    FORMAT_STRING = (string)propval;
                    prop = new PropertyData(propName, ConvertFromJson(props[k]));
                    cellValueData.Properties.Add(prop);
                }

                //if (val == null)
                //    cellValueData.DisplayValue = null;
                //else if (FORMAT_STRING != null)
                //    cellValueData.DisplayValue = ((double)val).ToString(FORMAT_STRING);
                //else
                //    cellValueData.DisplayValue = val.ToString();
                cellValueData.DisplayValue = DisplayValues[CellOrdinal++].ToString();

                prop = new PropertyData("FORMATTED_VALUE", cellValueData.DisplayValue);
                cellValueData.Properties.Add(prop);
                cellData.Value = cellValueData;
                
                Cells.Add(cellData);
            }
        }

        internal static CellSetData Deserialize(XmlReader reader)
        {
            if (reader != null)
            {
                try
                {
                    if (!(reader.NodeType == XmlNodeType.Element &&
                        reader.Name == XML_CellSetData))
                    {
                        reader.ReadToFollowing(XML_CellSetData);
                    }

                    CellSetData target = new CellSetData();

                    // Начало - CellSetData
                    reader.ReadStartElement(XML_CellSetData);

                    var data = Jayrock.Json.Conversion.JsonConvert.Import(reader.Value) as JsonArray;
                    
                    // Имя куба
                    target.CubeName = data[0] != null ? data[0].ToString() : String.Empty;

                    // Соединение
                    var connection = data.GetArray(1);
                    target.Connection.ConnectionID = connection[0].ToString();
                    target.Connection.ConnectionString = connection[1].ToString();

                    reader.Read();

                    // Оси
                    reader.ReadStartElement("Axes");

                    var axes = Jayrock.Json.Conversion.JsonConvert.Import(reader.Value) as JsonArray;
                    for (int a = 0; a < axes.Count; a++)
                    {
                        var axis_data = axes.GetArray(a);
                        AxisData axis = new AxisData();
                        axis.AxisNum = a;
                        // Название оси
                        axis.Name = axis_data[0].ToString();
                        // Позиции 
                        var positions = axis_data.GetArray(1);

                        for (int p = 0; p < positions.Count; p++)
                        {
                            var position_data = positions.GetArray(p);
                            PositionData pos = new PositionData();

                            for (int m = 0; m < position_data.Count; m++)
                            {
                                var member_data = position_data.GetArray(m);
                                PositionMemberData member = new PositionMemberData(Convert.ToInt32(member_data[0]));
                                member.DrilledDown = Convert.ToBoolean(member_data[1]);
                                pos.Members.Add(member);
                            }

                            axis.Positions.Add(pos);
                        }

                        // Названаия свойств
                        var PropertiesNames = axis_data.GetArray(2);
                        // Названаия пользовательских свойств
                        var MemberPropertiesNames = axis_data.GetArray(3);
                        // Элементы оси
                        var members = axis_data.GetArray(4);

                        // Варианты стиля
                        var equalsMemberProps = axis_data.GetArray(5);
                        // Описание стиля
                        var equalsMemberPropertiesNames = axis_data.GetArray(6);

                        for (int m = 0; m < members.Count; m++)
                        {
                            var member_data = members.GetArray(m);
                            var Settings = member_data.GetArray(0);
                            var PropertiesValues = member_data.GetArray(1);
                            var MemberPropertiesValues = member_data.GetArray(2);
                            int MemberPropertiesStyleId = Convert.ToInt32(member_data[3]);

                            MemberData member = new MemberData();
                            int x = 0;
                            member.Caption = Settings[x++].ToString();
                            member.Description = Settings[x++].ToString();
                            member.Name = Settings[x++].ToString();
                            member.UniqueName = Settings[x++].ToString();
                            member.ChildCount = Convert.ToInt32(Settings[x++].ToString());
                            member.DrilledDown = Convert.ToBoolean(Settings[x++].ToString());
                            member.LevelDepth = Convert.ToInt32(Settings[x++].ToString());
                            member.LevelName = Settings[x++].ToString();
                            member.HierarchyUniqueName = Settings[x++].ToString();
                            member.ParentSameAsPrevious = Convert.ToBoolean(Settings[x++].ToString());

                            for (int j = 0; j < PropertiesValues.Length; j++)
                            {
                                member.Properties.Add(new PropertyData(PropertiesNames[j].ToString(), ConvertFromJson(PropertiesValues[j])));
                            }
                            for (int j = 0; j < MemberPropertiesValues.Length; j++)
                            {
                                member.MemberProperties.Add(new PropertyData(MemberPropertiesNames[j].ToString(), ConvertFromJson(MemberPropertiesValues[j])));
                            }

                            var member_equalsPropsValues = equalsMemberProps.GetArray(MemberPropertiesStyleId);
                            for (int k = 0; k < equalsMemberPropertiesNames.Length; k++)
                            {
                                var propName = equalsMemberPropertiesNames[k].ToString();
                                member.MemberProperties.Add(new PropertyData(propName, ConvertFromJson(member_equalsPropsValues[k])));
                            }

                            axis.Members.Add(axis.Members.Count, member);
                        }

                        target.Axes.Add(axis);
                    }

                    reader.Read();

                    if (reader.NodeType == XmlNodeType.EndElement &&
                        reader.Name == "Axes")
                    {
                        reader.ReadEndElement();
                    }

                    // Ячейки
                    reader.ReadStartElement("Cells");

                    var strData = reader.Value;
                    target.DeserializeData(strData);
                    reader.Read();

                    if (reader.NodeType == XmlNodeType.EndElement &&
                        reader.Name == "Cells")
                    {
                        reader.ReadEndElement();
                    }
                    // Конец - CellSetData
                    if (reader.NodeType == XmlNodeType.EndElement &&
                        reader.Name == XML_CellSetData)
                    {
                        reader.ReadEndElement();
                    }
                    return target;
                }
                catch (XmlException)
                {
                    throw;
                }
            }
            return null;
        }

        public static object ConvertFromJson(object val)
        {
            object propval = val;
            if (propval != null)
            {
                if (propval is JsonString)
                {
                    propval = propval.ToString();
                }
                else if (propval is JsonNumber)
                {
                    var propvalStr = propval.ToString();
                    if (propvalStr.Contains('.'))
                        propval = ((JsonNumber)propval).ToDouble();
                    else
                    {
                        propval = ((JsonNumber)propval).ToInt64();
                    }
                }
            }
            return propval;
        }

        public static CellSetData Deserialize(String str)
        {
            if (!String.IsNullOrEmpty(str))
            {
                StringReader str_reader = new StringReader(str);
                XmlReader reader = XmlReader.Create(str_reader);

                return CellSetData.Deserialize(reader);
            }
            else
                return null;
        }
    }
}
