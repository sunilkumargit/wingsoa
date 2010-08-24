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

namespace Ranet.AgOlap.Controls.General
{
    public class CheckedHeaderControl : UserControl
    {
        TextBlock m_Caption;
        CheckBox m_CheckBox;

        public String Caption
        {
            get { return m_Caption.Text; }
            set { m_Caption.Text = value; }
        }

        public bool IsChecked
        {
            get { 
                if(m_CheckBox.IsChecked.HasValue)
                    return m_CheckBox.IsChecked.Value;
                return false; 
            }
            set {
                m_CheckBox.IsChecked = new bool?(value);
            }
        }

        public CheckedHeaderControl()
        {
            // Заголовок
            Grid HeaderLayoutRoot = new Grid();
            HeaderLayoutRoot.VerticalAlignment = VerticalAlignment.Center;
            HeaderLayoutRoot.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            HeaderLayoutRoot.ColumnDefinitions.Add(new ColumnDefinition());

            m_CheckBox = new CheckBox() { Width = 16, Height = 16 };
            m_CheckBox.Checked += new RoutedEventHandler(m_CheckBox_Checked);
            m_CheckBox.Unchecked += new RoutedEventHandler(m_CheckBox_Unchecked);
            HeaderLayoutRoot.Children.Add(m_CheckBox);

            m_Caption = new TextBlock() { Margin = new Thickness(3, 0, 0, 0) };
            HeaderLayoutRoot.Children.Add(m_Caption);
            Grid.SetColumn(m_Caption, 1);

            this.Content = HeaderLayoutRoot;
        }

        public event EventHandler CheckedChanged;
        void Raise_CheckedChanged()
        {
            EventHandler handler = CheckedChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }


        void m_CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Raise_CheckedChanged();
        }

        void m_CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Raise_CheckedChanged();
        }

        public CheckedHeaderControl(bool isChecked, String caption)
            : this()
        {
            IsChecked = isChecked;
            Caption = caption;
        }
    }
}
