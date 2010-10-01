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
    public class ServerConfigSection : IServerConfigSection
    {
        private ServerConfigSectionWrapper _wrapper;
        private ServerConfigManagerService _configManager;

        public ServerConfigSection(ServerConfigManagerService configManager, ServerConfigSectionWrapper wrapper)
        {
            _configManager = configManager;
            _wrapper = wrapper;
        }

        internal ServerConfigSectionWrapper Wrapper { get { return _wrapper; } }

        #region IServerConfigSection Members

        public string SectionName
        {
            get { return _wrapper.SectionName; }
        }

        private T ReadValue<T>(String name)
        {
            var item = Wrapper.Properties.FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (item != null)
                return ConvertUtils.Coerce<T>(item.Value);
            return default(T);
        }

        private void WriteValue(String name, Object value)
        {
            var item = Wrapper.Properties.FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (item == null)
            {
                item = new ServerConfigSectionEntry() { Name = name, Value = value };
                _wrapper.Properties.Add(item);
            }
            _configManager.SaveSection(this);
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
            return ReadValue<DateTime>(name);
        }

        public void Write(string name, string value)
        {
            WriteValue(name, value);
        }

        public void Write(string name, int value)
        {
            WriteValue(name, value);
        }

        public void Write(string name, bool value)
        {
            WriteValue(name, value);
        }

        public void Write(string name, DateTime value)
        {
            WriteValue(name, value);
        }
        #endregion
    }
}
