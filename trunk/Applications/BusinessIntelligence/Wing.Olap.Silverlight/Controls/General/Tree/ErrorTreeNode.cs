/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Windows.Controls;

namespace Wing.Olap.Controls.General.Tree
{
    public class ErrorTreeNode : TreeViewItem
    {
        public readonly String NODE_TEXT = Localization.TreeNode_ErrorLoadingData;

        public ErrorTreeNode()
        {
            TreeItemControl item_ctrl = new TreeItemControl();
            item_ctrl.Text = NODE_TEXT;
            item_ctrl.Icon = UriResources.Images.Error16;

            Header = item_ctrl;
        }
    }
}

