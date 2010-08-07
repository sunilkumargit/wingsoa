using Wing.Client.Sdk;
using Wing.Composite.Regions;

namespace Wing.Client.Modules.Shell
{
    public class ShellMainContentPresenter : ViewBagPresenter<ViewPresentationModel>
    {
        public ShellMainContentPresenter(IShellView view, IRegionManager regionManager)
            : base(new ViewPresentationModel(), view, regionManager, ShellRegionNames.ShellMainContent) { }
    }
}
