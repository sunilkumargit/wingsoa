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
using System.Windows.Threading;
using Wing.Utils;
using Wing.Client.Core;
using System.Reflection;

namespace Wing.Client
{
    public partial class EntryPage : UserControl, ISplashUI
    {
        public EntryPage()
        {
            InitializeComponent();
        }

        #region ISplashUI Members

        public void DisplayMessage(string message)
        {
            StatusText.Text = message;
        }

        public void DisplayLoadingBar()
        {
            InitProgressBar.Visibility = Visibility.Visible;
            InitProgressBar.IsIndeterminate = true;
        }

        public void DisplayProgressBar(int max)
        {
            InitProgressBar.Visibility = Visibility.Visible;
            InitProgressBar.IsIndeterminate = false;
            InitProgressBar.Maximum = max;
            InitProgressBar.Value = 0;
        }

        public void UpdateProgressBar(int absoluteValue, int relativeValue)
        {
            if (absoluteValue > 0)
                InitProgressBar.Value = absoluteValue;
            else
                InitProgressBar.Value += relativeValue;
        }

        public void HideProgressBar()
        {
            InitProgressBar.Visibility = Visibility.Collapsed;
        }

        #endregion
    }
}



