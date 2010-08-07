using Wing.Client.Sdk;

namespace Wing.Client.Modules.Home.Views.Home
{
    public class HomeViewPresentationModel : ViewPresentationModel, IHomeViewPresentationModel
    {
        public HomeViewPresentationModel()
        {
            Caption = "System";
            Title = "Home";
        }
    }
}
