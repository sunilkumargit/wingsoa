/*   
    Copyright (C) 2009 Galaktika Corporation ZAO

    This file is a part of Wing.UILibrary.Olap
 
    Wing.UILibrary.Olap is a free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
      
    You should have received a copy of the GNU General Public License
    along with Wing.UILibrary.Olap.  If not, see
  	<http://www.gnu.org/licenses/> 
  
    If GPL v.3 is not suitable for your products or company,
    Galaktika Corp provides Wing.UILibrary.Olap under a flexible commercial license
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

namespace Wing.AgOlap.Controls.General
{
    public partial class CustomPanel : UserControl
    {
        public new UIElement Content
        {
            get
            {
                if (grdContent.Children.Count > 0)
                    return grdContent.Children[0];
                return null;
            }
            set
            {
                grdContent.Children.Clear();
                grdContent.Children.Add(value);
            }
        }

        public CustomPanel()
        {
            InitializeComponent();
        }

        public new bool IsEnabled
        {
            get { return base.IsEnabled; }
            set
            {
                if (value)
                {
                    brdEnabled.Visibility = Visibility.Collapsed;
                }
                else
                {
                    brdEnabled.Visibility = Visibility.Visible;
                }
                base.IsEnabled = value;
            }
        }
    }
}
