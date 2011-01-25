using System;

namespace Wing.Settings
{
    public interface ISettingsGroup
    {
        String GroupName { get; }
        String[] GetSectionsNames();
        ISettingsSection GetSection(String name, bool createIfNotExists = true);
    }
}