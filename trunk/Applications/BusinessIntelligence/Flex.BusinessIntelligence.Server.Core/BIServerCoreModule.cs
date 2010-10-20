using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wing.Modularity;
using Wing.Server;
using Wing.Server.Soa;
using Wing.ServiceLocation;
using Flex.BusinessIntelligence.Data;
using Wing.Soa.Interop;
using Flex.BusinessIntelligence.Interop.Services;

namespace Flex.BusinessIntelligence.Server.Core
{
    [Module("BIServerCore")]
    [ModuleDescription("Nucleo de servidor do Business Intelligence")]
    [ModuleCategory(ModuleCategory.Init)]
    [ModulePriority(ModulePriority.High)]
    public class BIServerCoreModule : ModuleBase
    {
        public override void Initialize()
        {
            base.Initialize();

            // registrar as entidades de cubo
            IServerEntityStoreService entityStore = ServiceLocator.GetInstance<IServerEntityStoreService>();
            entityStore.RegisterEntity<CubeRegistrationInfo>();
            entityStore.RegisterEntity<CubeQueryGroup>();
            entityStore.RegisterEntity<CubeQueryInfo>();

            //registrar os servicos Soa
            ISoaServicesManager servicesManager = ServiceLocator.GetInstance<ISoaServicesManager>();
            servicesManager.RegisterService(new SoaServiceDescriptor("CubeInfoProvider", typeof(ICubeInfoProviderService), typeof(CubeInfoProviderService), true), true);
       
        }
    }
}
