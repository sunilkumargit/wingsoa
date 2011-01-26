using Wing.ServiceLocation;

namespace Wing.Server
{
    public static class SettingsManager
    {
        public static string[] GetGroupsNames()
        {
            return ServiceLocator.GetInstance<ISettingsManager>().GetGroupsNames();
        }

        public static ISettingsGroup GetGroup(string name, bool createIfNotExists = true)
        {
            return ServiceLocator.GetInstance<ISettingsManager>().GetGroup(name, createIfNotExists);
        }

        public static ISettingsSection GetSection(string groupName, string sectionName, bool createIfNotExists = true)
        {
            return ServiceLocator.GetInstance<ISettingsManager>().GetSection(groupName, sectionName, createIfNotExists);
        }
    }
}
