using System;
using Wing.ServiceLocation;

namespace Wing.Client.Sdk.Services
{
    public class NavigateCommandHandler : IGblCommandHandler
    {
        private IViewPresenter _presenter;
        private Type _presenterType;
        private Type _parentPresenterType;

        public NavigateCommandHandler(IViewPresenter presenter)
        {
            _presenter = presenter;
        }

        public NavigateCommandHandler(Type presenterType)
        {
            _presenterType = presenterType;
        }

        public NavigateCommandHandler(Type presenterType, Type parentPresenterType)
        {
            _presenterType = presenterType;
            _parentPresenterType = parentPresenterType;
        }


        public void QueryStatus(IGblCommandQueryStatusContext ctx)
        {
            ctx.Status = GblCommandStatus.Enabled;
            ctx.Handled = true;
        }

        public void Execute(IGblCommandExecuteContext ctx)
        {
            var shellService = ServiceLocator.GetInstance<IShellService>();
            if (_presenter != null)
                shellService.Navigate(_presenter);
            else if (_presenterType != null)
            {
                var presenter = (IViewPresenter)ServiceLocator.GetInstance(_presenterType);
                if (_parentPresenterType != null)
                {
                    var parent = (IViewBagPresenter)ServiceLocator.GetInstance(_parentPresenterType);
                    parent.Navigate(presenter);
                }
                shellService.Navigate(presenter);
            }
            ctx.Status = GblCommandExecStatus.Executed;
            ctx.Handled = true;
        }
    }
}