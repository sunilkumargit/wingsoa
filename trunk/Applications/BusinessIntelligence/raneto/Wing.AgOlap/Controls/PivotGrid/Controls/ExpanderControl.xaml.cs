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

namespace Ranet.AgOlap.Controls.PivotGrid.Controls
{
    public partial class ExpanderControl : UserControl
    {
        public ExpanderControl()
        {
            InitializeComponent();

            this.SizeChanged += new SizeChangedEventHandler(ExpanderControl_SizeChanged);
        }

        void ExpanderControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            border.BorderThickness = new Thickness(Math.Round(Math.Max(1, this.ActualHeight / 15)));
            LayoutRoot.Margin = new Thickness(Math.Round(Math.Max(1, (2* this.ActualHeight / 15))));

            //if (this.ActualHeight % 2 != 0)
            {
                LayoutRoot.ColumnDefinitions[1].Width = new GridLength(Math.Round(Math.Max(1, this.ActualHeight / 15)));
                LayoutRoot.RowDefinitions[1].Height = new GridLength(Math.Round(Math.Max(1, this.ActualHeight / 15)));
                //line1.StrokeThickness = 0.5 * this.ActualHeight / 13;
                //line2.StrokeThickness = 0.5 * this.ActualHeight / 13;
                border1.BorderThickness = new Thickness(Math.Round(Math.Max(1, this.ActualHeight / 15)));
                border2.BorderThickness = new Thickness(Math.Round(Math.Max(1, this.ActualHeight / 15)));
            }
        }

        public bool IsExpanded
        {
            get
            {
                if (border2.Visibility == Visibility.Visible)
                    return false;
                return true;
            }
            set {
                if (value)
                {
                    border2.Visibility = Visibility.Collapsed;
                }
                else
                {
                    border2.Visibility = Visibility.Visible;
                }
            }
        }
    }
}
