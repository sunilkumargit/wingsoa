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
using System.Windows.Controls.Primitives;

namespace Ranet.AgOlap.Controls.ToolBar
{
    public delegate void ItemClickEventHandler(object sender, ItemClickEventArgs e);

    public class ItemClickEventArgs : EventArgs
    {
        private UIElement item;

        public ItemClickEventArgs(UIElement item)
        {
            this.item = item;
        }

        public UIElement Item
        {
            get { return item; }
        }
    }

    public partial class RanetToolBar : UserControl
    {
        public event ItemClickEventHandler ItemClick;

        public RanetToolBar()
        {
            InitializeComponent();
        }

        public UIElementCollection Items
        {
            get
            {
                return stack.Children;
            }
        }

        public void AddItem(FrameworkElement item)
        {
           if(item != null)
           {
               ButtonBase btn = item as ButtonBase;
               if (btn != null)
               {
                   btn.Click -= new RoutedEventHandler(btn_Click);
                   btn.Click += new RoutedEventHandler(btn_Click);
               }
               stack.Children.Add(item);
           }
        }

        void btn_Click(object sender, RoutedEventArgs e)
        {
            Raise_ItemClick(new ItemClickEventArgs(sender as UIElement));
        }

        private void Raise_ItemClick(ItemClickEventArgs e)
        {
            ItemClickEventHandler handler = this.ItemClick;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
