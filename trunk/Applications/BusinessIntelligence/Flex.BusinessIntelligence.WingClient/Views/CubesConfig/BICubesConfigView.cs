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
using System.Collections.Generic;
using Flex.BusinessIntelligence.Data;

namespace Flex.BusinessIntelligence.WingClient.Views.CubesConfig
{
    public class BICubesConfigView : HeaderedPage
    {
        private ListView _listView;

        public BICubesConfigView()
        {
            _listView = new ListView();
            _listView.DefaultIconSource = "bi;/Assets/cube48.png";
            _listView.TextPropertyName = "CubeName";
            this.Content = _listView;
        }

        public void SetCubesInfo(IList<CubeRegistrationInfo> info)
        {
            _listView.DataSource = info;
        }
    }
}
