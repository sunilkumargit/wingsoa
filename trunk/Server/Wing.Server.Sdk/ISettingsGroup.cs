using System;

namespace Wing.Server
{
    public interface ISettingsGroup
    {
        String GroupName { get; }
        String[] GetSectionsNames();
        ISettingsSection GetSection(String name, bool createIfNotExists = true);
    }
}