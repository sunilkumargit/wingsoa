
using System;
using System.Security.Cryptography;
using System.Xml.Serialization;

namespace Wing.Utils
{
    public class AssemblyInfo
    {
        [XmlAttribute("name")]
        public String AssemblyName { get; set; }

        [XmlAttribute("size")]
        public int Size { get; set; }

        [XmlAttribute("hash")]
        public String HashString { get; set; }

        public static String CalculateHashString(byte[] assemblyData)
        {
            var hash = new SHA256Managed();
            var hashBytes = hash.ComputeHash(assemblyData);
            return Converter.ByteArrayToHexString(hashBytes);
        }
    }
}
