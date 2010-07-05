using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Security.Policy;
using Wing.Server.Core;

namespace Wing.Server.Modules.WingClientSupport
{
    [Serializable]
    public class AssemblyLoadOrderResolver
    {
        public List<String> LoadOrder { get; private set; }

        AppDomain _domain;

        public AssemblyLoadOrderResolver(IAssemblyStore store)
        {
            Evidence evidence = new Evidence(AppDomain.CurrentDomain.Evidence);
            AppDomainSetup setup = AppDomain.CurrentDomain.SetupInformation;
            _domain = AppDomain.CreateDomain("ReflectionOnlyDomain", evidence, setup);

            //assemblies data
            Dictionary<String, byte[]> asmData = new Dictionary<string, byte[]>();

            foreach (var asmName in store.GetAssemblyNames(PathMode.Store))
                asmData[asmName] = store.GetAssemblyData(asmName);

            var wrapper = _domain.CreateInstanceFrom(typeof(ReflectionOnlyAssemblyWrapper).Assembly.Location,
                typeof(ReflectionOnlyAssemblyWrapper).FullName).Unwrap();

            LoadOrder = ((ReflectionOnlyAssemblyWrapper)wrapper).GetAssemblies(asmData);

            asmData.Clear();
            AppDomain.Unload(_domain);
        }

        [Serializable]
        public class ReflectionOnlyAssemblyWrapper : MarshalByRefObject
        {
            private Dictionary<string, byte[]> _asmData;

            internal List<String> GetAssemblies(Dictionary<string, byte[]> asmData)
            {
                _asmData = asmData;
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_ReflectionOnlyAssemblyResolve);

                foreach (var file in _asmData.Keys)
                {
                    var loaded = AppDomain.CurrentDomain.ReflectionOnlyGetAssemblies()
                        .FirstOrDefault(asm => asm.GetName().Name.Equals(Path.GetFileNameWithoutExtension(file), StringComparison.OrdinalIgnoreCase));
                    if (loaded == null)
                        Assembly.ReflectionOnlyLoad(_asmData[file]);
                }

                var asmList = AppDomain.CurrentDomain.ReflectionOnlyGetAssemblies()
                    .Where(asm => _asmData.Keys.Any(k => Path.GetFileNameWithoutExtension(k).Equals(asm.GetName().Name, StringComparison.OrdinalIgnoreCase)))
                    .ToList();

                AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve -= CurrentDomain_ReflectionOnlyAssemblyResolve;

                var result = new List<String>();
                foreach (var asm in asmList)
                    ProcessAssembly(result, asm.GetName().Name, asmList);

                return result;
            }

            Assembly CurrentDomain_ReflectionOnlyAssemblyResolve(object sender, ResolveEventArgs args)
            {
                var asmName = new AssemblyName(args.Name);
                return Assembly.ReflectionOnlyLoad(_asmData[asmName.Name]);
            }

            private void ProcessAssembly(List<String> result, String asmName, List<Assembly> assemblies)
            {
                if (result.IndexOf(asmName) > -1)
                    return;

                var assembly = assemblies.FirstOrDefault(asm => asm.GetName().Name.Equals(asmName));

                if (assembly == null)
                    return;

                var dependencies = assembly.GetReferencedAssemblies().Select(ra => ra.Name).ToArray();

                foreach (var dependency in dependencies)
                    ProcessAssembly(result, dependency, assemblies);

                result.Add(asmName);
            }
        }
    }
}