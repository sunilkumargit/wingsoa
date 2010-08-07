using System;

namespace Wing.Client.Sdk.Services
{
    public interface ILoginService
    {
        bool PerformLogin(String userName, String password);
        bool PerformLogount();
        bool IsLoggedIn { get; }
    }
}
