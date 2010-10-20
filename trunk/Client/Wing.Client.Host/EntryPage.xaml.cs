using System.Windows;
using System.Windows.Controls;
using Wing.Client.Core;

namespace Wing.Client
{
    public partial class EntryPage : UserControl, ISplashUI
    {
        public EntryPage()
        {
            InitializeComponent();
            InitProgressBar.Dispatcher.BeginInvoke(() =>
            {
                InitProgressBar.IsIndeterminate = true;
            });
        }

        #region ISplashUI Members

        public void DisplayMessage(string message)
        {
            StatusText.Dispatcher.BeginInvoke(() =>
            {
                StatusText.Text = message;
            });
        }

        public void DisplayLoadingBar()
        {
            InitProgressBar.Dispatcher.BeginInvoke(() =>
            {
                InitProgressBar.Visibility = Visibility.Visible;
                InitProgressBar.IsIndeterminate = true;
            });
        }

        public void DisplayProgressBar(int max)
        {
            InitProgressBar.Dispatcher.BeginInvoke(() =>
            {
                InitProgressBar.IsIndeterminate = false;
                InitProgressBar.Maximum = max;
                InitProgressBar.Value = 0;
                InitProgressBar.Visibility = Visibility.Visible;
            });
        }

        public void UpdateProgressBar(int absoluteValue, int relativeValue)
        {
            InitProgressBar.Dispatcher.BeginInvoke(() =>
            {
                if (absoluteValue > 0)
                    InitProgressBar.Value = absoluteValue;
                else
                    InitProgressBar.Value += relativeValue;
            });
        }

        public void HideProgressBar()
        {
            InitProgressBar.Dispatcher.BeginInvoke(() =>
            {
                InitProgressBar.Visibility = Visibility.Collapsed;
            });
        }

        public void DisplayStatusMessage(string message)
        {
            StatusMessage.Dispatcher.BeginInvoke(() =>
            {
                StatusMessage.Text = message;
            });
        }

        #endregion
    }
}



