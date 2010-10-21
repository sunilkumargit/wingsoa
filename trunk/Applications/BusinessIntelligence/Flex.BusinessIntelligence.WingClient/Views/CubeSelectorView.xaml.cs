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
using Wing.Client.Sdk;
using Wing.Client.Sdk.Controls;
using System.Windows.Data;

namespace Flex.BusinessIntelligence.WingClient.Views
{
    public partial class CubeSelectorView : UserControl
    {
        private SimpleButton button;
        public CubeSelectorView()
        {
            InitializeComponent();
            button = new SimpleButton();
            button.Content = "Consultar";
            button.Width = 100;
            button.MinHeight = 30;
            button.FontSize = 16;
            button.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            button.Margin = new Thickness(5, 0, 0, 0);
            button.Command = CommandsManager.GetCommand(BICommandNames.NewCubeQuery).GetCommandAdapter();
            Holder.Children.Add(button);

            Combo.SelectionChanged += new SelectionChangedEventHandler(Combo_SelectionChanged);
        }

        void Combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            button.CommandParameter = Combo.SelectedItem;
            CommandsManager.GetCommand(BICommandNames.NewCubeQuery).FireStateChanged();
        }

        private void Image_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            ControlHelper.TryLoadImage(this, ((Image)sender).Source);
        }
    }
}
