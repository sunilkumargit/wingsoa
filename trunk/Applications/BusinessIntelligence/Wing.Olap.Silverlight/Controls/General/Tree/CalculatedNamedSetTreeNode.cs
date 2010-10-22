/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
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
