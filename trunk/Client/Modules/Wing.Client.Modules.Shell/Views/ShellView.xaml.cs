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
using System.Windows.Media.Imaging;

namespace Wing.Client.Modules.Shell.Views
{
	public partial class ShellView : UserControl
	{
		public ShellView()
		{
			// Required to initialize variables
			InitializeComponent();
            if (Application.Current.InstallState == InstallState.Installing || Application.Current.InstallState == InstallState.Installed || Application.Current.IsRunningOutOfBrowser)
                InstallButton.Visibility = System.Windows.Visibility.Collapsed;
		}

        private void HomeButton_OnButtonClick(object sender, MouseButtonEventArgs e)
        {
            HomeMenu.TroggleMenu();
        }

        private void HomeMenu_MouseLeave(object sender, MouseEventArgs e)
        {
            HomeMenu.HideMenu();
        }

        private void InstallButton_OnButtonClick(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Install();
        }

}
}