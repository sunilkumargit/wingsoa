using System;
using System.Linq;
using Wing.Utils;
using Wing.Server.Modules.ServerConfigManager.Entities;
using Wing.EntityStore;
using System.Collections.Generic;

namespace Wing.Server.Modules.ServerConfigManager
{
    public class EntityStoreSettingsSection<TGroupEntityType, TSectionEntityType, TPropertyEntityType> : ISettingsSection
        where TGroupEntityType : SettingsGroupEntityBase, new()
        where TSectionEntityType : SettingsSectionEntityBase, new()
        where TPropertyEntityType : SettingsPropertyEntityBase, new()
    {
        private IEntityStore _store;
        private bool _writePending;
        private Dictionary<String, TPropertyEntityType> _properties = new Dictionary<string, TPropertyEntityType>();
        private Object __lockObject = new Object();
        private EntityStoreSettingsGroup<TGroupEntityType, TSectionEntityType, TPropertyEntityType> _group;

        public EntityStoreSettingsSection(EntityStoreSettingsGroup<TGroupEntityType, TSectionEntityType, TPropertyEntityType> group,
            String sectionName, IEntityStore store, bool isNew)
        {
            _group = group;
            SectionName = sectionName;
            _store = store;
            _writePending = isNew;
        }

        public ISettingsGroup Group { get { return _group; } }
        public String SectionName { get; private set; }

        private TPropertyEntityType GetProperty(String propertyName)
        {
            TPropertyEntityType property = default(TPropertyEntityType);
            if (!_properties.TryGetValue(propertyName.ToLower(), out property))
            {
                lock (__lockObject)
                {
                    if (!_properties.TryGetValue(propertyName.ToLower(), out property))
                    {
                        var propertyQuery = _store.CreateQuery<TPropertyEntityType>();
                        propertyQuery.AddFilterEqual("GroupName", Group.GroupName);
                        propertyQuery.AddFilterEqual("SectionName", SectionName);
                        propertyQuery.AddFilterEqual("PropertyName", propertyName);
                        property = propertyQuery.FindFirst();

                        if (property == null)
                        {
                            property = new TPropertyEntityType()
                            {
                                GroupName = Group.GroupName,
                                SectionName = SectionName,
                                PropertyName = propertyName
                            };
                        }
                        _properties[propertyName.ToLower()] = property;
                    }
                }
            }
            return property;
        }

        private T ReadValue<T>(String name)
        {
            var property = GetProperty(name);
            return ConvertUtils.Coerce<T>(property.Value);
        }

        private void WriteValue(String name, String value)
        {
            if (_writePending)
            {
                lock (__lockObject)
                {
                    if (_writePending)
                    {
                        _writePending = false;
                        _group.SaveGroup();
                        var sectionEntity = new TSectionEntityType()
                        {
                            GroupName = _group.GroupName,
                            SectionName = SectionName
                        };
                        _store.Save(sectionEntity);
                    }
                }
            }
            var property = GetProperty(name);
            property.Value = value;
            _store.Save(property);
        }

        public string GetString(string name)
        {
            return ReadValue<String>(name);
        }

        public int GetInt(string name)
        {
            return ReadValue<int>(name);
        }

        public Boolean GetBoolean(string name)
        {
            return ReadValue<bool>(name);
        }

        public DateTime GetDateTime(string name)
        {
            var value = ReadValue<String>(name);
            if (value.IsEmpty())
                return DateTime.MinValue;
            return new DateTime(Convert.ToInt64(value));
        }

        public void Write(string name, string value)
        {
            WriteValue(name, value ?? "");
        }

        public void Write(string name, int value)
        {
            WriteValue(name, value.ToString());
        }

        public void Write(string name, bool value)
        {
            WriteValue(name, value.ToString());
        }

        public void Write(string name, DateTime value)
        {
            WriteValue(name, value.ToBinary().ToString());
        }
    }
}
