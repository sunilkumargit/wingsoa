using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wing.Events;

namespace Wing.Security
{
    public class LoginEvent : Event<EventArgs>
    {
    }

    public class LoginEventArgs : EventArgs
    {
        public LoginEventArgs(IUser user, LoginEventAction action)
        {
            Action = action;
            User = user;
            Cancel = false;
        }

        public LoginEventAction Action { get; private set; }
        public IUser User { get; private set; }
        public bool Cancel { get; set; }
        public String Message { get; set; }
    }
}
