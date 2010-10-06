using System;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Wing.Client.Core;
using Wing.Composite;
using Wing.Composite.Presentation.Regions;
using Wing.Composite.Presentation.Regions.Behaviors;
using Wing.Composite.Regions;
using Wing.Events;
using Wing.Logging;
using Wing.Modularity;
using Wing.ServiceLocation;
using Wing.Client.Sdk;
using Wing.Client.Sdk.Services;
using System.Windows;
using System.Threading;

namespace Wing.Client.Bootstrap
{
    public class Bootstrapper : IBootstrapper
    {

        #region IBootstrapper Members

        public void Run(BootstrapSettings settings)
        {
            settings.Splash.DisplayMessage("Iniciando o bootstrapper...");
            var _serviceLocator = new Wing.UnityServiceLocator.UnityServiceLocator(null);
            //registrar o ServiceLocator
            ServiceLocator.SetLocatorProvider(new ServiceLocatorProvider(() => _serviceLocator));

            ServiceLocator.Current.Register<ISyncContext>(new SyncContextService(Application.Current.RootVisual.Dispatcher));
            VisualContext.SetSyncService(ServiceLocator.Current.GetInstance<ISyncContext>());

            //registrar o locator
            ServiceLocator.Current.Register<IServiceLocator>(_serviceLocator);

            ServiceLocator.Current.Register<BootstrapSettings>(settings);

            ServiceLocator.Current.Register<ISplashUI>(settings.Splash);

            //registrar o logger
            ServiceLocator.Current.Register<ILogger, DebugLogger>(true);

            //registrar os servicos de modulos
            ServiceLocator.Current.Register<IModuleCatalog>(new InMemoryModuleCatalog(settings.Assemblies));

            ServiceLocator.Current.Register<IModuleInitializer, ScheduledModuleInitializer>(true);
            ServiceLocator.Current.Register<IModuleManager, ModuleManager>(true);

            //agregador de eventos
            ServiceLocator.Current.Register<IEventAggregator, EventAggregator>(true);

            //controlador do root visual
            ServiceLocator.Current.Register<IRootVisualManager>(settings.RootVisualManager);

            //composite
            ServiceLocator.Current.Register<RegionAdapterMappings, RegionAdapterMappings>(true);
            ServiceLocator.Current.Register<IRegionManager, RegionManager>(true);
            ServiceLocator.Current.Register<IRegionViewRegistry, RegionViewRegistry>(true);
            ServiceLocator.Current.Register<IRegionBehaviorFactory, RegionBehaviorFactory>(true);

            ConfigureRegionAdapterMappings();
            ConfigureDefaultRegionBehaviors();
            RegisterFrameworkExceptionTypes();

            ServiceLocator.Current.Register<IRegionManager, RegionManager>();

            SdkInitializer.Initialize();

            //carregar os modulos
            settings.Splash.DisplayMessage("Carregando os módulos...");

            ThreadPool.QueueUserWorkItem((a) =>
            {
                ServiceLocator.Current.GetInstance<IModuleManager>().Run();
            }, null);
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
            RegionAdapterMappings regionAdapterMappings = ServiceLocator.Current.GetInstance<RegionAdapterMappings>();
            if (regionAdapterMappings != null)
            {
#if SILVERLIGHT
                regionAdapterMappings.RegisterMapping(typeof(TabControl), ServiceLocator.Current.GetInstance<TabControlRegionAdapter>());
#endif
                regionAdapterMappings.RegisterMapping(typeof(Selector),
                    ServiceLocator.Current.GetInstance<SelectorRegionAdapter>());

                regionAdapterMappings.RegisterMapping(typeof(ItemsControl),
                    ServiceLocator.Current.GetInstance<ItemsControlRegionAdapter>());

                regionAdapterMappings.RegisterMapping(typeof(ContentControl),
                    ServiceLocator.Current.GetInstance<ContentControlRegionAdapter>());
            }

            return regionAdapterMappings;
        }

        /// <summary>
        /// Configures the <see cref="IRegionBehaviorFactory"/>. This will be the list of default
        /// behaviors that will be added to a region. 
        /// </summary>
        protected virtual IRegionBehaviorFactory ConfigureDefaultRegionBehaviors()
        {
            var defaultRegionBehaviorTypesDictionary = ServiceLocator.Current.GetInstance<IRegionBehaviorFactory>();

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