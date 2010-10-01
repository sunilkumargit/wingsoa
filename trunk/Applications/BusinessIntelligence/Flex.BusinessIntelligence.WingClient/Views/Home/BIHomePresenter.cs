using Wing.Client.Sdk;
using Wing.Composite.Regions;

namespace Flex.BusinessIntelligence.WingClient.Views.Home
{
    public class BIHomePresenter : ViewBagPresenter<ViewPresentationModel>, IBIHomePresenter
    {
        public BIHomePresenter(IBIHomeView view, IRegionManager regionManager)
            : base(new ViewPresentationModel("", ""), view, regionManager)
        {
        }
    }
}