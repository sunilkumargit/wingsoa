/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
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

namespace Wing.Olap.Controls.General.Tree
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
