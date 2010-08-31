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
using System.Windows.Controls;

namespace Wing.Olap.Controls.General.Tree
{
    public class LoadAllTreeNode :TreeViewItem
    {
        public readonly String NODE_TEXT = Localization.MemberChoice_LoadAll;

        public LoadAllTreeNode()
        {
            TreeItemControl item_ctrl = new TreeItemControl();
            item_ctrl.Text = NODE_TEXT;
            item_ctrl.Icon = UriResources.Images.LoadAll16;

            Header = item_ctrl;

        }
    }
}
