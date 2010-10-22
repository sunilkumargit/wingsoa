using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Wing.Client.Sdk.Controls
{
    public partial class LeftSideBar : UserControl
    {
        public LeftSideBar()
        {
            InitializeComponent();
            Title.Style = (Style)Application.Current.Resources["PageSideBarTitle"];
            LayoutRoot.Background = (Brush)Application.Current.Resources["LightToolbarBrush"];
        }

        public String Caption { get { return Title.Text; } set { Title.Text = value; } }

        public Object InnerContent { get { return ContentHolder.Content; } set { ContentHolder.Content = value; } }
    }
}
