using System.Windows;
using Wing.Composite.Presentation.Regions;
using Wing.Composite.Regions;

namespace Wing.Client.Modules.Shell.Views
{
    public class ShellController
    {
        IShellViewPresenter _shellViewPresenter;
        IRegionManager _regionManager;

        public ShellController(IShellViewPresenter shellViewPresenter, IRegionManager regionManager)
        {
            _shellViewPresenter = shellViewPresenter;
            _regionManager = regionManager;
            RegionManager.SetRegionManager((DependencyObject)_shellViewPresenter.View, _regionManager);
        }
    }
}
