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
using System.ComponentModel;
using Wing.Client.Sdk;
using Wing.Client.Sdk.Events;
using Wing.Composite.Presentation.Commands;

namespace Wing.Client.Modules.IdentityManager.Views
{
    public interface ILoginPresentationModel : IPresentationModel
    {
        String UserName { get; set; }
        String Password { get; set; }
        DelegateCommand<Object> LoginCommand { get; set; }
        event EventHandler PerfomLogin;
    }
}
