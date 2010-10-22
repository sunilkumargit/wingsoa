using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Flex.BusinessIntelligence.Data;
using Wing.Client.Sdk;
using Wing.Client.Sdk.Controls;

namespace Flex.BusinessIntelligence.WingClient.Views.CubesConfig
{
    public class BICubesConfigView : HeaderedPage
    {
        private ListView _listView;

        public BICubesConfigView()
        {
            var panel = new StackPanel() { Orientation = Orientation.Vertical };
            var toolbar = new Toolbar() { Margin = new Thickness(5) };
            var newButton = new SimpleButton()
            {
                Content = "Registrar um cubo",
                Command = CommandsManager.GetCommand(BICommandNames.RegisterCube).GetCommandAdapter(),
                Width = 200
            };
            toolbar.RightItems.Add(newButton);
            panel.Children.Add(toolbar);
            _listView = new ListView();
            _listView.DefaultIconSource = "bi;/Assets/cube48.png";
            _listView.TextPropertyName = "CubeName";
            _listView.ItemTriggered += new SingleEventHandler<ListView, object>(_listView_ItemTriggered);
            panel.Children.Add(_listView);
            this.Content = panel;
        }

        void _listView_ItemTriggered(ListView sender, object args)
        {
            if (ItemTriggered != null)
                ItemTriggered.Invoke(args as CubeRegistrationInfo);
        }

        public void SetCubesInfo(IList<CubeRegistrationInfo> info)
        {
            _listView.DataSource = info;
        }

        public event SingleEventHandler<CubeRegistrationInfo> ItemTriggered;
    }
}
