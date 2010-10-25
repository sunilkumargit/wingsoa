using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Wing.Client.Sdk.Events;
using Wing.Events;
using Wing.Client.Sdk;

namespace Wing.Client.Modules.IdentityManager.Views
{
    public partial class LoginView : UserControl, ILoginView
    {
        private SubscriptionToken _loginEventSubscription;

        public LoginView(ILoginPresentationModel model, IEventAggregator eventAggregator)
        {
            InitializeComponent();
            Model = model;

            // se inscrever no evento de login para alterar a aparencia da tela quando algo ocorrer...
            _loginEventSubscription = eventAggregator.GetEvent<UserLoginEvent>().Subscribe(_UserLoginEvent, false);

            Password.KeyUp += new KeyEventHandler(TextBox_KeyUp);
            UserName.KeyUp += new KeyEventHandler(TextBox_KeyUp);
            UserName.Focus();
        }

        public void _UserLoginEvent(UserLoginEventArgs args)
        {
            VisualContext.Async(() =>
            {
                if (args.Action == UserLoginAction.LoggedOut || args.Action == UserLoginAction.LoggingOut)
                {
                    LoginForm.Visibility = System.Windows.Visibility.Visible;
                    LoggingInMessage.Visibility = System.Windows.Visibility.Collapsed;
                }
                else
                {
                    LoginForm.Visibility = System.Windows.Visibility.Collapsed;
                    LoggingInMessage.Visibility = System.Windows.Visibility.Visible;
                }
            });
        }

        void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (Model != null)
            {
                Model.Password = Password.Password;
                Model.UserName = UserName.Text;
            }
        }

        private void Close_OnButtonClick(object sender, MouseButtonEventArgs e)
        {
            Application.Current.MainWindow.Close();
        }

        public ILoginPresentationModel Model
        {
            get { return DataContext as ILoginPresentationModel; }
            set { DataContext = value; }
        }
    }
}
