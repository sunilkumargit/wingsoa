using System;
using Wing.Client.Sdk;
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
