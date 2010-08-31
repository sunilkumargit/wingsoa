/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace Wing.Olap.Core
{
    /// <summary>
    /// Helper класс для работы с XML
    /// </summary>
    public static class XmlSerializationUtility
    {
        /// <summary>
        /// Сериализует объект в строку XML
        /// </summary>
        public static string Obj2XmlStr(object obj)
        {
            return Obj2XmlStr(obj, String.Empty);
        }

        /// <summary>
        /// Сериализует объект в строку XML
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="nameSpace"></param>
        /// <returns></returns>
        public static string Obj2XmlStr(object obj, string nameSpace)
        {
            if (obj == null) return string.Empty;
            XmlSerializer sr = SerializerCache.GetSerializer(obj.GetType()); //new XmlSerializer(obj.GetType(), nameSpace);
            
            StringBuilder sb = new StringBuilder();
            //MemoryStream stream = new MemoryStream();
            //XmlTextWriter writer = new XmlTextWriter(stream, Settings.Encoding);
            //writer.Formatting = Formatting.Indented;			
            StringWriterUTF8 w = new StringWriterUTF8(sb, System.Globalization.CultureInfo.InvariantCulture);
            
            sr.Serialize(
                w, 
                obj);
            //sb.Append(Settings.Encoding.GetChars(stream.ToArray()));
            //sb.Remove(0,40); 
            return sb.ToString();
        }


        public static T XmlStr2Obj<T>(string xml)
        {
            return (T)XmlStr2Obj(typeof(T), xml);
            //if (xml == null) throw new ArgumentException("Xml is null or empty");
            //if (xml == string.Empty) return (T)Activator.CreateInstance(typeof(T));

            //StringReader reader = new StringReader(xml);
            //XmlSerializer sr = SerializerCache.GetSerializer(typeof(T));//new XmlSerializer(type);
            //return (T)sr.Deserialize(reader);
        }

        public static object XmlStr2Obj(Type type, string xml)
        {
            if(type == null)
                throw new ArgumentNullException("type");
            if (xml == null) throw new ArgumentException("Xml is null or empty");
            if (xml == string.Empty) return Activator.CreateInstance(type);

            StringReader reader = new StringReader(xml);
            XmlSerializer sr = SerializerCache.GetSerializer(type);
            return sr.Deserialize(reader);
        }

        
    }

    internal class SerializerCache
    {
        private static Dictionary<string, XmlSerializer> hash = new Dictionary<string, XmlSerializer>();
        public static XmlSerializer GetSerializer(Type type)
        {
            XmlSerializer res = null;
            lock(hash)
            {
                hash.TryGetValue(type.FullName, out res);
                if(res == null)
                {
                    res = new XmlSerializer(type);
                    hash[type.FullName] = res;
                }
            }
            return res;
        }
    }

    internal class StringWriterUTF8 : StringWriter
    {
        public StringWriterUTF8() : base() {}
        public StringWriterUTF8(IFormatProvider provider) : base(provider) {}
        public StringWriterUTF8(StringBuilder builder) : base(builder) {}
        public StringWriterUTF8(StringBuilder builder, IFormatProvider provider) :
            base(builder, provider) {}

        public override Encoding Encoding
        {
            get
            {
                return Encoding.UTF8;
            }
        }

    }
}
