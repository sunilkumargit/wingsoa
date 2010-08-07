using Wing.Client.Modules.IdentityManager.Views;
using Wing.Client.Sdk.Services;
using Wing.Modularity;
using Wing.ServiceLocation;

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
            ServiceLocator.Current.Register<ILoginService, LoginService>(true);

            ServiceLocator.Current.Register<ILoginView, LoginView>();
            ServiceLocator.Current.Register<ILoginController, LoginController>(true);
            ServiceLocator.Current.Register<ILoginPresentationModel, LoginPresentationModel>();
        }

        public override void Run()
        {
            ServiceLocator.Current.GetInstance<ILoginController>().CheckLogin();
        }
    }
}
