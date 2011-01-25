using System;

namespace Wing.Settings
{
    public interface ISettingsSection
    {
        ISettingsGroup Group { get; }
        String SectionName { get; }
        String GetString(String name);
        int GetInt(String name);
        bool GetBoolean(String name);
        DateTime GetDateTime(String name);
        void Write(String name, String value);
        void Write(String name, int value);
        void Write(String name, bool value);
        void Write(String name, DateTime dateTime);
    }
}
