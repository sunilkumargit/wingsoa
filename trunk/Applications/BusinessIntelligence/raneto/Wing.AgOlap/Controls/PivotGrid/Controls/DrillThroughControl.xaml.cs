using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Ranet.Olap.Core.Providers;
using Ranet.Olap.Core.Data;

namespace Ranet.AgOlap.Controls.PivotGrid.Controls
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
