using System;
using System.Collections.Generic;
using System.Windows;
using Wing.Client.Core;
using Wing.Client.Sdk;
using Wing.Client.Sdk.Events;
using Wing.Composite.Events;
using Wing.Composite.Regions;
using Wing.Events;

namespace Wing.Client.Modules.Shell
{
    public class ShellService : IShellService
    {
        private IEventAggregator _eventAggregator;
        private IRootVisualManager _visualManager;
        private IShellPresentationModel _presentationModel;
        private List<IViewPresenter> _presenters;
        private IShellViewPresenter _shellPresenter;
        private INavigationHistoryService _navigationHistory;

        public ShellService(IShellViewPresenter shellPresenter, IEventAggregator eventAggregator, INavigationHistoryService navigationHistory, IRootVisualManager rootVisualManager)
        {
            _presenters = new List<IViewPresenter>();
            _eventAggregator = eventAggregator;
            _shellPresenter = shellPresenter;
            _visualManager = rootVisualManager;
            _navigationHistory = navigationHistory;
            _presentationModel = shellPresenter.Model;
            MainContentPresenter = new ShellMainContentPresenter((IShellView)shellPresenter.GetView(), shellPresenter.RegionManager);
        }

        public void StartShell()
        {
            //registrar-se no evento de login
            _eventAggregator.GetEvent<UserLoginEvent>().Subscribe(_UserLoginEvent, ThreadOption.UIThread);
            //registrar-se no evento de alteração de views ativas
            _eventAggregator.GetEvent<ViewBagActiveViewChangedEvent>().Subscribe(_ViewBagActiveViewChangedEvent);
            //registrar-se no evento 'Back' do shell
            _eventAggregator.GetEvent<ShellActionEvent>().Subscribe(_ShellActionEvent);
        }

        public void _UserLoginEvent(UserLoginEventArgs args)
        {
            if (args.Action == UserLoginAction.LoggedIn)
            {
                Helper.DelayExecution(TimeSpan.FromSeconds(1), () =>
                    {
                        _visualManager.SetRootElement((UIElement)_shellPresenter.GetView());
                        return false;
                    });
            }
        }

        public void _ViewBagActiveViewChangedEvent(ViewBagActiveViewChangedEventArgs args)
        {
            _presentationModel.ActiveViews = GetActiveViews();
        }

        public void _ShellActionEvent(ShellActionEventArgs args)
        {
            if (args.Action == ShellAction.NavigateBack)
                NavigateBack();
        }

        private List<IViewPresenter> GetActiveViews()
        {
            var result = new List<IViewPresenter>();
            var presenter = MainContentPresenter.ActivePresenter;
            while (presenter != null)
            {
                result.Add(presenter);
                if (presenter is IViewBagPresenter)
                    presenter = ((IViewBagPresenter)presenter).ActivePresenter;
                else
                    presenter = null;
            }
            return result;
        }

        public IRegionManager RegionManager
        {
            get { return _shellPresenter.RegionManager; }
        }

        public void StatusMessage(string message, params string[] values)
        {
            _presentationModel.StatusMessage = String.Format(message, values);
        }

        public void DisplayProgressBar(int max)
        {
            _presentationModel.ProgressBarIsVisible = true;
            _presentationModel.ProgressBarIsIndeterminate = false;
            _presentationModel.ProgressMaxValue = max;
        }

        public void DisplayWorkingBar()
        {
            _presentationModel.ProgressBarIsVisible = true;
            _presentationModel.ProgressBarIsIndeterminate = true;
        }

        public void UpdateProgressBarRelative(int relative)
        {
            _presentationModel.ProgressValue += relative;
        }

        public void UpdateProgressBarAbsolute(int value)
        {
            _presentationModel.ProgressValue = value;
        }

        public void HideProgressOrWorkingBar()
        {
            _presentationModel.ProgressBarIsVisible = false;
        }

        public void Navigate(IViewPresenter viewPresenter)
        {
            MainContentPresenter.Navigate(viewPresenter);
        }

        public void NavigateBack()
        {
            var previous = _navigationHistory.Pop();
            if (previous != null)
                MainContentPresenter.Navigate(previous, false);
        }

        public IViewBagPresenter MainContentPresenter { get; private set; }
    }
}