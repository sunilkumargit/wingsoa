using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wing.ServiceLocation;

namespace Wing.Server
{
    public static class SettingsManager
    {
        public static string[] GetGroupsNames()
        {
            return ServiceLocator.Current.GetInstance<ISettingsManager>().GetGroupsNames();
        }

        public static ISettingsGroup GetGroup(string name, bool createIfNotExists = true)
        {
            return ServiceLocator.Current.GetInstance<ISettingsManager>().GetGroup(name, createIfNotExists);
        }

        public static ISettingsSection GetSection(string groupName, string sectionName, bool createIfNotExists = true)
        {
            return ServiceLocator.Current.GetInstance<ISettingsManager>().GetSection(groupName, sectionName, createIfNotExists);
        }
    }
}
