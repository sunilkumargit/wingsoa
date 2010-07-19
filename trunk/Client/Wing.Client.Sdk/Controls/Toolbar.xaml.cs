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
