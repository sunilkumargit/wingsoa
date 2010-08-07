﻿using Wing.Client.Sdk;
using Wing.Composite.Regions;
using Wing.Modularity;
using Wing.ServiceLocation;
using Wing.Client.Modules.Home.Views.Home;
using Wing.Client.Modules.Home.Views.Root;

namespace Wing.Client.Modules.Home
{
    [Module("Home")]
    [ModuleCategory(ModuleCategory.Init)]
    [ModuleDescription("Home do usuário")]
    public class UserHomeModule : ModuleBase
    {
        public override void Initialize()
        {
            ServiceLocator.Current.Register<IHomeRootPresenter, HomeRootPresenter>(true);

            ServiceLocator.Current.Register<IHomeView, HomeView>();
            ServiceLocator.Current.Register<IHomeViewPresenter, HomeViewPresenter>(true);
            ServiceLocator.Current.Register<IHomeViewPresentationModel, HomeViewPresentationModel>(true);
        }

        public override void Run()
        {
            var homeRoot = ServiceLocator.Current.GetInstance<IHomeRootPresenter>();
            homeRoot.Navigate(ServiceLocator.Current.GetInstance<IHomeViewPresenter>());
            ServiceLocator.Current.GetInstance<IShellService>().Navigate(homeRoot);
        }
    }
}
