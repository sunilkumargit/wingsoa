/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using Wing.Olap.Core.Metadata;

namespace Wing.Olap.Controls.General.Tree
{
    public class KpiGoalTreeNode : InfoBaseTreeNode
    {
        public KpiGoalTreeNode(KpiInfo info)
            : base(info)
        {
            Icon = UriResources.Images.Kpi16;
            Text = "Goal";
        }
    }
}