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

namespace Wing.Client.Modules.Shell
{
    public partial class ShellView : UserControl
    {
        public ShellView()
        {
            InitializeComponent();
            if (Application.Current.InstallState == InstallState.Installed)
                InstallButton.Visibility = Visibility.Collapsed;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Install();
        }
    }
}
