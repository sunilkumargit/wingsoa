using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Wing.Utils;
using Wing.Logging;
using Wing.Server.Modules.ServerConfigManager.Entities;
using Wing.EntityStore;

namespace Wing.Server.Modules.ServerConfigManager
{
    public class EntityStoreSettingsGroup<TGroupEntityType, TSectionEntityType, TPropertyEntityType> : ISettingsGroup
        where TGroupEntityType : SettingsGroupEntityBase, new()
        where TSectionEntityType : SettingsSectionEntityBase, new()
        where TPropertyEntityType : SettingsPropertyEntityBase, new()
    {
        private Dictionary<String, ISettingsSection> _sections = new Dictionary<string, ISettingsSection>();
        private IEntityStore _store;
        private Object __lockObject = new Object();
        private bool _writePending;

        public EntityStoreSettingsGroup(String groupName, IEntityStore store, bool isNew)
        {
            GroupName = groupName;
            _store = store;
            _writePending = isNew;
        }

        public String GroupName { get; private set; }

        public string[] GetSectionsNames()
        {
            var query = _store.CreateQuery<TSectionEntityType>();
            query.AddFilterEqual("GroupName", GroupName);
            return query.Find().Select(s => s.SectionName).ToArray();
        }

        public ISettingsSection GetSection(string name, bool createIfNotExists = true)
        {
            ISettingsSection section = null;
            lock (this)
            {
                if (!_sections.TryGetValue(name.ToString().ToLower(), out section))
                {
                    var sectionEntity = GetSectionEntity(name);
                    if (sectionEntity != null || createIfNotExists)
                    {
                        var sectionName = name;
                        if (sectionEntity != null)
                            sectionName = sectionEntity.SectionName;
                        section = new EntityStoreSettingsSection<TGroupEntityType, TSectionEntityType, TPropertyEntityType>(this, sectionName, _store, sectionEntity == null);
                        _sections[section.SectionName.ToLower()] = section;
                        return section;
                    }
                    else
                        throw new Exception(String.Format("Section does not exists: {0}", name));
                }
            }
            return section;
        }

        private TSectionEntityType GetSectionEntity(String name)
        {
            var sectionQuery = _store.CreateQuery<TSectionEntityType>();
            sectionQuery.AddFilterEqual("GroupName", GroupName);
            sectionQuery.AddFilterEqual("SectionName", name);
            return sectionQuery.FindFirst();
        }

        internal void SaveGroup()
        {
            if (_writePending)
            {
                lock (__lockObject)
                {
                    if (_writePending)
                    {
                        _writePending = false;
                        var groupEntity = new TGroupEntityType()
                        {
                            GroupName = GroupName
                        };
                        _store.Save(groupEntity);
                    }
                }
            }
        }
    }
}