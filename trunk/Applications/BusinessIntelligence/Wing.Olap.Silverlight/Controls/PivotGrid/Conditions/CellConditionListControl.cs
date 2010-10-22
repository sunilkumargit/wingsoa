/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using Wing.Olap.Controls.General;
using Wing.Olap.Controls.General.Tree;

namespace Wing.Olap.Controls.PivotGrid.Conditions
{
    public class CellConditionListControl : ObjectsListControlBase<CellCondition>
    {
        public CellConditionListControl()
            : base()
        {
        }

        public override TreeNode<CellCondition> BuildTreeNode(CellCondition item)
        {
            return new TreeNode<CellCondition>(item.ToString(), item.ToImage(), item);
        }


        public override void Refresh()
        {
            foreach (object obj in Tree.Items)
            {
                RefreshItemNode(obj as TreeNode<CellCondition>);
            }
        }

        void RefreshItemNode(TreeNode<CellCondition> node)
        {
            if (node != null && node.Info != null)
            {
                node.Text = node.Info.ToString();
                node.Icon = node.Info.ToImage();
            }
        }
    }
}

