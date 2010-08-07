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
using Wing.Utils;
using Wing.Composite.Presentation.Commands;

namespace Wing.Client.Modules.IdentityManager.Views
{
    public class LoginPresentationModel : PresentationModel, ILoginPresentationModel
    {
        private string _userName;
        private string _password;
        private DelegateCommand<Object> _loginCommand;

        public LoginPresentationModel()
        {
            LoginCommand = new DelegateCommand<Object>(LoginCommandExecute, CanLoginCommandExecute);
        }

        private void LoginCommandExecute(Object value)
        {
            if (PerfomLogin != null)
                PerfomLogin.Invoke(this, new EventArgs());
        }

        private bool CanLoginCommandExecute(Object value)
        {
            return UserName.HasValue() && Password.HasValue();
        }

        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = value;
                NotifyPropertyChanged("UserName");
                LoginCommand.RaiseCanExecuteChanged();
            }
        }

        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                NotifyPropertyChanged("Password");
                LoginCommand.RaiseCanExecuteChanged();
            }
        }

        public DelegateCommand<Object> LoginCommand
        {
            get { return _loginCommand; }
            set { _loginCommand = value; NotifyPropertyChanged("LoginCommand"); }
        }

        public event EventHandler PerfomLogin;
    }
}
