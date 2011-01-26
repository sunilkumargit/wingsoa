using Wing.Modularity;
using Wing.Server.Soa;
using Wing.ServiceLocation;
using Wing.Soa.Interop;
using Wing.Utils;
using Wing.Logging;

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
            var servicesSection = SettingsManager.GetSection("Services", "BaseConfiguration");
            if (servicesSection.GetString("baseUri").IsEmpty())
                servicesSection.Write("baseUri", "http://127.0.0.1:8080/Wing/WngServices/");
            ServiceLocator.GetInstance<ILogger>().Log("Soa connections base uri: " + servicesSection.GetString("baseUri"), Category.Info, Priority.Medium);

            var builder = new SoaServiceHostBuilder();
            builder.Strategies.Add(new CreateSingletonInstanceStrategy());
            builder.Strategies.Add(new CreateServiceHostStrategy());
            builder.Strategies.Add(new CreateBasicHttpBindingStrategy());

            ServiceLocator.Register<ISoaServiceHostBuilder>(builder);
            ServiceLocator.Register<ISoaServicesManager, SoaServicesManager>(true);
        }

        public override void Initialized()
        {
            base.Initialized();
            //registrar o serviço provedor de metadados ao cliente
            var manager = ServiceLocator.GetInstance<ISoaServicesManager>();
            manager.RegisterService(new SoaServiceDescriptor("SoaMetadataProvider", typeof(ISoaMetadataProviderService), typeof(SoaMetadataProviderService), true), true);
        }
    }
}
