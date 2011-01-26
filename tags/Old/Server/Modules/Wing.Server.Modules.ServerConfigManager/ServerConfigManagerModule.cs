using Wing.Modularity;
using Wing.ServiceLocation;
using System;

namespace Wing.Server.Modules.ServerConfigManager
{
    [Module("ServerConfigManager")]
    [ModuleDescription("Gerenciador de configurações do servidor")]
    [ModuleCategory(ModuleCategory.Core)]
    [ModulePriority(ModulePriority.Higher)]
    [ModuleDependency("ServerStorage")]
    public class ServerConfigManagerModule : ModuleBase
    {
        public override void Initialize()
        {
            base.Initialize();
            var entityStore = ServiceLocator.GetInstance<IServerEntityStoreService>();
            entityStore.RegisterEntity<SettingsGroup>();
            entityStore.RegisterEntity<SettingsSection>();
            entityStore.RegisterEntity<SettingsProperty>();
            ServiceLocator.Register<ISettingsManager, ServerEntityStoreSettingsManager>(true);
            var service = ServiceLocator.GetInstance<ISettingsManager>();
            service.GetSection("ServerSettings", "StartUp").Write("Last", DateTime.Now);
        }
    }
}