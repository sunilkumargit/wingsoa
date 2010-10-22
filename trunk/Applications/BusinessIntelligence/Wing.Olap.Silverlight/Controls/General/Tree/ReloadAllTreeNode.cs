/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Windows.Controls;
using System.Windows.Media;

namespace Wing.Olap.Controls.General.Tree
{
    public class ReloadAllTreeNode : TreeViewItem
    {
        public readonly String NODE_TEXT = Localization.MemberChoice_LoadAll;

        public ReloadAllTreeNode()
        {
            TreeItemControl item_ctrl = new TreeItemControl();
            item_ctrl.Text = NODE_TEXT;
            item_ctrl.Icon = UriResources.Images.LoadAll16;
            item_ctrl.ItemText.Foreground = new SolidColorBrush(Colors.Blue);

            Header = item_ctrl;

        }
    }
}
