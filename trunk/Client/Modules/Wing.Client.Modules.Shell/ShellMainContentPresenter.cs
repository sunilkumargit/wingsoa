using System;
using System.Collections.Generic;
using System.Windows;
using Wing.Client.Core;
using Wing.Client.Modules.Shell.Views;
using Wing.Client.Sdk;
using Wing.Client.Sdk.Events;
using Wing.Composite.Events;
using Wing.Composite.Regions;
using Wing.Events;

namespace Wing.Client.Modules.Shell
{
    public class ShellMainContentPresenter : ViewBagPresenter<ViewPresentationModel>
    {
        public ShellMainContentPresenter(IShellView view, IRegionManager regionManager)
            : base(new ViewPresentationModel(), view, regionManager, ShellRegionNames.ShellMainContent) { }
    }
}
