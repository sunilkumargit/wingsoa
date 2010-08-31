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
using Wing.Olap.Controls.MdxDesigner.CalculatedMembers;

namespace Wing.Olap.Controls.General.Tree
{
    public class CalculatedNamedSetTreeNode : CustomTreeNode
    {
        CalculatedNamedSetInfo m_Info = null;
        public CalculatedNamedSetInfo Info
        {
            get
            {
                return m_Info;
            }
        }

        public CalculatedNamedSetTreeNode(CalculatedNamedSetInfo info)
        {
            if (info == null)
                throw new ArgumentNullException("info");

            Icon = UriResources.Images.CustomNamedSet16;
            Text = info.Name;
            m_Info = info;
        }
    }
}
