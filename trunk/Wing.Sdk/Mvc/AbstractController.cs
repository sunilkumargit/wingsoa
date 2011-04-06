using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Wing.Security;

namespace Wing.Mvc
{
    public class AbstractController : Controller
    {
        public IUser CurrentUser
        {
            get { return ServiceLocator.GetInstance<IAuthenticationService>().GetCurrentUser(); }
        }

        public bool IsLoggedIn
        {
            get { return CurrentUser != null; }
        }
    }
}
