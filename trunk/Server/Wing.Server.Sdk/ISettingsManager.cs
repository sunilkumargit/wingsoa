using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wing.Server
{
    public interface ISettingsManager
    {
        String[] GetGroupsNames();
        ISettingsGroup GetGroup(String name, bool createIfNotExists = true);
        ISettingsSection GetSection(String groupName, String sectionName, bool createIfNotExists = true);
    }
}
