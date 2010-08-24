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
using Ranet.Olap.Core.Data;
using Ranet.AgOlap.Providers;
using System.Windows.Data;

namespace Ranet.AgOlap.Controls.Data
{
    public class RanetDataGrid : DataGrid
    {
        public RanetDataGrid()
        {
           DefaultStyleKey = typeof(RanetDataGrid);
        }

        public void Initialize(DataTableWrapper data)
        {
            this.Columns.Clear();
            this.ItemsSource = null;
            
            if (data != null)
            {
                this.Columns.Clear();
                this.ItemsSource = null;
                var source = DataTable.Create(data);
                this.AutoGenerateColumns = false;
                int i = 0;
                foreach (var entry in source.Columns)
                {
                    this.Columns.Add(new DataGridTextColumn() { Header = entry.Name, Binding = new Binding(String.Format("[{0}].Value", i)) });
                    i++;
                }
                this.ItemsSource = source.Rows;
                this.SelectedIndex = 0;
            }
        }
    }
}
