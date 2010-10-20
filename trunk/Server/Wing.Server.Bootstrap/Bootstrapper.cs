using Wing.Events;
using Wing.Logging;
using Wing.Modularity;
using Wing.Server.Core;
using Wing.ServiceLocation;

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
            ServiceLocator.Register<IServiceLocator>(_serviceLocator);

            //registrar uma assembly resolver para o AppDomain
            ServiceLocator.Register<IAssemblyResolver>(new StoreAssemblyResolver(settings.ServerAssemblyStorePath, null));

            ServiceLocator.Register<BootstrapSettings>(settings);

            //registrar o logger
            ServiceLocator.Register<ILogger, TraceLogger>(true);

            //registrar os servicos de modulos
            ServiceLocator.Register<IModuleCatalog>(new DirectoryModuleCatalog()
            {
                ModulePath = settings.ServerAssemblyStorePath
            });

            ServiceLocator.Register<IModuleInitializer, ModuleInitializer>(true);
            ServiceLocator.Register<IModuleManager, ModuleManager>(true);

            //agregador de eventos
            ServiceLocator.Register<IEventAggregator, EventAggregator>(true);

            //localizador de controllers mvc
            ServiceLocator.Register<IMvcControllerTypeLocator, MvcControllerTypeLocator>(true);

            System.Web.Mvc.ControllerBuilder.Current.SetControllerFactory(new MvcControllerFactory());

            //carregar os modulos
            ServiceLocator.GetInstance<IModuleManager>().Run();
        }

        #endregion
    }

}
