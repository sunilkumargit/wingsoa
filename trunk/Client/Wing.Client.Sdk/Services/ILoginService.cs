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

namespace Wing.Client.Sdk.Services
{
    public interface ILoginService
    {
        bool PerformLogin(String userName, String password);
        bool PerformLogount();
        bool IsLoggedIn { get; }
    }
}
