/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using Wing.Olap.Core.Metadata;

namespace Wing.Olap.Controls.General.Tree
{
    public class HierarchyTreeNode : InfoBaseTreeNode
    {
        public HierarchyTreeNode(HierarchyInfo info)
            : base(info)
        {
            switch (info.HierarchyOrigin)
            {
                case HierarchyInfoOrigin.AttributeHierarchy:
                    Icon = UriResources.Images.AttributeHierarchy16;
                    break;
                case HierarchyInfoOrigin.ParentChildHierarchy:
                    Icon = UriResources.Images.ParentChildHierarchy16;
                    break;
                case HierarchyInfoOrigin.UserHierarchy:
                    Icon = UriResources.Images.UserHierarchy16;
                    break;
            }
        }
    }
}
