using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Wing.Client.Core;
using Wing.Client.Sdk;
using Wing.Client.Sdk.Services;
using Wing.Composite;
using Wing.Composite.Presentation.Regions;
using Wing.Composite.Presentation.Regions.Behaviors;
using Wing.Composite.Regions;
using Wing.Events;
using Wing.Logging;
using Wing.Modularity;
using Wing.ServiceLocation;

namespace Wing.Client.Bootstrap
{
    public class Bootstrapper : IBootstrapper
    {
        private ISplashUI _splashUi;

        #region IBootstrapper Members

        public void Run(BootstrapSettings settings)
        {
            settings.Splash.DisplayStatusMessage("Iniciando...");
            var _serviceLocator = new Wing.UnityServiceLocator.UnityServiceLocator(null);
            //registrar o ServiceLocator
            ServiceLocator.SetLocatorProvider(new ServiceLocatorProvider(() => _serviceLocator));

            ServiceLocator.Register<ISyncBroker>(new SyncBrokerService(Application.Current.RootVisual.Dispatcher));
            VisualContext.SetSyncBroker(ServiceLocator.GetInstance<ISyncBroker>());

            //registrar o locator
            ServiceLocator.Register<IServiceLocator>(_serviceLocator);

            ServiceLocator.Register<BootstrapSettings>(settings);

            ServiceLocator.Register<ISplashUI>(settings.Splash);
            _splashUi = settings.Splash;

            //registrar o logger
            ServiceLocator.Register<ILogger, DebugLogger>(true);

            //registrar os servicos de modulos
            ServiceLocator.Register<IModuleCatalog>(new InMemoryModuleCatalog(settings.Assemblies));

            ServiceLocator.Register<IModuleInitializer, ScheduledModuleInitializer>(true);
            ServiceLocator.Register<IModuleManager, ModuleManager>(true);

            //agregador de eventos
            ServiceLocator.Register<IEventAggregator, EventAggregator>(true);

            //controlador do root visual
            ServiceLocator.Register<IRootVisualManager>(settings.RootVisualManager);

            //composite
            ServiceLocator.Register<RegionAdapterMappings, RegionAdapterMappings>(true);
            ServiceLocator.Register<IRegionManager, RegionManager>(true);
            ServiceLocator.Register<IRegionViewRegistry, RegionViewRegistry>(true);
            ServiceLocator.Register<IRegionBehaviorFactory, RegionBehaviorFactory>(true);

            ConfigureRegionAdapterMappings();
            ConfigureDefaultRegionBehaviors();
            RegisterFrameworkExceptionTypes();

            ServiceLocator.Register<IRegionManager, RegionManager>();

            SdkInitializer.Initialize();

            WorkContext.Async(() =>
            {
                // inscrever-se nos eventos do module manager para informar o usuário do andamento da inicialização
                var moduleManager = ServiceLocator.GetInstance<IModuleManager>();

                moduleManager.BeginLoadModules += new EventHandler<ModuleManagerEventArgs>(moduleManager_BeginLoadModules);
                moduleManager.ModuleInitialized += new EventHandler<ModuleManagerEventArgs>(moduleManager_ModuleInitialized);
                moduleManager.ModuleRunning += new EventHandler<ModuleManagerEventArgs>(moduleManager_ModuleRunning);
                moduleManager.EndLoadModules += new EventHandler<ModuleManagerEventArgs>(moduleManager_EndLoadModules);

                moduleManager.Run();
            });
        }

        void moduleManager_EndLoadModules(object sender, ModuleManagerEventArgs e)
        {
            // retirar as inscrições nos eventos
            var moduleManager = ServiceLocator.GetInstance<IModuleManager>();
            moduleManager.BeginLoadModules -= moduleManager_BeginLoadModules;
            moduleManager.ModuleInitialized -= moduleManager_ModuleInitialized;
            moduleManager.ModuleRunning -= moduleManager_ModuleRunning;
            moduleManager.EndLoadModules -= moduleManager_EndLoadModules;
        }

