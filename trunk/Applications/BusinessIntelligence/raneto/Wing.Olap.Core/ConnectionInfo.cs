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

namespace Ranet.Olap.Core
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
