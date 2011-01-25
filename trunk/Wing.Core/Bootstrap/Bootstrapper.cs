using System;
using Wing.Events;
using Wing.Logging;
using Wing.Modularity;
using Wing.Adapters.ServiceLocation.UnityServiceLocator;
using Wing.Services;
using Wing.EntityStore;
using Wing.Adapters.Logging;
using Wing.Worker;

namespace Wing.Bootstrap
{
    public class Bootstrapper : IBootstrapper
    {
        #region IBootstrapper Members

        public void Run(IBootLogger bootLogger, IPathMapper pathMapper)
        {
            var logger = new ServerBootLoggerAdapter(bootLogger);
            logger.Log("Bootstrapper starting...", Category.Info, Priority.None);
            try
            {
                logger.Log("Creating services container of type " + typeof(UnityServiceLocator).FullName, Category.Debug, Priority.None);
                var serviceLocator = new UnityServiceLocator(null, logger);

                logger.Log("Setting service locator provider", Category.Debug, Priority.None);
                //registrar o ServiceLocator
                ServiceLocator.SetLocatorProvider(() => serviceLocator);

                //registrar o locator
                ServiceLocator.Register<IServiceLocator>(serviceLocator);

                // registrar o path mapper
                ServiceLocator.Register<IPathMapper>(pathMapper);

                var workServicesManager = new WorkerServicesManager();
                workServicesManager.Start();
                ServiceLocator.Register<IWorkerServicesManager>(workServicesManager);

                // registrar o gerenciador de 'EntityStores'
                ServiceLocator.Register<IEntityStoreManager>(new EntityStoreManager());

                // registrar o buffer do logger
                ServiceLocator.Register<LogWriterBufferService, LogWriterBufferService>(true);

                // registrar o factory de logs para o entity store
                ServiceLocator.Register<ILoggerFactory>(new EntityStoreLoggerFactory());

                // registrar o gerenciador de log
                ServiceLocator.Register<ILogManager>(new LogManager());

                // registrar o logger do sistema
                ServiceLocator.GetInstance<ILogManager>().RegisterLogger(logger);

                // registrar o worker que fará a gravação do log
                ServiceLocator.GetInstance<IWorkerServicesManager>().RegisterService(
                    "LogWriter",
                    new LogWriterWorkerService(),
                    null);

                //registrar os servicos de modulos
                ServiceLocator.Register<IModuleCatalog>(new DirectoryModuleCatalog()
                {
                    ModulePath = pathMapper.MapPath("~/bin")
                });

                ServiceLocator.Register<IModuleInitializer, ModuleInitializer>(true);
                ServiceLocator.Register<IModuleManager, ModuleManager>(true);

                //agregador de eventos
                ServiceLocator.Register<IEventAggregator, EventAggregator>(true);

                //localizador de controllers mvc
                //ServiceLocator.Register<IMvcControllerTypeLocator, MvcControllerTypeLocator>(true);

                //System.Web.Mvc.ControllerBuilder.Current.SetControllerFactory(new MvcControllerFactory());

                logger.Log("Loading modules", Category.Info, Priority.None);
                //carregar os modulos
                ServiceLocator.GetInstance<IModuleManager>().Run(null);

                logger.Log("Server started", Category.Info, Priority.None);

                ServiceLocator.GetInstance<ILogManager>()
                    .GetLogger("START_UP")
                    .Log("Server started", Category.Debug, Priority.None);
            }
            catch (Exception ex)
            {
                logger.LogException("Fatal error on server bootstrap", ex, Priority.High);
            }
        }

        #endregion
    }

}
