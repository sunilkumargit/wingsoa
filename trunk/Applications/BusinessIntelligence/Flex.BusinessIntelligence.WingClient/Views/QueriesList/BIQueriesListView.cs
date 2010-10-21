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
using Wing.Client.Sdk.Controls;
using Flex.BusinessIntelligence.Client.Interop;
using Wing.Client.Sdk;

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
