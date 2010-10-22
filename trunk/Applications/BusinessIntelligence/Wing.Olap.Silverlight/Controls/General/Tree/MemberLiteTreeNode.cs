/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using Wing.Olap.Core.Data;

namespace Wing.Olap.Controls.General.Tree
{
    public class MemberLiteTreeNode : CustomTreeNode
    {
        public readonly MemberData Info = null;

        public MemberLiteTreeNode(MemberData info)
            : base()
        {
            Info = info;
            Icon = UriResources.Images.MemberSmall16;
            MemberVisualizationType = MemberVisualizationTypes.Caption;
        }

        MemberVisualizationTypes m_MemberVisualizationType = MemberVisualizationTypes.Caption;
        public MemberVisualizationTypes MemberVisualizationType
        {
            get { return m_MemberVisualizationType; }
            set
            {
                m_MemberVisualizationType = value;
                // Определяем что именно нужно светить в контроле
                if (Info != null && Info != null)
                    Text = Info.GetText(m_MemberVisualizationType);
                else
                    Text = String.Empty;
            }
        }
    }
}
