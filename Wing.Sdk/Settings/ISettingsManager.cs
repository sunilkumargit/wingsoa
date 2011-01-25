using System;

namespace Wing.Settings
{
    public interface ISettingsManager
    {
        String[] GetGroupsNames();
        ISettingsGroup GetGroup(String name, bool createIfNotExists = true);
        ISettingsSection GetSection(String groupName, String sectionName, bool createIfNotExists = true);
    }
}
