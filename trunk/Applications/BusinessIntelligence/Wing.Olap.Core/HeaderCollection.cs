/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
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
