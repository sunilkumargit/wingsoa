using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wing.Events;

namespace Wing.Security
{
    class AuthenticationService : IAuthenticationService
    {
        public const string USER_SESSION_KEY = "_user_";

        private LoginEventArgs RaiseEvent(IUser user, LoginEventAction action)
        {
            var args = new LoginEventArgs(user, action);
            ServiceLocator.GetInstance<IEventAggregator>().GetEvent<LoginEvent>().Publish(args);
            return args;
        }

        public bool PerformLogin(string login, string password, out string message)
        {
            message = "Usuário ou senha inválido(a).";
            if (GetCurrentUser() != null && !PerformLogout())
                throw new AuthenticationException("Não foi possivel encerrar a sessão do usuário atual para fazer um novo login.");

            var account = ServiceLocator.GetInstance<IAccountService>();
            //procurar pelo usuário
            var user = account.GetUser(login);
            if (user == null)
                return false;

            // verificar o password
            if (!user.IsValidPassword(password))
                return false;

            // disparar o evento de login
            var args = RaiseEvent(user, LoginEventAction.LoggingIn);

            if (args.Cancel)
            {
                message = args.Message;
                return false;
            }

            ClientSession.Current.SetUser(user);

            RaiseEvent(user, LoginEventAction.LoggedIn);

            return true;
        }

        public bool PerformLogout()
        {
            var user = GetCurrentUser();
            if (user != null)
            {
                var args = RaiseEvent(user, LoginEventAction.LoggingOut);
                if (args.Cancel)
                {
                    return false;
                }
                else
                {
                    ClientSession.Current.SetUser(null);
                    RaiseEvent(user, LoginEventAction.LoggedOut);
                }
            }
            ClientSession.Current.Close();
            return true;
        }

        public IUser GetCurrentUser()
        {
            return ClientSession.Current.User;
        }
    }
}
