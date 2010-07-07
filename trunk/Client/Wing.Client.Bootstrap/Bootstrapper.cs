using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Wing.Client.Core;
using Wing.ServiceLocation;
using Wing.Modularity;
using Wing.Events;
using Wing.Logging;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using System.Reflection;
using Wing.Composite.Presentation.Regions;
using Wing.Composite.Regions;
using System.Windows.Controls.Primitives;
using Wing.Composite.Presentation.Regions.Behaviors;
using Wing.Composite;

namespace Wing.Client.Bootstrap
{
    public class Bootstrapper : IBootstrapper
    {

        #region IBootstrapper Members

        public void Run(BootstrapSettings settings)
        {
            var _serviceLocator = new Wing.UnityServiceLocator.UnityServiceLocator(null);
            //registrar o ServiceLocator
            ServiceLocator.SetLocatorProvider(new ServiceLocatorProvider(() => _serviceLocator));

            //registrar o locator
            ServiceLocator.Current.Register<IServiceLocator>(_serviceLocator);

            //registrar o logger
            ServiceLocator.Current.Register<ILogger, DebugLogger>(true);

            //registrar os servicos de modulos
            ServiceLocator.Current.Register<IModuleCatalog>(new InMemoryModuleCatalog(settings.Assemblies));

            ServiceLocator.Current.Register<IModuleInitializer, ModuleInitializer>(true);
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


            //carregar os modulos
            ServiceLocator.Current.GetInstance<IModuleManager>().Run();
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

            ExceptionExtensions.RegisterFrameworkExceptionType(
                typeof(Microsoft.Practices.ObjectBuilder2.BuildFailedException));
        }
    }
}