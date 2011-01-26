using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Wing.Logging;
using Wing.EntityStore;
using Wing.Server.Modules.ServerConfigManager.Entities;

namespace Wing.Server.Modules.ServerConfigManager
{
    public class EntityStoreSettingsManagerBase<TGroupEntityType, TSectionEntityType, TPropertyEntityType> : ISettingsManager
        where TGroupEntityType : SettingsGroupEntityBase, new()
        where TSectionEntityType : SettingsSectionEntityBase, new()
        where TPropertyEntityType : SettingsPropertyEntityBase, new()
    {
        private Dictionary<String, ISettingsGroup> _groups = new Dictionary<string, ISettingsGroup>();
        private IEntityStore _store;
        private Object __lockObject = new Object();

        public EntityStoreSettingsManagerBase(IEntityStore store)
        {
            _store = store;
        }

        public string[] GetGroupsNames()
        {
            var query = _store.CreateQuery<TGroupEntityType>();
            return query.Find().Select(g => g.GroupName).ToArray();
        }

        public ISettingsGroup GetGroup(string name, bool createIfNotExists = true)
        {
            ISettingsGroup group = null;
            if (!_groups.TryGetValue(name.ToString().ToLower(), out group))
            {
                lock (__lockObject)
                {
                    if (!_groups.TryGetValue(name.ToString().ToLower(), out group))
                    {
                        var groupQuery = _store.CreateQuery<TGroupEntityType>();
                        groupQuery.AddFilterEqual("GroupName", name);
                        var groupEntity = groupQuery.FindFirst();

                        if (groupEntity != null || createIfNotExists)
                        {
                            var groupName = name;
                            if (groupEntity != null)
                                groupName = groupEntity.GroupName;
                            group = new EntityStoreSettingsGroup<TGroupEntityType, TSectionEntityType, TPropertyEntityType>(groupName, _store, groupEntity == null);
                            _groups[group.GroupName.ToLower()] = group;
                        }
                        else
                            throw new Exception(String.Format("Section group does not exists: {0}", name));
                    }
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