using Wing.Client.Sdk;
using Wing.Composite.Regions;
using Wing.Modularity;
using Wing.ServiceLocation;

namespace Wing.Client.Modules.Home
{
    [Module("Home")]
    [ModuleCategory(ModuleCategory.Init)]
    [ModuleDescription("Home do usuário")]
    public class UserHomeModule : ModuleBase
    {
        public override void Initialize()
        {
            var regionManager = ServiceLocator.Current.GetInstance<IRegionManager>();
            ServiceLocator.Current.Register<HomeView, HomeView>(true);
            regionManager.RegisterViewWithRegion(RegionNames.ShellMainContent, () =>
            {
                return ServiceLocator.Current.GetInstance<HomeView>();
            });
        }
    }
}
