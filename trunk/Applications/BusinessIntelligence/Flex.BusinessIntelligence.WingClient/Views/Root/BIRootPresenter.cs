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
using System.Windows.Media.Imaging;

namespace Flex.BusinessIntelligence.WingClient.Views.Root
{
    public class BIRootPresenter : ViewBagPresenter, IBIRootPresenter
    {
        private IShellService _shellService;

        public BIRootPresenter(ViewBagDefaultContainer view, IRegionManager regionManager, IShellService shellService)
            : base(new ViewPresentationModel("Business Intelligence", "Business Intelligence"), view, regionManager)
        {
            _shellService = shellService;
            _shellService.RegionManager.Regions[ShellRegionNames.ShellMainBar].Add(CreateBIButton());
        }

        private Object CreateBIButton()
        {
            var button = new ToolButton();
            button.Title = "Business Intelligence";
            button.ImageSource = new BitmapImage(new Uri("../Assets/bi-icon.png", UriKind.Relative));
            button.OnButtonClick += new EventHandler<MouseButtonEventArgs>(button_OnButtonClick);
            return button;
        }

        void button_OnButtonClick(object sender, MouseButtonEventArgs e)
        {
            _shellService.Navigate(this);
        }
    }
}
