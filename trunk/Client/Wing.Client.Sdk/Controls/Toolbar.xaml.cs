using System.Windows;
using System.Windows.Controls;

namespace Wing.Client.Sdk.Controls
{
    public partial class Toolbar : UserControl
    {
        public Toolbar()
        {
            InitializeComponent();
        }

        public Style BarStyle { get { return OuterBorder.Style; } set { OuterBorder.Style = value; } }

        public ItemCollection LeftItems { get { return LeftHolder.Items; } }

        public ItemCollection RightItems { get { return RightHolder.Items; } }
    }
}
