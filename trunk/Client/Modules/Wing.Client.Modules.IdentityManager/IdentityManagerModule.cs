using Wing.Client.Modules.IdentityManager.Views;
using Wing.Client.Sdk.Services;
using Wing.Modularity;
using Wing.ServiceLocation;
using Wing.Client.Core;
using System;
using Wing.Client.Sdk;

namespace Wing.Client.Modules.IdentityManager
{
    [Module("UserIdentityManager")]
    [ModuleDescription("Cliente do gerenciador de identidades do Wing")]
    [ModuleCategory(ModuleCategory.Core)]
    [ModulePriority(ModulePriority.High)]
    public class IdentityManagerModule : ModuleBase
    {
        public override void Initialize()
        {
            ServiceLocator.Register<ILoginService, LoginService>(true);

            ServiceLocator.Register<ILoginView, LoginView>();
            ServiceLocator.Register<ILoginController, LoginController>(true);
            ServiceLocator.Register<ILoginPresentationModel, LoginPresentationModel>();
        }

        public override void Run()
        {
            WorkContext.Sync(() =>
            {
                VisualContext.DelayAsync(TimeSpan.FromMilliseconds(2000),
                    () => ServiceLocator.GetInstance<ILoginController>().CheckLogin());
            });
        }
    }
}
