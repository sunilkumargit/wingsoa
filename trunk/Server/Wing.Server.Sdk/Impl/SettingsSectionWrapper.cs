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

namespace Wing.Server.Impl
{
    [XmlRoot("config-section")]
    public class SettingsSectionWrapper
    {
        private string _sectionName;

        public SettingsSectionWrapper()
        {
            Properties = new List<SettingsSectionItem>();
        }

        [XmlIgnore]
        public String SectionId { get; private set; }

        [XmlAttribute("section-name")]
        public String SectionName
        {
            get { return _sectionName; }
            set
            {
                _sectionName = value;
                SectionId = FormatUtils.CreateComponentName(_sectionName) + ".section";
            }
        }

        [XmlElement("property")]
        public List<SettingsSectionItem> Properties { get; set; }
    }
}
