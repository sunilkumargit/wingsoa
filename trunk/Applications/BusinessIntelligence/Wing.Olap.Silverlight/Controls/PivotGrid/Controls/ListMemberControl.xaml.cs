using System.Windows;
using System.Windows.Controls;

namespace Wing.Olap.Controls.PivotGrid.Controls
{
    public partial class ListMemberControl : UserControl
    {
        public ListMemberControl()
        {
            InitializeComponent();

            this.SizeChanged += new SizeChangedEventHandler(ListMemberControl_SizeChanged);
        }

        void ListMemberControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            LayoutRoot.Width = LayoutRoot.Height = this.ActualHeight;
            border4.CornerRadius = new CornerRadius(this.ActualHeight / 2);
        }
    }
}
