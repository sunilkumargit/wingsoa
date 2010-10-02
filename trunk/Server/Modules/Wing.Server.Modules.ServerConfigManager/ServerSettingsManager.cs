using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wing.Modularity;
using Wing.EntityStore;
using Wing.ServiceLocation;
using Wing.Server.Core;
using System.IO;
using Wing.Server.Impl;

namespace Wing.Server.Modules.ServerConfigManager
{
    public class ServerSettingsManager : SettingsManagerBase
    {
        public ServerSettingsManager(BootstrapSettings bootstrapSettings) : base(Path.Combine(bootstrapSettings.ServerDataStorePath, "Settings")) { }
    }
}
