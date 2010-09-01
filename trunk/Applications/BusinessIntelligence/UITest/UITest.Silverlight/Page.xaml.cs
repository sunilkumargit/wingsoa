using System;
using System.Windows;
using System.Globalization;

namespace UILibrary.Olap.UITestApplication
{
    public partial class Page : System.Windows.Controls.UserControl
    {
        string WSDataUrl { get { return Config.OlapWebServiceUrl; } }
        string ConnectionStringId { get { return Config.ConnectionStringId; } }

        public Page()
        {
            this.InitializeComponent();
            this.BindData();
        }

        private void LayoutRoot_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        }
    }
}
