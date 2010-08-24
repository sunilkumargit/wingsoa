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
using System.Windows.Media.Imaging;

namespace Ranet.AgOlap.Controls.General
{
    public class HeaderControl : UserControl
    {
        TextBlock m_Caption;
        Image m_Icon;
        Image m_TransparentIcon;

        public String Caption
        {
            get { return m_Caption.Text; }
            set { m_Caption.Text = value; }
        }

        public BitmapImage Icon
        {
            get { return m_Icon.Source as BitmapImage; }
            set { m_Icon.Source = value; }
        }

        public BitmapImage TransparentIcon
        {
            get { return m_TransparentIcon.Source as BitmapImage; }
            set { m_TransparentIcon.Source = value; }
        }

        public HeaderControl()
        {
            // Заголовок
            Grid HeaderLayoutRoot = new Grid();
            HeaderLayoutRoot.VerticalAlignment = VerticalAlignment.Center;
            HeaderLayoutRoot.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            HeaderLayoutRoot.ColumnDefinitions.Add(new ColumnDefinition());

            m_Icon = new Image() { Width = 16, Height = 16 };
            HeaderLayoutRoot.Children.Add(m_Icon);

            m_TransparentIcon = new Image() { Width = 16, Height = 16 };
            HeaderLayoutRoot.Children.Add(m_TransparentIcon);

            m_Caption = new TextBlock() { Margin = new Thickness(3, 0, 0, 0) };
            HeaderLayoutRoot.Children.Add(m_Caption);
            Grid.SetColumn(m_Caption, 1);

            this.Content = HeaderLayoutRoot;
        }

        public HeaderControl(BitmapImage icon, String caption)
            : this()
        {
            Icon = icon;
            Caption = caption;
        }
    }
}
