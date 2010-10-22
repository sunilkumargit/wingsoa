using System;
using System.Windows.Controls;
using Wing.Olap.Core.Data;
using Wing.Olap.Core.Providers;

namespace Wing.Olap.Controls.PivotGrid.Controls
{
    public partial class DrillThroughControl : UserControl
    {
        public DrillThroughControl()
        {
            InitializeComponent();
            lblDrillThrough.Text = Localization.Drillthrough_Label;
            lblTuple.Text = Localization.ValueDeliveryControl_DeliveredCellSettings;
            lblValue.Text = Localization.CellValue_Label;
        }

        public void Initialize(CellInfo cell, DataTableWrapper data)
        {
            if (cell != null &&
                cell.CellDescr != null &&
                cell.CellDescr.Value != null)
            {
                txtValue.Text = cell.CellDescr.Value.DisplayValue;
            }
            else
            {
                txtValue.Text = String.Empty;
            }
            tupleCtrl.Initialize(cell);
            dataGrid.Initialize(data);
        }
    }
}
