using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wing.Client.Sdk;
using Wing.Composite.Regions;

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
