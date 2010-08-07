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
using Wing.Client.Sdk;
using Wing.Client.Sdk.Controls;
using Wing.Composite.Regions;
using Wing.Events;

namespace Wing.Client.Modules.Home.Views.Root
{
    public class HomeRootPresenter : ViewBagPresenter, IHomeRootPresenter
    {
        public HomeRootPresenter(ViewBagDefaultContainer view, IRegionManager regionManager, IEventAggregator eventAggregator)
            : base(new ViewPresentationModel("Home", "Home"), view, regionManager, eventAggregator)
        {
        }
    }
}
