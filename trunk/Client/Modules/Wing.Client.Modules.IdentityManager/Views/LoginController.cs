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
using Wing.Client.Core;
using Wing.Client.Sdk.Services;

namespace Wing.Client.Modules.IdentityManager.Views
{
    public class LoginController : ILoginController
    {
        private ILoginViewPresenter _loginPresenter;
        private IServiceLocator _serviceLocator;
        private IRootVisualManager _visualManager;
        private ILoginService _loginService;

        public LoginController(ILoginViewPresenter loginPresenter, IRootVisualManager visualManager, ILoginService loginService, IServiceLocator serviceLocator)
        {
            _loginPresenter = loginPresenter;
            _serviceLocator = serviceLocator;
            _visualManager = visualManager;
            _loginService = loginService;
        }

        public void OnLoggingIn(string userName, string password)
        {
            _loginService.PerformLogin(userName, password);
        }

        public void CheckLogin()
        {
            if (!_loginService.IsLoggedIn)
                _visualManager.SetRootElement((UIElement)_loginPresenter.View);
        }
    }
}