        void moduleManager_ModuleRunning(object sender, ModuleManagerEventArgs e)
        {
            _splashUi.DisplayStatusMessage(e.CurrentModule.ModuleName + " pronto");
            _splashUi.UpdateProgressBar(0, 1);
        }

        void moduleManager_ModuleInitialized(object sender, ModuleManagerEventArgs e)
        {
            _splashUi.DisplayStatusMessage(e.CurrentModule.ModuleName + " iniciado");
            _splashUi.UpdateProgressBar(0, 1);
        }

        void moduleManager_BeginLoadModules(object sender, ModuleManagerEventArgs e)
        {
            _splashUi.DisplayProgressBar(e.Modules.Length * 2);  //  * 2 pois o progresso será informado na inicialização do modulo e também quando este estiver pronto.
        }

        #endregion

        /// <summary>
        /// Configures the default region adapter mappings to use in the application, in order
        /// to adapt UI controls defined in XAML to use a region and register it automatically.
        /// May be overwritten in a derived class to add specific mappings required by the application.
        /// </summary>
        /// <returns>The <see cref="RegionAdapterMappings"/> instance containing all the mappings.</returns>
        protected virtual RegionAdapterMappings ConfigureRegionAdapterMappings()
        {
            RegionAdapterMappings regionAdapterMappings = ServiceLocator.GetInstance<RegionAdapterMappings>();
            if (regionAdapterMappings != null)
            {
#if SILVERLIGHT
                regionAdapterMappings.RegisterMapping(typeof(TabControl), ServiceLocator.GetInstance<TabControlRegionAdapter>());
#endif
                regionAdapterMappings.RegisterMapping(typeof(Selector),
                    ServiceLocator.GetInstance<SelectorRegionAdapter>());

                regionAdapterMappings.RegisterMapping(typeof(ItemsControl),
                    ServiceLocator.GetInstance<ItemsControlRegionAdapter>());

                regionAdapterMappings.RegisterMapping(typeof(ContentControl),
                    ServiceLocator.GetInstance<ContentControlRegionAdapter>());
            }

            return regionAdapterMappings;
        }

        /// <summary>
        /// Configures the <see cref="IRegionBehaviorFactory"/>. This will be the list of default
        /// behaviors that will be added to a region. 
        /// </summary>
        protected virtual IRegionBehaviorFactory ConfigureDefaultRegionBehaviors()
        {
            var defaultRegionBehaviorTypesDictionary = ServiceLocator.GetInstance<IRegionBehaviorFactory>();

            if (defaultRegionBehaviorTypesDictionary != null)
            {
                defaultRegionBehaviorTypesDictionary.AddIfMissing(AutoPopulateRegionBehavior.BehaviorKey,
                    typeof(AutoPopulateRegionBehavior));

                defaultRegionBehaviorTypesDictionary.AddIfMissing(BindRegionContextToDependencyObjectBehavior.BehaviorKey,
                    typeof(BindRegionContextToDependencyObjectBehavior));

                defaultRegionBehaviorTypesDictionary.AddIfMissing(RegionActiveAwareBehavior.BehaviorKey,
                    typeof(RegionActiveAwareBehavior));

                defaultRegionBehaviorTypesDictionary.AddIfMissing(SyncRegionContextWithHostBehavior.BehaviorKey,
                    typeof(SyncRegionContextWithHostBehavior));

                defaultRegionBehaviorTypesDictionary.AddIfMissing(RegionManagerRegistrationBehavior.BehaviorKey,
                    typeof(RegionManagerRegistrationBehavior));

            }
            return defaultRegionBehaviorTypesDictionary;

        }

        /// <summary>
        /// Registers in the <see cref="IUnityContainer"/> the <see cref="Type"/> of the Exceptions
        /// that are not considered root exceptions by the <see cref="ExceptionExtensions"/>.
        /// </summary>
        protected virtual void RegisterFrameworkExceptionTypes()
        {
            ExceptionExtensions.RegisterFrameworkExceptionType(
                typeof(Wing.ServiceLocation.ActivationException));

            ExceptionExtensions.RegisterFrameworkExceptionType(
                typeof(Microsoft.Practices.Unity.ResolutionFailedException));
        }
    }
}