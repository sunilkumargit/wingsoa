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
using Wing.Composite.Regions;
using Wing.ServiceLocation;
using Wing.Client.Sdk;

namespace Wing.Client.Modules.Home
{
    [Module("Home")]
    [ModuleCategory(ModuleCategory.Init)]
    [ModuleDescription("Home do usuário")]
    public class UserHomeModule : IModule
    {
        public void Initialize()
        {
            var regionManager = ServiceLocator.Current.GetInstance<IRegionManager>();
            var homeView = new HomeView();
            ServiceLocator.Current.Register<HomeView>(homeView);
            regionManager.RegisterViewWithRegion(RegionNames.ShellMainContent, () => homeView);
        }

        public void Initialized()
        {
            var regionManager = ServiceLocator.Current.GetInstance<IRegionManager>();
            regionManager.Regions[RegionNames.ShellMainContent].Activate(ServiceLocator.Current.GetInstance<HomeView>());
        }
    }
}
