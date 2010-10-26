using Wing.Client.Sdk.Controls;
using Wing.Olap.Controls;
using System.Windows.Controls;
using System.Windows;
using System;
using Wing.Client.Sdk;

namespace Flex.BusinessIntelligence.WingClient.Views.PivotGrid
{
    public class PivotGridView : HeaderedPage
    {
        private PivotMdxDesignerControl designer;

        public PivotGridView()
        {
            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(25) });
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });

            var toolbar = new Toolbar();
            var saveQueryButton = new SimpleButton() { Content = "Salvar esta consulta... ", Width = 200 };
            saveQueryButton.Click += new RoutedEventHandler(saveQueryButton_Click);
            toolbar.LeftItems.Add(saveQueryButton);
            grid.Children.Add(toolbar);

            designer = new PivotMdxDesignerControl();
            designer.SetValue(Grid.RowProperty, 1);
            grid.Children.Add(designer);
            designer.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            designer.VerticalContentAlignment = System.Windows.VerticalAlignment.Stretch;
            designer.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            designer.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Stretch;
            this.Content = grid;
        }

        void saveQueryButton_Click(object sender, RoutedEventArgs e)
        {
            var layoutInfo = designer.ExportMdxLayoutInfo();
            if (SaveQueryTriggered != null)
                SaveQueryTriggered.Invoke(layoutInfo);
        }

        internal void SetModel(PivotGridPresentationModel Model)
        {
            designer.Connection = Model.CubeInfo.GetConnectionString();
            designer.CubeName = Model.CubeInfo.CubeName;
            designer.Initialize();
            if (Model.QueryInfo != null)
                designer.ImportMdxLayoutInfo(Model.QueryInfo.QueryData);
        }

        public event SingleEventHandler<String> SaveQueryTriggered;
    }
}
