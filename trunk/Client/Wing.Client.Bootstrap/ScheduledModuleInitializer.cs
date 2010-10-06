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
using Wing.Modularity;
using Wing.Client.Sdk;
using Wing.ServiceLocation;
using Wing.Logging;

namespace Wing.Client.Bootstrap
{
    public class ScheduledModuleInitializer : ModuleInitializer
    {
        public ScheduledModuleInitializer(IServiceLocator serviceLocator, ILogger loggerFacade)
            : base(serviceLocator, loggerFacade) { }

        protected override void InitializeModuleInternal(ModuleInfo moduleInfo)
        {
            WorkContext.Sync(() => ExecuteModuleMethod(moduleInfo, moduleInfo.ModuleInstance.Initialize));
        }

        public override void PostInitializeModuleInternal(ModuleInfo moduleInfo)
        {
            WorkContext.Sync(() => ExecuteModuleMethod(moduleInfo, moduleInfo.ModuleInstance.Initialized));
        }

        public override void RunModuleInternal(ModuleInfo moduleInfo)
        {
            WorkContext.Sync(() => ExecuteModuleMethod(moduleInfo, moduleInfo.ModuleInstance.Run));
        }
    }
}
