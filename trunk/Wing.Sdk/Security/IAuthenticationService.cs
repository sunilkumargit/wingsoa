using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wing.Security
{
    public interface IAuthenticationService
    {
        bool PerformLogin(String login, String password, out string message);
        bool PerformLogout();
        IUser GetCurrentUser();
    }
}
