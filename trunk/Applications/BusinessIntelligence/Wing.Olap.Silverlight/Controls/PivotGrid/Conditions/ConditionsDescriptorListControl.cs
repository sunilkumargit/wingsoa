/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System.Windows;
using System.Windows.Media.Imaging;
using Wing.Olap.Controls.General;
using Wing.Olap.Controls.General.Tree;

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

