using System.Collections.Generic;
using System.Windows.Media.Imaging;
using Wing.Olap.Controls.General;
using Wing.Olap.Controls.General.Tree;

namespace Wing.Olap.Controls.MdxDesigner.CalculatedMembers
{
    public class CalcMembersListControl : ObjectsListControlBase<CalculationInfoBase>
    {
        public override TreeNode<CalculationInfoBase> BuildTreeNode(CalculationInfoBase item)
        {
            BitmapImage icon = null;

            if (item is CalcMemberInfo)
                icon = UriResources.Images.Add16;
            if (item is CalculatedNamedSetInfo)
                icon = UriResources.Images.AddHot16;

            return new TreeNode<CalculationInfoBase>(item.Name, icon, item);
        }

        public override void Refresh()
        {
            foreach (object obj in Tree.Items)
            {
                RefreshItemNode(obj as TreeNode<CalculationInfoBase>);
            }
        }

        void RefreshItemNode(TreeNode<CalculationInfoBase> node)
        {
            if (node != null && node.Info != null)
            {
                node.Text = node.Info.Name;
            }
        }

        /// <summary>
        /// Список пользовательских вычисляемых элементов
        /// </summary>
        List<CalcMemberInfo> CalculatedMembers
        {
            get
            {
                List<CalcMemberInfo> list = new List<CalcMemberInfo>();
                foreach (CalculationInfoBase info in List)
                {
                    CalcMemberInfo memberInfo = info as CalcMemberInfo;
                    if (memberInfo != null)
                    {
                        list.Add(memberInfo);
                    }
                }
                return list;
            }
        }

        /// <summary>
        /// Список пользовательских вычисляемых именованных наборов
        /// </summary>
        List<CalculatedNamedSetInfo> CalculatedNamedSets
        {
            get
            {
                List<CalculatedNamedSetInfo> list = new List<CalculatedNamedSetInfo>();
                foreach (CalculationInfoBase info in List)
                {
                    CalculatedNamedSetInfo memberInfo = info as CalculatedNamedSetInfo;
                    if (memberInfo != null)
                    {
                        list.Add(memberInfo);
                    }
                }
                return list;
            }
        }
    }
}