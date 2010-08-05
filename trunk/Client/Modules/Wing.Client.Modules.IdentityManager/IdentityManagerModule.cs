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
using Wing.ServiceLocation;
using Wing.Client.Sdk.Services;
using Wing.Client.Modules.IdentityManager.Views;

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
            ServiceLocator.Current.Register<ILoginViewPresenter, LoginViewPresenter>();
        }

        public override void Run()
        {
            ServiceLocator.Current.GetInstance<ILoginController>().CheckLogin();
        }
    }
}
