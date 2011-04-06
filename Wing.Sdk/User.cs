using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wing.Security;

namespace Wing
{
    public static class User
    {
        public static IUser Current { get { return ServiceLocator.GetInstance<IAuthenticationService>().GetCurrentUser(); } }


    }
}
