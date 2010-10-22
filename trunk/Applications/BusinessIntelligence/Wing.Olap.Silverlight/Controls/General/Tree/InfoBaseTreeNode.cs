/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using Wing.Olap.Core.Metadata;

namespace Wing.Olap.Controls.General.Tree
{
    public class InfoBaseTreeNode : CustomTreeNode
    {
        InfoBase m_Info = null;
        public InfoBase Info
        {
            get
            {
                return m_Info;
            }
        }

        public InfoBaseTreeNode(InfoBase info)
        {
            if (info == null)
                throw new ArgumentNullException("info");

            m_Info = info;
            Text = info.Caption;
        }
    }
}
