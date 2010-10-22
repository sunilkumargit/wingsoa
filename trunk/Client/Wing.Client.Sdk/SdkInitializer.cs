using Wing.Client.Sdk.Services;
using Wing.ServiceLocation;

namespace Wing.Client.Sdk
{
    public static class SdkInitializer
    {
        public static void Initialize()
        {
            var serviceLocator = ServiceLocator.GetCurrent();

            WorkContext.Start();

            //registrar os serviços default
            serviceLocator.Register<IGblCommandsManager, DefaultGlobalCommandsManager>(true);
            CommandsManager.SetCommandsManager(serviceLocator.GetInstance<IGblCommandsManager>());
        }
    }
}
