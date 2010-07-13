using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Wing.Client.Modules.Shell.Views
{
	public partial class ShellMenu : UserControl
	{
		public ShellMenu()
		{
			// Required to initialize variables
			InitializeComponent();
            this.HideMenuAnim.Completed += new EventHandler((e, args) =>
            {
                this.Visibility = System.Windows.Visibility.Collapsed;
            });
		}

        public void ShowMenu()
        {
            this.Visibility = System.Windows.Visibility.Visible;
            this.ShowMenuAnim.Begin();
        }

        public void HideMenu()
        {
            this.HideMenuAnim.Begin();
        }

        public void TroggleMenu()
        {
            if (Visibility == System.Windows.Visibility.Collapsed)
                ShowMenu();
            else
                HideMenu();
        }
	}
}