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
using Wing.Events;
using Wing.Client.Sdk.Events;

namespace Wing.Client.Modules.Shell.Views
{
    public class ShellViewPresenter : ViewPresenter<IShellPresentationModel>, IShellViewPresenter
    {
        private IEventAggregator _eventAggregator;

        public ShellViewPresenter(IShellPresentationModel model, IShellView view, IRegionManager regionManager, IEventAggregator eventAggregator)
            : base(model, view, regionManager)
        {
            view.Model = model;
            _eventAggregator = eventAggregator;
            view.BackButtonClicked += new EventHandler(view_BackButtonClicked);
            view.HomeButtonClicked += new EventHandler(view_HomeButtonClicked);
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
