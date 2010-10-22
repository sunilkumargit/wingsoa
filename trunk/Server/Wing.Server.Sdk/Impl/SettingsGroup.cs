using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Wing.Utils;

namespace Wing.Server.Impl
{
    public class SettingsGroup : ISettingsGroup
    {
        private string _configPath;
        private Dictionary<String, ISettingsSection> _sections = new Dictionary<string, ISettingsSection>();
        private TypedXmlSerializer<SettingsSectionWrapper> _sectionSerializer = new TypedXmlSerializer<SettingsSectionWrapper>();

        public SettingsGroup(String directoryPath, String name)
        {
            //open file
            _configPath = directoryPath;
            this.GroupName = name;
            //carregar a lista de seções
            LoadSections();
        }

        public String GroupName { get; private set; }

        private void LoadSections()
        {
            _sections.Clear();
            if (Directory.Exists(_configPath))
            {
                var files = Directory.GetFiles(_configPath, "*.section");
                foreach (var fileName in files)
                {
                    try
                    {
                        var wrapper = _sectionSerializer.DeserializeFromFile(fileName);
                        _sections[wrapper.SectionName.ToLower()] = new SettingsSection(this, wrapper);
                    }
                    catch { }
                }
            }
        }

        public string[] GetSectionsNames()
        {
            return _sections.Values.Select(c => c.SectionName).ToArray();
        }

        public ISettingsSection GetSection(string name, bool createIfNotExists = true)
        {
            ISettingsSection section = null;
            lock (this)
            {
                if (!_sections.TryGetValue(name.ToString().ToLower(), out section))
                {
                    if (createIfNotExists)
                    {
                        section = new SettingsSection(this, new SettingsSectionWrapper() { SectionName = name });
                        _sections[section.SectionName.ToLower()] = section;
                        return section;
                    }
                    else
                        throw new Exception(String.Format("Section does not exists: {0}", name));
                }
            }
            return section;
        }

        internal void SaveSection(SettingsSection configSection)
        {
            var wrapper = configSection.Wrapper;
            var fileName = Path.Combine(_configPath, wrapper.SectionId);
            Directory.CreateDirectory(_configPath);
            _sectionSerializer.SerializeToFile(fileName, wrapper);
        }
    }
}