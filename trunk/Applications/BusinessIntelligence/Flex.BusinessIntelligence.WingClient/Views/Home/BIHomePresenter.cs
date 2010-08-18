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
using Wing.Composite.Regions;

namespace Flex.BusinessIntelligence.WingClient.Views.Home
{
    public class BIHomePresenter : ViewPresenter<ViewPresentationModel>
    {
        public BIHomePresenter(IBIHomeView view, IRegionManager regionManager) : base(new ViewPresentationModel("Pivot table - teste", "Pivot table - teste"), view, regionManager) { }
    }
}
