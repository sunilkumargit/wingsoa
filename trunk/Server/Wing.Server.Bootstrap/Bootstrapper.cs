using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wing.Server.Core;
using Wing.ServiceLocation;
using Wing.Logging;
using Wing.Modularity;
using System.Configuration;
using Wing.Events;

namespace Wing.Server.Bootstrap
{
    public class Bootstrapper : IBootstrapper
    {
        private Wing.UnityServiceLocator.UnityServiceLocator _serviceLocator;

        #region IBootstrapper Members

        public void Run(BootstrapSettings settings)
        {
            _serviceLocator = new Wing.UnityServiceLocator.UnityServiceLocator(null);
            //registrar o ServiceLocator
            ServiceLocator.SetLocatorProvider(() => _serviceLocator);

            //registrar o locator
            ServiceLocator.Current.Register<IServiceLocator>(_serviceLocator);

            //registrar uma assembly resolver para o AppDomain
            ServiceLocator.Current.Register<IAssemblyResolver>(new StoreAssemblyResolver(settings.ServerAssemblyStorePath, null));

            ServiceLocator.Current.Register<BootstrapSettings>(settings);

            //registrar o logger
            ServiceLocator.Current.Register<ILogger, TraceLogger>(true);

            //registrar os servicos de modulos
            ServiceLocator.Current.Register<IModuleCatalog>(new DirectoryModuleCatalog()
            {
                ModulePath = settings.ServerAssemblyStorePath
            });

            ServiceLocator.Current.Register<IModuleInitializer, ModuleInitializer>(true);
            ServiceLocator.Current.Register<IModuleManager, ModuleManager>(true);

            //agregador de eventos
            ServiceLocator.Current.Register<IEventAggregator, EventAggregator>(true);

            //carregar os modulos
            ServiceLocator.Current.GetInstance<IModuleManager>().Run();
        }

        #endregion
    }
}
