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
    public class ServerConfigManagerService : IServerConfigManagerService
    {
        private string _configPath;
        private Dictionary<String, IServerConfigSection> _sections = new Dictionary<string, IServerConfigSection>();
        private TypedXmlSerializer<ServerConfigSectionWrapper> _sectionSerializer = new Utils.TypedXmlSerializer<ServerConfigSectionWrapper>();

        public ServerConfigManagerService(BootstrapSettings settings)
        {
            //open file
            _configPath = Path.Combine(settings.ServerDataStorePath, "Config");
            Directory.CreateDirectory(_configPath);
            //carregar a lista de seções
            LoadSections();
        }

        private void LoadSections()
        {
            _sections.Clear();
            var files = Directory.GetFiles(_configPath, "*.section");
            foreach (var fileName in files)
            {
                try
                {
                    var wrapper = _sectionSerializer.DeserializeFromFile(fileName);
                    _sections[wrapper.SectionName.ToLower()] = new ServerConfigSection(this, wrapper);
                }
                catch { }
            }
        }

        #region IServerConfigManagerService Members

        public string[] GetSectionsNames()
        {
            return _sections.Values.Select(c => c.SectionName).ToArray();
        }

        public IServerConfigSection GetSection(string name, bool createIfNotExists = true)
        {
            IServerConfigSection section = null;
            lock (this)
            {
                if (!_sections.TryGetValue(name.ToString().ToLower(), out section))
                {
                    if (createIfNotExists)
                    {
                        section = new ServerConfigSection(this, new ServerConfigSectionWrapper() { SectionName = name });
                        _sections[section.SectionName.ToLower()] = section;
                        return section;
                    }
                    else
                        throw new Exception(String.Format("Section does not exists: {0}", name));
                }
            }
            return section;
        }

        #endregion

        internal void SaveSection(ServerConfigSection configSection)
        {
            var wrapper = configSection.Wrapper;
            var fileName = Path.Combine(_configPath, wrapper.SectionId + ".section");
            _sectionSerializer.SerializeToFile(fileName, wrapper);
        }
    }
}