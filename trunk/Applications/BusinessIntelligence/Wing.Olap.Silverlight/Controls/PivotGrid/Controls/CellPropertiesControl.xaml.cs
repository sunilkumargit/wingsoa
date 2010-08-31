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
using Wing.Olap.Core.Providers;
using Wing.AgOlap.Controls.General;
using Wing.Olap.Core.Data;
using Wing.AgOlap.Controls.ValueDelivery;
using Wing.AgOlap.Controls.Tab;
using Wing.AgOlap.Controls.ToolBar;
using Wing.AgOlap.Features;
using System.Text;

namespace Wing.AgOlap.Controls.PivotGrid.Controls
{
    public partial class CellPropertiesControl : UserControl
    {
        PropertiesListControl PropertiesCtrl;
        CellTupleControl TupleCtrl;

        public CellPropertiesControl()
        {
            InitializeComponent();

            PropertiesCtrl = new PropertiesListControl();
            TupleCtrl = new CellTupleControl();

            TabItem PropertiesTab = new TabItem();
            PropertiesTab.Header = Localization.Properties;
            PropertiesTab.Style = TabCtrl.Resources["TabControlOutputItem"] as Style;
            PropertiesTab.Content = PropertiesCtrl;

            TabItem TupleTab = new TabItem();
            TupleTab.Header = Localization.Tuple;
            TupleTab.Style = TabCtrl.Resources["TabControlOutputItem"] as Style;
            TupleTab.Content = TupleCtrl;

            TabCtrl.TabCtrl.Items.Add(PropertiesTab);
            TabCtrl.TabCtrl.Items.Add(TupleTab);

            TupleCtrl.HierarchyColumn.Width = PropertiesCtrl.PropertyColumnWidth;
            TupleCtrl.MemberColumn.Width = PropertiesCtrl.ValueColumnWidth;

            PropertiesTab.Header = Localization.Properties;
            TupleTab.Header = Localization.Tuple;

            TabToolBar toolBar = TabCtrl.ToolBar;
            if (toolBar != null)
            {
                RanetToolBarButton copyBtn = new RanetToolBarButton();
                ToolTipService.SetToolTip(copyBtn, Localization.CopyToClipboard_ToolTip);
                copyBtn.Content = UiHelper.CreateIcon(UriResources.Images.Copy16);
                toolBar.Stack.Children.Add(copyBtn);
                copyBtn.Click += new RoutedEventHandler(CopyButton_Click);
            }
        }

        void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder sb = new StringBuilder();

            if (TabCtrl.TabCtrl.SelectedIndex == 0)
            {
                if (m_Cell != null)
                {
                    if (m_Cell.CellDescr != null && m_Cell.CellDescr.Value != null)
                    {
                        foreach (PropertyData pair in m_Cell.CellDescr.Value.Properties)
                        {
                            sb.AppendFormat("{0}\t", pair.Name);
                            if (pair.Value != null)
                                sb.Append(pair.Value.ToString());
                            sb.Append(Environment.NewLine);
                        }
                    }
                }
            }
            if (TabCtrl.TabCtrl.SelectedIndex == 1)
            {
                foreach (var uniqueName in m_Cell.Tuple.Values)
                {
                    if (sb.Length != 0)
                        sb.Append(", ");
                    sb.Append(uniqueName);
                }
            }
            Wing.AgOlap.Features.Clipboard.SetClipboardText(sb.ToString());
        }

        CellInfo m_Cell;
        public void Initialize(CellInfo cell)
        {
            m_Cell = cell;

            List<PropertyItem> properties_list = new List<PropertyItem>();
            List<PropertyItem> tuple_list = new List<PropertyItem>();

            if (cell != null)
            {
                if (cell.CellDescr != null && cell.CellDescr.Value != null)
                {
                    foreach (PropertyData pair in cell.CellDescr.Value.Properties)
                    {
                        PropertyItem item = new PropertyItem();
                        item.Property = pair.Name;
                        if (pair.Value != null)
                        {
                            item.Value = pair.Value.ToString();
                        }
                        else
                        {
                            item.Value = String.Empty;
                        }
                        properties_list.Add(item);
                    }
                }
            }

            PropertiesCtrl.Initialize(properties_list);
            TupleCtrl.Initialize(cell);
        }
    }
}
