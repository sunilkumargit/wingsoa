using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Wing.Utils;

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
