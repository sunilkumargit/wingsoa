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
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Ranet.AgOlap.Controls.ToolBar
{
    public class ZoomingToolBarControl : UserControl
    {
        NumericUpDown m_UpDown;
        public ZoomingToolBarControl()
        {
            Grid LayoutRoot = new Grid();
            m_UpDown = new NumericUpDown();
            m_UpDown.Minimum = 10;
            m_UpDown.Maximum = 1000000;
            m_UpDown.Increment = 10;
            m_UpDown.Value = 100;
            Height = 22;
            Width = 50;
            this.Margin = new Thickness(2, 0, 0, 0);
            LayoutRoot.Children.Add(m_UpDown);
            m_UpDown.ValueChanged += new RoutedPropertyChangedEventHandler<double>(m_UpDown_ValueChanged);

            this.Content = LayoutRoot;
        }

        public event EventHandler ValueChanged;
        void m_UpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            EventHandler handle = ValueChanged;
            if (handle != null)
            {
                handle(this, EventArgs.Empty);
            }
        }

        public double Value
        { 
            get{
                return m_UpDown.Value;
            }
            set {
                m_UpDown.Value = value;
            }
        }
    }
}
