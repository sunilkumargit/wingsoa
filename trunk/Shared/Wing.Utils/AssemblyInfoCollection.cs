using System;
using System.Net;
using System.Xml.Serialization;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Wing.Utils
{
    [XmlRoot("assembly-info")]
    public class AssemblyInfoCollection
    {
        public AssemblyInfoCollection()
        {
            Assemblies = new List<AssemblyInfo>();
        }

        [XmlElement("assembly")]
        public List<AssemblyInfo> Assemblies { get; set; }

        public List<AssemblyInfo> GetModifiedAssemblies(List<AssemblyInfo> source)
        {
            //procurar nessa lista os assemblies que não existam não lista source ou que tenham o hash diferente.
            return Assemblies.Where(a =>
            {
                var sourceInfo = source.FirstOrDefault(b => b.AssemblyName.Equals(a.AssemblyName, StringComparison.OrdinalIgnoreCase));
                return sourceInfo == null
                    || (!sourceInfo.HashString.Equals(a.HashString, StringComparison.OrdinalIgnoreCase));
            }).ToList();
        }

        public string SerializeToXml()
        {
            var serializer = new XmlSerializer(this.GetType());
            var sb = new StringBuilder();
            var writer = new StringWriter(sb);
            serializer.Serialize(writer, this);
            writer.Close();
            writer.Dispose();
            return sb.ToString();
        }

        public static AssemblyInfoCollection DeserializeFromXml(String xml)
        {
            var serializer = new XmlSerializer(typeof(AssemblyInfoCollection));
            var reader = new StringReader(xml);
            var result = (AssemblyInfoCollection)serializer.Deserialize(reader);
            reader.Dispose();
            return result;
        }
    }
}
