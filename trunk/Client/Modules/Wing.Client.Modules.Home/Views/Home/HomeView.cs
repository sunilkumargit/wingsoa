using Wing.Client.Sdk;
using Wing.Composite.Regions;
using Wing.Client.Sdk.Controls;

namespace Wing.Client.Modules.Home.Views.Home
{
    public class HomeView : HeaderedPage, IHomeView
    {
        public HomeView()
        {
            PageTitle = "Home";
            Subtitle = "Bem-vindo";
        }

        public IHomeViewPresentationModel Model
        {
            get
            {
                return (HomeViewPresentationModel)this.DataContext;
            }
            set
            {
                this.DataContext = value;
            }
        }
    }
}
