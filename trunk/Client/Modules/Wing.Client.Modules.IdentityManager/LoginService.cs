using Wing.Client.Sdk.Events;
using Wing.Client.Sdk.Services;
using Wing.Events;

namespace Wing.Client.Modules.IdentityManager
{
    public class LoginService : ILoginService
    {
        private IEventAggregator _eventAggregator;


        public LoginService(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public bool PerformLogin(string userName, string password)
        {
            var eventArgs = new UserLoginEventArgs(userName, UserLoginAction.LoggingOn);
            _eventAggregator.GetEvent<UserLoginEvent>().Publish(eventArgs);
            if (eventArgs.Interrupt)
                return false;

            // executar os serviços de login aqui...

            _eventAggregator.GetEvent<UserLoginEvent>().Publish(new UserLoginEventArgs(userName, UserLoginAction.LoggedIn));

            IsLoggedIn = true;

            return true;
        }

        public bool PerformLogount()
        {
            var eventArgs = new UserLoginEventArgs("", UserLoginAction.LoggingOut);
            _eventAggregator.GetEvent<UserLoginEvent>().Publish(eventArgs);
            if (eventArgs.Interrupt)
                return false;

            // executar o serviço de logout aqui...

            _eventAggregator.GetEvent<UserLoginEvent>().Publish(new UserLoginEventArgs("", UserLoginAction.LoggedOut));

            IsLoggedIn = false;

            return true;
        }

        public bool IsLoggedIn { get; private set; }
    }
}
