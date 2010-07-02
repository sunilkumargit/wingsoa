using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wing.Modularity;
using Wing.ServiceLocation;
using Wing.Soa;

namespace Wing.Server.Modules.SoaProvider
{
    [ModuleSystem]
    [ModuleDescription("Controlador de serviços SOA")]
    public class SoaProviderModule : IModule
    {
        #region IModule Members

        public void Initialize()
        {
            //ServiceLocator.Current.Register<ISoaServicesProvider, SoaServicesProvider>(true);
        }

        #endregion
    }
}
