using Flex.BusinessIntelligence.WingClient.Views.Root;
using Wing.Modularity;
using Wing.ServiceLocation;
using Flex.BusinessIntelligence.WingClient.Views.Home;

namespace Flex.BusinessIntelligence.WingClient
{
    [Module("FlexBI")]
    [ModuleDescription("Flex Business Intelligence - UI")]
    [ModuleCategory(ModuleCategory.Common)]
    [ModulePriority(ModulePriority.High)]
    public class FlexBusinessIntelligenceModule : ModuleBase
    {
        public override void Initialize()
        {
            ServiceLocator.Current.Register<IBIRootPresenter, BIRootPresenter>();

            ServiceLocator.Current.Register<IBIHomeView, BIHomeView>();
            ServiceLocator.Current.Register<BIHomePresenter, BIHomePresenter>();
        }

        public override void Initialized()
        {
            // ativar o presenter;
            var presenter = ServiceLocator.Current.GetInstance<IBIRootPresenter>();
            presenter.Navigate(ServiceLocator.Current.GetInstance<BIHomePresenter>());
        }
    }

}
