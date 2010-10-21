using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Wing.Client.Sdk;
using Wing.Client.Sdk.Controls;
using Wing.Olap.Controls;

namespace Flex.BusinessIntelligence.WingClient.Views.PivotGrid
{
    public class PivotGridView : HeaderedPage
    {
        public PivotGridView()
        {
            var designer = new PivotMdxDesignerControl();
            designer.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            designer.VerticalContentAlignment = System.Windows.VerticalAlignment.Stretch;
            designer.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            designer.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Stretch;
            this.Content = designer;
        }
    }
}
