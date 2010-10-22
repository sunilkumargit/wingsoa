using Wing.Client.Sdk;
using Wing.Composite.Regions;
using Wing.Client.Sdk.Controls;

namespace Wing.Client.Modules.Home.Views.Home
{
    public class HomeViewPresenter : ViewPresenter, IHomeViewPresenter
    {
        public HomeViewPresenter(IHomeViewPresentationModel model, IHomeView view, IRegionManager regionManager)
            : base(model, view, regionManager)
        {
            view.Model = model;
        }
    }
}