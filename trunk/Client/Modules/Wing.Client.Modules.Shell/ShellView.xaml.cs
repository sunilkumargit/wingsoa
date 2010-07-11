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
            MenuHolder.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            MenuHolder.Visibility = System.Windows.Visibility.Visible;
        }

        private void MenuHolder_MouseLeave(object sender, MouseEventArgs e)
        {
            MenuHolder.Visibility = System.Windows.Visibility.Collapsed;
        }

       

    }
}
