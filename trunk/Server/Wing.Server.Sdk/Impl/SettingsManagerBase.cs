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
    public class SettingsManagerBase : ISettingsManager
    {
        private string _basePath;
        private Dictionary<String, ISettingsGroup> _groups = new Dictionary<string, ISettingsGroup>();

        public SettingsManagerBase(String basePath)
        {
            _basePath = basePath;
        }

        public string[] GetGroupsNames()
        {
            if (Directory.Exists(_basePath))
                return Directory.GetDirectories(_basePath, "*.group").Select(s => s.Replace(".group", "")).ToArray();
            return new String[0];
        }

        public ISettingsGroup GetGroup(string name, bool createIfNotExists = true)
        {
            ISettingsGroup group = null;
            lock (this)
            {
                if (!_groups.TryGetValue(name.ToString().ToLower(), out group))
                {
                    var groupPath = Path.Combine(_basePath, name + ".group");
                    //existe o diretorio
                    if (Directory.Exists(groupPath) || createIfNotExists)
                    {
                        group = new SettingsGroup(groupPath, name);
                        _groups[group.GroupName.ToLower()] = group;
                        return group;
                    }
                    else
                        throw new Exception(String.Format("Section group does not exists: {0}", name));
                }
            }
            return group;
        }

        public ISettingsSection GetSection(String groupName, String sectionName, bool createIfNotExists = true)
        {
            return GetGroup(groupName, createIfNotExists).GetSection(sectionName, createIfNotExists);
        }
    }
}