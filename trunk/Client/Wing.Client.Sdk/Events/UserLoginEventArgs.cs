using System;

namespace Wing.Client.Sdk.Events
{
    public class UserLoginEventArgs
    {
        public UserLoginEventArgs(String username, UserLoginAction action)
        {
            Username = username;
            Action = action;
        }

        public String Username { get; set; }
        public UserLoginAction Action { get; private set; }
        public bool Interrupt { get; set; }
    }
}
