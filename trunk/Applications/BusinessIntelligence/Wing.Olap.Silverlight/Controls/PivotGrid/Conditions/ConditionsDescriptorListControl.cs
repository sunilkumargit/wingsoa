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
using Wing.Olap.Controls.General.Tree;
using Wing.Olap.Controls.General;
using System.Collections.Generic;
using Wing.Olap.Controls.ToolBar;
using System.Windows.Media.Imaging;

namespace Wing.Olap.Controls.PivotGrid.Conditions
{
    public class ConditionsDescriptorListControl : ObjectsListControlBase<CellConditionsDescriptor>
    {
        public ConditionsDescriptorListControl()
            : base()
        {
            ToolBar.Visibility = Visibility.Collapsed;
        }

        public override TreeNode<CellConditionsDescriptor> BuildTreeNode(CellConditionsDescriptor item)
        {
            BitmapImage icon = UriResources.Images.CellStyle16;
            return new TreeNode<CellConditionsDescriptor>(item.MemberUniqueName, icon, item);
        }

        public override bool Contains(string name)
        {
            if (List != null)
            {
                foreach (CellConditionsDescriptor descr in List)
                {
                    if (descr.MemberUniqueName == name)
                        return true;
                }
            }
            return false;
        }

        public override void Refresh()
        {
            foreach (object obj in Tree.Items)
            {
                RefreshItemNode(obj as TreeNode<CellConditionsDescriptor>);
            }
        }

        void RefreshItemNode(TreeNode<CellConditionsDescriptor> node)
        {
            if (node != null && node.Info != null)
            {
                node.Text = node.Info.MemberUniqueName;
            }
        }

        public void AddNew()
        {
            OnAddButtonClick();
        }

        public void DeleteCurrent()
        {
            OnDeleteButtonClick();
        }

        public void DeleteAll()
        {
            OnDeleteAllButtonClick();
        }
    }
}

