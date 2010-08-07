using Wing.Client.Sdk;
using Wing.Client.Sdk.Controls;
using Wing.Composite.Regions;

namespace Wing.Client.Modules.Home.Views.Root
{
    public class HomeRootPresenter : ViewBagPresenter, IHomeRootPresenter
    {
        public HomeRootPresenter(ViewBagDefaultContainer view, IRegionManager regionManager)
            : base(new ViewPresentationModel("Home", "Home"), view, regionManager)
        { }
    }
}
