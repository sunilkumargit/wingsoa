using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Wing.Utils
{
    public class TypedXmlSerializer<TType>
    {
        private XmlSerializer _serializer;

        public TypedXmlSerializer()
        {
            _serializer = new XmlSerializer(typeof(TType));
        }

        public TType Deserialize(Stream stream)
        {
            return (TType)_serializer.Deserialize(stream);
        }

        public TType DeserializeFromFile(String path)
        {
            Stream stream = File.OpenRead(path);
            TType result = Deserialize(stream);
            stream.Close();
            return result;
        }

        public void Serialize(Stream stream, TType instance)
        {
            _serializer.Serialize(stream, instance);
        }

        public void SerializeToFile(String path, TType instance)
        {
            var sb = new StringBuilder();
            var stream = new StringWriter(sb);
            _serializer.Serialize(stream, instance);
            stream.Close();
            File.WriteAllText(path, sb.ToString());
        }

        public String SerializeToString(TType instance)
        {
            MemoryStream stream = new MemoryStream();
            try
            {
                Serialize(stream, instance);
                byte[] buffer = new byte[stream.Length];
                stream.Position = 0;
                stream.Read(buffer, 0, buffer.Length);
                return UTF8Encoding.UTF8.GetString(buffer);
            }
            finally
            {
                stream.Close();
            }
        }

        public TType DeserializeFromString(String xml)
        {
            MemoryStream stream = new MemoryStream();
            try
            {
                byte[] buffer = UTF8Encoding.UTF8.GetBytes(xml);
                stream.Write(buffer, 0, buffer.Length);
                stream.Position = 0;
                return Deserialize(stream);
            }
            finally
            {
                stream.Close();
            }
        }
    }
}
