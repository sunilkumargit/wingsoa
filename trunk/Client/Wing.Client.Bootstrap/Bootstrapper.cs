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

            //carregar os modulos
            ServiceLocator.Current.GetInstance<IModuleManager>().Run();
        }

        #endregion
    }
}