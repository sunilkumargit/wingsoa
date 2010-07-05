using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wing.Modularity;
using Wing.ServiceLocation;
using Wing.Server.Core;
using Wing.Utils;

namespace Wing.Server.Modules.WingClientSupport
{
    public class ClientAssemblyInfoBuilder
    {
        public ClientAssemblyInfoBuilder() { }

        public AssemblyInfoCollection BuildAssemblyInfo(IAssemblyStore store)
        {
            var meta = new AssemblyInfoCollection();
            var assemblies = store.GetAssemblyNames(PathMode.Store);
            var asmInfoCollection = new AssemblyInfoCollection();
            var info = new List<AssemblyInfo>();

            foreach (var asmName in assemblies)
            {
                var asmData = store.GetAssemblyData(asmName);
                var asmInfo = new AssemblyInfo()
                {
                    AssemblyName = asmName,
                    Size = asmData.Length
                };
                asmInfo.HashString = asmInfo.CalculateHashString(asmData);
                info.Add(asmInfo);
            }

            //ordenar por ordem de carregamento
            var orderResolver = new AssemblyLoadOrderResolver(store);

            info = info.OrderBy(i =>
            {
                return orderResolver.LoadOrder.IndexOf(System.IO.Path.GetFileNameWithoutExtension(i.AssemblyName));
            }).ToList();

            asmInfoCollection.Assemblies.AddRange(info);

            return asmInfoCollection;
        }
    }
}
