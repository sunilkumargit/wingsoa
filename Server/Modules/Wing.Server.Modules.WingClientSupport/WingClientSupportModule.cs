using Wing.Modularity;
using Wing.Server.Core;
using Wing.ServiceLocation;
using Wing.Utils;

namespace Wing.Server.Modules.WingClientSupport
{
    [Module("WingClientSupport")]
    [ModuleDescription("Suporte ao client Wing")]
    [ModuleCategory(ModuleCategory.Core)]
    [ModulePriority(ModulePriority.Lower)]
    [ModuleDependency("SoaServicesManager")]
    public class WingClientSupportModule : ModuleBase
    {
        #region IModule Members

        public override void Initialize()
        {
            RegisterClientAssemblyStore();
            RegisterClientAssemblyInfo();
            RegisterClientSupportController();
        }

        private void RegisterClientAssemblyStore()
        {
            var bootSettings = ServiceLocator.GetInstance<BootstrapSettings>();
            var clientAssemblyStore = new FileSystemAssemblyStore();
            clientAssemblyStore.SetBasePath(bootSettings.ClientAssemblyStorePath);
            clientAssemblyStore.ConsolidateStore();
            ServiceLocator.Register<IAssemblyStore>(clientAssemblyStore, "Client");
        }

        private void RegisterClientAssemblyInfo()
        {
            var store = ServiceLocator.GetInstance<IAssemblyStore>("Client");
            var infoBuilder = new ClientAssemblyInfoBuilder();
            ServiceLocator.Register<AssemblyInfoCollection>(infoBuilder.BuildAssemblyInfo(store), "Client");
        }

        private void RegisterClientSupportController()
        {
            ServiceLocator.GetInstance<IMvcControllerTypeLocator>()
                .RegisterControllerType(null, typeof(WingCltAppSupportController));
        }

        #endregion
    }
}