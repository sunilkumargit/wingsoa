using System;
using System.Linq;
using Wing.Utils;

namespace Wing.Server.Impl
{
    public class SettingsSection : ISettingsSection
    {
        private SettingsSectionWrapper _wrapper;
        private SettingsGroup _configManager;

        public SettingsSection(SettingsGroup configManager, SettingsSectionWrapper wrapper)
        {
            _configManager = configManager;
            _wrapper = wrapper;
        }

        internal SettingsSectionWrapper Wrapper { get { return _wrapper; } }

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
                item = new SettingsSectionItem() { Name = name, Value = value };
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
    }
}
