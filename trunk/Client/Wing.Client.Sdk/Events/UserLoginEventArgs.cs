using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Wing.Composite.Events;

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
