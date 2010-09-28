using System;
using Wing.Client.Sdk;
using Wing.Client.Sdk.Events;
using Wing.Composite.Regions;
using Wing.Events;

namespace Wing.Client.Modules.Shell.Views
{
    public class ShellViewPresenter : ViewPresenter<IShellPresentationModel>, IShellViewPresenter
    {
        private IEventAggregator _eventAggregator;

        public ShellViewPresenter(IShellPresentationModel model, IShellView view, IRegionManager regionManager, IEventAggregator eventAggregator, INavigationHistoryService navigationHistory)
            : base(model, view, regionManager)
        {
            view.Model = model;
            _eventAggregator = eventAggregator;
            navigationHistory.OnHistoryChanged += new EventHandler(navigationHistory_OnHistoryChanged);
            view.BackButtonClicked += new EventHandler(view_BackButtonClicked);
            view.HomeButtonClicked += new EventHandler(view_HomeButtonClicked);
        }

        void navigationHistory_OnHistoryChanged(object sender, EventArgs e)
        {
            INavigationHistoryService history = (INavigationHistoryService)sender;
            Model.BackButtonEnabled = history.StackSize > 0;
        }

        public void view_HomeButtonClicked(object sender, EventArgs e)
        {
            CallShellActionEvent(ShellAction.NavigateHome);
        }

        public void view_BackButtonClicked(object sender, EventArgs e)
        {
            CallShellActionEvent(ShellAction.NavigateBack);
        }

        private void CallShellActionEvent(ShellAction shellAction)
        {
            _eventAggregator.GetEvent<ShellActionEvent>()
                .Publish(new ShellActionEventArgs(shellAction));
        }
    }
}
