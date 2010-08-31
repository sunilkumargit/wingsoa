/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Windows;
using System.Windows.Controls;
using Wing.Olap.Core.Providers;

namespace Wing.Olap.Controls.General.DataGrid
{
    public class IconDataGridColumn : DataGridTemplateColumn
    {
        public IconDataGridColumn()
        {
            this.IsReadOnly = true;            
        }

        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            if (!string.IsNullOrEmpty(this.Resource) && !string.IsNullOrEmpty(this.CustomResourceFormat))
            {
                Image icon = UiHelper.CreateIcon(UriResources.GetImage(String.Format(this.CustomResourceFormat,this.Resource)));
                return icon;
            }
            if (!string.IsNullOrEmpty(this.Resource))
            {             
                if (dataItem is KpiView && Resource == "Status")
                {
                    Image icon = UiHelper.CreateIcon(UriResources.GetImage(GetKpiIconByPath(this.Resource,(dataItem as KpiView).StatusGraphic)));
                    return icon;
                }
                if (dataItem is KpiView && Resource == "Trend")
                {
                    Image icon = UiHelper.CreateIcon(UriResources.GetImage(GetKpiIconByPath(this.Resource,(dataItem as KpiView).TrendGraphic)));
                    return icon;
                }
            }
            TextBlock block = new TextBlock();
            block.Margin = new Thickness(4.0);
            block.VerticalAlignment = VerticalAlignment.Center;
            block.Text = dataItem.ToString();
            return block;
        }

        private string GetKpiIconByPath(string path, string name)
        {
            return String.Format("/Wing.Olap.Silverlight;component/Controls/Images/OLAP/KPI/{0}/{1}", path, name);
        }
        
        public string Resource
        {
            get; set;
        }

        public string CustomResourceFormat
        {
            get; set;
        }
    }
}
