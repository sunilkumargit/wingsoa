using System;

namespace Wing.Client.Sdk.Events
{
    public class UserLoginEventArgs
    {
        public UserLoginEventArgs(String username, bool isLogon)
        {
            Username = username;
            IsLogon = isLogon;
        }

        public String Username { get; set; }
        public bool IsLogon { get; private set; }
    }
}
