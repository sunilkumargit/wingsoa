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
using Wing.Composite.Regions;
using Wing.Composite.Presentation.Regions;

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
