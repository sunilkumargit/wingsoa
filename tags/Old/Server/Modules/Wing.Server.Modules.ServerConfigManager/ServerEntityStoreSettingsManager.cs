using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wing.Server.Modules.ServerConfigManager
{
    public class ServerEntityStoreSettingsManager : EntityStoreSettingsManagerBase<SettingsGroup, SettingsSection, SettingsProperty>
    {
        public ServerEntityStoreSettingsManager(IServerEntityStoreService store) : base(store) { }
    }
}
