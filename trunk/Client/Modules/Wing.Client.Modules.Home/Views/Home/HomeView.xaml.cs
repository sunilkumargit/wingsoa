using System.Windows.Controls;

namespace Wing.Client.Modules.Home.Views.Home
{
    public partial class HomeView : UserControl, IHomeView
    {
        public HomeView()
        {
            InitializeComponent();
        }

        public IHomeViewPresentationModel Model
        {
            get { return DataContext as IHomeViewPresentationModel; }
            set { DataContext = value; }
        }
    }
}
