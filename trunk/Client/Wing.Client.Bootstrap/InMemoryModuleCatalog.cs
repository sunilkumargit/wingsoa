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
    public class InMemoryModuleCatalog : ModuleCatalog
    {
        public IEnumerable<Assembly> Assemblies { get; set; }

        public InMemoryModuleCatalog(IEnumerable<Assembly> assemblies)
        {
            Assemblies = assemblies;
        }

        protected override void InnerLoad()
        {
            base.InnerLoad();
            if (Assemblies == null)
                return;
            var moduleInfoBuilder = new ModuleInfoBuilder();

            foreach (var assembly in Assemblies)
            {
                foreach (var moduleType in assembly.GetExportedTypes()
                    .Where(type => typeof(IModule).IsAssignableFrom(type)
                        && !type.IsInterface
                        && !type.IsAbstract
                        && !type.IsGenericType))
                {
                    AddModule(moduleInfoBuilder.BuildFromType(moduleType));
                }
            }
        }
    }

    public class TestModule : IModule
    {
        #region IModule Members

        public void Initialize()
        {
            ServiceLocator.Current.GetInstance<ILogger>().Log("module initialized", Category.Info, Priority.Low);
        }

        #endregion
    }
}
