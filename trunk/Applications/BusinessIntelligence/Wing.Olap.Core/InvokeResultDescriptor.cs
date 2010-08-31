/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace Wing.Olap.Core
{
    public class InvokeResultDescriptor
    {
        public const String CONNECTION_ID = "ConnectionId";
        public const String SESSION_ID = "SessionId";
        public String Content = String.Empty;
        public InvokeContentType ContentType = InvokeContentType.Unknown;
        public bool IsArchive = false;
        public String SessionId = String.Empty;

        HeaderCollection m_Headers;
        public HeaderCollection Headers
        {
            get {
                if (m_Headers == null) { m_Headers = new HeaderCollection(); }
                return m_Headers;
            }
        }

        public InvokeResultDescriptor()
        { 
        }

        public InvokeResultDescriptor(String content)
        {
            Content = content;
        }

        public InvokeResultDescriptor(String content, InvokeContentType contentType)
            : this(content)
        {
            ContentType = contentType;
        }

        internal void Serialize(XmlWriter writer)
        {
            if (writer == null)
                return;

            // Начало
            writer.WriteStartElement("InvokeResultDescriptor");
            // Свойства
            writer.WriteElementString("Content", this.Content.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString("ContentType", this.ContentType.ToString());
            writer.WriteElementString("IsArchive", this.IsArchive.ToString(CultureInfo.InvariantCulture));
            writer.WriteStartElement("Headers");
            foreach (Header item in Headers)
            {
                item.Serialize(writer);
            }
            writer.WriteEndElement();
            // Конец
            writer.WriteEndElement();
        }

        public static String Serialize(InvokeResultDescriptor cs)
        {
            StringBuilder sb = new StringBuilder();

            if (cs != null)
            {
                XmlWriter writer = XmlWriter.Create(sb);
                cs.Serialize(writer);
                writer.Close();
            }
            return sb.ToString();
        }

        internal static InvokeResultDescriptor Deserialize(XmlReader reader)
        {
            if (reader != null)
            {
                try
                {
                    if (!(reader.NodeType == XmlNodeType.Element &&
                        reader.Name == "InvokeResultDescriptor"))
                    {
                        reader.ReadToFollowing("InvokeResultDescriptor");
                    }

                    InvokeResultDescriptor target = new InvokeResultDescriptor();
                    // Начало - InvokeResultDescriptor
                    reader.ReadStartElement("InvokeResultDescriptor");

                    // Свойства
                    reader.ReadStartElement("Content");
                    if (reader.NodeType == XmlNodeType.Text)
                    {
                        target.Content = reader.ReadContentAsString();
                        reader.ReadEndElement();
                    }

                    reader.ReadStartElement("ContentType");
                    if (reader.NodeType == XmlNodeType.Text)
                    {
                        target.ContentType = (InvokeContentType)(InvokeContentType.Parse(typeof(InvokeContentType), reader.ReadContentAsString(), true));
                        reader.ReadEndElement();
                    }

                    reader.ReadStartElement("IsArchive");
                    if (reader.NodeType == XmlNodeType.Text)
                    {
                        target.IsArchive = bool.Parse(reader.ReadContentAsString());
                        reader.ReadEndElement();
                    }

                    // Элементы
                    Header header = null;
                    reader.ReadStartElement("Headers");
                    do
                    {
                        header = Header.Deserialize(reader);
                        if (header != null)
                            target.Headers.Add(header);
                    } while (header != null);
                    if (reader.NodeType == XmlNodeType.EndElement &&
                        reader.Name == "Headers")
                    {
                        reader.ReadEndElement();
                    }

                    // Конец - InvokeResultDescriptor
                    if (reader.NodeType == XmlNodeType.EndElement &&
                        reader.Name == "InvokeResultDescriptor")
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

        public static InvokeResultDescriptor Deserialize(String str)
        {
            if (!String.IsNullOrEmpty(str))
            {
                StringReader str_reader = new StringReader(str);
                XmlReader reader = XmlReader.Create(str_reader);

                return InvokeResultDescriptor.Deserialize(reader);
            }
            else
                return null;
        }
    }

    public enum InvokeContentType
    {
        Error,
        UpdateResult,
        MultidimData,
        Unknown
    }
}

