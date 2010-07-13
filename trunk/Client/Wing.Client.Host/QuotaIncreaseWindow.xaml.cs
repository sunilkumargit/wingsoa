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
using System.IO.IsolatedStorage;

namespace Wing.Client.Host
{
    public partial class QuotaIncreaseWindow : ChildWindow
    {
        public QuotaIncreaseWindow()
        {
            InitializeComponent();
        }

        private Action OkClick;

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            var requiredSize = Wing.Client.Core.Constants.ClientQuotaSize;
            var storage = IsolatedStorageFile.GetUserStoreForApplication();
            this.DialogResult = storage.IncreaseQuotaTo(requiredSize);
            if (OkClick != null)
                OkClick();
        }

        public static void CheckQuotaSize(Action onOk)
        {
            var requiredSize = Wing.Client.Core.Constants.ClientQuotaSize;
            var storage = IsolatedStorageFile.GetUserStoreForApplication();
            if (storage.Quota < requiredSize)
            {
                var window = new QuotaIncreaseWindow();
                window.OkClick = onOk;
                window.Show();
            }
            else if (onOk != null)
                onOk();
        }
    }
}

