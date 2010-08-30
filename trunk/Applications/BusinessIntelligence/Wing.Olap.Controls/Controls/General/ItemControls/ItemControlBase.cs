﻿/*   
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

namespace Wing.AgOlap.Controls.General.ItemControls
{
    public class ItemControlBase : UserControl
    {
        StackPanel LayoutRoot = null;
        //Grid LayoutRoot = null;
        Image ItemIcon = null;
        TextBlock ItemText = null;

        public ItemControlBase()
            : this(true)
        {
        }

        public ItemControlBase(bool useIcon)
        {
            LayoutRoot = new StackPanel();
            LayoutRoot.Orientation = Orientation.Horizontal;
            //LayoutRoot = new Grid();
            //LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto } );
            //LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition());

            if(useIcon)
            {
                ItemIcon = new Image() { Width = 16, Height = 16 };
                ItemIcon.MouseLeftButtonDown += new MouseButtonEventHandler(ItemIcon_MouseLeftButtonDown);
                LayoutRoot.Children.Add(ItemIcon);
            }
            
            ItemText = new TextBlock() { Margin = new Thickness(3, 0, 0, 0) };
            ItemText.VerticalAlignment = VerticalAlignment.Center;
            ItemText.MouseLeftButtonDown += new MouseButtonEventHandler(ItemText_MouseLeftButtonDown);
            LayoutRoot.Children.Add(ItemText);
            //Grid.SetColumn(ItemText, 1);

            this.Content = LayoutRoot;
        }

        public String Text
        {
            set
            {
                ItemText.Text = value;
            }
            get
            {
                return ItemText.Text;
            }
        }

        public BitmapImage Icon
        {
            get
            {
                return ItemIcon.Source as BitmapImage;
            }
            set
            {
                if (ItemIcon != null)
                {
                    ItemIcon.Source = value;
                }
            }
        }

        
        public event EventHandler IconClick;
        void ItemIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            EventHandler handler = IconClick;
            if (handler != null)
            {
                e.Handled = true;
                handler(this, EventArgs.Empty);
            }
        }

        public event EventHandler TextClick;
        void ItemText_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            EventHandler handler = TextClick;
            if (handler != null)
            {
                e.Handled = true;
                handler(this, EventArgs.Empty);
            }
        }
    }
}

