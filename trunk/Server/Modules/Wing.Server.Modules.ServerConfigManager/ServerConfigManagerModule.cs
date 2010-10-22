using Wing.Modularity;
using Wing.ServiceLocation;

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
            //var service = ServiceLocator.GetInstance<ISettingsManager>();
            //service.GetSection("ServerSettings", "StartUp").Write("Last", DateTime.Now);
        }
    }
}