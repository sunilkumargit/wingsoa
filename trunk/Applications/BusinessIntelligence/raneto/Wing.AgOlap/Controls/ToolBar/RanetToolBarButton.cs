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

namespace Ranet.AgOlap.Controls.ToolBar
{
    public class RanetToolBarButton :Button
    {
        public RanetToolBarButton()
        {
            DefaultStyleKey = typeof(RanetToolBarButton);
            this.Height = 22;
            this.Width = 22;
            this.Margin = new Thickness(2, 0, 0, 0);
        }

        public RanetToolBarButton(BitmapImage icon, String text)
        {
            Image ItemIcon = null;
            TextBlock ItemText = null;
            StackPanel LayoutRoot = null;

            DefaultStyleKey = typeof(RanetToolBarButton);
            this.Height = 22;
            this.MinWidth = 22;
            this.Margin = new Thickness(2, 0, 0, 0);

            LayoutRoot = new StackPanel();
            LayoutRoot.Orientation = Orientation.Horizontal;

            ItemIcon = new Image() { Width = 16, Height = 16 };
            ItemIcon.Source = icon;
            LayoutRoot.Children.Add(ItemIcon);

            ItemText = new TextBlock() { Margin = new Thickness(3, 0, 3, 0) };
            ItemText.VerticalAlignment = VerticalAlignment.Center;
            ItemText.Text = text;
            LayoutRoot.Children.Add(ItemText);

            this.Content = LayoutRoot;
        }
    }
}
