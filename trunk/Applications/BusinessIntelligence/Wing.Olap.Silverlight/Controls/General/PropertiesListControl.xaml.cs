/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

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
using Wing.AgOlap.Controls.PivotGrid.Data;
using Wing.Olap.Core.Data;
using Wing.Olap.Core.Providers;

namespace Wing.AgOlap.Controls.General
{
    public class PropertyItem
    {
        public String Property { get; set; }
        public String Value { get; set; }

        public PropertyItem()
        {
        }

        public PropertyItem(String property, String value)
        {
            Property = property;
            Value = value;
        }
    }

    public partial class PropertiesListControl : UserControl
    {
        internal readonly DataGridTextColumn propertyColumn = null;
        internal readonly DataGridTextColumn valueColumn = null;

        public PropertiesListControl()
        {
            InitializeComponent();

            grid.AutoGenerateColumns = false;
            grid.IsReadOnly = true;
            grid.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            grid.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;

            propertyColumn = new DataGridTextColumn();
            propertyColumn.Header = Localization.PropertiesList_PropertyColumn;
            propertyColumn.Binding = new System.Windows.Data.Binding("Property");
            propertyColumn.CanUserResize = true;
            propertyColumn.Width = new DataGridLength(200);
            grid.Columns.Add(propertyColumn);

            valueColumn = new DataGridTextColumn();
            valueColumn.Header = Localization.PropertiesList_ValueColumn;
            valueColumn.Binding = new System.Windows.Data.Binding("Value");
            valueColumn.CanUserResize = true;
            valueColumn.Width = new DataGridLength(200);
            grid.Columns.Add(valueColumn);

            //grid.ItemsSource = "H e l l o W o r l d !".Split();
            Clear();
            grid.RowHeight = 22;
        }

        public DataGridLength PropertyColumnWidth
        {
            get { return propertyColumn.Width; }
            set { propertyColumn.Width = value; }
        }

        public DataGridLength ValueColumnWidth
        {
            get { return valueColumn.Width; }
            set { valueColumn.Width = value; }
        }

        public void Clear()
        {
            List<PropertyItem> list = new List<PropertyItem>();
            grid.ItemsSource = list;
        }

        public void Initialize(List<PropertyItem> list)
        {
            if (list == null)
            {
                list = new List<PropertyItem>();
            }
            grid.ItemsSource = list;
        }

        public List<PropertyItem> List
        {
            get {
                var list = grid.ItemsSource as List<PropertyItem>;
                if(list != null)
                    return list; 
                else
                    return new List<PropertyItem>();
            }
        }

    }
}
