using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wing.Modularity;
using Wing.ServiceLocation;
using Wing.Server.Soa;
using Wing.Soa.Interop;

namespace Wing.Server.Modules.SoaServicesManager
{
    [Module("SoaServicesManager")]
    [ModuleDescription("Controlador de serviços de internet")]
    [ModuleCategory(ModuleCategory.Core)]
    [ModulePriority(ModulePriority.High)]
    public class SoaServicesManagerModule : ModuleBase
    {
        public override void Initialize()
        {
            base.Initialize();
            var builder = new SoaServiceHostBuilder();
            builder.Strategies.Add(new CreateSingletonInstanceStrategy());
            builder.Strategies.Add(new CreateServiceHostStrategy());
            builder.Strategies.Add(new CreateNetTcpBindingStrategy());

            ServiceLocator.Current.Register<ISoaServiceHostBuilder>(builder);
            ServiceLocator.Current.Register<ISoaServicesManager, SoaServicesManager>(true);
        }

        public override void Initialized()
        {
            base.Initialized();
            //registrar o serviço provedor de metadados ao cliente
            var manager = ServiceLocator.Current.GetInstance<ISoaServicesManager>();
            manager.RegisterService(new SoaServiceDescriptor("SoaMetadataProviderService", typeof(ISoaMetadataProviderService), typeof(SoaMetadataProviderService), true), true);
        }
    }
}
