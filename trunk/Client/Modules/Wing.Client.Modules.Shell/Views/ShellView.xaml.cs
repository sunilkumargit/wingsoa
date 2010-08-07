using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Wing.Client.Sdk;

namespace Wing.Client.Modules.Shell.Views
{
    public partial class ShellView : UserControl, IShellView
    {
        //private bool isMaximized;

        public ShellView()
        {
            InitializeComponent();
            //isMaximized = false;
            CheckWindowState();
        }

        private void Close_OnButtonClick(object sender, MouseButtonEventArgs e)
        {
            Application.Current.MainWindow.Close();
        }

        private void Minimize_OnButtonClick(object sender, MouseButtonEventArgs e)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        private void MaximizeButton_OnButtonClick(object sender, MouseButtonEventArgs e)
        {
            Application.Current.MainWindow.WindowState = WindowState.Maximized;
            //isMaximized = true;
            CheckWindowState();
        }

        private void RestoreButton_OnButtonClick(object sender, MouseButtonEventArgs e)
        {
            Application.Current.MainWindow.WindowState = WindowState.Normal;
            //isMaximized = false;
            CheckWindowState();
        }

        private void CheckWindowState()
        {
            MaximizeButton.Visibility = System.Windows.Visibility.Collapsed;
            RestoreButton.Visibility = System.Windows.Visibility.Collapsed;
            /*   MaximizeButton.Visibility = isMaximized ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
               RestoreButton.Visibility = !isMaximized ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
             */
        }

        public IShellPresentationModel Model
        {
            get { return DataContext as IShellPresentationModel; }
            set { DataContext = value; }
        }

        public event EventHandler HomeButtonClicked;

        public event EventHandler BackButtonClicked;

        private void BackButton_OnButtonClick(object sender, MouseButtonEventArgs e)
        {
            if (BackButtonClicked != null)
                BackButtonClicked.Invoke(this, new EventArgs());
        }

        private void HomeButton_OnButtonClick(object sender, MouseButtonEventArgs e)
        {
            if (HomeButtonClicked != null)
                HomeButtonClicked.Invoke(this, new EventArgs());
        }
    }
}
