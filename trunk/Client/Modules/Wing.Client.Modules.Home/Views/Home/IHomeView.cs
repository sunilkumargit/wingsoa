using System.Windows.Controls;
using Wing.Client.Sdk;

namespace Wing.Client.Modules.Home.Views.Home
{
    public interface IHomeView
    {
        IHomeViewPresentationModel Model { get; set; }
    }
}
