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

namespace Ranet.AgOlap.Controls.General.Tree
{
    public class TreeNode<T> : TreeViewItem
    {
        public readonly T Info = default(T);
        TreeItemControl item_ctrl;

        public TreeNode(String caption, T info)
        {
            Info = info;
            item_ctrl = new TreeItemControl(false);
            item_ctrl.Text = caption;
            item_ctrl.MouseDoubleClick += new MouseDoubleClickEventHandler(item_ctrl_MouseDoubleClick);
            Header = item_ctrl;
        }

        public TreeNode(String caption, BitmapImage icon, T info)
        {
            Info = info;
            item_ctrl = new TreeItemControl();
            item_ctrl.Text = caption;
            item_ctrl.Icon = icon;
            item_ctrl.MouseDoubleClick += new MouseDoubleClickEventHandler(item_ctrl_MouseDoubleClick);
            Header = item_ctrl;
        }

        void item_ctrl_MouseDoubleClick(object sender, EventArgs e)
        {
            Raise_MouseDoubleClick(e);
        }

        public String Text
        {
            get { return item_ctrl.Text; }
            set { item_ctrl.Text = value; }
        }

        public BitmapImage Icon
        {
            get { return item_ctrl.Icon; }
            set { item_ctrl.Icon = value; }
        }

        public event MouseDoubleClickEventHandler MouseDoubleClick;
        void Raise_MouseDoubleClick(EventArgs e)
        {
            MouseDoubleClickEventHandler handler = MouseDoubleClick;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
