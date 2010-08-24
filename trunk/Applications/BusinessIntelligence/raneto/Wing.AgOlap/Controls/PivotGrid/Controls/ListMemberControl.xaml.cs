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

namespace Ranet.AgOlap.Controls.PivotGrid.Controls
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
