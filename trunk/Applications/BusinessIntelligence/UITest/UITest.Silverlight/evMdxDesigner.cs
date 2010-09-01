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
            this.pivotMdxDesignerControl.CanSelectCube = true;
            this.pivotMdxDesignerControl.AutoExecuteQuery = false;
            this.pivotMdxDesignerControl.UpdateScript = tbUpdateScript.Text;

            this.pivotMdxDesignerControl.Initialize();
        }
        void exportMdxLayoutButton_Click(object sender, RoutedEventArgs e)
        {
            tbMdxDesignerLayout.Text = pivotMdxDesignerControl.ExportMdxLayoutInfo();
            MessageBox.Show("Mdx Designer Layout was exported. See Mdx Designer Layout tab.", "Information", MessageBoxButton.OK);
        }
        void importMdxLayoutButton_Click(object sender, RoutedEventArgs e)
        {
            pivotMdxDesignerControl.ImportMdxLayoutInfo(tbMdxDesignerLayout.Text);
        }
        private void exportMDXQueryButton_Click(object sender, RoutedEventArgs e)
        {
            tbMdxQuery.Text = pivotMdxDesignerControl.GetCurrentMdxQuery();
            MessageBox.Show("Mdx query was exported. See Mdx Query tab.", "Information", MessageBoxButton.OK);
        }
    }
}
