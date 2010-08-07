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
        private ILoginView _loginView;
        private IRootVisualManager _visualManager;
        private ILoginService _loginService;

        public LoginController(ILoginView view, IRootVisualManager visualManager, ILoginService loginService)
        {
            _visualManager = visualManager;
            _loginService = loginService;
            _loginView = view;
            _loginView.Model.UserName = "System";
            _loginView.Model.Password = "System";
            _loginView.Model.PerfomLogin += new EventHandler(Model_PerfomLogin);
        }

        void Model_PerfomLogin(object sender, EventArgs e)
        {
            _loginService.PerformLogin(_loginView.Model.UserName, _loginView.Model.Password);
        }

        public void CheckLogin()
        {
            if (!_loginService.IsLoggedIn)
                _visualManager.SetRootElement((UIElement)_loginView);
        }
    }
}
