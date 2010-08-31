/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using Wing.Olap.Core.Metadata;
using System.Windows.Controls;

namespace Wing.Olap.Controls.General.Tree
{
    public class CubeTreeNode : InfoBaseTreeNode
    {
        public CubeTreeNode(CubeDefInfo info) : base(info)
        {
            Icon = UriResources.Images.Cube16;
        }

        public override bool IsInitialized
        {
            get
            {
                //Если у элемента один дочерний и он "WaitNode", то значит данные не грузились
                if (IsWaiting)
                {
                    if(Items.Count == 1)
                        return false;

                    if (Items.Count == 3)
                    {
                        foreach (TreeViewItem item in Items)
                        {
                            MeasuresFolderTreeNode measuresNode = item as MeasuresFolderTreeNode;
                            if (measuresNode != null)
                                continue;
                            KPIsFolderTreeNode kpiNode = item as KPIsFolderTreeNode;
                            if (kpiNode != null)
                                continue;
                            WaitTreeNode waitNode = item as WaitTreeNode;
                            if (waitNode != null)
                                continue;
                            return true;
                        }
                        return false;
                    }
                }
                return true;
            }
        }
    }
}
