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
using Wing.Olap.Core.Data;

namespace Wing.AgOlap.Controls.General.Tree
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
