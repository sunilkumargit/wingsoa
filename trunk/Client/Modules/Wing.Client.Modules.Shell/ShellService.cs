﻿using System;
using System.Collections.Generic;
using System.Windows;
using Wing.Client.Core;
using Wing.Client.Sdk;
using Wing.Client.Sdk.Events;
using Wing.Composite.Events;
using Wing.Composite.Regions;
using Wing.Events;
using Wing.Client.Sdk.Services;
using Wing.ServiceLocation;
using Wing.Client.Sdk.Controls;
using System.Windows.Controls;

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
        private ModalDialog _dialog;

        public ShellService(IShellViewPresenter shellPresenter, IEventAggregator eventAggregator, INavigationHistoryService navigationHistory, IRootVisualManager rootVisualManager, ISyncContext syncContext)
        {
            _presenters = new List<IViewPresenter>();
            _eventAggregator = eventAggregator;
            _shellPresenter = shellPresenter;
            _visualManager = rootVisualManager;
            _navigationHistory = navigationHistory;
            _presentationModel = shellPresenter.Model;
            _syncContext = syncContext;
            MainContentPresenter = new ShellMainContentPresenter((IShellView)shellPresenter.GetView(), shellPresenter.RegionManager);
        }

        public void StartShell()
        {
            //registrar-se no evento de login
            _eventAggregator.GetEvent<UserLoginEvent>().Subscribe(_UserLoginEvent, ThreadOption.UIThread);
            //registrar-se no evento de alteração de views ativas
            _eventAggregator.GetEvent<ViewBagActiveViewChangedEvent>().Subscribe(_ViewBagActiveViewChangedEvent);
            // registrar o handler de navigate back
            CommandsManager.GetCommand(ShellCommandsNames.NavigateBack).AddHandler(new ShellNavigateBackCommandHandler());
            // navigateTo
            CommandsManager.GetCommand(ShellCommandsNames.NavigateTo).AddHandler(new ShellNavigateToCommandHandler());
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
            _syncContext.Async(() => _presentationModel.StatusMessage = String.Format(message, values));
        }

        public void Alert(String message)
        {
            _syncContext.Async(() =>
            {
                if (_dialog == null)
                {
                    _dialog = new ModalDialog();
                    _dialog.Caption = "Atenção";
                    _dialog.DialogStyle = ModalDialogStyles.OK;
                    _dialog.Content = new TextBlock()
                    {
                        TextWrapping = TextWrapping.Wrap,
                        MinHeight = 30,
                        MinWidth = 150,
                        MaxWidth = 450,
                        FontSize = 11,
                        Margin = new Thickness(5),
                        TextAlignment = System.Windows.TextAlignment.Center,
                        FontFamily = new System.Windows.Media.FontFamily("Tahoma")
                    };

                }
                ((TextBlock)_dialog.Content).Text = message;
                _dialog.Show();
            });
        }

        public void ShowPopup(IPopupWindowPresenter presenter)
        {
            var dialog = new ModalDialog();
            dialog.Caption = presenter.Caption;
            dialog.DialogStyle = ModalDialogStyles.OK;
            var windowPresenter = presenter as IPopupWindowPresenter;
            if (windowPresenter != null)
                windowPresenter.CallbackSetWindowHandler(new ModalDialogWindowHandler(dialog));
        }


        public void DisplayProgressBar(int max)
        {
            _syncContext.Async(() =>
                {
                    _presentationModel.ProgressBarIsVisible = true;
                    _presentationModel.ProgressBarIsIndeterminate = false;
                    _presentationModel.ProgressMaxValue = max;
                });
        }

        public void DisplayWorkingBar()
        {
            _syncContext.Async(() =>
                {
                    _presentationModel.ProgressBarIsVisible = true;
                    _presentationModel.ProgressBarIsIndeterminate = true;
                });
        }

        public void UpdateProgressBarRelative(int relative)
        {
            _syncContext.Async(() => _presentationModel.ProgressValue += relative);
        }

        public void UpdateProgressBarAbsolute(int value)
        {
            _syncContext.Async(() => _presentationModel.ProgressValue = value);
        }

        public void HideProgressOrWorkingBar()
        {
            _syncContext.Async(() => _presentationModel.ProgressBarIsVisible = false);
        }

        public void Navigate(IViewPresenter viewPresenter)
        {
            _syncContext.Async(() => MainContentPresenter.Navigate(viewPresenter));
        }

        public void NavigateBack()
        {
            _syncContext.Sync(() =>
            {
                var previous = _navigationHistory.Pop();
                if (previous != null)
                    MainContentPresenter.Navigate(previous, false);
            });
        }

        public IViewBagPresenter MainContentPresenter { get; private set; }

        // handler para o NavigateBack
        private class ShellNavigateBackCommandHandler : IGlobalCommandHandler
        {

            #region IGlobalCommandHandler Members

            public void QueryStatus(IGlobalCommand command, ref object parameter, ref GblCommandStatus status, ref bool handled)
            {
                status = GblCommandStatus.Enabled;
                handled = true;
            }

            public void Execute(IGlobalCommand command, ref object parameter, ref GblCommandExecStatus execStatus, ref bool handled, ref string outMessage)
            {
                execStatus = GblCommandExecStatus.Executed;
                handled = true;
                ServiceLocator.Current.GetInstance<IShellService>().NavigateBack();
            }

            #endregion
        }

        // handler para o NavigateTo
        private class ShellNavigateToCommandHandler : IGlobalCommandHandler
        {

            #region IGlobalCommandHandler Members

            public void QueryStatus(IGlobalCommand command, ref object parameter, ref GblCommandStatus status, ref bool handled)
            {
                handled = true;
                status = GblCommandStatus.Enabled;
            }

            public void Execute(IGlobalCommand command, ref object parameter, ref GblCommandExecStatus execStatus, ref bool handled, ref string outMessage)
            {
                var presenter = parameter as IViewPresenter;
                if (presenter != null)
                {
                    ServiceLocator.Current.GetInstance<IShellService>().Navigate(presenter);
                    execStatus = GblCommandExecStatus.Executed;
                    handled = true;
                }
                else
                    execStatus = GblCommandExecStatus.Error;
            }

            #endregion
        }


        public ISyncContext _syncContext { get; set; }
    }
}