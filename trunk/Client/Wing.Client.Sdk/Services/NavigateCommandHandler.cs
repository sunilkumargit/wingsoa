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
using Wing.ServiceLocation;

namespace Wing.Client.Sdk.Services
{
    public class NavigateCommandHandler : IGlobalCommandHandler
    {
        private IViewPresenter _presenter;
        private Type _presenterType;

        public NavigateCommandHandler(IViewPresenter presenter)
        {
            _presenter = presenter;
        }

        public NavigateCommandHandler(Type presenterType)
        {
            _presenterType = presenterType;
        }

        public void QueryStatus(IGlobalCommand command, object parameter, ref GblCommandStatus status, ref bool handled)
        {
            status = GblCommandStatus.Enabled;
            handled = true;
        }

        public void Execute(IGlobalCommand command, object parameter, ref GblCommandExecStatus execStatus, ref bool handled, ref string outMessage)
        {
            var shellService = ServiceLocator.Current.GetInstance<IShellService>();
            if (_presenter != null)
                shellService.Navigate(_presenter);
            else if (_presenterType != null)
                shellService.Navigate((IViewPresenter)ServiceLocator.Current.GetInstance(_presenterType));
            execStatus = GblCommandExecStatus.Executed;
            handled = true;
        }
    }
}
