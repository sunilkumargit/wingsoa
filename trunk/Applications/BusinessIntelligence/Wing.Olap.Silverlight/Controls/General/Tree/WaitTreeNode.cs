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
using System.Windows.Controls;

namespace Wing.AgOlap.Controls.General.Tree
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
