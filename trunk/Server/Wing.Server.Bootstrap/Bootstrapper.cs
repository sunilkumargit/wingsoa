using System;
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
            var logger = new ServerBootLoggerAdapter(settings.BootLogger);
            logger.Log("Bootstrapper starting...", Category.Info, Priority.None);
            try
            {
                logger.Log("Creating services container", Category.Debug, Priority.None);
                _serviceLocator = new Wing.UnityServiceLocator.UnityServiceLocator(null, logger);

                logger.Log("Setting service locator provider", Category.Debug, Priority.None);
                //registrar o ServiceLocator
                ServiceLocator.SetLocatorProvider(() => _serviceLocator);

                //registrar o locator
                ServiceLocator.Register<IServiceLocator>(_serviceLocator);

                //registrar o logger
                ServiceLocator.Register<ILogger>(logger);

                logger.Log("Creating server assembly store resolver", Category.Debug, Priority.None);
                //registrar uma assembly resolver para o AppDomain
                ServiceLocator.Register<IAssemblyResolver>(new StoreAssemblyResolver(settings.ServerAssemblyStorePath, null));

                ServiceLocator.Register<BootstrapSettings>(settings);

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

                logger.Log("Loading modules", Category.Info, Priority.None);
                //carregar os modulos
                ServiceLocator.GetInstance<IModuleManager>().Run(null);

                logger.Log("Server started", Category.Info, Priority.None);
            }
            catch (Exception ex)
            {
                logger.LogException("Fatal error on server bootstrap", ex, Priority.High);
            }
        }

        #endregion
    }

}
