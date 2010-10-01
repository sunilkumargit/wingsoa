using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wing.Modularity;
using Wing.EntityStore;
using Wing.Utils;
using Wing.Server.Core;
using System.IO;
using System.Xml.Serialization;
using System.Collections.ObjectModel;
using Wing.Utils;

namespace Wing.Server.Modules.ServerConfigManager
{
    public class ServerConfigSectionWrapper
    {
        private string _sectionName;

        public ServerConfigSectionWrapper()
        {
            Properties = new List<ServerConfigSectionEntry>();
            SectionId = Guid.NewGuid().ToString("N");
        }

        [XmlAttribute("section-id")]
        public String SectionId { get; set; }

        [XmlAttribute("section-name")]
        public String SectionName { get; set; }

        [XmlElement("property")]
        public List<ServerConfigSectionEntry> Properties { get; set; }
    }
}
