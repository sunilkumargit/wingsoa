/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Windows.Controls;

namespace Wing.Olap.Controls.General.Tree
{
    public class WaitTreeNode : TreeViewItem
    {
        public readonly String NODE_TEXT = Localization.Loading;

        WaitTreeControl m_ItemCtrl = null;

        public WaitTreeNode()
        {
            //Header = NODE_TEXT;
            m_ItemCtrl = new WaitTreeControl();
            Header = m_ItemCtrl;

            m_ItemCtrl.Text = NODE_TEXT;
        }
    }
}
