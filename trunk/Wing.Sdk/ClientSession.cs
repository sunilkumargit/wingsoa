using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.SessionState;
using System.Web;
using System.Collections;
using Wing.Client;

namespace Wing
{
    public static class ClientSession
    {
        public static IClientSession Current
        {
            get
            {
                return ServiceLocator.GetInstance<IClientSessionManager>().CurrentSession;
            }
        }
    }
}
