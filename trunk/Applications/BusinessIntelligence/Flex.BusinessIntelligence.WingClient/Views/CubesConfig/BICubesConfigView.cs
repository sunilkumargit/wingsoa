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
using Flex.BusinessIntelligence.Client.Interop;
using Wing.Client.Sdk.Controls;

namespace Flex.BusinessIntelligence.WingClient.Views.CubesConfig
{
    public class BICubesConfigView : HeaderedPage
    {
        public BICubesConfigView()
        {
            var listView = new ListView();
            this.Content = listView;
        }
    }
}
