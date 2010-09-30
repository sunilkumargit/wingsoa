using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Wing.Utils;
using Wing.Client.Sdk.Services;

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
