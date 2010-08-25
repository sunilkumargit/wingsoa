/*   
    Copyright (C) 2009 Galaktika Corporation ZAO

    This file is a part of Wing.UILibrary.Olap
 
    Wing.UILibrary.Olap is a free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
      
    You should have received a copy of the GNU General Public License
    along with Wing.UILibrary.Olap.  If not, see <http://www.gnu.org/licenses/>.
  
    If GPL v.3 is not suitable for your products or company,
    Galaktika Corp provides Wing.UILibrary.Olap under a flexible commercial license
    designed to meet your specific usage and distribution requirements.
    If you have already obtained a commercial license from Galaktika Corp,
    you can use this file under those license terms.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Xml;
using System.Globalization;

namespace Wing.Olap.Core
{
    public class Header
    {
        public String Key { get; set; }
        public String Value { get; set; }

        public Header()
        {
        }

        public Header(string key, string value)
        {
            Key = key;
            Value = value;
        }

        const string XML_Header = "Header";
        const string XML_Key = "key";
        const string XML_Value = "value";

        public void Serialize(XmlWriter writer)
        {
            if (writer == null)
                return;

            writer.WriteStartElement(XML_Header);

            // Свойства
            writer.WriteAttributeString(XML_Key, this.Key.ToString(CultureInfo.InvariantCulture));
            writer.WriteAttributeString(XML_Value, this.Value.ToString(CultureInfo.InvariantCulture));

            writer.WriteEndElement();
        }

        public static Header Deserialize(XmlReader reader)
        {
            if (reader != null)
            {
                try
                {
                    Header target = null;
                    if (reader.NodeType == XmlNodeType.Element &&
                        reader.Name == XML_Header)
                    {
                        target = new Header();

                        if (reader.MoveToAttribute(XML_Key))
                        {
                            target.Key = reader.GetAttribute(XML_Key);
                        }

                        if (reader.MoveToAttribute(XML_Value))
                        {
                            target.Value = reader.GetAttribute(XML_Value);
                        }

                        reader.ReadStartElement(XML_Header);
                        if (reader.NodeType == XmlNodeType.EndElement &&
                            reader.Name == XML_Header)
                        {
                            reader.ReadEndElement();
                        }
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
    }

    public class HeaderCollection : System.Collections.ObjectModel.KeyedCollection<String, Header>
    {
        public HeaderCollection()
            : base()
        {
        }

        protected override String GetKeyForItem(Header item)
        {
            // In this example, the key is the part number.
            return item.Key;
        }

        public new IEnumerator GetEnumerator()
        {
            if (base.Dictionary != null)
                return base.Dictionary.Values.GetEnumerator();
            return new List<Header>().GetEnumerator();
        }
    }
}
