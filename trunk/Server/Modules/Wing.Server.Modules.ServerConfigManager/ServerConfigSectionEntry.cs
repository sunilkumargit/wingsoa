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
    public class ServerConfigSectionEntry
    {
        [XmlAttribute("name")]
        public String Name { get; set; }

        [XmlElement("value")]
        public Object Value { get; set; }
    }
}
