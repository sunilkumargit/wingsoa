/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using Wing.Olap.Core.Metadata;

namespace Wing.Olap.Controls.General.Tree
{
    public class DimensionTreeNode : InfoBaseTreeNode
    {
        public DimensionTreeNode(DimensionInfo info)
            : base(info)
        {
            if (info.DimensionType == DimensionInfoTypeEnum.Measure)
            {
                Icon = UriResources.Images.Measure16;
            }
            else
            {
                Icon = UriResources.Images.Dimension16;
            }
        }
    }
}
