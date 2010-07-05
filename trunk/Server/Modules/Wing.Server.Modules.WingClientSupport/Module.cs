using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wing.Modularity;
using Wing.ServiceLocation;
using Wing.Server.Core;
using Wing.Utils;

namespace Wing.Server.Modules.WingClientSupport
{
    [ModuleDescription("Suporte ao client Wing")]
    [ModuleCategory(ModuleCategory.Core)]
    public class Module : IModule
    {
        #region IModule Members

        public void Initialize()
        {
            RegisterClientAssemblyStore();
            RegisterClientAssemblyInfo();
            RegisterClientSupportController();
        }

        private void RegisterClientAssemblyStore()
        {
            var bootSettings = ServiceLocator.Current.GetInstance<BootstrapSettings>();
            var clientAssemblyStore = new FileSystemAssemblyStore();
            clientAssemblyStore.SetBasePath(bootSettings.ClientAssemblyStorePath);
            clientAssemblyStore.ConsolidateStore();
            ServiceLocator.Current.Register<IAssemblyStore>(clientAssemblyStore, "Client");
        }

        private void RegisterClientAssemblyInfo()
        {
            var store = ServiceLocator.Current.GetInstance<IAssemblyStore>("Client");
            var infoBuilder = new ClientAssemblyInfoBuilder();
            ServiceLocator.Current.Register<AssemblyInfoCollection>(infoBuilder.BuildAssemblyInfo(store), "Client");
        }

        private void RegisterClientSupportController()
        {
            ServiceLocator.Current.GetInstance<IMvcControllerTypeLocator>()
                .RegisterControllerType(null, typeof(WingCltAppSupportController));
        }

        #endregion
    }
}