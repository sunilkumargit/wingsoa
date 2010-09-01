using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;
using Wing.Olap.Controls.PivotGrid.Conditions;

namespace UILibrary.Olap.UITestApplication
{
    public partial class Page
    {
        void initPivotGridButton_Click(object sender, RoutedEventArgs e)
        {
            // pivotGridControl.URL = WSDataUrl;
            pivotGridControl.Connection = ConnectionStringId;
            pivotGridControl.Query = tbMdxQuery.Text;
            pivotGridControl.UpdateScript = tbUpdateScript.Text;
            pivotGridControl.MembersViewMode = (Wing.Olap.Controls.ViewModeTypes)cbMembersViewMode.SelectedIndex;
            pivotGridControl.MemberVisualizationType = (Wing.Olap.Core.Data.MemberVisualizationTypes)cbMemberVisualizationType.SelectedIndex;
            pivotGridControl.DataReorganizationType = (Wing.Olap.Core.Providers.DataReorganizationTypes)cbDataReorganizationType.SelectedIndex;
            pivotGridControl.DefaultMemberAction = (Wing.Olap.Controls.MemberClickBehaviorTypes)cbDefaultMemberAction.SelectedIndex;
            pivotGridControl.ColumnTitleClickBehavior = (Wing.Olap.Controls.ColumnTitleClickBehavior)cbColumnTitleClickBehavior.SelectedIndex;
            pivotGridControl.DrillDownMode = (Wing.Olap.Controls.DrillDownMode)cbDrillDownMode.SelectedIndex;
            pivotGridControl.IsUpdateable = ckbIsUpdateable.IsChecked.Value;
            pivotGridControl.AutoWidthColumns = ckbAutoWidthColumns.IsChecked.Value;
            pivotGridControl.ColumnsIsInteractive = ckbColumnsIsInteractive.IsChecked.Value;
            pivotGridControl.RowsIsInteractive = ckbRowsIsInteractive.IsChecked.Value;
            pivotGridControl.UseColumnsAreaHint = ckbUseColumnsAreaHint.IsChecked.Value;
            pivotGridControl.UseRowsAreaHint = ckbUseRowsAreaHint.IsChecked.Value;
            pivotGridControl.UseCellsAreaHint = ckbUseCellsAreaHint.IsChecked.Value;
            pivotGridControl.UseCellConditionsDesigner = ckbUseCellConditionsDesigner.IsChecked.Value;
            if (!pivotGridControl.UseCellConditionsDesigner)
            {
                var conds = new CellConditionsDescriptor("[Measures].[Internet Sales Amount]");
                var cellApp = new CellAppearanceObject(Colors.Cyan, Colors.Black, Colors.Black);
                cellApp.Options.UseBackColor = true;
                // cellApp.Options.UseBorderColor=true;
                var cond = new CellCondition(CellConditionType.GreaterOrEqual, 1000000.0, 1000000.0, cellApp);
                conds.Conditions.Add(cond);
                pivotGridControl.CustomCellsConditions = new List<CellConditionsDescriptor>();
                pivotGridControl.CustomCellsConditions.Add(conds);
            }
            pivotGridControl.Initialize();
        }
    }
}
