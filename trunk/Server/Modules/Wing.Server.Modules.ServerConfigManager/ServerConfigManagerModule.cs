using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wing.Modularity;
using Wing.EntityStore;
using Wing.ServiceLocation;
using Wing.Server.Core;
using System.IO;

namespace Wing.Server.Modules.ServerConfigManager
{
    [Module("ServerConfigManager")]
    [ModuleDescription("Gerenciador de configurações do servidor")]
    [ModuleCategory(ModuleCategory.Core)]
    [ModulePriority(ModulePriority.Higher)]
    public class ServerConfigManagerModule : ModuleBase
    {
        public override void Initialize()
        {
            base.Initialize();
            ServiceLocator.Register<ISettingsManager, ServerSettingsManager>(true);
            var service = ServiceLocator.GetInstance<ISettingsManager>();
            service.GetSection("ServerSettings", "StartUp").Write("Last", DateTime.Now);
        }
    }
}