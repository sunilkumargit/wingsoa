/*   
    Copyright (C) 2009 Galaktika Corporation ZAO

    This file is a part of Ranet.UILibrary.Olap
 
    Ranet.UILibrary.Olap is a free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
      
    You should have received a copy of the GNU General Public License
    along with Ranet.UILibrary.Olap.  If not, see
  	<http://www.gnu.org/licenses/> 
  
    If GPL v.3 is not suitable for your products or company,
    Galaktika Corp provides Ranet.UILibrary.Olap under a flexible commercial license
    designed to meet your specific usage and distribution requirements.
    If you have already obtained a commercial license from Galaktika Corp,
    you can use this file under those license terms.
*/

using System;
using System.Windows;
using System.Windows.Controls;
using Ranet.Olap.Core.Providers;

namespace Ranet.AgOlap.Controls.General.DataGrid
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
            return String.Format("/Ranet.AgOlap;component/Controls/Images/OLAP/KPI/{0}/{1}", path, name);
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
