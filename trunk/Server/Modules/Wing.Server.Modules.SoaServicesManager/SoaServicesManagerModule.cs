using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wing.Modularity;
using Wing.ServiceLocation;
using Wing.Server.Soa;
using Wing.Soa.Interop;
using Wing.Utils;

namespace Wing.Server.Modules.SoaServicesManager
{
    [Module("SoaServicesManager")]
    [ModuleDescription("Controlador de serviços de internet")]
    [ModuleCategory(ModuleCategory.Core)]
    [ModulePriority(ModulePriority.High)]
    [ModuleDependency("ServerConfigManager")]
    public class SoaServicesManagerModule : ModuleBase
    {
        public override void Initialize()
        {
            base.Initialize();
            //verificar se existe uma configuração de uri padrão
            var configManager = ServiceLocator.Current.GetInstance<IServerConfigManagerService>();
            var servicesSection = configManager.GetSection("Services");
            if (servicesSection.GetString("baseUri").IsEmpty())
                servicesSection.Write("baseUri", "http://127.0.0.1:4305/WngServices/");

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
