using System.Windows;
using System.Windows.Controls;

namespace Wing.Client.Sdk.Controls
{
    public partial class ButtonCloseForm : UserControl
    {
        public ButtonCloseForm()
        {
            // Required to initialize variables
            InitializeComponent();

            ToolTipService.SetToolTip(this, "Fechar");
        }

        public event RoutedEventHandler Click;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            RoutedEventHandler handler = Click;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}