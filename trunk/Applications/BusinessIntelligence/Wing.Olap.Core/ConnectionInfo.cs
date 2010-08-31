/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Globalization;
using System.Xml;

namespace Wing.Olap.Core
{
    public class ConnectionInfo
    {
        public ConnectionInfo()
        { }

        public ConnectionInfo(String connectionID, String connectionString)
        {
            m_ConnectionID = connectionID;
            m_ConnectionString = connectionString;
        }

        String m_ConnectionString = String.Empty;
        public String ConnectionString
        {
            get { return m_ConnectionString; }
            set { m_ConnectionString = value; }
        }

        String m_ConnectionID = String.Empty;
        public String ConnectionID
        {
            get { return m_ConnectionID; }
            set { m_ConnectionID = value; }
        }

        internal void Serialize(XmlWriter writer)
        {
            if (writer == null)
                return;

            writer.WriteStartElement("ConnectionInfo");
            // Свойства
            writer.WriteElementString("ConnectionString", this.ConnectionString.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString("ConnectionID", this.ConnectionID.ToString(CultureInfo.InvariantCulture));

            writer.WriteEndElement();
        }

        internal static ConnectionInfo Deserialize(XmlReader reader)
        {
            if (reader != null)
            {
                try
                {
                    ConnectionInfo target = new ConnectionInfo();
                    reader.ReadStartElement("ConnectionInfo");

                    // Свойства
                    reader.ReadStartElement("ConnectionString");
                    if (reader.NodeType == XmlNodeType.Text)
                    {

                        target.ConnectionString = reader.ReadContentAsString();
                        reader.ReadEndElement();
                    }
                    reader.ReadStartElement("ConnectionID");
                    if (reader.NodeType == XmlNodeType.Text)
                    {

                        target.ConnectionID = reader.ReadContentAsString();
                        reader.ReadEndElement();
                    }

                    if (reader.NodeType == XmlNodeType.EndElement &&
                        reader.Name == "ConnectionInfo")
                    {
                        reader.ReadEndElement();
                    }
                    return target;
                }
                catch (XmlException)
                {
                    throw;
                    //return null;
                }
            }
            return null;
        }
    }
}
