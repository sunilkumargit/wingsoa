using System.Windows;
using System.Windows.Controls;
using Wing.Client.Core;
using System;

namespace Wing.Client
{
    public partial class EntryPage : UserControl, ISplashUI
    {
        private System.Action _quotaIncreaseCallback;
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

        public void ShowLoadingView()
        {
            this.LoadInfo.Visibility = System.Windows.Visibility.Visible;
            this.QuotaIncrease.Visibility = System.Windows.Visibility.Collapsed;
        }

        public void ShowQuotaIncreaseView(System.Action callback)
        {
            this._quotaIncreaseCallback = callback;
            this.LoadInfo.Visibility = System.Windows.Visibility.Collapsed;
            this.QuotaIncrease.Visibility = System.Windows.Visibility.Visible;
        }

        public void DispatchToUI(Action action)
        {
            this.Dispatcher.BeginInvoke(action);
        }

        #endregion

        private void QuotaIncreaseButton_Click(object sender, RoutedEventArgs e)
        {
            _quotaIncreaseCallback();
            ShowLoadingView();
        }

    }
}



