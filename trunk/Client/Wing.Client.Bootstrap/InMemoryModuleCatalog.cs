using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Wing.Modularity;

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
                    AddModule(moduleInfoBuilder.BuildModuleInfo(moduleType));
                }
            }
        }
    }
}
