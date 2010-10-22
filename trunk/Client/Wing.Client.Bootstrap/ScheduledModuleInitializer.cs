using Wing.Client.Sdk;
using Wing.Logging;
using Wing.Modularity;
using Wing.ServiceLocation;

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
