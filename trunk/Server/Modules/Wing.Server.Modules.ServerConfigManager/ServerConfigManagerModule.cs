﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wing.Modularity;
using Wing.EntityStore;
using Wing.ServiceLocation;

namespace Wing.Server.Modules.ServerConfigManager
{
    [Module("ServerConfigManager")]
    [ModuleDescription("Gerenciador de configurações do servidor")]
    [ModuleCategory(ModuleCategory.Core)]
    [ModulePriority(ModulePriority.Higher)]
    public class ServerConfigManagerModule : ModuleBase
    {
        public override void Initialize()
        {
            base.Initialize();
            ServiceLocator.Current.Register<IServerConfigManagerService, ServerConfigManagerService>(true);
            var service = ServiceLocator.Current.GetInstance<IServerConfigManagerService>();
            service.GetSection("StartUp").Write("Last", DateTime.Now);
        }

    }
}