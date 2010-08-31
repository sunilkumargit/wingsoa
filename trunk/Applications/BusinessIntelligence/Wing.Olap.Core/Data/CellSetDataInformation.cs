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

namespace Ranet.Olap.Core.Data
{
    public class CellSetDataInformation
    {
        const string XML_PropertiesInformation = "info";
        const string XML_Types = "ti";
        const string XML_CellsPropertiesNames = "cp";
        const string XML_Type = "t";
        const string XML_Name = "n";
        const string XML_Values = "vi";
        const string XML_Value = "v";


        Dictionary<int, Type> m_Types;
        /// <summary>
        /// Типы свойств
        /// </summary>
        public Dictionary<int, Type> Types
        {
            get
            {
                if (m_Types == null)
                {
                    m_Types = new Dictionary<int, Type>();
                }
                return m_Types;
            }
        }

        Dictionary<int, String> m_CellsPropertiesNames;
        /// <summary>
        /// Названия свойств ячеек
        /// </summary>
        public Dictionary<int, String> CellsPropertiesNames
        {
            get
            {
                if (m_CellsPropertiesNames == null)
                {
                    m_CellsPropertiesNames = new Dictionary<int, String>();
                }
                return m_CellsPropertiesNames;
            }
        }

        Dictionary<int, String> m_Values;
        /// <summary>
        /// Значения
        /// </summary>
        public Dictionary<int, String> Values
        {
            get
            {
                if (m_Values == null)
                {
                    m_Values = new Dictionary<int, String>();
                }
                return m_Values;
            }
        }

        internal void Serialize(XmlWriter writer)
        {
            if (writer == null)
                return;

            writer.WriteStartElement(XML_PropertiesInformation);
            
            // Типы - начало
            writer.WriteStartElement(XML_Types);
            int i = 0;
            foreach (KeyValuePair<int, Type> pair in Types)
            {
                writer.WriteAttributeString(XML_Type + i.ToString(), pair.Value.FullName.ToString(CultureInfo.InvariantCulture));
                i++;
            }
            // Типы - конец
            writer.WriteEndElement();

            // Названия - начало
            writer.WriteStartElement(XML_CellsPropertiesNames);
            i = 0;
            foreach (KeyValuePair<int, String> pair in CellsPropertiesNames)
            {
                writer.WriteAttributeString(XML_Name + i.ToString(), pair.Value.ToString(CultureInfo.InvariantCulture));
                i++;
            }
            // Названия - конец
            writer.WriteEndElement();

             // Значения - начало
            writer.WriteStartElement(XML_Values);
            i = 0;
            foreach (KeyValuePair<int, String> pair in Values)
            {
                writer.WriteAttributeString(XML_Value + i.ToString(), pair.Value.ToString(CultureInfo.InvariantCulture));
                i++;
            }
            // Значения - конец
            writer.WriteEndElement();

            writer.WriteEndElement();
        }

        internal static CellSetDataInformation Deserialize(XmlReader reader)
        {
            if (reader != null)
            {
                try
                {
                    CellSetDataInformation target = null;
                    if (reader.NodeType == XmlNodeType.Element &&
                        reader.Name == XML_PropertiesInformation)
                    {
                        target = new CellSetDataInformation();

                        reader.ReadStartElement(XML_PropertiesInformation);

                        // Типы
                        int i = 0;
                        while(reader.MoveToAttribute(XML_Type + i.ToString()))
                        {
                            String type = reader.GetAttribute(XML_Type + i.ToString());
                            if(!String.IsNullOrEmpty(type))
                            {
                                target.Types.Add(i, Type.GetType(type));
                                i++;
                            }
                        }
                        reader.ReadStartElement(XML_Types);
                        if (reader.NodeType == XmlNodeType.EndElement &&
                            reader.Name == XML_Types)
                        {
                            reader.ReadEndElement();
                        }

                        // Названия свойств ячеек
                        i = 0;
                        while (reader.MoveToAttribute(XML_Name + i.ToString()))
                        {
                            String name = reader.GetAttribute(XML_Name + i.ToString());
                            target.CellsPropertiesNames.Add(i, name);
                            i++;
                        }
                        reader.ReadStartElement(XML_CellsPropertiesNames);
                        if (reader.NodeType == XmlNodeType.EndElement &&
                            reader.Name == XML_CellsPropertiesNames)
                        {
                            reader.ReadEndElement();
                        }

                        // Значения
                        i = 0;
                        while (reader.MoveToAttribute(XML_Value + i.ToString()))
                        {
                            String formatString = reader.GetAttribute(XML_Value + i.ToString());
                            target.Values.Add(i, formatString);
                            i++;
                        }
                        reader.ReadStartElement(XML_Values);
                        if (reader.NodeType == XmlNodeType.EndElement &&
                            reader.Name == XML_Values)
                        {
                            reader.ReadEndElement();
                        } 

                        if (reader.NodeType == XmlNodeType.EndElement &&
                            reader.Name == XML_PropertiesInformation)
                        {
                            reader.ReadEndElement();
                        }
                    }
                    return target;
                }
                catch (XmlException ex)
                {
                    throw ex;
                    //return null;
                }
            }
            return null;
        }
    }
}
