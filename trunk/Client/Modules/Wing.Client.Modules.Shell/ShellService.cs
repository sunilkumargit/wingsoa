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
    public class ShellService : IShellService
    {
        private IRegionManager _regionManager;
        private IEventAggregator _eventAggregator;
        private IRootVisualManager _visualManager;
        private IShellView _shellView;
        private IShellPresentationModel _presentationModel;
        private List<IViewPresenter> _presenters;

        public ShellService(IShellView view, IShellPresentationModel presentation, IRegionManager regionManager, IEventAggregator eventAggregator, IRootVisualManager rootVisualManager)
        {
            _presenters = new List<IViewPresenter>();
            _shellView = view;
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
            _visualManager = rootVisualManager;
            _presentationModel = presentation;
            view.Model = presentation;
            Wing.Composite.Presentation.Regions.RegionManager.SetRegionManager((DependencyObject)view, _regionManager);
            MainContentPresenter = new ShellMainContentPresenter(view, regionManager, eventAggregator);
        }

        public void StartShell()
        {
            //registrar-se no evento de login
            _eventAggregator.GetEvent<UserLoginEvent>().Subscribe(_UserLoginEvent, ThreadOption.UIThread);
            //registrar-se no evento de alteração de views ativas
            _eventAggregator.GetEvent<ViewBagActiveViewChangedEvent>().Subscribe(_ViewBagActiveViewChangedEvent);
        }

        public void _UserLoginEvent(UserLoginEventArgs args)
        {
            if (args.Action == UserLoginAction.LoggedIn)
            {
                Helper.DelayExecution(TimeSpan.FromSeconds(2), () =>
                    {
                        _visualManager.SetRootElement((UIElement)_shellView);
                        return false;
                    });
            }
        }

        public void _ViewBagActiveViewChangedEvent(ViewBagActiveViewChangedEventArgs args)
        {
            _presentationModel.ActiveViews = GetActiveViews();
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
            get { return _regionManager; }
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

        public void NavigateBack() { }

        public IViewBagPresenter MainContentPresenter { get; private set; }
    }

    public class ShellMainContentPresenter : ViewBagPresenter<ViewPresentationModel>
    {
        public ShellMainContentPresenter(IShellView view, IRegionManager regionManager, IEventAggregator eventAggregator)
            : base(new ViewPresentationModel(), view, regionManager, eventAggregator, ShellRegionNames.ShellMainContent) { }
    }
}
