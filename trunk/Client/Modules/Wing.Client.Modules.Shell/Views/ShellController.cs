using System.Windows;
using Wing.Composite.Presentation.Regions;
using Wing.Composite.Regions;
using Wing.Events;
using Wing.Client.Sdk.Events;
using Wing.Client.Core;
using Wing.Composite.Events;

namespace Wing.Client.Modules.Shell.Views
{
    public class ShellController
    {
        private IShellViewPresenter _shellViewPresenter;
        private IRegionManager _regionManager;
        private IEventAggregator _eventAggregator;
        private IRootVisualManager _visualManager;

        public ShellController(IShellViewPresenter shellViewPresenter, IRegionManager regionManager, IEventAggregator eventAggregator, IRootVisualManager rootVisualManager)
        {
            _shellViewPresenter = shellViewPresenter;
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
            _visualManager = rootVisualManager;
            RegionManager.SetRegionManager((DependencyObject)_shellViewPresenter.View, _regionManager);

        }

        public void StartShell()
        {
            //registrar-se no evento de login
            _eventAggregator.GetEvent<UserLoginEvent>().Subscribe(_UserLoginEvent, ThreadOption.UIThread);
        }

        public void _UserLoginEvent(UserLoginEventArgs args)
        {
            if (args.Action == UserLoginAction.LoggedIn)
                _visualManager.SetRootElement((UIElement)_shellViewPresenter.View);
        }
    }
}
