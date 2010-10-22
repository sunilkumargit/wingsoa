/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using Wing.Olap.Core.Metadata;

namespace Wing.Olap.Controls.General.Tree
{
    public class MeasureTreeNode : InfoBaseTreeNode
    {
        public MeasureTreeNode(MeasureInfo info)
            : base(info)
        {
            if (String.IsNullOrEmpty(info.Expression))
            {
                Icon = UriResources.Images.Measure16;
            }
            else
            {
                Icon = UriResources.Images.MeasureCalc16;
            }
        }
    }
}
