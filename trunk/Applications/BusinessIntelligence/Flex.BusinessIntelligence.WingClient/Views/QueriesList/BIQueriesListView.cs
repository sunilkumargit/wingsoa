using System.Windows.Controls;
using Flex.BusinessIntelligence.Client.Interop;
using Wing.Client.Sdk;
using Wing.Client.Sdk.Controls;
using Flex.BusinessIntelligence.Data;
using System.Windows;

namespace Flex.BusinessIntelligence.WingClient.Views.QueriesList
{
    public class BIQueriesListView : HeaderedPage
    {
        private CubeSelectorView _cubeSelector;
        private ListView _listView;

        public BIQueriesListView()
        {
            var panel = new StackPanel() { Name = "ItemsHolder" };
            _cubeSelector = new CubeSelectorView();
            panel.Children.Add(_cubeSelector);
            _listView = new ListView();
            _listView.Margin = new Thickness(0, 20, 0, 25);
            _listView.DefaultIconSource = "bi;/Assets/report48.png";
            _listView.TextPropertyName = "Name";
            _listView.ItemTriggered += new SingleEventHandler<ListView, object>(_listView_ItemTriggered);
            panel.Children.Add(_listView);
            this.Content = panel;
        }

        void _listView_ItemTriggered(ListView sender, object args)
        {
            if (ItemTriggered != null)
                ItemTriggered.Invoke(args as CubeQueryInfo);
        }

        public event SingleEventHandler<CubeQueryInfo> ItemTriggered;

        public void SetModel(BIQueriesListPresentationModel model)
        {
            VisualContext.Sync(() =>
            {
                this.DataContext = model;
                _cubeSelector.DataContext = model.Cubes;
                _listView.DataSource = model.Queries;
            });
        }
    }
}
