using System.Windows.Controls;
using Flex.BusinessIntelligence.Client.Interop;
using Wing.Client.Sdk;
using Wing.Client.Sdk.Controls;

namespace Flex.BusinessIntelligence.WingClient.Views.QueriesList
{
    public class BIQueriesListView : HeaderedPage
    {
        private CubeSelectorView _cubeSelector;

        public BIQueriesListView()
        {
            var panel = new StackPanel();
            _cubeSelector = new CubeSelectorView();
            panel.Children.Add(_cubeSelector);
            this.Content = panel;
        }

        public void SetModel(BIQueriesListPresentationModel model)
        {
            VisualContext.Sync(() =>
            {
                this.DataContext = model;
                _cubeSelector.DataContext = model.Cubes;
            });
        }
    }
}
