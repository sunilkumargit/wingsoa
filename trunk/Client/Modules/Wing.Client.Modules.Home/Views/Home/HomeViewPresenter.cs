using Wing.Client.Sdk;
using Wing.Composite.Regions;
using Wing.Events;
using Wing.Client.Sdk.Events;

namespace Wing.Client.Modules.Home.Views.Home
{
    public class HomeViewPresenter : ViewPresenter, IHomeViewPresenter
    {
        private IEventAggregator _eventAggregator;
        private IShellService _shellService;

        public HomeViewPresenter(IHomeViewPresentationModel model, IHomeView view, IRegionManager regionManager, IEventAggregator eventAggregator, IShellService shellService)
            : base(model, view, regionManager)
        {
            view.Model = model;
            _eventAggregator = eventAggregator;
            _shellService = shellService;
            eventAggregator.GetEvent<ShellActionEvent>().Subscribe(_ShellActionEvent);
        }

        public void _ShellActionEvent(ShellActionEventArgs args)
        {
            if (args.Action == ShellAction.NavigateHome)
                _shellService.Navigate(this);
        }
    }
}
