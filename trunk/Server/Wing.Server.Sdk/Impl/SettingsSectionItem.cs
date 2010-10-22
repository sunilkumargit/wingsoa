using System;
using System.Xml.Serialization;

namespace Wing.Server.Impl
{
    public class SettingsSectionItem
    {
        [XmlAttribute("name")]
        public String Name { get; set; }

        [XmlElement("value")]
        public Object Value { get; set; }
    }
}
