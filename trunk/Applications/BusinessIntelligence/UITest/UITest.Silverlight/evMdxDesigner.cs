using System;
using System.Text;
using System.Windows;

namespace UILibrary.Olap.UITestApplication
{
    public class MyDesigner : Wing.Olap.Controls.PivotMdxDesignerControl
    {
        protected override void InitializePivotGrid(string query)
        {
            //PivotGrid.DefaultMemberAction = Wing.Olap.Controls.MemberClickBehaviorTypes.ExpandCollapse;
            PivotGrid.ColumnTitleClickBehavior = Wing.Olap.Controls.ColumnTitleClickBehavior.SortByValue;
            PivotGrid.DrillDownMode = Wing.Olap.Controls.DrillDownMode.ByCurrentTupleShowSelf;
            base.InitializePivotGrid(query);
        }
        public string GetCurrentMdxQuery()
        {
            return this.PivotGrid.DataManager.GetDataSourceInfo(null).MDXQuery;
        }
    }

    public partial class Page
    {
        void initmdxDesignerButton_Click(object sender, RoutedEventArgs e)
        {
            // NON required property
            // by default URL= <BackToApplicationClientBin>\..\OlapWebService.asmx
            // pivotMdxDesignerControl.URL = WSDataUrl;

            this.pivotMdxDesignerControl.Connection = ConnectionStringId;
            this.pivotMdxDesignerControl.CanSelectCube = false;
            this.pivotMdxDesignerControl.CubeName = "Adventure Works";
            this.pivotMdxDesignerControl.AutoExecuteQuery = true;
            this.pivotMdxDesignerControl.UpdateScript = tbUpdateScript.Text;

            this.pivotMdxDesignerControl.Initialize();
        }
        void exportMdxLayoutButton_Click(object sender, RoutedEventArgs e)
        {
            tbMdxDesignerLayout.Text = pivotMdxDesignerControl.ExportMdxLayoutInfo();
            MessageBox.Show("Ok", "", MessageBoxButton.OK);
        }
        void importMdxLayoutButton_Click(object sender, RoutedEventArgs e)
        {
            pivotMdxDesignerControl.ImportMdxLayoutInfo(tbMdxDesignerLayout.Text);
        }
        private void exportMDXQueryButton_Click(object sender, RoutedEventArgs e)
        {
            tbMdxQuery.Text = pivotMdxDesignerControl.GetCurrentMdxQuery();
            MessageBox.Show("Ok", "", MessageBoxButton.OK);
        }
    }
}
