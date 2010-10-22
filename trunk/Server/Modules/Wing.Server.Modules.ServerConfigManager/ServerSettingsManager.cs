using System.IO;
using Wing.Server.Core;
using Wing.Server.Impl;

namespace Wing.Server.Modules.ServerConfigManager
{
    public class ServerSettingsManager : SettingsManagerBase
    {
        public ServerSettingsManager(BootstrapSettings bootstrapSettings) 
            : base(Path.Combine(bootstrapSettings.ServerDataStorePath, "Settings")) { }
    }
}
