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
