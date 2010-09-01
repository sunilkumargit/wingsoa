﻿using System;
using System.Windows.Controls;
using System.Windows.Data;
using Wing.Olap.Core.Data;
using Wing.Olap.Providers;

namespace Wing.Olap.Controls.Data
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